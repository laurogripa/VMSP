using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class SequenceAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] animationTo0, animationTo1, animationTo2, animationTo3;
    [SerializeField] private int lighterID;
    private float disabledTime;

    private void FixedUpdate()
    {
        if (disabledTime > 0)
        {
            disabledTime -= Time.deltaTime;
        }
    }

    public IEnumerator RunAnimation(bool inverse, int destinationID)
    {
        int currentFrame;
        Sprite[] destinationSprite = new Sprite[0];

        switch (destinationID)
        {
            case 0:
                if (animationTo0.Length == 0)
                {
                    animationTo0 = LoadSpriteGroup(destinationID);
                }
                destinationSprite = animationTo0;
                break;
            case 1:
                if (animationTo1.Length == 0)
                {
                    animationTo1 = LoadSpriteGroup(destinationID);
                }
                destinationSprite = animationTo1;
                break;
            case 2:
                if (animationTo2.Length == 0)
                {
                    animationTo2 = LoadSpriteGroup(destinationID);
                }
                destinationSprite = animationTo2;
                break;
            case 3:
                if (animationTo3.Length == 0)
                {
                    animationTo3 = LoadSpriteGroup(destinationID);
                }
                destinationSprite = animationTo3;
                break;
        }

        if (inverse)
        {
            currentFrame = (destinationID == gameObject.GetComponent<SendID>().id) ? lighterID - 10 : destinationSprite.Length - 1;
            int limitID = (destinationID == gameObject.GetComponent<SendID>().id) ? 0 : lighterID - 6;
            while (currentFrame > limitID)
            {
                if (currentFrame == lighterID && disabledTime <= 0f)
                {
                    disabledTime = 0.5f;
                }
                else if (lighterID <= limitID && disabledTime <= 0f && currentFrame == limitID + 1)
                {
                    disabledTime = 0.5f;
                }

                gameObject.GetComponent<SpriteRenderer>().sprite = destinationSprite[currentFrame];
                yield return new WaitForSeconds(0.015F);
                currentFrame--;
            }
            gameObject.GetComponent<SpriteRenderer>().sprite = destinationSprite[0];
        }
        else
        {
            currentFrame = (destinationID == gameObject.GetComponent<SendID>().id) ? 0 : lighterID;
            int limitID = (destinationID == gameObject.GetComponent<SendID>().id) ? lighterID : destinationSprite.Length;
            while (currentFrame < limitID)
            {
                if (currentFrame == lighterID && disabledTime <= 0f)
                {
                    disabledTime = 0.5f;
                }
                else if (lighterID >= limitID && disabledTime <= 0f && currentFrame == limitID - 1)
                {
                    disabledTime = 0.5f;
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = destinationSprite[currentFrame];
                yield return new WaitForSeconds(0.015F);
                currentFrame++;
            }
            gameObject.GetComponent<SpriteRenderer>().sprite = destinationSprite[0];
        }
    }

    private Sprite LoadSprite(string relativePath)
    {
        var sprite = Resources.Load("Genius/" + relativePath, typeof(Sprite)) as Sprite;
        return sprite;
    }

    private Object[] LoadAllSprites(string relativeDir)
    {
        var objs = Resources.LoadAll("Genius/" + relativeDir, typeof(Sprite));
        return objs;
    }

    private Sprite[] LoadSpriteGroup(int dID)
    {
        Sprite[] result = new Sprite[0];
        if (dID == gameObject.GetComponent<SendID>().id)
        {
            switch (gameObject.name)
            {
                case "Blue":
                    result = new Sprite[36];
                    for (int i = 0; i < 36; i++)
                    {
                        if (i < 9)
                        {
                            result[i] = LoadSprite(gameObject.name + "/toRed/0" + (i + 1));
                        }
                        else
                        {
                            result[i] = LoadSprite(gameObject.name + "/toRed/" + (i + 1));
                        }
                    }
                    break;
                case "Green":
                    result = new Sprite[36];
                    for (int i = 0; i < 36; i++)
                    {
                        if (i < 9)
                        {
                            result[i] = LoadSprite(gameObject.name + "/toRed/0" + (i + 1));
                        }
                        else
                        {
                            result[i] = LoadSprite(gameObject.name + "/toRed/" + (i + 1));
                        }
                    }
                    break;
                case "Red":
                    result = new Sprite[32];
                    for (int i = 0; i < 32; i++)
                    {
                        if (i < 9)
                        {
                            result[i] = LoadSprite(gameObject.name + "/toYellow/0" + (i + 1));
                        }
                        else
                        {
                            result[i] = LoadSprite(gameObject.name + "/toYellow/" + (i + 1));
                        }
                    }
                    break;
                case "Yellow":
                    result = new Sprite[60];
                    for (int i = 0; i < 60; i++)
                    {
                        if (i < 9)
                        {
                            result[i] = LoadSprite(gameObject.name + "/toRed/0" + (i + 1));
                        }
                        else
                        {
                            result[i] = LoadSprite(gameObject.name + "/toRed/" + (i + 1));
                        }
                    }
                    break;
            }
        }
        else
        {
            Object[] loadedSprites = new Object[0];
            switch (dID)
            {
                case 0:
                    loadedSprites = LoadAllSprites(gameObject.name + "/toBlue/");
                    break;
                case 1:
                    loadedSprites = LoadAllSprites(gameObject.name + "/toGreen/");
                    break;
                case 2:
                    loadedSprites = LoadAllSprites(gameObject.name + "/toRed/");
                    break;
                case 3:
                    loadedSprites = LoadAllSprites(gameObject.name + "/toYellow/");
                    break;
            }
            result = new Sprite[loadedSprites.Length];
            for (int x = 0; x < loadedSprites.Length; x++)
            {
                result[x] = (Sprite)loadedSprites[x];
            }
        }
        return result;
    }
}
