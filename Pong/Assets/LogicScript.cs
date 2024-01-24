using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public int leftScore = 0;
    public Text leftText;
    public int rightScore = 0;
    public Text rightText;
    public BallScript ball;

    public void addLeftScore()
    {
        ++leftScore;
        leftText.text = leftScore.ToString();
    }
    public void addRightScore()
    {
        ++rightScore;
        rightText.text = rightScore.ToString();
    }

    public void resetBall()
    {
        ball.resetBall();
    }
}
