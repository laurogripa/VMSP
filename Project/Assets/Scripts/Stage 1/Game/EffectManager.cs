using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3[] linePositions;
    private int currentVector;
    public int buttonID;

    void Start()
    {
        currentVector = 0;
        lineRenderer = gameObject.transform.parent.gameObject.GetComponent<LineRenderer>();
        linePositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePositions);
    }

    void FixedUpdate()
    {
        if(!BackAction.onPause)
        {
            transform.position = Vector3.MoveTowards(transform.position, linePositions[currentVector], 0.05f);
            if (Vector3.Distance(transform.position, linePositions[currentVector]) < 0.02f && currentVector < linePositions.Length)
            {
                currentVector++;
            }
            if (currentVector >= linePositions.Length)
            {
                if (!GameObject.Find("Manager").GetComponent<NextController>().checkWin[buttonID])
                {
                    GameObject.Find("Manager").GetComponent<NextController>().checkWin[buttonID] = true;
                    GameObject.Find("Manager").GetComponent<NextController>().CheckButtons();
                }
                Destroy(gameObject);
            }
        }
    }
}
