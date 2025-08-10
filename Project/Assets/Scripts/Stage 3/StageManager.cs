using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public GameObject counterObject;
    public Text counterText;
    [SerializeField]
    private GameObject gameOverScreen, winScreen, gameElements;
    [SerializeField]
    private GameObject[] gameScreens, lifeUIs;
  
    public void SetGameOver()
    {
        BackAction.onGameOver = true;
        for (int i = 0; i < gameScreens.Length; i++)
        {
            gameScreens[i].SetActive(false);
        }
        gameOverScreen.SetActive(true);
    }
    public void SetWin()
    {
        BackAction.onGameOver = true;
        for (int i = 0; i < gameScreens.Length; i++)
        {
            gameScreens[i].SetActive(false);
        }
        winScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        BackAction.onGameOver = false;
        SceneManager.LoadScene("Stage 0");
    }

    public void PlayIntro()
    {
        EnemyManager eM;
        if (GameObject.Find("Enemies") == null)
        {
            eM = GameObject.Find("Enemies(Clone)").GetComponent<EnemyManager>();
        }
        else
        {
            eM = GameObject.Find("Enemies").GetComponent<EnemyManager>();
        }
        eM.IncreaseLevel();
        gameScreens[0].SetActive(false);
        GameObject.Find("Player").GetComponent<PlayerBehavior>().waiting = false;
    }

    public void NoIntro()
    {
        EnemyManager eM;
        if (GameObject.Find("Enemies") == null)
        {
            eM = GameObject.Find("Enemies(Clone)").GetComponent<EnemyManager>();
        }
        else
        {
            eM = GameObject.Find("Enemies").GetComponent<EnemyManager>();
        }
        eM.level = 0;
        eM.IncreaseLevel();
        gameScreens[0].SetActive(false);
        if(GameObject.Find("Player") == null)
        {
            GameObject.Find("Player(Clone)").GetComponent<PlayerBehavior>().waiting = false;
        }
        else
        {
            GameObject.Find("Player").GetComponent<PlayerBehavior>().waiting = false;
        }
    }

    public void PlayAgain()
    {
        BackAction.onGameOver = false;
        for (int i = 0; i < gameScreens.Length; i++)
        {
            gameScreens[i].SetActive(true);
        }
        for (int i = 0; i < lifeUIs.Length; i++)
        {
            lifeUIs[i].SetActive(true);
        }
        Instantiate(gameElements);
        if (winScreen.activeSelf)
        {
            winScreen.SetActive(false);
        }
        else if(gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(false);
        }
    }
}
