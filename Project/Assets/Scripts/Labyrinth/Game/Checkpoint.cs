using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private LineCreator lineCreator;
    private int id;
    private void Start()
    {
        id = (int.Parse((gameObject.name[7].ToString())) - 1);
        lineCreator = GameObject.Find("Manager").GetComponent<LineCreator>();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameObject.Find("Line") != null && collision.gameObject.tag.Equals("Line"))
        {
            GameObject.Find("Line").GetComponent<Rigidbody2D>().isKinematic = true;
            GameObject.Find("Line").GetComponent<EdgeCollider2D>().enabled = false;
            lineCreator.SuccessLine(id);
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (GameObject.Find("Line") != null && collision.gameObject.tag.Equals("Line"))
        {
            GameObject.Find("Line").GetComponent<Rigidbody2D>().isKinematic = true;
            GameObject.Find("Line").GetComponent<EdgeCollider2D>().enabled = false;
            lineCreator.SuccessLine(id);
        }
    }
}
