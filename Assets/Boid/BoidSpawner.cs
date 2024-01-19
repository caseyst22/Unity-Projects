using System.Collections.Generic;
using System.Xml.Serialization;
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
    float detectionRadius = 1f, minRadius = .25f;
    [SerializeField, Range(0, 360)]
    int viewAngle;

    [SerializeField]
    int maxDetectableBoids = 5;

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
                Random.Range(-hBound, hBound),
                Random.Range(-vBound, vBound)
            );
            boidDirection[i] = Random.insideUnitCircle.normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update Direction
        for (int i = 0; i < population; i++) {
            //AvoidObstacles(i);
            List<int> nearbyBoids = GetNearbyBoids(i);
            AvoidOtherBoids(i);
            MoveWithNearbyBoids(i, nearbyBoids);
        } 
        //Update positions
        for (int i = 0; i < population; i++) {
            Vector3 position = boids[i].transform.localPosition;
            position += boidDirection[i] * speed * Time.deltaTime;
             
            
            if (position.x > hBound) position = new Vector3(-hBound, position.y);
            else if (position.x < -hBound) position = new Vector3(hBound, position.y);
            else if (position.y > vBound) position = new Vector3(position.x, -vBound);
            else if (position.y < -vBound) position = new Vector3(position.x, vBound);

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

    private List<int> GetNearbyBoids(int boidID) {
        Transform boid = boids[boidID].transform;
        List<int> nearbyBoids = new List<int>(maxDetectableBoids);
        for (int i = 0; i < population; i++) {
            float distance = Vector3.Distance(boid.position, boids[i].transform.position);
            if (distance > detectionRadius) continue;
            nearbyBoids.Add(i);
            if (nearbyBoids.Count == maxDetectableBoids) break;
        }
        return nearbyBoids;
    }

    private void MoveWithNearbyBoids(int boidID, List<int> nearbyBoids) {
        Vector3 direction = boidDirection[boidID];
        for (int i = 0; i < nearbyBoids.Count; i++) {
            direction += boidDirection[nearbyBoids[i]];
        }
        direction = (direction / (nearbyBoids.Count + 1)).normalized;
        boidDirection[boidID] = direction;
    }

    private void JostleForPosition(int boidID, List<int> nearbyBoids) {

    }

    private void AvoidOtherBoids(int boidID) {
        Collider2D collider = boids[boidID].GetComponent<Collider2D>();
        RaycastHit2D[] results = new RaycastHit2D[1];
        if (collider.Raycast(boidDirection[boidID], results, detectionRadius, LayerMask.GetMask("Default")) != 0) {
            Vector3 direction = boidDirection[boidID];
            for (int i = 10; i <= viewAngle / 2; i += 10) {
                int angle = (Random.Range(0, 2) == 0) ? i : -i;
                if (collider.Raycast(Quaternion.AngleAxis(angle, Vector3.forward) * direction,
                    results, detectionRadius, LayerMask.GetMask("Default")) == 0) {
                    boidDirection[boidID] = Quaternion.AngleAxis(10 * (angle / Mathf.Abs(angle)), Vector3.forward) * direction;
                    return;
                }
                if (collider.Raycast(Quaternion.AngleAxis(-angle, Vector3.forward) * direction,
                    results, detectionRadius, LayerMask.GetMask("Default")) == 0) {
                    boidDirection[boidID] = Quaternion.AngleAxis(10 * (-angle / Mathf.Abs(angle)), Vector3.forward) * direction;
                    return;
                }
            }
        }
    }

    private void AvoidObstacles(int boidID) {
        Collider2D collider = boids[boidID].GetComponent<Collider2D>();
        RaycastHit2D[] results = new RaycastHit2D[1];
        if (collider.Raycast(boidDirection[boidID], results, detectionRadius, LayerMask.GetMask("Obstacles")) != 0) {
            Vector3 direction = boidDirection[boidID];
            for (int i = 10; i <= viewAngle / 2; i += 10) {
                if (collider.Raycast(Quaternion.AngleAxis(-i, Vector3.forward) * direction,
                    results, detectionRadius, LayerMask.GetMask("Obstacles")) == 0) {
                    boidDirection[boidID] = Quaternion.AngleAxis(-i - 20, Vector3.forward) * direction;
                    return;
                }
                if (collider.Raycast(Quaternion.AngleAxis(i, Vector3.forward) * direction,
                    results, detectionRadius, LayerMask.GetMask("Obstacles")) == 0) {
                    boidDirection[boidID] = Quaternion.AngleAxis(i + 20, Vector3.forward) * direction;
                    return;
                }
            }
        }
    }
}