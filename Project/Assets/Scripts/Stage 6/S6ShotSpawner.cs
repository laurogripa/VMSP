using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S6ShotSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject shot, reference;
    [SerializeField]
    private int id;

    public void Shoot()
    {
        if (id != 2)
        {
            Instantiate(shot, reference.transform.position, transform.parent.rotation).transform.SetParent(GameObject.Find("Shots").transform);
        }
        else
        {
            Instantiate(shot, reference.transform.position, transform.rotation).transform.SetParent(GameObject.Find("Shots").transform);
        }
    }
}
