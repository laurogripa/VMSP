using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordScore : MonoBehaviour
{
    private int score;
    [SerializeField] private Text scoreText; 

    public void SetScore(int newScore)
    {
        score = newScore;
        scoreText.text = score.ToString();
    }
}
