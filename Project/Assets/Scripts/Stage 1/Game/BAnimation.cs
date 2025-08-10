using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BAnimation : MonoBehaviour
{
    private Image background;
    [SerializeField]
    private Sprite[] bAnimation;
    private float count;
    private int currentFrame, frameRate;

    void Start()
    {
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
    }
}
