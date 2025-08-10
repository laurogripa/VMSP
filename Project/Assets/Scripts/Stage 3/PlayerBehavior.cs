using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject player;
    private float speed, spentTime, hitAnimationTime, timeToBlink;
    private bool gameOver, counterStarted;
    private int timeTo;
    public bool waiting, hitAnimation, blink;
    [SerializeField]
    private GameObject enemyManager, shieldUI;
    private Vector3 originalSize;
    

    void Start()
    {
        enemyManager = GameObject.Find("Enemies");
        shieldUI = GameObject.Find("Shield");
        speed = 1f;
        timeTo = 3;
        blink = true;
        originalSize = new Vector3(0.5f, 0.5f, 0.5f);
        if (enemyManager.GetComponent<EnemyManager>().onShield)
        {
            gameObject.transform.localScale = gameObject.transform.localScale * 1.25f;
        }
    }

    void FixedUpdate()
    {
        if(!BackAction.onPause)
        {
            PlayerMovement();
            if (counterStarted)
            {
                spentTime += Time.deltaTime;
                if (spentTime >= 1)
                {
                    timeTo -= 1;
                    enemyManager.GetComponent<EnemyManager>().counter.text = timeTo.ToString();
                    spentTime = 0;
                }
                if (timeTo == 0)
                {
                    counterStarted = false;
                    waiting = false;
                    timeTo = 3;
                    enemyManager.GetComponent<EnemyManager>().counterObject.SetActive(false);
                    enemyManager.GetComponent<EnemyManager>().IncreaseLevel();
                    GameObject P = Instantiate(Resources.Load("Stage 3/Player") as GameObject);
                    if (GameObject.Find("GameElements") == null)
                    {
                        P.transform.SetParent(GameObject.Find("GameElements(Clone)").transform);
                    }
                    else
                    {
                        P.transform.SetParent(GameObject.Find("GameElements").transform);
                    }
                    Destroy(gameObject);
                }
            }
            if(hitAnimation)
            {
                timeToBlink += Time.deltaTime;
                hitAnimationTime += Time.deltaTime;
                if(timeToBlink >= 0.25)
                {
                    blink = !blink;
                    if (blink)
                    {
                        shieldUI.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    }
                    else
                    {
                        shieldUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
                        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
                    }
                    timeToBlink = 0;
                }

                if(hitAnimationTime > 5)
                {
                    hitAnimation = false;
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
                    shieldUI.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    blink = true;
                }
            }
        }
    }

    private void PlayerMovement()
    {
        if(!gameOver && !waiting)
        {
            Vector2 mousePos;
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    mousePos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                    Vector2 distance = new Vector2(transform.position.x - mousePos.x, transform.position.y - mousePos.y);
                    distance.x = distance.x < 0 ? distance.x * -1 : distance.x;
                    distance.y = distance.y < 0 ? distance.y * -1 : distance.y;
                    if (distance.x > 0.1f || distance.y > 0.1f)
                    {
                        Vector3 target = new Vector3(mousePos.x, mousePos.y, transform.position.z);
                        Vector3 normalizedDirection = (target - transform.position).normalized;
                        transform.position += normalizedDirection * speed * Time.deltaTime;
                    }
                    float AngleRad = Mathf.Atan2(transform.position.y - mousePos.y, transform.position.x - mousePos.x);
                    float AngleDeg = Mathf.Rad2Deg * AngleRad;
                    this.transform.rotation = Quaternion.AngleAxis(AngleDeg, Vector3.forward);
                }
            }
            else
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 distance = new Vector2(transform.position.x - mousePos.x, transform.position.y - mousePos.y);
                distance.x = distance.x < 0 ? distance.x * -1 : distance.x;
                distance.y = distance.y < 0 ? distance.y * -1 : distance.y;
                if (distance.x > 0.1f || distance.y > 0.1f)
                {
                    Vector3 target = new Vector3(mousePos.x, mousePos.y, transform.position.z);
                    Vector3 normalizedDirection = (target - transform.position).normalized;
                    transform.position += normalizedDirection * speed * Time.deltaTime;
                }
                float AngleRad = Mathf.Atan2(transform.position.y - mousePos.y, transform.position.x - mousePos.x);
                float AngleDeg = Mathf.Rad2Deg * AngleRad;
                this.transform.rotation = Quaternion.AngleAxis(AngleDeg, Vector3.forward);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Enemy") && !counterStarted && enemyManager.GetComponent<EnemyManager>().onShield)
        {
            TurnShieldOff();
        }
        else if(collision.tag.Equals("Enemy") && !counterStarted && !hitAnimation)
        {
            gameOver = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<PolygonCollider2D>().enabled = false;
            GetComponentInChildren<ParticleSystem>().Play();
            Invoke("restart", 2f);
        }
        else if (collision.tag.Equals("WinWall") && !hitAnimation)
        {
            waiting = true;
            if(enemyManager.GetComponent<EnemyManager>().level < 10)
            {
                counterStarted = true;
                enemyManager.GetComponent<EnemyManager>().counterObject.SetActive(true);
                enemyManager.GetComponent<EnemyManager>().counter.text = "3";
            }
            else
            {
                Camera.current.gameObject.GetComponent<StageManager>().SetWin();
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }

    private void restart()
    {
        enemyManager.GetComponent<EnemyManager>().ChangeLife(-1);
        if (enemyManager.GetComponent<EnemyManager>().lifes > 0)
        {
            GameObject P = Instantiate(Resources.Load("Stage 3/Player") as GameObject);
            if(GameObject.Find("GameElements") == null)
            {
                P.transform.SetParent(GameObject.Find("GameElements(Clone)").transform);
            }
            else
            {
                P.transform.SetParent(GameObject.Find("GameElements").transform);
            }
            Destroy(gameObject);
        }
        else
        {
            Camera.current.gameObject.GetComponent<StageManager>().SetGameOver();
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    public void ManageBuff(int id)
    {
        switch(id)
        {
            case 0:
                enemyManager.GetComponent<EnemyManager>().ChangeLife(1);
                break;
            case 1:
                TurnShieldOn();
                break;
        }
    }

    private void TurnShieldOn()
    {
        enemyManager.GetComponent<EnemyManager>().onShield = true;
        gameObject.transform.localScale = gameObject.transform.localScale * 1.25f;
        shieldUI.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
    private void TurnShieldOff()
    {
        enemyManager.GetComponent<EnemyManager>().onShield = false;
        hitAnimation = true;
        gameObject.transform.localScale = originalSize;
    }
}
