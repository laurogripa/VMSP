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

    // Defense survival HUD/state
    private Text survivalCountdownText;
    private Text killsText;
    private float survivalRemaining = 60f;
    private bool winTriggered;
    private int kills;

    void Start()
    {
        lifesUI = new GameObject[5];
        for (int i = 0; i < lifesUI.Length; i++)
        {
            lifesUI[i] = GameObject.Find("Life_" + i);
        }
        counter = Camera.main.gameObject.GetComponent<StageManager>().counterText;
        counterObject = Camera.main.gameObject.GetComponent<StageManager>().counterObject;
        activeGameObjects = new List<GameObject>();
        GameObject.Find("Player").GetComponent<PlayerBehavior>().waiting = true;
        lifes = 5;
        level = -1;

        EnsureSurvivalHud();
    }

    void Update()
    {
        if (!BackAction.onPause && !BackAction.onGameOver && !winTriggered)
        {
            survivalRemaining -= Time.deltaTime;
            if (survivalRemaining < 0) survivalRemaining = 0;
            UpdateHud();
            if (survivalRemaining <= 0f)
            {
                winTriggered = true;
                Camera.main.gameObject.GetComponent<StageManager>().SetWin();
            }
        }
    }

    public void IncreaseLevel()
    {
        foreach (GameObject e in activeGameObjects)
        {
            Destroy(e);
        }

        level += 1;
        if (lifes < 5)
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

        // Defense: create a hidden template and remove fixed enemies
        var spawner = GameObject.FindObjectOfType<DefenseEnemySpawner>();
        var fixedEnemies = SubStage.GetComponentsInChildren<EnemyBehavior>(true);
        GameObject templateClone = null;
        if (fixedEnemies != null && fixedEnemies.Length > 0)
        {
            templateClone = Instantiate(fixedEnemies[0].gameObject);
            templateClone.name = fixedEnemies[0].gameObject.name + "_Template";
            templateClone.SetActive(false);
            var ebOnTemplate = templateClone.GetComponent<EnemyBehavior>();
            if (ebOnTemplate != null) ebOnTemplate.enabled = false;
            if (GameObject.Find("GameElements") == null)
            {
                templateClone.transform.SetParent(GameObject.Find("GameElements(Clone)").transform);
            }
            else
            {
                templateClone.transform.SetParent(GameObject.Find("GameElements").transform);
            }
        }
        for (int i = 0; i < fixedEnemies.Length; i++)
        {
            Destroy(fixedEnemies[i].gameObject);
        }
        if (spawner != null && templateClone != null)
        {
            spawner.SetTemplate(templateClone);
        }
    }

    public void RegisterKill()
    {
        kills += 1;
        UpdateHud();
    }

    private void EnsureSurvivalHud()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            var canvasGo = new GameObject("Canvas");
            canvas = canvasGo;
            var c = canvasGo.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        var countdownGo = GameObject.Find("SurvivalCountdown");
        if (countdownGo == null)
        {
            countdownGo = new GameObject("SurvivalCountdown");
            countdownGo.transform.SetParent(canvas.transform);
            var rt = countdownGo.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-20f, 20f);
            survivalCountdownText = countdownGo.AddComponent<Text>();
            survivalCountdownText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            survivalCountdownText.alignment = TextAnchor.LowerRight;
            survivalCountdownText.fontSize = 24;
            survivalCountdownText.color = Color.white;
        }
        else
        {
            survivalCountdownText = countdownGo.GetComponent<Text>();
        }

        var killsGo = GameObject.Find("KillsText");
        if (killsGo == null)
        {
            killsGo = new GameObject("KillsText");
            killsGo.transform.SetParent(canvas.transform);
            var rt2 = killsGo.AddComponent<RectTransform>();
            rt2.anchorMin = new Vector2(1f, 0f);
            rt2.anchorMax = new Vector2(1f, 0f);
            rt2.pivot = new Vector2(1f, 0f);
            rt2.anchoredPosition = new Vector2(-20f, 50f);
            killsText = killsGo.AddComponent<Text>();
            killsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            killsText.alignment = TextAnchor.LowerRight;
            killsText.fontSize = 20;
            killsText.color = Color.white;
        }
        else
        {
            killsText = killsGo.GetComponent<Text>();
        }

        UpdateHud();
    }

    private void UpdateHud()
    {
        if (survivalCountdownText != null)
        {
            int seconds = Mathf.CeilToInt(survivalRemaining);
            survivalCountdownText.text = seconds.ToString() + "s";
        }
        if (killsText != null)
        {
            killsText.text = "Kills: " + kills.ToString();
        }
    }

    public void ChangeLife(int number)
    {
        if (level > 0)
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
