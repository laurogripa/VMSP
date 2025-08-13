using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;


public class SendID : MonoBehaviour
{
    public int id;
    private bool over;
    [SerializeField] private Sprite lighter, darker;
    [SerializeField] private SequenceManager manager;
    
    private bool wasPresent = true;
    private bool hasFocus = false;
    private float blinkTimer = 0f;

    private void Update()
    {
        // Check if this object is being focused by gaze
        GameObject focusedObject = TobiiAPI.GetFocusedObject();
        hasFocus = (focusedObject == gameObject);
        
        // Handle desktop/PC platform with eye tracking
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            // Get user presence to detect blinks
            UserPresence userPresence = TobiiAPI.GetUserPresence();
            bool isPresent = userPresence.IsPresent;
            
            // Detect blink: when user goes from present to not present, then back to present
            if (!wasPresent && isPresent && hasFocus && manager.isClickable && blinkTimer < 0.5f)
            {
                // User just "came back" after a brief absence (blink) while looking at this object
                StartCoroutine(Wait());
                blinkTimer = 0f;
            }
            
            // Track absence duration
            if (!isPresent)
            {
                blinkTimer += Time.deltaTime;
            }
            else
            {
                blinkTimer = 0f;
            }
            
            wasPresent = isPresent;
        }
    }

    private void OnMouseUp()
    {
        if(manager.isClickable)
        {
            if (over)
            {
                StartCoroutine(Wait());
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = darker;
            }
        }
    }

    private void OnMouseDown()
    {
        if (manager.isClickable)
        {
            if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = lighter;
            }
            else if(over)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = lighter;
            }
        }
    }

    private void OnMouseEnter()
    {
        over = true;
    }

    private void OnMouseExit()
    {
        over = false;
    }

    private void ClickAction()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = darker;
        manager.globalSequence.Add(id);
    }

    IEnumerator Wait()
    {
        manager.isClickable = false;
        yield return new WaitForSeconds(0.5F);
        manager.isClickable = true;
        gameObject.GetComponent<SequenceAnimation>().audios.Play();
        ClickAction();
    }
}
