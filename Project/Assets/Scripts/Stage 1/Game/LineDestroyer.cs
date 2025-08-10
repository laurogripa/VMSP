using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDestroyer : MonoBehaviour
{
    private LineCreator lineCreator;

    private void Start()
    {
        lineCreator = GameObject.Find("Manager").GetComponent<LineCreator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameObject.Find("Line") != null && collision.gameObject.tag.Equals("Line") && !lineCreator.waiting)
        {
            lineCreator.destroyLine = true;
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (GameObject.Find("Line") != null && collision.gameObject.tag.Equals("Line") && !lineCreator.waiting)
        {
            lineCreator.destroyLine = true;
        }
    }
}
