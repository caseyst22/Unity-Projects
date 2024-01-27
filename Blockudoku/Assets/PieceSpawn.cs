using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawn : MonoBehaviour
{
    public GameObject[] pieces;
    private float[] rotations = { 0, 90, 180, 270 };
    // Start is called before the first frame update
    void Start()
    {
        SpawnPiece();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPiece()
    {
        GameObject piece = Instantiate(pieces[Random.Range(0, pieces.Length)], transform.position, 
            Quaternion.Euler(new Vector3(0, 0, rotations[Random.Range(0, rotations.Length)])));
        piece.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
