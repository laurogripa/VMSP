using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] substages;
    [SerializeField]
    private GameObject[] lifesUI;
    [SerializeField]
    public Text counter;
    [SerializeField]
    public GameObject counterObject;
    List<GameObject> activeGameObjects;
    public int level;
    public int lifes;
    public bool onShield;

    void Start()
    {
        lifesUI = new GameObject[5];
        for(int i = 0; i < lifesUI.Length; i++)
        {
            lifesUI[i] = GameObject.Find("Life_" + i);
        }
        counter = Camera.main.gameObject.GetComponent<StageManager>().counterText;
        counterObject = Camera.main.gameObject.GetComponent<StageManager>().counterObject;
        activeGameObjects = new List<GameObject>();
        GameObject.Find("Player").GetComponent<PlayerBehavior>().waiting = true;
        lifes = 5;
        level = -1;
    }

    public void IncreaseLevel()
    {
        foreach(GameObject e in activeGameObjects)
        {
            Destroy(e);
        }

        level += 1;
        if(lifes < 5)
        {
            ChangeLife(1);
        }
        GameObject SubStage = Instantiate(substages[level]);
        activeGameObjects.Add(SubStage);
        if (GameObject.Find("GameElements") == null)
        {
            SubStage.transform.SetParent(GameObject.Find("GameElements(Clone)").transform);
        }
        else
        {
            SubStage.transform.SetParent(GameObject.Find("GameElements").transform);
        }
    }

    public void ChangeLife(int number)
    {
        if(level > 0)
        {
            lifes += number;
        }
        switch (lifes)
        {
            case 0:
                for (int i = 0; i < 5; i++)
                {
                    lifesUI[i].SetActive(false);
                }
                break;
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    if (i > 0)
                    {
                        lifesUI[i].SetActive(false);
                    }
                    else
                    {
                        lifesUI[i].SetActive(true);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < 5; i++)
                {
                    if (i > 1)
                    {
                        lifesUI[i].SetActive(false);
                    }
                    else
                    {
                        lifesUI[i].SetActive(true);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < 5; i++)
                {
                    if (i > 2)
                    {
                        lifesUI[i].SetActive(false);
                    }
                    else
                    {
                        lifesUI[i].SetActive(true);
                    }
                }
                break;
            case 4:
                for (int i = 0; i < 5; i++)
                {
                    if (i > 3)
                    {
                        lifesUI[i].SetActive(false);
                    }
                    else
                    {
                        lifesUI[i].SetActive(true);
                    }
                }
                break;
            case 5:
                for (int i = 0; i < 5; i++)
                {
                    lifesUI[i].SetActive(true);
                }
                break;
        }
    }
}
