using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapManager : MonoBehaviour
{
    private LineCreator lineManager;
    private bool pressStatus;

    void Start()
    {
        pressStatus = false;
        lineManager = GameObject.Find("Manager").GetComponent<LineCreator>();
    }

    private void Update()
    {
        if(!pressStatus && Input.GetMouseButtonDown(0))
        {
            pressStatus = true;
        }
        else if(pressStatus && Input.GetMouseButtonUp(0))
        {
            pressStatus = false;
        }
        if (!pressStatus && GameObject.Find("Line") != null && !lineManager.waiting)
        {
            Destroy(GameObject.Find("Line"));
        }
    }
}
