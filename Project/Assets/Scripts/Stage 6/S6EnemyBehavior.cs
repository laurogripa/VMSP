using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S6EnemyBehavior : MonoBehaviour
{
    private float velocity;
    private bool enableAutoDestruction, active;
    private float lifeTime, timeToShoot;
    [SerializeField]
    private GameObject enemyShot;
    void Start()
    {
        velocity = 0.5f;
    }

    // Update is called once per frame
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
        if(gameObject.tag.Equals("ActiveEnemy") && active && !enableAutoDestruction)
        {
            EnableSpawner();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Player":
                collider.gameObject.GetComponent<S6PlayerManager>().SetDamage(0);
                break;
            case "Top":
                active = true;
                break;
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

    private void EnableSpawner()
    {
        if (timeToShoot < 1f)
        {
            timeToShoot += Time.deltaTime;
        }
        else
        {
            timeToShoot = 0;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Instantiate(enemyShot, gameObject.transform.GetChild(i).position, gameObject.transform.GetChild(i).rotation).transform.SetParent(GameObject.Find("Shots").transform);
            }
        }
    }
}
