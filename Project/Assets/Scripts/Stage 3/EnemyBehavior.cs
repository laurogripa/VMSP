using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float speed, limit, timeToShoot;
    private float counterToShoot;
    public int moveStage;
    private Vector2 initialPos;
    [SerializeField] GameObject bullet;
    [SerializeField] private List<GameObject> bullets;

    void Start()
    {
        bullets = new List<GameObject>();
        initialPos = transform.position;
    }

    void FixedUpdate()
    {
        if (!BackAction.onPause)
        {
            
            switch (gameObject.name)
            {
                case "Enemy-1":
                    Enemy1Behavior();
                    break;
                case "Enemy-2":
                    Enemy2Behavior();
                    break;
                case "Enemy-3":
                    Enemy3Behavior();
                    break;
                case "Enemy-4":
                    Enemy4Behavior();
                    break;
                case "Enemy-5":
                    Enemy5Behavior();
                    break;
                case "Enemy-6":
                    Enemy6Behavior();
                    break;
            }
            
        }
    }

    private void Enemy1Behavior()
    {
        Enemy1Movement();
    }

    private void Enemy1Movement()
    {
        if (transform.position.x >= initialPos.x + limit || transform.position.x <= initialPos.x - limit)
        {
            transform.Rotate(0f, 0f, 180f);
        }

        transform.Translate(-speed, 0f, 0f);
    }//Horizontal

    private void Enemy2Behavior()
    {
        Enemy2Movement();
    }

    private void Enemy2Movement()
    {
        transform.Rotate(0, 0, 360 / limit);
        transform.Translate(-speed, 0f, 0f);
    }//Orbit Around

    private void Enemy3Behavior()
    {
        Enemy3Movement();
    }

    private void Enemy3Movement()
    {
        switch (moveStage)
        {
            case 0:
                if (transform.position.x <= initialPos.x - limit)
                {
                    transform.Rotate(0f, 0f, 90f);
                    moveStage++;
                }
                break;
            case 1:
                if (transform.position.y <= initialPos.y - limit)
                {
                    transform.Rotate(0f, 0f, 90f);
                    moveStage++;
                }
                break;
            case 2:
                if (transform.position.x > initialPos.x)
                {
                    transform.Rotate(0f, 0f, 90f);
                    moveStage++;
                }
                break;
            case 3:
                if (transform.position.y >= initialPos.y)
                {
                    transform.Rotate(0f, 0f, 90f);
                    moveStage = 0;
                }
                break;
        }
        transform.Translate(-speed, 0, 0);
    }//Quad Movement

    private void Enemy4Behavior()
    {
        Enemy4Movement();
    }

    private void Enemy4Movement()
    {
        if (transform.position.y >= initialPos.y + limit || transform.position.y <= initialPos.y - limit)
        {
            transform.Rotate(0f, 0f, 180f);
        }
        transform.Translate(-speed, 0f, 0f);
    }//Vertical
    private void Enemy5Behavior()
    {
        Enemy5Movement();
    }
    private void Enemy5Movement()
    {
        switch (moveStage)
        {
            case 0:
                if (transform.position.x <= initialPos.x - limit)
                {
                    transform.Rotate(0f, 0f, 90f);
                    moveStage++;
                }
                break;
            case 1:
                if (transform.position.y <= initialPos.y - limit / 2)
                {
                    transform.Rotate(0f, 0f, 90f);
                    moveStage++;
                }
                break;
            case 2:
                if (transform.position.x >= initialPos.x + limit)
                {
                    transform.Rotate(0f, 0f, 90f);
                    moveStage++;
                }
                break;
            case 3:
                if (transform.position.y >= initialPos.y)
                {
                    transform.Rotate(0f, 0f, 90f);
                    moveStage = 0;
                }
                break;
        }
        transform.Translate(-speed, 0, 0);
    }

    private void Enemy6Behavior()
    {
        Enemy6Movement();
    }
    private void Enemy6Movement()
    {
        
        if(counterToShoot < timeToShoot)
        {
            counterToShoot += Time.deltaTime;
        }
        else
        {
            GameObject lastBullet = Instantiate(bullet, new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z), Quaternion.identity);
            bullets.Add(lastBullet);
            if (GameObject.Find("GameElements") == null)
            {
                lastBullet.transform.SetParent(GameObject.Find("GameElements(Clone)").transform);
            }
            else
            {
                lastBullet.transform.SetParent(GameObject.Find("GameElements").transform);
            }
            counterToShoot = 0;
        }
        for(int i = 0; i < bullets.Count; i++)
        {
            if(bullets[i] != null)
            {
                bullets[i].transform.Translate(-speed, 0, 0);
            }
            else 
            {
                bullets.Remove(bullets[i]);
            }
        }
    }
}
