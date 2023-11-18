using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPaddleScript : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    private Rigidbody2D body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var vel = body.velocity;

        if (Input.GetKey(KeyCode.UpArrow) && transform.position.y < 5)
        {
            vel.y = moveSpeed;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && transform.position.y > -5)
        {
            vel.y = -moveSpeed;
        }
        else
        {
            vel.y = 0;
        }

        body.velocity = vel;
    }
}
