using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3PowerUpBehavior : MonoBehaviour
{
    [SerializeField] 
    private int id;
    

    void Start()
    {
        switch (id)
        {
            case 0:
                gameObject.name = "Power Up #1";
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                break;

            case 1:
                gameObject.name = "Power Up #2";
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag.Equals("Player"))
        {
            PlayerBehavior PlayerScript = collider.gameObject.GetComponent<PlayerBehavior>();
            PlayerScript.ManageBuff(id);
            Destroy(gameObject);
        }
    }
}
