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
            neurotransmissor = Instantiate(effect, new Vector3(gameObject.GetComponent<LineRenderer>().GetPosition(0).x, gameObject.GetComponent<LineRenderer>().GetPosition(0).y, gameObject.GetComponent<LineRenderer>().GetPosition(0).z), Quaternion.identity);
            neurotransmissor.transform.SetParent(transform);
            neurotransmissor.GetComponent<EffectManager>().buttonID = buttonID;
        }
        timeCounter += Time.deltaTime;
    }
}
