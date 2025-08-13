using UnityEngine;
using Tobii.Gaming;

public class VisibleGazeDebug : MonoBehaviour
{
    public bool enableLogging = true;
    public Color gazeColor = Color.red;
    public float gazeSize = 30f;
    
    private Camera mainCamera;
    private GameObject gazeCanvas;
    private RectTransform gazeRect;
    private UnityEngine.UI.Image gazeImage;
    private float lastLogTime = 0f;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
            
        CreateScreenOverlayGaze();
        Debug.Log("VisibleGazeDebug: Created screen overlay gaze point");
    }
    
    void Update()
    {
        try
        {
            GazePoint gaze = TobiiAPI.GetGazePoint();
            
            if (gaze.IsRecent())
            {
                // Update gaze position - direct screen coordinates
                Vector2 screenPos = gaze.Screen;
                gazeRect.anchoredPosition = screenPos;
                gazeCanvas.SetActive(true);
                
                // Log every 0.5 seconds
                if (enableLogging && Time.time - lastLogTime > 0.5f)
                {
                    Debug.Log($"Gaze: Screen({screenPos.x:F1}, {screenPos.y:F1}) -> UI({gazeRect.anchoredPosition})");
                    lastLogTime = Time.time;
                }
            }
            else
            {
                gazeCanvas.SetActive(false);
                
                if (enableLogging && Time.time - lastLogTime > 2f)
                {
                    Debug.Log("No gaze data - hiding point");
                    lastLogTime = Time.time;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Gaze error: {e.Message}");
        }
    }
    
    void CreateScreenOverlayGaze()
    {
        // Create canvas that renders on top of everything
        gazeCanvas = new GameObject("GazeOverlay");
        Canvas canvas = gazeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Render on top
        
        // Create gaze point as UI element
        GameObject gazePoint = new GameObject("GazePoint");
        gazePoint.transform.SetParent(gazeCanvas.transform);
        
        gazeImage = gazePoint.AddComponent<UnityEngine.UI.Image>();
        gazeImage.color = gazeColor;
        
        // Create simple circle texture
        Texture2D tex = new Texture2D(32, 32);
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                tex.SetPixel(x, y, dist <= 14 ? Color.white : Color.clear);
            }
        }
        tex.Apply();
        gazeImage.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        
        // Setup rect transform
        gazeRect = gazePoint.GetComponent<RectTransform>();
        gazeRect.sizeDelta = new Vector2(gazeSize, gazeSize);
        gazeRect.anchorMin = Vector2.zero;
        gazeRect.anchorMax = Vector2.zero;
        gazeRect.pivot = new Vector2(0.5f, 0.5f);
        
        gazeCanvas.SetActive(false);
    }
    
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 100), "");
        GUI.Label(new Rect(15, 15, 190, 20), $"Gaze Visible: {gazeCanvas.activeInHierarchy}");
        GUI.Label(new Rect(15, 35, 190, 20), $"Canvas Created: {gazeCanvas != null}");
        if (gazeRect != null)
            GUI.Label(new Rect(15, 55, 190, 20), $"Position: {gazeRect.anchoredPosition}");
        GUI.Label(new Rect(15, 75, 190, 20), $"Size: {gazeSize}");
    }
}