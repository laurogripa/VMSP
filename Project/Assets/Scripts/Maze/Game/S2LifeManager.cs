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
    private Text gameOverMessageText;
    private bool HasFade => fadeImage != null;

    void Start()
    {
        if (fadeImage == null && fade != null)
        {
            fadeImage = fade.GetComponent<RawImage>();
        }
        ConfigureGameOverScreen();
        lifes = 6;
    }

    private void FixedUpdate()
    {
        if (fadeIn)
        {
            if (!HasFade)
            {
                fadeIn = false;
                ExecutePendingAction();
                return;
            }
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
                ExecutePendingAction();
                fadeOut = true;
            }
        }
        else if (fadeOut)
        {
            if (!HasFade)
            {
                fadeOut = false;
                ResetTransitionFlags();
                return;
            }
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
                ResetTransitionFlags();
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

    private void ExecutePendingAction()
    {
        if (changeToGO)
        {
            BackAction.onGameOver = true;
            Destroy(gameElements);
            gameOverScreen.SetActive(true);
        }
        else if (changeToWin)
        {
            BackAction.onGameOver = true;
            Destroy(gameElements);
            for (int i = 0; i < gameComponents.Length; i++)
            {
                if (gameComponents[i] != null)
                {
                    gameComponents[i].SetActive(false);
                }
            }
            winScreen.SetActive(true);
        }
        else if (restartGame)
        {
            BackAction.onGameOver = false;
            SceneManager.LoadScene("Maze");
        }
        else if (goToMenu)
        {
            BackAction.onGameOver = false;
            SceneManager.LoadScene("Main Menu");
        }
    }

    private void ResetTransitionFlags()
    {
        changeToGO = false;
        changeToWin = false;
        goToMenu = false;
        restartGame = false;
    }

    private void ConfigureGameOverScreen()
    {
        if (gameOverScreen == null)
        {
            return;
        }

        RectTransform screenRect = gameOverScreen.GetComponent<RectTransform>();
        StretchRect(screenRect);

        Transform backgroundTransform = gameOverScreen.transform.Find("Background");
        if (backgroundTransform != null)
        {
            StretchRect(backgroundTransform as RectTransform);
            Image background = backgroundTransform.GetComponent<Image>();
            if (background != null)
            {
                background.sprite = null;
                background.color = new Color(0.07f, 0.1f, 0.22f, 1f);
            }
        }

        Transform messageTransform = gameOverScreen.transform.Find("Message");
        if (messageTransform == null)
        {
            GameObject messageObject = new GameObject("Message", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            messageTransform = messageObject.transform;
            messageTransform.SetParent(gameOverScreen.transform, false);
            messageTransform.SetSiblingIndex(1);

            RectTransform messageRect = messageObject.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.5f, 1f);
            messageRect.anchorMax = new Vector2(0.5f, 1f);
            messageRect.pivot = new Vector2(0.5f, 1f);
            messageRect.anchoredPosition = new Vector2(0f, -72f);
            messageRect.sizeDelta = new Vector2(920f, 220f);

            gameOverMessageText = messageObject.GetComponent<Text>();
            Font messageFont = null;
            if (gameComponents != null && gameComponents.Length > 0 && gameComponents[0] != null)
            {
                messageFont = gameComponents[0].GetComponent<Text>()?.font;
            }
            gameOverMessageText.font = messageFont != null
                ? messageFont
                : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            gameOverMessageText.alignment = TextAnchor.UpperCenter;
            gameOverMessageText.supportRichText = true;
            gameOverMessageText.horizontalOverflow = HorizontalWrapMode.Wrap;
            gameOverMessageText.verticalOverflow = VerticalWrapMode.Overflow;
            gameOverMessageText.text = "<color=#FC5C1D><size=68><b>Fim de jogo!</b></size></color>\n<size=44><color=#EAF6FF>Tentar novamente?</color></size>";
            gameOverMessageText.color = Color.white;

            Outline outline = messageObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.04f, 0.06f, 0.14f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);
        }
        else
        {
            gameOverMessageText = messageTransform.GetComponent<Text>();
        }

        if (gameOverMessageText != null)
        {
            gameOverMessageText.text = "<color=#FC5C1D><size=68><b>Fim de jogo!</b></size></color>\n<size=44><color=#EAF6FF>Tentar novamente?</color></size>";
        }
    }

    private static void StretchRect(RectTransform rect)
    {
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
