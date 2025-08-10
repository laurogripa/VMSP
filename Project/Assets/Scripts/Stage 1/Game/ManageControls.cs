using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageControls : MonoBehaviour
{
    public bool allowDraw;
    public bool showCanvas;
    private float time;
    void Start()
    {
        allowDraw = false;
        showCanvas = true;
    }
    private void FixedUpdate()
    {
        if(showCanvas)
        {
            time += Time.deltaTime;
        }
        if (time >= 0.1f)
        {
            showCanvas = false;
            time = 0;
        }
    }
    private void OnMouseEnter()
    {
        allowDraw = true;
    }

    private void OnMouseExit()
    {
        allowDraw = false;
    }
}
