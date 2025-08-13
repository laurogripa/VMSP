using UnityEngine;
using Tobii.Gaming;

public class SimpleTobiiDebug : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableLogging = true;
    public bool createGazePoint = true;
    public bool verboseLogging = false;
    
    [Header("Visual Settings")]
    public Color gazePointColor = Color.red;
    public float gazePointSize = 1.0f;
    public float gazeDistance = 5.0f;
    
    private Camera mainCamera;
    private GameObject gazePoint;
    private Renderer gazeRenderer;
    private float lastLogTime = 0f;
    private bool gazePointCreated = false;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
            
        Debug.Log($"SimpleTobiiDebug: Camera found: {mainCamera?.name}");
        Debug.Log($"SimpleTobiiDebug: Screen size: {Screen.width}x{Screen.height}");
            
        if (createGazePoint)
        {
            CreateGazePoint();
        }
        
        Debug.Log("SimpleTobiiDebug: Starting eye tracking debug");
    }
    
    void Update()
    {
        try
        {
            GazePoint gaze = TobiiAPI.GetGazePoint();
            
            if (gaze.IsRecent())
            {
                // Update visual gaze point
                if (gazePoint != null)
                {
                    UpdateGazeVisualization(gaze);
                }
                
                // Log data every 0.5 seconds
                if (enableLogging && Time.time - lastLogTime > 0.5f)
                {
                    Debug.Log($"Gaze: ({gaze.Screen.x:F1}, {gaze.Screen.y:F1}) at {gaze.Timestamp:F3}");
                    
                    if (verboseLogging && gazePoint != null)
                    {
                        Debug.Log($"Gaze world pos: {gazePoint.transform.position}");
                        Debug.Log($"Gaze active: {gazePoint.activeInHierarchy}");
                        Debug.Log($"Camera pos: {mainCamera.transform.position}");
                    }
                    
                    lastLogTime = Time.time;
                }
            }
            else
            {
                if (gazePoint != null)
                    gazePoint.SetActive(false);
                    
                if (enableLogging && Time.time - lastLogTime > 2f)
                {
                    Debug.Log("No recent gaze data");
                    lastLogTime = Time.time;
                }
            }
            
            // Check focused object
            GameObject focused = TobiiAPI.GetFocusedObject();
            if (focused != null && enableLogging && verboseLogging)
            {
                Debug.Log($"Looking at: {focused.name}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Tobii error: {e.Message}");
        }
    }
    
    void UpdateGazeVisualization(GazePoint gaze)
    {
        Vector3 screenPos = gaze.Screen;
        
        // Try different positioning methods
        Vector3 worldPos;
        
        // Method 1: Use a fixed distance from camera
        screenPos.z = gazeDistance;
        worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        
        gazePoint.transform.position = worldPos;
        gazePoint.SetActive(true);
        
        // Make sure it's visible by adjusting material
        if (gazeRenderer != null)
        {
            gazeRenderer.material.color = gazePointColor;
            gazeRenderer.enabled = true;
        }
    }
    
    void CreateGazePoint()
    {
        gazePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gazePoint.name = "GazeDebugPoint";
        gazePoint.transform.localScale = Vector3.one * gazePointSize;
        
        gazeRenderer = gazePoint.GetComponent<Renderer>();
        
        // Create a bright, unlit material
        Material gazeMaterial = new Material(Shader.Find("Sprites/Default"));
        gazeMaterial.color = gazePointColor;
        gazeRenderer.material = gazeMaterial;
        
        // Remove collider
        Collider col = gazePoint.GetComponent<Collider>();
        if (col != null) DestroyImmediate(col);
        
        // Make sure it renders on top
        gazeRenderer.material.renderQueue = 3000;
        
        gazePoint.SetActive(false);
        gazePointCreated = true;
        
        Debug.Log($"Gaze point created: {gazePoint.name} with scale {gazePointSize}");
    }
    
    void OnGUI()
    {
        if (!enableLogging) return;
        
        // Simple on-screen debug info
        GUI.color = Color.white;
        GUI.Label(new Rect(10, 10, 300, 20), $"Camera: {mainCamera?.name ?? "None"}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Gaze Point Created: {gazePointCreated}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Gaze Point Active: {gazePoint?.activeInHierarchy ?? false}");
        
        if (gazePoint != null)
        {
            GUI.Label(new Rect(10, 70, 300, 20), $"Gaze Position: {gazePoint.transform.position}");
        }
        
        // Manual controls
        if (GUI.Button(new Rect(10, 100, 150, 30), "Toggle Verbose Log"))
        {
            verboseLogging = !verboseLogging;
        }
        
        if (GUI.Button(new Rect(10, 140, 150, 30), "Recreate Gaze Point"))
        {
            if (gazePoint != null)
                DestroyImmediate(gazePoint);
            CreateGazePoint();
        }
        
        // Size controls
        GUI.Label(new Rect(10, 180, 100, 20), "Gaze Size:");
        gazePointSize = GUI.HorizontalSlider(new Rect(110, 185, 100, 20), gazePointSize, 0.1f, 3.0f);
        if (gazePoint != null)
            gazePoint.transform.localScale = Vector3.one * gazePointSize;
        
        // Distance controls
        GUI.Label(new Rect(10, 210, 100, 20), "Gaze Distance:");
        gazeDistance = GUI.HorizontalSlider(new Rect(110, 215, 100, 20), gazeDistance, 1.0f, 20.0f);
    }
}