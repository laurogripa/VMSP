using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _playButton;
    private Button _exitButton;
    
    // Visual elements for different screens
    private VisualElement _buttonsElement;
    private VisualElement _stagesElement;
    
    // Stage buttons
    private Button _labyrinthButton;
    private Button _geniusButton;
    private Button _shieldButton;
    private Button _selectButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _playButton = _document.rootVisualElement.Q<Button>("Play");
        _exitButton = _document.rootVisualElement.Q<Button>("Exit");
        
        // Get Buttons and Stages visual elements
        _buttonsElement = _document.rootVisualElement.Q<VisualElement>("Buttons");
        _stagesElement = _document.rootVisualElement.Q<VisualElement>("Stages");
        
        // Get stage buttons
        _labyrinthButton = _document.rootVisualElement.Q<Button>("Labyrinth");
        _geniusButton = _document.rootVisualElement.Q<Button>("Genius");
        _shieldButton = _document.rootVisualElement.Q<Button>("Shield");
        _selectButton = _document.rootVisualElement.Q<Button>("Select");

        // Register callbacks
        _playButton.RegisterCallback<ClickEvent>(OnPlayButtonClicked);
        _exitButton.RegisterCallback<ClickEvent>(OnExitButtonClicked);
        
        // Register stage button callbacks
        if (_labyrinthButton != null)
            _labyrinthButton.RegisterCallback<ClickEvent>(OnLabyrinthButtonClicked);
        if (_geniusButton != null)
            _geniusButton.RegisterCallback<ClickEvent>(OnGeniusButtonClicked);
        if (_shieldButton != null)
            _shieldButton.RegisterCallback<ClickEvent>(OnShieldButtonClicked);
        if (_selectButton != null)
            _selectButton.RegisterCallback<ClickEvent>(OnSelectButtonClicked);
    }

    private void OnPlayButtonClicked(ClickEvent evt)
    {
        // Hide Buttons and show Stages
        if (_buttonsElement != null)
            _buttonsElement.style.display = DisplayStyle.None;
            
        if (_stagesElement != null)
            _stagesElement.style.display = DisplayStyle.Flex;
    }

    private void OnExitButtonClicked(ClickEvent evt)
    {
        Application.Quit();
    }
    
    // Stage button handlers
    private void OnLabyrinthButtonClicked(ClickEvent evt)
    {
        Debug.Log("Loading Stage 1 - Labyrinth");
        SceneManager.LoadScene(1);
    }
    
    private void OnGeniusButtonClicked(ClickEvent evt)
    {
        Debug.Log("Loading Stage 2 - Genius");
        SceneManager.LoadScene(2);
    }
    
    private void OnShieldButtonClicked(ClickEvent evt)
    {
        Debug.Log("Loading Stage 3 - Shield");
        SceneManager.LoadScene(3);
    }
    
    private void OnSelectButtonClicked(ClickEvent evt)
    {
        Debug.Log("Loading Stage 4 - Select");
        SceneManager.LoadScene(4);
    }

    private void OnDisable()
    {
        _playButton.UnregisterCallback<ClickEvent>(OnPlayButtonClicked);
        _exitButton.UnregisterCallback<ClickEvent>(OnExitButtonClicked);
        
        // Unregister stage button callbacks
        if (_labyrinthButton != null)
            _labyrinthButton.UnregisterCallback<ClickEvent>(OnLabyrinthButtonClicked);
        if (_geniusButton != null)
            _geniusButton.UnregisterCallback<ClickEvent>(OnGeniusButtonClicked);
        if (_shieldButton != null)
            _shieldButton.UnregisterCallback<ClickEvent>(OnShieldButtonClicked);
        if (_selectButton != null)
            _selectButton.UnregisterCallback<ClickEvent>(OnSelectButtonClicked);
    }
}