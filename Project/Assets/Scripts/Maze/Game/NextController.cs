using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextController : MonoBehaviour
{
    public bool[] checkWin;
    public bool paused;
    private bool win, fadeIn, fadeOut;
    private int level, cTime;
    private float alpha, sec;
    [SerializeField] private GameObject[] mazes;
    [SerializeField] RawImage fadeImage;
    private GameObject lineBox, currentMaze;
    [SerializeField] private Text levelText;
    private ManageControls controls;
    private Transform gameElementsRoot;
    private int requiredCheckpoints;
    private GameObject levelCompleteOverlay;
    private RectTransform levelCompletePanelRect;
    private Image levelCompletePanelImage;
    private Text levelCompleteMessageText;
    private bool HasFade => fadeImage != null;

    private void Awake()
    {
        if (fadeImage == null)
        {
            Transform fadeTransform = transform.root.Find("Fade");
            if (fadeTransform != null)
            {
                fadeImage = fadeTransform.GetComponent<RawImage>();
            }
        }
        levelText = GameObject.Find("Level").GetComponent<Text>();
    }

    void Start()
    {
        fadeOut = HasFade;
        alpha = HasFade ? 1f : 0f;
        lineBox = GameObject.Find("Line Box");
        controls = GameObject.Find("Neuron").GetComponent<ManageControls>();
        gameElementsRoot = transform.parent;
        level = 1;
        currentMaze = Instantiate(mazes[level - 1]);
        ParentCurrentMaze();
        InitializeWinTracking();
        string newRegistry = PlayerPrefs.GetString("PlayerRegistry");
        newRegistry += "\n" + System.DateTime.Now.ToString("dd / MM / yyyy") + " às " + System.DateTime.Now.ToString("HH: mm: ss") + "\n#Start | Estágio 1: Fase " + level + "\n";
        PlayerPrefs.SetString("PlayerRegistry", newRegistry);
        //rTimeT.text = "Recorde: " + GetRecord(level.ToString());
    }

    void FixedUpdate()
    {
        if(!BackAction.onPause)
        {
            if (sec < 1)
            {
                sec += Time.deltaTime;
            }
            else
            {
                sec = 0;
                cTime++;
                //cTimeT.text = "Tempo: " + cTime.ToString();
            }
            if (level <= mazes.Length)
            {
                if (fadeIn)
                {
                    if (!HasFade)
                    {
                        CompleteLevelTransition();
                        return;
                    }
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
                        CompleteLevelTransition();
                    }
                }
                else if (fadeOut)
                {
                    if (!HasFade)
                    {
                        fadeOut = false;
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
                    }
                }
                else if (win)
                {
                    controls.showCanvas = true;
                    if (HasFade)
                    {
                        EnsureFadeActive();
                        fadeIn = true;
                        alpha = fadeImage.color.a;
                    }
                    else
                    {
                        CompleteLevelTransition();
                    }
                }

                AnimateLevelCompleteOverlay();
            }
        }
    }

    public void CheckButtons()
    {
        int testing = 0;
        for (int i = 0; i < checkWin.Length; i++)
        {
            if (checkWin[i])
            {
                testing++;
            }
        }
        if (testing >= requiredCheckpoints)
        {
            win = true;
            gameObject.GetComponent<LineCreator>().successLines.Clear();
        }
    }

    private void InitializeWinTracking()
    {
        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>(true);
        requiredCheckpoints = 0;
        int maxButtonId = -1;

        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (!checkpoints[i].gameObject.activeInHierarchy)
            {
                continue;
            }

            requiredCheckpoints++;
            int buttonId = int.Parse(checkpoints[i].gameObject.name[7].ToString()) - 1;
            if (buttonId > maxButtonId)
            {
                maxButtonId = buttonId;
            }
        }

        if (requiredCheckpoints == 0)
        {
            requiredCheckpoints = 1;
        }

        checkWin = new bool[Mathf.Max(maxButtonId + 1, 1)];
    }

    private void EnsureLevelCompleteOverlay()
    {
        if (levelCompleteOverlay != null || levelText == null)
        {
            return;
        }

        Transform uiCanvas = fadeImage != null
            ? fadeImage.transform.parent
            : levelText.transform.parent.parent;

        levelCompleteOverlay = new GameObject("LevelCompleteOverlay", typeof(RectTransform));
        RectTransform overlayRect = levelCompleteOverlay.GetComponent<RectTransform>();
        overlayRect.SetParent(uiCanvas, false);
        StretchRect(overlayRect);
        overlayRect.SetAsLastSibling();

        GameObject panel = new GameObject("Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        levelCompletePanelRect = panel.GetComponent<RectTransform>();
        levelCompletePanelRect.SetParent(overlayRect, false);
        levelCompletePanelRect.anchorMin = levelCompletePanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        levelCompletePanelRect.pivot = new Vector2(0.5f, 0.5f);
        levelCompletePanelRect.sizeDelta = new Vector2(920f, 340f);
        levelCompletePanelRect.anchoredPosition = Vector2.zero;

        levelCompletePanelImage = panel.GetComponent<Image>();
        levelCompletePanelImage.color = new Color(0.07f, 0.1f, 0.22f, 0.94f);

        GameObject message = new GameObject("Message", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        RectTransform messageRect = message.GetComponent<RectTransform>();
        messageRect.SetParent(levelCompletePanelRect, false);
        StretchRect(messageRect);
        messageRect.offsetMin = new Vector2(32f, 32f);
        messageRect.offsetMax = new Vector2(-32f, -32f);

        levelCompleteMessageText = message.GetComponent<Text>();
        levelCompleteMessageText.font = levelText.font;
        levelCompleteMessageText.alignment = TextAnchor.MiddleCenter;
        levelCompleteMessageText.supportRichText = true;
        levelCompleteMessageText.horizontalOverflow = HorizontalWrapMode.Wrap;
        levelCompleteMessageText.verticalOverflow = VerticalWrapMode.Overflow;
        levelCompleteMessageText.text = "<color=#FC5C1D><size=68><b>Parabéns!</b></size></color>\n<size=44><color=#EAF6FF>Carregando próximo nível.</color></size>";
        levelCompleteMessageText.color = Color.white;

        Outline outline = message.AddComponent<Outline>();
        outline.effectColor = new Color(0.04f, 0.06f, 0.14f, 0.85f);
        outline.effectDistance = new Vector2(2f, -2f);

        levelCompleteOverlay.SetActive(false);
    }

    public void ShowLevelCompleteOverlay()
    {
        EnsureLevelCompleteOverlay();
        if (levelCompleteOverlay == null)
        {
            return;
        }

        levelCompleteOverlay.SetActive(true);
        levelCompleteOverlay.transform.SetAsLastSibling();
    }

    private void HideLevelCompleteOverlay()
    {
        if (levelCompleteOverlay != null)
        {
            levelCompleteOverlay.SetActive(false);
        }
    }

    private void AnimateLevelCompleteOverlay()
    {
        if (levelCompleteOverlay == null || !levelCompleteOverlay.activeSelf || levelCompletePanelRect == null)
        {
            return;
        }

        float pulse = 0.92f + 0.08f * Mathf.PingPong(Time.time * 1.4f, 1f);
        levelCompletePanelRect.localScale = Vector3.one * pulse;
    }

    private void EnsureFadeActive()
    {
        if (fadeImage != null && !fadeImage.gameObject.activeSelf)
        {
            fadeImage.gameObject.SetActive(true);
        }
    }

    private static void StretchRect(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void DestroyLines()
    {
        Transform[] lines;
        lines = lineBox.GetComponentsInChildren<Transform>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].gameObject != null)
            {
                Destroy(lines[i].gameObject);
            }
        }
    }

    private void LoadNewLevel()
    {
        DestroyLines();
        Destroy(currentMaze);
        level++;
        GameObject.Find("Lifes").GetComponent<S2LifeManager>().ChangeLife(false);
        currentMaze = Instantiate(mazes[level - 1]);
        ParentCurrentMaze();
        levelText.text = "Nivel " + level;
        InitializeWinTracking();
        win = false;
        string newRegistry = PlayerPrefs.GetString("PlayerRegistry");
        newRegistry += "\n" + System.DateTime.Now.ToString("dd / MM / yyyy") + " às " + System.DateTime.Now.ToString("HH: mm: ss") + "\n#Start | Estágio 1: Fase " + level + "\n";
        PlayerPrefs.SetString("PlayerRegistry", newRegistry);
    }

    private void SetRecord(string stage, int time)
    {
        PlayerPrefs.SetInt(stage, time);
    }
    private int GetRecord(string stage)
    {
        if (PlayerPrefs.HasKey(stage))
        {
            return PlayerPrefs.GetInt(stage);
        }
        else
        {
            return 0;
        }
    }
    public int getLevel()
    {
        return level;
    }

    private void CompleteLevelTransition()
    {
        fadeIn = false;
        if (level < mazes.Length)
        {
            if (GetRecord(level.ToString()) == 0 || GetRecord(level.ToString()) > cTime)
            {
                SetRecord(level.ToString(), cTime);
            }
            string newRegistry = PlayerPrefs.GetString("PlayerRegistry");
            newRegistry += "✓ Fase " + level + " concluida em " + cTime + " segundos.\n";
            PlayerPrefs.SetString("PlayerRegistry", newRegistry);
            LoadNewLevel();
            cTime = 0;
            HideLevelCompleteOverlay();
            fadeOut = HasFade;
        }
    }

    private void ParentCurrentMaze()
    {
        currentMaze.transform.SetParent(gameElementsRoot, false);
        currentMaze.transform.localPosition = new Vector3(0.2f, 3.2f, -1144.8628f);
        currentMaze.transform.localRotation = Quaternion.identity;
    }
}
