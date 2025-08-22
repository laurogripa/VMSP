using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S2LifeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lifesGO;
    private int lifes;
    private bool fadeIn, fadeOut, changeToGO, changeToWin, restartGame, goToMenu;
    private float alpha;
    [SerializeField]
    private GameObject fade, gameElements, gameOverScreen, winScreen;
    [SerializeField]
    private GameObject[] gameComponents;
    [SerializeField]
    private RawImage fadeImage;

    void Start()
    {
        fadeImage = GameObject.Find("Fade").GetComponent<RawImage>();
        lifes = 6;
    }

    private void FixedUpdate()
    {
        if (fadeIn)
        {
            fade.SetActive(true);
            if (alpha < 1f)
            {
                alpha += Time.deltaTime / 1.5f; //1.5 sec
                if (alpha > 1f)
                {
                    alpha = 1f;
                }
                fadeImage.color = new Color(0f, 0f, 0f, alpha);
            }
            else
            {
                fadeIn = false;
                if (changeToGO)
                {
                    BackAction.onGameOver = true;
                    Destroy(gameElements);
                    /*for (int i = 0; i < gameComponents.Length; i++)
                    {
                        gameComponents[i].SetActive(false);
                    }*/
                    gameOverScreen.SetActive(true);
                }
                else if (changeToWin)
                {
                    BackAction.onGameOver = true;
                    Destroy(gameElements);
                    for (int i = 0; i < gameComponents.Length; i++)
                    {
                        gameComponents[i].SetActive(false);
                    }
                    winScreen.SetActive(true);
                }
                else if (restartGame)
                {
                    BackAction.onGameOver = false;
                    SceneManager.LoadScene("Stage 1");
                }
                else if (goToMenu)
                {
                    BackAction.onGameOver = false;
                    SceneManager.LoadScene("Main Menu");
                }
                fadeOut = true;
            }
        }
        else if (fadeOut)
        {
            if (alpha > 0f)
            {
                alpha -= Time.deltaTime / 1.5f; //1.5 sec
                if (alpha < 0f)
                {
                    alpha = 0f;
                }
                fadeImage.color = new Color(0f, 0f, 0f, alpha);
            }
            else
            {
                fadeOut = false;
                changeToGO = false;
                changeToWin = false;
                goToMenu = false;
                restartGame = false;
                fade.SetActive(false);
            }
        }
    }

    public void ChangeLife(bool decrease)
    {
        if (decrease && lifes > 0)
        {
            lifes--;
        }
        else if (!decrease && lifes < 6)
        {
            lifes++;
        }
        UpdateLife();
    }
    private void UpdateLife()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i < lifes)
            {
                lifesGO[i].SetActive(true);
            }
            else
            {
                lifesGO[i].SetActive(false);
            }
            if (lifes == 0)
            {
                GameOver();
            }
        }
    }
    public void RestartGame()
    {
        restartGame = true;
        fadeIn = true;
    }

    private void GameOver()
    {
        changeToGO = true;
        fadeIn = true;
    }
    public void GoToMenu()
    {
        goToMenu = true;
        fadeIn = true;
    }
}
