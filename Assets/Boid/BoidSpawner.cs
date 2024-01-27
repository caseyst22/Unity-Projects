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
    float boidDetection = 1f, obstacleDetection = 2f, collisionDetection = 0.5f;

    [SerializeField, Range(0, 360)]
    int viewAngle;

    struct Boid {
        public GameObject body;
        public Vector3 direction;
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
            //If obstacle is detected, avoid it during this frame
            //if (AvoidObstacles(i)) continue;
            //Otherwise, get other boids that are visible by this boid
            List<int> visibleBoids = GetVisibleBoids(i);
            //If collision with nearbyBoid detected, avoid collision during this frame
            if (AvoidVisibleBoids(i, visibleBoids)) continue;
            //Otherwise move with visible boids and jostle for position
            //MoveWithVisibleBoids(i, visibleBoids);
            //JostleForPosition(i, visibleBoids);
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

    private bool AvoidVisibleBoids(int boidID, List<int> visibleBoids) {
        Boid boid = boids[boidID];
        Vector3 nextPosition = GetNextPosition(boid.body.transform.localPosition, boid.direction);
        bool collisionDetected = false;
        foreach (int i in visibleBoids) {
            Vector3 checkingPosition = GetNextPosition(boids[i].body.transform.localPosition, boids[i].direction);
            if ((nextPosition - checkingPosition).magnitude <= collisionDetection) {
                boid.direction = Quaternion.AngleAxis(20, Vector3.forward) * boid.direction; 
                collisionDetected = true;
            }
        }
        return collisionDetected;
    }

    private void MoveWithVisibleBoids(int boidID, List<int> nearbyBoids) {
        Vector3 direction = boids[boidID].direction;
        for (int i = 0; i < nearbyBoids.Count; i++) {
            direction += boids[nearbyBoids[i]].direction;
        }
        direction = (direction / (nearbyBoids.Count + 1)).normalized;
        boids[boidID].direction = direction;
    }

    private void JostleForPosition(int boidID, List<int> nearbyBoids) {

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