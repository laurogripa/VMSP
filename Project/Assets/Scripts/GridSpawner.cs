using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private Sprite purpleSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite yellowSprite;
    [SerializeField] private Sprite targetSprite;
    private List<Image> cells = new List<Image>();
    private List<Sprite> spritesToAssign = new List<Sprite>();
    private int score = 0;
    private bool canSelect = false;
    private int targetCellIndex = -1;
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
        GetAllCells();
        StartCoroutine(GameLoop());
    }

    private void GetAllCells()
    {
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                string n = "Cell_" + r + "_" + c;
                Transform t = transform.Find(n);
                if (t != null)
                {
                    Image img = t.GetComponent<Image>();
                    if (img != null)
                    {
                        cells.Add(img);
                        int idx = cells.Count - 1;
                        img.raycastTarget = true;
                        Button btn = img.gameObject.AddComponent<Button>();
                        btn.targetGraphic = img;
                        Navigation nav = new Navigation();
                        nav.mode = Navigation.Mode.None;
                        btn.navigation = nav;
                        int cellIdx = idx;
                        btn.onClick.AddListener(() => { OnCellSelected(cellIdx); });
                    }
                }
            }
        }
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

    private IEnumerator GameLoop()
    {
        while (true)
        {
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
            GenerateBalancedSprites();
            ShowSprites();
            yield return new WaitForSeconds(0.5f);
            HideSprites();
            canSelect = true;
            yield return new WaitUntil(() => !canSelect);
        }
    }

    private void OnCellSelected(int idx)
    {
        if (!canSelect)
            return;
        canSelect = false;
        cells[idx].sprite = spritesToAssign[idx];
        if (idx == targetCellIndex)
        {
            score++;
            scoreText.text = "Pontos: " + score;
            feedbackText.text = "Acertou!";
            feedbackText.color = new Color(0, 1, 0, 1);
            StartCoroutine(ClearFeedback());
        }
        else
        {
            feedbackText.text = "Tente novamente!";
            feedbackText.color = new Color(1, 0, 0, 1);
            StartCoroutine(ClearFeedback());
        }
    }

    private IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(1f);
        feedbackText.text = "";
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

    private void GenerateBalancedSprites()
    {
        spritesToAssign.Clear();
        spritesToAssign.Add(targetSprite);
        spritesToAssign.Add(purpleSprite);
        spritesToAssign.Add(purpleSprite);
        spritesToAssign.Add(greenSprite);
        spritesToAssign.Add(greenSprite);
        spritesToAssign.Add(redSprite);
        spritesToAssign.Add(redSprite);
        spritesToAssign.Add(yellowSprite);
        spritesToAssign.Add(yellowSprite);
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
