using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

public class BAnimation : MonoBehaviour
{
    private Image background;
    [SerializeField]
    private Sprite[] bAnimation;
    private float count;
    private int currentFrame, frameRate;

    public Text xCoord;
    public Text yCoord;
    public GameObject GazePoint;

    private float _pauseTimer;

    void Start()
    {
        if (TobiiAPI.IsConnected)
        {
            Debug.Log("Tobii Eye Tracker is connected.");
        }
        else
        {
            Debug.LogError("Tobii Eye Tracker is not detected!");
        }

        background = gameObject.GetComponent<Image>();
        currentFrame = 0;
        frameRate = 20;
    }

    void FixedUpdate()
    {
        count += Time.deltaTime;
        if(count >= 1f/frameRate)
        {
            currentFrame++; 
            if(currentFrame >= bAnimation.Length)
            {
                currentFrame = 0;
            }
            background.sprite = bAnimation[currentFrame];
            count = 0;
        }

        if (_pauseTimer > 0)
        {
            _pauseTimer -= Time.deltaTime;
            return;
        }
    }
}
