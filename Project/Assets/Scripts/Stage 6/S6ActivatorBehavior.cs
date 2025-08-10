using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S6ActivatorBehavior : MonoBehaviour
{
    private float velocity;
    [SerializeField]
    private int id;
    [SerializeField]
    private bool isBuff;
    private bool enableAutoDestruction;
    private float lifeTime;

    void Start()
    {
        velocity = 0.5f;
    }
    void FixedUpdate()
    {
        transform.position += transform.up * Time.deltaTime * velocity;
        if (lifeTime >= 0.5f)
        {
            Destroy(gameObject);
        }
        else if (enableAutoDestruction)
        {
            lifeTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag.Equals("Player"))
        {
            if(isBuff)
            {
                GameObject.Find("Player").GetComponent<S6PlayerManager>().ManageBuff(id);
            }
            else
            {
                GameObject.Find("Player").GetComponent<S6PlayerManager>().SetSelectedShot(id);
            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Bottom":
                enableAutoDestruction = true;
                break;
        }
    }
}
