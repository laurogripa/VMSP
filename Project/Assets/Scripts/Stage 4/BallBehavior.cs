using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PolygonCollider2D col;
    [HideInInspector] public Vector3 pos { get { return transform.position; } }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
    }
    public void Push(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    public void ActivateRb()
    {
        rb.isKinematic = false;
    }
    public void DesactivateRb()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }
}
