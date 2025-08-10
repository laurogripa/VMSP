using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Tobii.Gaming;


public class SendID : MonoBehaviour
{
    public int id;
    private bool over;
    [SerializeField] private Sprite lighter, darker;
    [SerializeField] private SequenceManager manager;

    private void Update()
    {
        //GameObject focusedObject = TobiiAPI.GetFocusedObject();
        //Debug.Log(focusedObject.name);
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
