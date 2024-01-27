using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceLogic : MonoBehaviour
{
    public GameObject selectedPiece;
    Vector3 offset;
    Vector3 previousPosition;
    static Transform[,] grid = new Transform[9, 9];
    public GameObject Spawner1;
    public GameObject Spawner2;
    public GameObject Spawner3;
    private int placementCount = 0;
    public Text text;
    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if all pieces have been placed, spawns new pieces if so
        if (placementCount == 3)
        {
            placementCount = 0;
            Spawner1.GetComponent<PieceSpawn>().SpawnPiece();
            Spawner2.GetComponent<PieceSpawn>().SpawnPiece();
            Spawner3.GetComponent<PieceSpawn>().SpawnPiece();
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        //Select game object on mouse down
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D targetPiece = Physics2D.OverlapPoint(mousePosition);
            if (targetPiece)
            {
                if (targetPiece.transform.parent)
                {
                    selectedPiece = targetPiece.transform.parent.gameObject;
                    previousPosition = selectedPiece.transform.position;
                    offset = selectedPiece.transform.position - mousePosition;
                }
                else
                {
                    selectedPiece = targetPiece.transform.gameObject;
                    previousPosition = selectedPiece.transform.position;
                    offset = selectedPiece.transform.position - mousePosition;
                }
            }

        }

        //Checks if piece is selected and is moveable, then moves with cursor
        if (selectedPiece && Moveable(previousPosition))
        {
            selectedPiece.transform.position = mousePosition + offset;
            selectedPiece.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            selectedPiece = null;
        }

        //Decide what to do with piece after mouse button is released
        if (Input.GetMouseButtonUp(0) && selectedPiece)
        {
            int currX = Mathf.RoundToInt(selectedPiece.transform.position.x);
            int currY = Mathf.RoundToInt(selectedPiece.transform.position.y);
            selectedPiece.transform.position = new Vector3(currX, currY, 1);

            if (ValidPlacement(selectedPiece) == false)
            {
                selectedPiece.transform.position = previousPosition;
                selectedPiece.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                selectedPiece = null;
            }
            else
            {
                AddToGrid(selectedPiece);
                int score = CheckForLines();
                addScore(score);
                selectedPiece = null;
                ++placementCount;
            }

        }
    }

    void addScore(int toAdd)
    {
        score += toAdd;
        text.text = score.ToString();
    }

    bool HasHorizontal(int i)
    {
        for (int j = 0; j < 9; ++j)
        {
            if (grid[j, i] == null)
                return false;
        }
        return true;
    }

    bool HasVertical(int i)
    {
        for (int j = 0; j < 9; ++j)
        {
            if (grid[i, j] == null)
                return false;
        }
        return true;
    }

    bool HasSquare(int i)
    {
        int x = 0;
        int y = 0;

        switch (i)
        {
            case 0:
                x = 0;
                y = 0;
                break;

            case 1:
                x = 3;
                y = 0;
                break;

            case 2:
                x = 6;
                y = 0;
                break;

            case 3:
                x = 0;
                y = 3;
                break;

            case 4:
                x = 3;
                y = 3;
                break;

            case 5:
                x = 6;
                y = 3;
                break;

            case 6:
                x = 0;
                y = 6;
                break;

            case 7:
                x = 3;
                y = 6;
                break;

            case 8:
                x = 6;
                y = 6;
                break;
        }

        for (int j = x; j < x + 3; ++j)
        {
            for (int k = y; k < y + 3; ++k)
            {
                if (grid[k, j] == null)
                    return false;
            }
        }

        return true;
    }

    void DeleteHorizontal(int i)
    {
        for (int j = 0; j < 9; ++j)
        {
            if (grid[j, i] == null)
                continue;
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }
    }

    void DeleteVertical(int i)
    {
        for (int j = 0; j < 9; ++j)
        {
            if (grid[i, j] == null)
                continue;
            Destroy(grid[i, j].gameObject);
            grid[i, j] = null;
        }
    }

    void DeleteSquare(int i)
    {
        int x = 0;
        int y = 0;

        switch (i)
        {
            case 0:
                x = 0;
                y = 0;
                break;

            case 1:
                x = 3;
                y = 0;
                break;

            case 2:
                x = 6;
                y = 0;
                break;

            case 3:
                x = 0;
                y = 3;
                break;

            case 4:
                x = 3;
                y = 3;
                break;

            case 5:
                x = 6;
                y = 3;
                break;

            case 6:
                x = 0;
                y = 6;
                break;

            case 7:
                x = 3;
                y = 6;
                break;

            case 8:
                x = 6;
                y = 6;
                break;
        }

        for (int j = x; j < x + 3; ++j)
        {
            for (int k = y; k < y + 3; ++k)
            {
                if (grid[k, j] == null)
                    continue;
                Destroy(grid[k, j].gameObject);
                grid[k, j] = null;
            }
        }
    }

    int CheckForLines()
    {
        int scoreCount = 0;

        bool[] horizontal = { false, false, false, false, false, false, false, false, false };
        bool[] vertical = { false, false, false, false, false, false, false, false, false };
        bool[] square = { false, false, false, false, false, false, false, false, false };

        for (int i = 0; i < 9; ++i)
        {
            if (HasHorizontal(i))
            {
                horizontal[i] = true;
            }
            if (HasVertical(i))
            {
                vertical[i] = true;
            }
            if (HasSquare(i))
            {
                square[i] = true;
            }
        }

        for (int i = 0; i < 9; ++i)
        {
            if (horizontal[i])
            {
                ++scoreCount;
                DeleteHorizontal(i);
                horizontal[i] = false;
            }
            if (vertical[i])
            {
                ++scoreCount;
                DeleteVertical(i);
                vertical[i] = false;
            }
            if (square[i])
            {
                ++scoreCount;
                DeleteSquare(i);
                square[i] = false;
            }
        }

        if (scoreCount == 0)
            return 0;

        return scoreCount * 100 + ((scoreCount - 1) * 50);
    }

    void AddToGrid(GameObject selectedPiece)
    {
        if (selectedPiece.transform.childCount > 0)
        {
            foreach (Transform children in selectedPiece.transform)
            {
                int roundedX = Mathf.RoundToInt(children.transform.position.x);
                int roundedY = Mathf.RoundToInt(children.transform.position.y);

                grid[roundedX, roundedY] = children;
            }
        }
        else
        {
            int roundedX = Mathf.RoundToInt(selectedPiece.transform.position.x);
            int roundedY = Mathf.RoundToInt(selectedPiece.transform.position.y);

            grid[roundedX, roundedY] = selectedPiece.transform;
        }
    }

    bool ValidPlacement(GameObject selectedPiece)
    {
        if (selectedPiece.transform.childCount > 0)
        {
            foreach (Transform children in selectedPiece.transform)
            {
                int roundedX = Mathf.RoundToInt(children.transform.position.x);
                int roundedY = Mathf.RoundToInt(children.transform.position.y);

                if (roundedX < 0 || roundedX > 8 || roundedY < 0 || roundedY > 8)
                    return false;

                if (grid[roundedX, roundedY] != null)
                    return false;
            }
        }
        else
        {
            int roundedX = Mathf.RoundToInt(selectedPiece.transform.position.x);
            int roundedY = Mathf.RoundToInt(selectedPiece.transform.position.y);

            if (roundedX < 0 || roundedX > 8 || roundedY < 0 || roundedY > 8)
                return false;

            if (grid[roundedX, roundedY] != null)
                return false;
        }
        
        return true;
    }

    bool Moveable(Vector3 previousLocation)
    {
        if (previousLocation.x >= 0 && previousLocation.x <= 8 
            && previousLocation.y >= 0 && previousLocation.y <= 8)
            return false;

        return true;
    }
}
