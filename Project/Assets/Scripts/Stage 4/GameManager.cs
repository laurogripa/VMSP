using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton class: GameManager
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    #endregion

    private Camera cam;
    public BallBehavior ballBehavior;
    public Trajectory trajectory;

    [SerializeField] float pushForce = 4f;
    private float distance;
    bool isDragging = false;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector2 direction;
    private Vector2 force;
    

    void Start()
    {
        cam = Camera.main;
        ballBehavior.DesactivateRb();
    }

    void Update()
    {
        if(!BackAction.onPause)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount >= 0)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        isDragging = true;
                        OnDragStart();
                    }
                    if (Input.touches[0].phase == TouchPhase.Ended)
                    {
                        isDragging = false;
                        OnDragEnd();
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDragging = true;
                    OnDragStart();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;
                    OnDragEnd();
                }
            }

            if (isDragging)
            {
                OnDrag();
            }
        }
        
    }

    void OnDragStart()
    {
        ballBehavior.DesactivateRb();
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                startPoint = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            }
        }
        else
        {
            startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        trajectory.Show();
    }

    void OnDrag()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                endPoint = cam.ScreenToWorldPoint(Input.touches[0].position);
            }
        }
        else
        {
            endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        } 
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        force = direction * distance * pushForce;
        Debug.DrawLine(startPoint, endPoint);
        trajectory.UpdateDots(ballBehavior.pos, force);
    }

    void OnDragEnd()
    {
        ballBehavior.ActivateRb();
        ballBehavior.Push(force);
        trajectory.Hide();
    }
}
