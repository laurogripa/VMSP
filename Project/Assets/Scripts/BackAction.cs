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
        if (Application.platform == RuntimePlatform.Android && !onGameOver)
        {
            if (Input.GetKey(KeyCode.Escape) && !SceneManager.GetActiveScene().name.Equals("Stage 0"))
            {
                Time.timeScale = 0;
                pauseScreen.SetActive(true);
                ManageSoundImage();
                onPause = true;
            }
        }
        if (Input.GetKey(KeyCode.RightArrow) && !onGameOver && !SceneManager.GetActiveScene().name.Equals("Stage 0"))
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            ManageSoundImage();
            onPause = true;
        }
        if(warningText != null)
        {
            if (warningText.color.a > 0f)
            {
                float newAlpha = warningText.color.a;
                newAlpha -= 0.001f;
                warningText.color = new Color(1, 0, 0, newAlpha);
            }
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
        if(PlayerPrefs.GetInt("Sound") == 1)
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
        if(loginScreenState)
        {
            if(inputName.text.Equals(""))
            {
                warningText.color = new Color(1, 0, 0, 1);
            }
            else if (inputName.text == PlayerPrefs.GetString("PlayerName"))
            {
                warningText.color = new Color(1, 0, 0, 0);
                loginScreenState = false;
                loginScreen.SetActive(false);
            }
            else if(PlayerPrefs.GetString("PlayerRegistry").Equals(""))
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
            if(!PlayerPrefs.GetString("PlayerName").Equals(""))
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
        SceneManager.LoadScene("Stage 0");
    }

    public void ExportData()
    {
        if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
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
