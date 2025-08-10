using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3BulletBehavior : MonoBehaviour
{
    public int id;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
