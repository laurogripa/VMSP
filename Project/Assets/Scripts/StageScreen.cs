using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageScreen : MonoBehaviour
{
    private bool loadScene = false;
    public int currentS = 0;

    [SerializeField]
    private int scene;
    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private GameObject spriteHolder;
    [SerializeField]
    private GameObject[] menuScenes, selectionScenes;

    public Slider slider;

    void Update()
    { 
        if (loadScene)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }
    }

    public void GenLoad()
    {
        SceneManager.LoadScene(scene);
    }

    private void ActiveMenuScene(int sceneID)
    {
        for (int i = 0; i < menuScenes.Length; i++)
        {
            menuScenes[i].SetActive(false);
        }
        menuScenes[sceneID].SetActive(true);
    }
    private void ActiveSelectionScene(int sceneID)
    {
        for (int i = 0; i < selectionScenes.Length; i++)
        {
            selectionScenes[i].SetActive(false);
        }
        selectionScenes[sceneID].SetActive(true);
    }

    public void DecreaseSelection()
    {
        currentS--;
        ActiveSelectionScene(currentS);
    }

    public void IncreaseSelection()
    {
        currentS++;
        ActiveSelectionScene(currentS);
    }

    public void LoadMenu()
    {
        ActiveMenuScene(0);
    }

    public void LoadSelectScene()
    {
        ActiveMenuScene(1);
        currentS = 0;
        ActiveSelectionScene(currentS);
    }

    public void LoadInstructions()
    {
        ActiveMenuScene(2);
    }
    public void LoadCredits()
    {
        ActiveMenuScene(3);
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void LoadStage1()
    {
        scene = 1;
        GenLoad();
    }
    public void LoadStage2()
    {
        scene = 2;
        Instantiate(spriteHolder);
    }
    public void LoadStage3()
    {
        scene = 3;
        GenLoad();
    }
    public void LoadStage4()
    {
        scene = 4;
        GenLoad();
    }
    public void LoadStage5()
    {
        scene = 5;
        GenLoad();
    }
    public void LoadStage6()
    {
        scene = 6;
        GenLoad();
    }

    IEnumerator LoadNewScene()
    { 
        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(3);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        
        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            loadingText.text = "Carregando...\n" + (async.progress * 100) + "%";
            yield return null;
        }

    }
}
