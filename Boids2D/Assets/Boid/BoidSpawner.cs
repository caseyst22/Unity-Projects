using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject boidPrefab;

    [SerializeField, Range(0, 400)]
    int population;

    [SerializeField]
    float vBound, hBound;

    [SerializeField, Range(0,10)]    
    int speed; 

    [SerializeField]
    float visualRadius = 2f, avoidRadius = 1f;

    [SerializeField, Range(0, 360)]
    int viewAngle;

    struct Boid {
        public GameObject body;
        public Vector3 velocity;
    }

    Boid[] boids;

    void Awake()
    {
        boids = new Boid[population];
        for (int i = 0; i < population; i++) {
            GameObject boid = boids[i].body = Instantiate(boidPrefab);
            boid.transform.SetParent(transform, false);
            boid.transform.localPosition = new Vector3(
                Random.Range(-hBound, hBound),
                Random.Range(-vBound, vBound)
            );
            boids[i].direction = Random.insideUnitCircle.normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update direction for each boid
        for (int i = 0; i < population; i++) {
            //List<int> visibleBoids = GetVisibleBoids(i);
            Separation(i);
            Alignment(i);
            Cohesion(i);
        } 
        //Update position for each boid
        for (int i = 0; i < population; i++) {
            Vector3 position = boids[i].body.transform.localPosition;
            position = GetNextPosition(position, boids[i].direction);
             
            if (position.x > hBound) position = new Vector3(-hBound, position.y);
            else if (position.x < -hBound) position = new Vector3(hBound, position.y);
            else if (position.y > vBound) position = new Vector3(position.x, -vBound);
            else if (position.y < -vBound) position = new Vector3(position.x, vBound);

            boids[i].body.transform.localRotation = RotateTowardsMovement(boids[i].body.transform.localPosition, position); 
            boids[i].body.transform.localPosition = position;
        }
    }

    private Vector3 GetNextPosition(Vector3 currentPosition, Vector3 direction) {
        return currentPosition + (direction * speed * Time.deltaTime);
    }

    private Quaternion RotateTowardsMovement(Vector3 currPos, Vector3 nextPos) {
        Vector3 direction = nextPos - currPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        return rotation;
    }

    //Gets all boids within cone of sight of boids[boidID] 
    private List<int> GetVisibleBoids(int boidID) {
        Boid currentBoid = boids[boidID];
        List<int> visibleBoids = new List<int>();
        for (int i = 0; i < population; i++) {
            if (i == boidID) continue;
            //Check if boid is within cone of sight
            Boid toCheck = boids[i];
            float distance = (toCheck.body.transform.position - currentBoid.body.transform.position).magnitude; 
            if (distance <= boidDetection) {
                Vector3 direction = toCheck.body.transform.position  - currentBoid.body.transform.position;
                float angle = Vector3.Angle(currentBoid.direction, direction);
                if (angle < viewAngle / 2) {
                    visibleBoids.Add(i);
                }
            }
        }
        return visibleBoids;
    }

    private void Separation(int boidID) {
        Boid currBoid = boids[boidID];

    }

    private void Alignment(int boidID) {

    }

    private void Cohesion(int boidID) {

    }

    private bool AvoidObstacles(int boidID) {
        Collider2D collider = boids[boidID].body.GetComponent<Collider2D>();
        RaycastHit2D[] results = new RaycastHit2D[1];
        Vector3 direction = boids[boidID].direction;
        //If there is an obstacle in the way, search for next direction without obstacle
        if (collider.Raycast(direction, results, obstacleDetection, LayerMask.GetMask("Obstacles")) != 0) {
            //Randomizes which direction this boid searches first
            int angleModifier = (Random.Range(0, 2) == 0) ? 1 : -1;
            for (int i = 10; i <= viewAngle / 2; i += 10) {
                Vector3 newDirection = Quaternion.AngleAxis((i + 20) * angleModifier, Vector3.forward) * direction;
                if (collider.Raycast(newDirection, results, obstacleDetection, LayerMask.GetMask("Obstacles")) == 0) {
                    boids[boidID].direction = newDirection;
                    return true;
                }
                newDirection = Quaternion.AngleAxis(i * -angleModifier, Vector3.forward) * direction;
                if (collider.Raycast(newDirection, results, obstacleDetection, LayerMask.GetMask("Obstacles")) == 0) {
                    boids[boidID].direction = Quaternion.AngleAxis((i + 20) * -angleModifier, Vector3.forward) * direction;
                    return true;
                }
            }
        }
        //If no obstacle found, return false
        return false;
    }
}