using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject boidPrefab;

    [SerializeField, Range(0, 400)]
    int population;

    [SerializeField]
    float topBound, bottomBound, leftBound, rightBound;

    [SerializeField, Range(0,10)]    
    int speed; 

    [SerializeField]
    float detectionRadius = 1f;
    [SerializeField, Range(0, 360)]
    int viewAngle;

    GameObject[] boids; 
    Vector3[] boidDirection;

    void Awake()
    {
        boids = new GameObject[population];
        boidDirection = new Vector3[population];
        for (int i = 0; i < population; i++) {
            GameObject boid = boids[i] = Instantiate(boidPrefab);
            boid.transform.SetParent(transform, false);
            boid.transform.localPosition = new Vector3(
                Random.Range(leftBound, rightBound),
                Random.Range(topBound, bottomBound)
            );
            boidDirection[i] = Random.insideUnitCircle.normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < population; i++) {
            DetectObstacles(i);
            DetectOtherBoids(i);
            Vector3 position = boids[i].transform.localPosition;
            position += boidDirection[i] * speed * Time.deltaTime;
             
            if (position.x > rightBound) position = new Vector3(leftBound, position.y);
            else if (position.x < leftBound) position = new Vector3(rightBound, position.y);
            else if (position.y > topBound) position = new Vector3(position.x, bottomBound);
            else if (position.y < bottomBound) position = new Vector3(position.x, topBound);
            /*           
            if (position.x > rightBound || position.x < leftBound || position.y > topBound || position.y < bottomBound)
                boidDirection[i] = Quaternion.AngleAxis(Random.Range(180f, 360f), Vector3.forward) * boidDirection[i];
            */
            boids[i].transform.localRotation = RotateTowardsMovement(boids[i].transform.localPosition, position); 
            boids[i].transform.localPosition = position;
        } 
    }

    private Quaternion RotateTowardsMovement(Vector3 currPos, Vector3 nextPos) {
        Vector3 direction = nextPos - currPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        return rotation;
    }

    private void DetectOtherBoids(int boidID) {
        Transform boid = boids[boidID].transform;
        List<int> nearbyBoids = new List<int>(population);
        for (int i = 0; i < population; i++) {
            if (Vector3.Distance(boid.position, boids[i].transform.position) <= detectionRadius) {
                nearbyBoids.Add(i);
            } 
        }
        Vector3 direction = boidDirection[boidID];
        //Vector3 position = Vector3.zero;
        for (int i = 0; i < nearbyBoids.Count; i++) {
            direction += (boidDirection[nearbyBoids[i]] + boidDirection[boidID]).normalized;
            //position += boids[nearbyBoids[i]].transform.position;
        }
        direction /= nearbyBoids.Count + 1;
        //position /= nearbyBoids.Count + 1;
        //boidDirection[boidID] = (direction + position).normalized;
        boidDirection[boidID] = direction;
    }

    private void DetectObstacles(int boidID) {
        Collider2D collider = boids[boidID].GetComponent<Collider2D>();
        RaycastHit2D[] results = new RaycastHit2D[1];
        if (collider.Raycast(boidDirection[boidID], results, detectionRadius) != 0) {
            Vector3 direction = boidDirection[boidID];
            //List<Vector3> possibleDirections = new List<Vector3>();
            for (int i = 10; i <= viewAngle / 2; i += 10) {
                if (collider.Raycast(Quaternion.AngleAxis(-i, Vector3.forward) * direction, results, detectionRadius) == 0) {
                    //possibleDirections.Add(Quaternion.AngleAxis(-i, Vector3.forward) * direction);
                    boidDirection[boidID] = Quaternion.AngleAxis(-10, Vector3.forward) * direction;
                    return;
                }
                if (collider.Raycast(Quaternion.AngleAxis(i, Vector3.forward) * direction, results, detectionRadius) == 0) {
                    //possibleDirections.Add(Quaternion.AngleAxis(i, Vector3.forward) * direction);
                    boidDirection[boidID] = Quaternion.AngleAxis(10, Vector3.forward) * direction;
                    return;
                }
            }
            //boidDirection[boidID] = possibleDirections[Random.Range(0, possibleDirections.Count)]; 
            if (Random.Range(0, 2) == 0) { 
                boidDirection[boidID] = Quaternion.AngleAxis(-10, Vector3.forward) * direction;
            }
            else {
                boidDirection[boidID] = Quaternion.AngleAxis(10, Vector3.forward) * direction;
            }
        }
    }
}