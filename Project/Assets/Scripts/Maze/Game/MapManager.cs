using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapManager : MonoBehaviour
{
    private LineCreator lineManager;

    void Start()
    {
        lineManager = GameObject.Find("Manager").GetComponent<LineCreator>();
    }

    private void Update()
    {
        if (!lineManager.IsAutoDrawActive() && lineManager.currentLine != null && !lineManager.waiting)
        {
            Destroy(lineManager.currentLine);
        }
    }
}
