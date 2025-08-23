using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] substages;
    [SerializeField]
    private GameObject[] livesUI;
    [SerializeField]
    public Text counter;
    [SerializeField]
    public GameObject counterObject;
    List<GameObject> activeGameObjects;
    public int level;
    public int lives;
    public bool onShield;

    [SerializeField]
    private Text survivalCountdownText;
    [SerializeField]
    private Text killsText;
    private float survivalRemaining = 60f;
    private bool winTriggered;
    private int kills;

    void Start()
    {
        lives = 3;
        livesUI = new GameObject[lives];
        for (int i = 0; i < livesUI.Length; i++)
        {
            livesUI[i] = GameObject.Find("Life_" + i);
        }
        counter = Camera.main.gameObject.GetComponent<StageManager>().counterText;
        counterObject = Camera.main.gameObject.GetComponent<StageManager>().counterObject;
        activeGameObjects = new List<GameObject>();
        GameObject.Find("Player").GetComponent<PlayerBehavior>().waiting = true;
        level = -1;

        UpdateHud();
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
        if (lives < livesUI.Length)
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

    private void UpdateHud()
    {
        int seconds = Mathf.CeilToInt(survivalRemaining);
        survivalCountdownText.text = "Tempo: " + seconds.ToString() + "s";
        killsText.text = "Acertos: " + kills.ToString();
    }

    public void ChangeLife(int delta)
    {
        if (level > 0)
        {
            int maxLives = livesUI.Length;
            lives = Mathf.Clamp(lives + delta, 0, maxLives);
        }
        for (int i = 0; i < livesUI.Length; i++)
        {
            livesUI[i].SetActive(i < lives);
        }
    }
}
