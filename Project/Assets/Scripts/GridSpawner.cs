using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private Sprite purpleSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite yellowSprite;
    [SerializeField] private Sprite targetSprite;
    [SerializeField] private GameObject grid3x3;
    [SerializeField] private GameObject grid4x4;

    private List<Image> cells = new List<Image>();
    private List<Sprite> spritesToAssign = new List<Sprite>();
    private int score = 0;
    private int targetCellIndex = -1;
    private int gridWidth = 3;
    private int gridHeight = 3;
    private bool canSelect = false;
    private bool isSwitching = false;
    private Text scoreText;
    private Text feedbackText;
    private Text timerText;
    private GameObject countdownPanel;
    private GameObject feedbackGO;

    private void Start()
    {
        CreateScoreDisplay();
        CreateTimerDisplay();
        CreateFeedbackDisplay();
        GetAllCells(3, 3);
        StartCoroutine(GameLoop());
    }

    private void CreateScoreDisplay()
    {
        Canvas c = FindObjectOfType<Canvas>();
        GameObject sg = new GameObject("ScoreText");
        sg.transform.SetParent(c.transform, false);
        scoreText = sg.AddComponent<Text>();
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.text = "Pontos: 0";
        scoreText.fontSize = 42;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.MiddleCenter;
        RectTransform r = sg.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = new Vector2(-220, 380);
        r.sizeDelta = new Vector2(400, 80);
    }

    private void CreateTimerDisplay()
    {
        Canvas c = FindObjectOfType<Canvas>();
        GameObject tg = new GameObject("TimerText");
        tg.transform.SetParent(c.transform, false);
        timerText = tg.AddComponent<Text>();
        timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        timerText.text = "Tempo: 500ms";
        timerText.fontSize = 42;
        timerText.fontStyle = FontStyle.Bold;
        timerText.color = Color.white;
        timerText.alignment = TextAnchor.MiddleCenter;
        RectTransform r = tg.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = new Vector2(170, 380);
        r.sizeDelta = new Vector2(400, 80);
    }

    private void CreateFeedbackDisplay()
    {
        Canvas c = FindObjectOfType<Canvas>();
        feedbackGO = new GameObject("FeedbackText");
        feedbackGO.transform.SetParent(c.transform, false);
        feedbackText = feedbackGO.AddComponent<Text>();
        feedbackText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        feedbackText.text = "";
        feedbackText.fontSize = 60;
        feedbackText.fontStyle = FontStyle.Bold;
        feedbackText.alignment = TextAnchor.MiddleCenter;
        feedbackText.color = new Color(1, 1, 1, 1);
        CanvasGroup cg = feedbackGO.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 1f;
        RectTransform r = feedbackGO.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = new Vector2(0, 200);
        r.sizeDelta = new Vector2(800, 200);
    }

    private void GetAllCells(int gridX, int gridY)
    {
        cells.Clear();
        GameObject activeGrid = (gridX == 4) ? grid4x4 : grid3x3;
        
        for (int r = 0; r < gridY; r++)
        {
            for (int c = 0; c < gridX; c++)
            {
                string suffix = (gridX == 4) ? "1" : "0";
                string n = "Cell_" + r + "_" + c + "_" + suffix;
                Transform t = activeGrid.transform.Find(n);
                if (t != null)
                {
                    Image img = t.GetComponent<Image>();
                    if (img != null)
                    {
                        cells.Add(img);
                        int idx = cells.Count - 1;
                        img.raycastTarget = true;
                        Button btn = img.gameObject.GetComponent<Button>();
                        if (btn == null)
                            btn = img.gameObject.AddComponent<Button>();
                        btn.targetGraphic = img;
                        int cellIdx = idx;
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => OnCellSelected(cellIdx));
                    }
                }
            }
        }
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            if (isSwitching)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            
            canSelect = false;
            countdownPanel = CreateCountdownOverlay();
            Text tx = countdownPanel.GetComponentInChildren<Text>();
            for (int i = 3; i >= 1; i--)
            {
                tx.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
            if (countdownPanel != null)
                Destroy(countdownPanel);
            
            GenerateBalancedSprites(gridWidth, gridHeight);
            ShowSprites();
            yield return new WaitForSeconds(0.5f);
            HideSprites();
            canSelect = true;
            yield return new WaitUntil(() => !canSelect);
        }
    }

    private GameObject CreateCountdownOverlay()
    {
        Canvas c = FindObjectOfType<Canvas>();
        GameObject p = new GameObject("CountdownPanel");
        p.transform.SetParent(c.transform, false);
        Image im = p.AddComponent<Image>();
        im.color = new Color(0, 0, 0, 0.7f);
        im.raycastTarget = false;
        RectTransform rt = p.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        GameObject tg = new GameObject("Text");
        tg.transform.SetParent(p.transform, false);
        Text tc = tg.AddComponent<Text>();
        tc.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tc.text = "3";
        tc.fontSize = 120;
        tc.color = Color.white;
        tc.alignment = TextAnchor.MiddleCenter;
        RectTransform tr = tg.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;
        return p;
    }

    private void OnCellSelected(int idx)
    {
        if (!canSelect) return;
        canSelect = false;
        cells[idx].sprite = spritesToAssign[idx];
        
        if (idx == targetCellIndex)
        {
            score++;
            scoreText.text = "Pontos: " + score;
            feedbackText.text = "Acertou!";
            feedbackText.color = new Color(0, 1, 0, 1);
            
            if (score == 5)
            {
                StartCoroutine(CorrectAnswerThenSwitch());
            }
            else
            {
                StartCoroutine(ClearFeedback());
            }
        }
        else
        {
            feedbackText.text = "Tente novamente!";
            feedbackText.color = new Color(1, 0, 0, 1);
            StartCoroutine(ClearFeedback());
        }
    }

    private IEnumerator CorrectAnswerThenSwitch()
    {
        yield return new WaitForSeconds(1f);
        
        isSwitching = true;
        feedbackText.text = "Aumentando células...";
        feedbackText.color = new Color(1, 1, 0, 1);
        yield return new WaitForSeconds(2f);
        
        grid3x3.SetActive(false);
        grid4x4.SetActive(true);
        gridWidth = 4;
        gridHeight = 4;
        GetAllCells(4, 4);
        HideSprites();
        feedbackText.text = "";
        isSwitching = false;
    }

    private IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(1f);
        feedbackText.text = "";
    }

    private void GenerateBalancedSprites(int gridX, int gridY)
    {
        spritesToAssign.Clear();
        int total = gridX * gridY;
        
        spritesToAssign.Add(targetSprite);
        
        Sprite[] colors = { purpleSprite, greenSprite, redSprite, yellowSprite };
        int remaining = total - 1;
        int colorIndex = 0;
        
        while (remaining > 0)
        {
            spritesToAssign.Add(colors[colorIndex % 4]);
            spritesToAssign.Add(colors[colorIndex % 4]);
            colorIndex++;
            remaining -= 2;
        }
        
        for (int i = spritesToAssign.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            Sprite tmp = spritesToAssign[i];
            spritesToAssign[i] = spritesToAssign[rnd];
            spritesToAssign[rnd] = tmp;
        }
        targetCellIndex = spritesToAssign.IndexOf(targetSprite);
    }

    private void ShowSprites()
    {
        for (int i = 0; i < cells.Count; i++)
            cells[i].sprite = spritesToAssign[i];
    }

    private void HideSprites()
    {
        for (int i = 0; i < cells.Count; i++)
            cells[i].sprite = null;
    }
}
