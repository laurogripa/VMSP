using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _playButton;
    private Button _exitButton;
    private Button _returnButton;

    private VisualElement _buttonsElement;
    private VisualElement _stagesElement;

    private Button _labyrinthButton;
    private Button _geniusButton;
    private Button _shieldButton;
    private Button _selectButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        _buttonsElement = _document.rootVisualElement.Q<VisualElement>("Buttons");
        _stagesElement = _document.rootVisualElement.Q<VisualElement>("Stages");

        _playButton = _document.rootVisualElement.Q<Button>("Play");
        _exitButton = _document.rootVisualElement.Q<Button>("Exit");
        _returnButton = _document.rootVisualElement.Q<Button>("Return");

        _labyrinthButton = _document.rootVisualElement.Q<Button>("Labyrinth");
        _geniusButton = _document.rootVisualElement.Q<Button>("Genius");
        _shieldButton = _document.rootVisualElement.Q<Button>("Shield");
        _selectButton = _document.rootVisualElement.Q<Button>("Select");

        _playButton.RegisterCallback<ClickEvent>(OnPlayButtonClicked);
        _exitButton.RegisterCallback<ClickEvent>(OnExitButtonClicked);
        _returnButton.RegisterCallback<ClickEvent>(OnReturnButtonClicked);

        _labyrinthButton.RegisterCallback<ClickEvent>(OnLabyrinthButtonClicked);
        _geniusButton.RegisterCallback<ClickEvent>(OnGeniusButtonClicked);
        _shieldButton.RegisterCallback<ClickEvent>(OnShieldButtonClicked);
        _selectButton.RegisterCallback<ClickEvent>(OnSelectButtonClicked);
    }

    private void OnPlayButtonClicked(ClickEvent evt)
    {
        _buttonsElement.style.display = DisplayStyle.None;
        _stagesElement.style.display = DisplayStyle.Flex;
    }
    private void OnReturnButtonClicked(ClickEvent evt)
    {
        _buttonsElement.style.display = DisplayStyle.Flex;
        _stagesElement.style.display = DisplayStyle.None;
    }

    private void OnExitButtonClicked(ClickEvent evt)
    {
        Application.Quit();
    }

    private void OnLabyrinthButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("Labyrinth");
    }

    private void OnGeniusButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("Genius");
    }

    private void OnShieldButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("Shield");
    }

    private void OnSelectButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("Select");
    }

    private void OnDisable()
    {
        _playButton.UnregisterCallback<ClickEvent>(OnPlayButtonClicked);
        _exitButton.UnregisterCallback<ClickEvent>(OnExitButtonClicked);
        _labyrinthButton.UnregisterCallback<ClickEvent>(OnLabyrinthButtonClicked);
        _geniusButton.UnregisterCallback<ClickEvent>(OnGeniusButtonClicked);
        _shieldButton.UnregisterCallback<ClickEvent>(OnShieldButtonClicked);
        _selectButton.UnregisterCallback<ClickEvent>(OnSelectButtonClicked);
        _returnButton.UnregisterCallback<ClickEvent>(OnReturnButtonClicked);
    }
}
