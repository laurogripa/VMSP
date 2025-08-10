using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S6ShotBehavior : MonoBehaviour
{
    private float velocity;
    private bool enableAutoDestruction;
    private float lifeTime;
    void Start()
    {
        velocity = 5f;
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
        
        switch (collider.gameObject.tag)
        {
            case "Enemy":
                if(!enableAutoDestruction && !gameObject.tag.Equals("EnemyShot"))
                {
                    Destroy(collider.gameObject);
                    Destroy(gameObject);
                }
                break;
            case "Barrier":
                Destroy(gameObject);
                break;
            case "ActiveEnemy":
                Destroy(gameObject);
                break;
            case "Walls":
                enableAutoDestruction = true;
                break;
            case "Bottom":
                enableAutoDestruction = true;
                break;
            case "Top":
                enableAutoDestruction = true;
                break;
        }
    }
}