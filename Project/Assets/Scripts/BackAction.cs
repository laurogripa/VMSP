using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class BackAction : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseScreen, loginScreen, newLogin;
    [SerializeField]
    private AudioSource[] sounds;
    [SerializeField]
    private Button soundImage;
    [SerializeField]
    private Sprite[] sImages;
    [SerializeField]
    private Text inputName;
    [SerializeField]
    private TextMeshProUGUI warningText;

    Button test;

    private bool loginScreenState;

    public static bool onGameOver = false;
    public static bool onPause = false;

    private void Start()
    {
        loginScreenState = false;
        Time.timeScale = 1;

        if (pauseScreen == null)
        {
            var found = GameObject.Find("PauseScreen");
            if (found != null)
            {
                pauseScreen = found;
            }
            else
            {
                var prefab = Resources.Load<GameObject>("PauseScreen");
                if (prefab != null)
                {
                    var canvas = GameObject.FindObjectOfType<Canvas>();
                    if (canvas != null)
                    {
                        pauseScreen = Instantiate(prefab, canvas.transform);
                    }
                    else
                    {
                        pauseScreen = Instantiate(prefab);
                    }
                }
            }
        }
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (PlayerPrefs.GetInt("Sound") == 0)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].mute = false;
            }
        }
        else
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].mute = true;
            }
        }
        ManageSoundImage();
    }

    void Update()
    {
        if (!onGameOver && !SceneManager.GetActiveScene().name.Equals("Main Menu"))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!onPause)
                {
                    PauseGame();
                }
                else
                {
                    BackToGame();
                }
            }
        }
        if (warningText != null)
        {
            if (warningText.color.a > 0f)
            {
                float newAlpha = warningText.color.a;
                newAlpha -= 0.001f;
                warningText.color = new Color(1, 0, 0, newAlpha);
            }
        }
    }

    private void PauseGame()
    {
        if (pauseScreen == null)
        {
            var found = GameObject.Find("PauseScreen");
            if (found != null) pauseScreen = found;
        }
        if (pauseScreen != null)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            ManageSoundImage();
            onPause = true;
        }
    }

    private void ManageSoundImage()
    {
        if (PlayerPrefs.GetInt("Sound") == 0)
        {
            soundImage.image.sprite = sImages[0];
        }
        else
        {
            soundImage.image.sprite = sImages[1];
        }
    }

    public void SoundManager()
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            PlayerPrefs.SetInt("Sound", 0);
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].mute = false;
            }
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].mute = true;
            }
        }
        ManageSoundImage();
    }

    public void ShowLoginManager()
    {
        if (loginScreenState)
        {
            if (inputName.text.Equals(""))
            {
                warningText.color = new Color(1, 0, 0, 1);
            }
            else if (inputName.text == PlayerPrefs.GetString("PlayerName"))
            {
                warningText.color = new Color(1, 0, 0, 0);
                loginScreenState = false;
                loginScreen.SetActive(false);
            }
            else if (PlayerPrefs.GetString("PlayerRegistry").Equals(""))
            {
                warningText.color = new Color(1, 0, 0, 0);
                string newRegistry = "Novo Login\n" + "Data: " + System.DateTime.Now.ToString("dd/MM/yyyy") + " às " + System.DateTime.Now.ToString("HH:mm:ss") + "\n" + "Player: " + inputName.text + "\n";
                PlayerPrefs.SetString("PlayerName", inputName.text);
                PlayerPrefs.SetString("PlayerRegistry", newRegistry);
                Debug.Log(newRegistry);
                loginScreenState = false;
                loginScreen.SetActive(false);
            }
            else
            {
                newLogin.SetActive(true);
            }
        }
        else
        {
            loginScreen.SetActive(true);
            if (!PlayerPrefs.GetString("PlayerName").Equals(""))
            {
                GameObject.Find("PutName").GetComponent<InputField>().text = PlayerPrefs.GetString("PlayerName");
            }
            loginScreenState = true;
        }
    }

    public void ConfirmNewLogin()
    {
        newLogin.SetActive(false);
        warningText.color = new Color(1, 0, 0, 0);
        string newRegistry = "Novo Login\n" + "Data: " + System.DateTime.Now.ToString("dd/MM/yyyy") + " às " + System.DateTime.Now.ToString("HH:mm:ss") + "\n" + "Player: " + inputName.text + "\n";
        Debug.Log(newRegistry);
        PlayerPrefs.SetString("PlayerName", inputName.text);
        PlayerPrefs.SetString("PlayerRegistry", newRegistry);
        loginScreenState = false;
        loginScreen.SetActive(false);
    }

    public void DenyNewLogin()
    {
        newLogin.SetActive(false);
    }

    public void GoToMenu()
    {
        onPause = false;
        SceneManager.LoadScene("Main Menu");
    }

    public void ExportData()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            string filePath = Path.Combine(Application.temporaryCachePath, "text.txt");
            File.WriteAllText(filePath, PlayerPrefs.GetString("PlayerRegistry"));
            new NativeShare().AddFile(filePath).Share();
        }
        else
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = path + "\\VMSPregistry.txt";
            File.WriteAllText(filePath, PlayerPrefs.GetString("PlayerRegistry"));
        }
    }

    public void BackToGame()
    {
        onPause = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }
}
