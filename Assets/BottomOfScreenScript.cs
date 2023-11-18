using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomOfScreenScript : MonoBehaviour
{
    public LogicScript logic;
    public BirdScript bird;

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        bird = GameObject.FindGameObjectWithTag("Bird").GetComponent<BirdScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        logic.gameOver();
        bird.birdIsAlive = false;
    }
}
