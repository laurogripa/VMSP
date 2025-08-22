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
    private GameObject gameOverScreen, winScreen, gameElements, gameScreen;
    [SerializeField]
    private GameObject[] lifeUIs;

    public void SetGameOver()
    {
        BackAction.onGameOver = true;
        if (gameScreen != null) gameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }
    public void SetWin()
    {
        BackAction.onGameOver = true;
        if (gameScreen != null) gameScreen.SetActive(false);
        winScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        BackAction.onGameOver = false;
        SceneManager.LoadScene("Main Menu");
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Defense")
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
            if (gameScreen != null) gameScreen.SetActive(false);
            var player = GameObject.Find("Player") ?? GameObject.Find("Player(Clone)");
            if (player != null)
            {
                player.GetComponent<PlayerBehavior>().waiting = false;
            }
        }
    }



    public void PlayAgain()
    {
        BackAction.onGameOver = false;
        if (gameScreen != null) gameScreen.SetActive(true);
        for (int i = 0; i < lifeUIs.Length; i++)
        {
            lifeUIs[i].SetActive(true);
        }
        Instantiate(gameElements);
        if (winScreen.activeSelf)
        {
            winScreen.SetActive(false);
        }
        else if (gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(false);
        }
    }
}
