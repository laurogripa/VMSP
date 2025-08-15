using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _playButton;
    private Button _exitButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _playButton = _document.rootVisualElement.Q<Button>("Play");
        _exitButton = _document.rootVisualElement.Q<Button>("Exit");

        _playButton.RegisterCallback<ClickEvent>(OnPlayButtonClicked);
        _exitButton.RegisterCallback<ClickEvent>(OnExitButtonClicked);
    }

    private void OnPlayButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene(2);
    }

    private void OnExitButtonClicked(ClickEvent evt)
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        _playButton.UnregisterCallback<ClickEvent>(OnPlayButtonClicked);
        _exitButton.UnregisterCallback<ClickEvent>(OnExitButtonClicked);
    }
}
