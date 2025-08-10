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

    private void Awake()
    {
        fadeImage = GameObject.Find("Fade").GetComponent<RawImage>();
        levelText = GameObject.Find("Level").GetComponent<Text>();
    }

    void Start()
    {
        fadeOut = true;
        alpha = 1f;
        lineBox = GameObject.Find("Line Box");
        controls = GameObject.Find("Neuron").GetComponent<ManageControls>();
        level = 1;
        currentMaze = Instantiate(mazes[level - 1]);
        currentMaze.transform.SetParent(GameObject.Find("GameElements").transform);
        checkWin = new bool[3];
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
                            /*if (GetRecord(level.ToString()) == 0)
                            {
                                rTimeT.text = "Recorde: X";
                            }
                            else
                            {
                                rTimeT.text = "Recorde: " + GetRecord(level.ToString());
                            }*/
                            cTime = 0;
                            //cTimeT.text = cTime.ToString();
                            fadeOut = true;
                        }
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
                    }
                }
                else if (win)
                {
                    controls.showCanvas = true;
                    fadeIn = true;
                    alpha = fadeImage.color.a;
                }
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
        if (testing == checkWin.Length)
        {
            win = true;
            gameObject.GetComponent<LineCreator>().successLines.Clear();
        }
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
        currentMaze.transform.SetParent(GameObject.Find("GameElements").transform);
        levelText.text = "Nivel " + level;
        for (int i = 0; i < checkWin.Length; i++)
        {
            checkWin[i] = false;
        }
        win = false;
        string newRegistry = PlayerPrefs.GetString("PlayerRegistry");
        newRegistry += "\n"+System.DateTime.Now.ToString("dd / MM / yyyy") + " às " + System.DateTime.Now.ToString("HH: mm: ss") + "\n#Start | Estágio 1: Fase " + level + "\n";
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
}
