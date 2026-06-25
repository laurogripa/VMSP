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
            Vector3 spawnPosition = transform.TransformPoint(gameObject.GetComponent<LineRenderer>().GetPosition(0));
            neurotransmissor = Instantiate(effect, spawnPosition, Quaternion.identity);
            neurotransmissor.transform.SetParent(transform);
            neurotransmissor.GetComponent<EffectManager>().buttonID = buttonID;
        }
        timeCounter += Time.deltaTime;
    }
}
