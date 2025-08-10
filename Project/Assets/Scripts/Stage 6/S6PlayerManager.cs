using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S6PlayerManager : MonoBehaviour
{
    private FixedJoystick joystickScript;
    [SerializeField]
    private int selectedShot, buffDuration, buffedID;
    private bool onShooting, buffOn;
    private float moveH, moveV, speed, timeToDebuff;
    private SpriteRenderer playerRenderer;
    private Rigidbody2D rb;
    [SerializeField]
    private GameObject[] creators, shotSpawners;
    private GameObject[] lastSpawners;
    private float waitNextShot;

    private void Awake()
    {
        joystickScript = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
    }
    void Start()
    {
        playerRenderer = gameObject.GetComponent<SpriteRenderer>();
        speed = 1.5f;
        SetPlayerShot(selectedShot);
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMovement();
        CreatorsBehavior();
        if (waitNextShot < 0.25f)
        {
            waitNextShot += Time.deltaTime;
        }
        else if (onShooting)
        {
            waitNextShot = 0;
            for(int i = 0; i < shotSpawners.Length; i++)
            {
                shotSpawners[i].GetComponent<S6ShotSpawner>().Shoot();
            }
        }
        if(timeToDebuff >= buffDuration)
        {
            timeToDebuff = 0f;
            buffOn = false;
            shotSpawners = lastSpawners;
            creators[buffedID].SetActive(false);
        }
        else if(buffOn)
        {
            timeToDebuff += Time.deltaTime;
        }
    }

    private void CreatorsBehavior()
    {
        if(creators[1].activeSelf)
        { 
            creators[1].transform.Rotate(0, 0, 2f);    
        }
    }

    public void isShootButtonDown()
    {
        onShooting = true;
    }

    public void isShootButtonUp()
    {
        onShooting = false;
    }

    private void PlayerMovement()
    {
        moveH = joystickScript.Horizontal;
        moveV = joystickScript.Vertical;
        rb.linearVelocity = new Vector3(moveH * speed, moveV * speed, rb.linearVelocity.y);
    }

    private void SetPlayerShot(int chosedShot)
    {
        for (int i = 0; i < creators.Length; i++)
        {
            creators[i].SetActive(false);
        }
        switch (chosedShot)
        {
            case 0:
                playerRenderer.color = new Color(0.08334541f, 1f, 0f);
                creators[chosedShot].SetActive(true);
                shotSpawners = new GameObject[1];
                shotSpawners[0] = creators[selectedShot].transform.GetChild(0).gameObject;
                break;
            case 1:
                playerRenderer.color = new Color(0.9905428f, 1f, 0f);
                creators[chosedShot].SetActive(true);
                shotSpawners = new GameObject[1];
                shotSpawners[0] = creators[selectedShot].transform.GetChild(0).gameObject;
                break;
            case 2:
                playerRenderer.color = new Color(0f, 0.8143575f, 1f);
                creators[chosedShot].SetActive(true);
                shotSpawners = new GameObject[creators[selectedShot].transform.childCount];
                for(int i = 0; i < creators[selectedShot].transform.childCount; i++)
                {
                    shotSpawners[i] = creators[selectedShot].transform.GetChild(i).gameObject;
                }
                break;
        }
    }

    public void SetDamage(int type)
    {
        switch(type)
        {
            case 0:
                Debug.Log("Game Over");
                break;
        }
    }

    public void SetSelectedShot(int shotID)
    {
        selectedShot = shotID;
        SetPlayerShot(selectedShot);
    }

    public void ManageBuff(int buffID)
    {
        if (buffID != selectedShot)
        {
            if (buffOn)
            {
                shotSpawners = lastSpawners;
            
                    creators[buffedID].SetActive(false);
            
            }
            lastSpawners = shotSpawners;
            creators[buffID].SetActive(true);
            shotSpawners = new GameObject[creators[buffID].transform.childCount + shotSpawners.Length];
            for (int i = 0; i < shotSpawners.Length; i++)
            {
                if(i < lastSpawners.Length)
                {
                    shotSpawners[i] = lastSpawners[i];
                }
                else
                {
                    shotSpawners[i] = creators[buffID].transform.GetChild(i - lastSpawners.Length).gameObject;
                }
            }
            timeToDebuff = 0f;
            buffOn = true;
            buffedID = buffID;
        }
    }
}
