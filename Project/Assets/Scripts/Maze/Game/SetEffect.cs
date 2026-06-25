using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEffect : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    public int buttonID;
    private GameObject neurotransmissor;
    private float timeCounter;

    private void Start()
    {
        timeCounter = 0.4f;   
    }

    private void FixedUpdate()
    {
        if(timeCounter >= 0.4f)
        {
            timeCounter = 0f;
            LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
            neurotransmissor = Instantiate(effect);
            neurotransmissor.transform.SetParent(transform, false);
            neurotransmissor.transform.localPosition = lineRenderer.GetPosition(0);
            neurotransmissor.transform.localRotation = Quaternion.identity;
            neurotransmissor.GetComponent<EffectManager>().buttonID = buttonID;
        }
        timeCounter += Time.deltaTime;
    }
}
