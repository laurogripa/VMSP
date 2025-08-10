using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    

    private bool gameOver = false;
    private float cpt;
    private int scr;

    Rigidbody2D rb;
    Camera cam;
    [SerializeField] Text score;

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        if (!gameOver)
        {
            setScore();
            if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    if (touch.position.x < Screen.width / 2)
                    {
                        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
                    }
                    else if (touch.position.x > Screen.width / 2)
                    {
                        transform.Rotate(Vector3.forward * (-rotationSpeed) * Time.deltaTime);
                    }
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Rotate(Vector3.forward * (-rotationSpeed) * Time.deltaTime);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
                }
            }
        }        
    }

    void FixedUpdate()
    {
        if (!gameOver)
        {
            rb.AddRelativeForce(new Vector3(moveSpeed * Time.fixedDeltaTime, 0f, 0f));
        }
    }

    void LateUpdate()
    {
        if (!gameOver)
        {
            cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, cam.transform.position.z);
        }
    }

    void OnCollisionEnter2D()
    {
        if(!gameOver)
        {
            gameOver = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<PolygonCollider2D>().enabled = false;
            GetComponentInChildren<ParticleSystem>().Play();
            Invoke("restart", 2f);
        }
    }

    void restart()
    {
        SceneManager.LoadScene("Stage 5");
    }

    void setScore()
    {
        cpt += Time.deltaTime;
        if(cpt > 0.5f)
        {
            cpt = 0;
            scr++;
            score.text = scr.ToString();
        }
    }
}