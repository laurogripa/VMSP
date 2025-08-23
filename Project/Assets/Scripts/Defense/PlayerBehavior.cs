using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject player;
    private float speed, spentTime, hitAnimationTime, timeToBlink;
    private float invincibilityDuration = 5f;
    private bool gameOver, counterStarted;
    private int timeTo;
    public bool waiting, hitAnimation, blink;
    [SerializeField]
    private GameObject enemyManager, shieldUI;
    private Vector3 originalSize;
    public GameObject gazeIndicator;


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

        if (gazeIndicator == null)
        {
            gazeIndicator = new GameObject("GazeIndicator");
            var renderer = gazeIndicator.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateCircleSprite();
            renderer.color = Color.red;
            gazeIndicator.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        }
    }

    void FixedUpdate()
    {
        if (!BackAction.onPause)
        {
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
                    GameObject P = Instantiate(Resources.Load("Defense/Player") as GameObject);
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
            if (hitAnimation)
            {
                timeToBlink += Time.deltaTime;
                hitAnimationTime += Time.deltaTime;
                if (timeToBlink >= 0.25)
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

                if (hitAnimationTime > invincibilityDuration)
                {
                    hitAnimation = false;
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    shieldUI.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    blink = true;
                }
            }
        }
    }

    private Sprite CreateCircleSprite()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color transparent = new Color(0, 0, 0, 0);
        Color red = Color.red;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (distanceToCenter <= 31)
                {
                    texture.SetPixel(x, y, red);
                }
                else
                {
                    texture.SetPixel(x, y, transparent);
                }
            }
        }
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy") && !counterStarted && enemyManager.GetComponent<EnemyManager>().onShield)
        {
            TurnShieldOff();
        }
        else if (collision.tag.Equals("Enemy") && !counterStarted)
        {
            var chaser = collision.GetComponent<ChaserEnemy>();
            if (chaser != null)
            {
                chaser.Explode(false);
            }
            if (!hitAnimation)
            {
                gameOver = true;
                gazeIndicator.SetActive(false);
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<PolygonCollider2D>().enabled = false;
                GetComponentInChildren<ParticleSystem>().Play();

                var em = enemyManager.GetComponent<EnemyManager>();
                em.ChangeLife(-1);
                if (em.lives > 0)
                {
                    Invoke("restart", 2f);
                }
                else
                {
                    Invoke("HandleGameOver", 2f);
                }
            }
        }
        else if (collision.tag.Equals("WinWall") && !hitAnimation)
        {
            waiting = true;
            if (enemyManager.GetComponent<EnemyManager>().level < 10)
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
        GameObject P = Instantiate(Resources.Load("Defense/Player") as GameObject);
        if (GameObject.Find("GameElements") == null)
        {
            P.transform.SetParent(GameObject.Find("GameElements(Clone)").transform);
        }
        else
        {
            P.transform.SetParent(GameObject.Find("GameElements").transform);
        }
        P.transform.localPosition = Vector3.zero;
        var pb = P.GetComponent<PlayerBehavior>();
        if (pb != null)
        {
            pb.ActivateInvincibility(5f);
        }
        Destroy(gameObject);
    }

    private void HandleGameOver()
    {
        if (Camera.main != null)
        {
            Camera.main.gameObject.GetComponent<StageManager>().SetGameOver();
        }
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void ManageBuff(int id)
    {
        switch (id)
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

    public void ActivateInvincibility(float durationSeconds)
    {
        invincibilityDuration = durationSeconds;
        hitAnimation = true;
        hitAnimationTime = 0f;
        timeToBlink = 0f;
        if (shieldUI != null)
        {
            shieldUI.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
}
