using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody2D body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        Invoke("OnStart", 2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnStart()
    {
        float rand = Random.Range(0, 2);
        if (rand < 1)
        {
            body.AddForce(new Vector2(20, -15));
        }
        else
        {
            body.AddForce(new Vector2(-20, -15));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Paddle"))
        {
            Vector2 vel;
            vel.x = (float)(body.velocity.x);
            vel.y = (body.velocity.y / 2) + (collision.collider.attachedRigidbody.velocity.y / 2);
            body.velocity = vel;
        }
    }

    public void resetBall()
    {
        body.velocity = new Vector2(0, 0);
        transform.position = new Vector3(0, 2, 0);
        Invoke("OnStart", 1);
    }
}
