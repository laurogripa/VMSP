using UnityEngine;
using Tobii.Gaming;

public class SoftGazeBubble : MonoBehaviour
{
    [Header("Bubble Appearance")]
    public Color bubbleColor = new Color(0.2f, 0.6f, 1.0f, 0.5f);
    public float bubbleSize = 50f;
    public bool enableLogging = false;
    
    [Header("Smoothing")]
    [Range(0.05f, 0.5f)]
    public float smoothingFactor = 0.15f;
    
    private GameObject gazeCanvas;
    private RectTransform gazeRect;
    private UnityEngine.UI.Image gazeImage;
    private Vector2 historicPosition;
    private bool hasHistoricPosition;
    private float lastLogTime;
    
    void Start()
    {
        CreateTransparentBubble();
        Debug.Log("SoftGazeBubble: Created transparent gaze bubble");
    }
    
    void Update()
    {
        try
        {
            GazePoint gaze = TobiiAPI.GetGazePoint();
            
            if (gaze.IsRecent())
            {
                Vector2 smoothedPos = ApplySmoothing(gaze.Screen);
                gazeRect.anchoredPosition = smoothedPos;
                gazeCanvas.SetActive(true);
                
                if (enableLogging && Time.time - lastLogTime > 0.5f)
                {
                    Debug.Log($"Bubble: ({smoothedPos.x:F1}, {smoothedPos.y:F1})");
                    lastLogTime = Time.time;
                }
            }
            else
            {
                gazeCanvas.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Gaze bubble error: {e.Message}");
        }
    }
    
    Vector2 ApplySmoothing(Vector2 targetPos)
    {
        if (!hasHistoricPosition)
        {
            historicPosition = targetPos;
            hasHistoricPosition = true;
            return targetPos;
        }
        
        Vector2 smoothed = Vector2.Lerp(targetPos, historicPosition, smoothingFactor);
        historicPosition = smoothed;
        return smoothed;
    }
    
    void CreateTransparentBubble()
    {
        // Create overlay canvas
        gazeCanvas = new GameObject("GazeBubbleCanvas");
        Canvas canvas = gazeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 998;
        
        // Create bubble
        GameObject bubble = new GameObject("GazeBubble");
        bubble.transform.SetParent(gazeCanvas.transform);
        
        gazeImage = bubble.AddComponent<UnityEngine.UI.Image>();
        gazeImage.color = bubbleColor;
        gazeImage.sprite = CreateBubbleTexture();
        
        gazeRect = bubble.GetComponent<RectTransform>();
        gazeRect.sizeDelta = new Vector2(bubbleSize, bubbleSize);
        gazeRect.anchorMin = Vector2.zero;
        gazeRect.anchorMax = Vector2.zero;
        gazeRect.pivot = new Vector2(0.5f, 0.5f);
        
        gazeCanvas.SetActive(false);
    }
    
    Sprite CreateBubbleTexture()
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 4f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                
                if (dist <= radius)
                {
                    // Create bubble effect: transparent center, more opaque edges
                    float normalizedDist = dist / radius;
                    float alpha;
                    
                    if (normalizedDist < 0.4f)
                    {
                        // Inner area: very transparent
                        alpha = 0.1f;
                    }
                    else if (normalizedDist < 0.8f)
                    {
                        // Middle ring: gradient
                        float t = (normalizedDist - 0.4f) / 0.4f;
                        alpha = Mathf.Lerp(0.1f, 0.6f, t);
                    }
                    else
                    {
                        // Outer edge: fade out
                        float t = (normalizedDist - 0.8f) / 0.2f;
                        alpha = Mathf.Lerp(0.6f, 0.0f, t);
                    }
                    
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 80), "");
        GUI.Label(new Rect(15, 15, 180, 20), $"Bubble: {gazeCanvas.activeInHierarchy}");
        GUI.Label(new Rect(15, 35, 180, 20), $"Smooth: {smoothingFactor:F2}");
        if (gazeRect != null)
            GUI.Label(new Rect(15, 55, 180, 20), $"Pos: ({gazeRect.anchoredPosition.x:F0}, {gazeRect.anchoredPosition.y:F0})");
    }
}