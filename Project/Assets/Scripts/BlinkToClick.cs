using UnityEngine;
using Tobii.Gaming;
using System.Collections;

public class BlinkToClick : MonoBehaviour
{
    [Header("Click Settings")]
    public bool enableSpacebarClick = true;
    public float clickCooldown = 0.5f;
    
    [Header("Detection")]
    public bool showDebugInfo = true;
    
    private GameObject focusedObject;
    private float lastClickTime = 0f;
    
    // Visual feedback
    private GameObject clickFeedback;
    
    void Start()
    {
        CreateClickFeedback();
        Debug.Log("BlinkToClick: Spacebar clicking enabled (blink functionality removed)");
    }
    
    void Update()
    {
        if (!enableSpacebarClick) return;
        
        try
        {
            GameObject newFocused = TobiiAPI.GetFocusedObject();
            
            // Update focused object
            if (newFocused != focusedObject)
            {
                if (focusedObject != null)
                    SendMouseEvent(focusedObject, "OnMouseExit");
                if (newFocused != null)
                    SendMouseEvent(newFocused, "OnMouseEnter");
                focusedObject = newFocused;
            }
            
            // Check for spacebar input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TriggerSpacebarClick();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Click detection error: {e.Message}");
        }
    }
    
    void TriggerSpacebarClick()
    {
        if (Time.time - lastClickTime < clickCooldown) return;
        
        if (focusedObject != null)
        {
            Debug.Log($"SPACEBAR CLICK on {focusedObject.name}!");
            SimulateClick(focusedObject);
            ShowClickFeedback();
            lastClickTime = Time.time;
        }
        else
        {
            Debug.Log("SPACEBAR pressed but no object focused");
        }
    }
    
    void SimulateClick(GameObject target)
    {
        SendMouseEvent(target, "OnMouseDown");
        StartCoroutine(DelayedMouseUp(target));
    }
    
    IEnumerator DelayedMouseUp(GameObject target)
    {
        yield return new WaitForSeconds(0.1f);
        SendMouseEvent(target, "OnMouseUp");
    }
    
    void SendMouseEvent(GameObject target, string eventMethod)
    {
        if (target != null)
            target.SendMessage(eventMethod, SendMessageOptions.DontRequireReceiver);
    }
    
    void CreateClickFeedback()
    {
        clickFeedback = new GameObject("ClickFeedback");
        Canvas canvas = clickFeedback.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1001;
        
        GameObject indicator = new GameObject("Indicator");
        indicator.transform.SetParent(clickFeedback.transform);
        
        var image = indicator.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.blue;  // Changed to blue to distinguish from blink feedback
        
        // Simple circle texture
        Texture2D tex = new Texture2D(16, 16);
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(8, 8));
                tex.SetPixel(x, y, dist <= 6 ? Color.white : Color.clear);
            }
        }
        tex.Apply();
        image.sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f));
        
        var rect = indicator.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(40, 40);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        
        clickFeedback.SetActive(false);
    }
    
    void ShowClickFeedback()
    {
        StartCoroutine(FlashFeedback());
    }
    
    IEnumerator FlashFeedback()
    {
        clickFeedback.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        clickFeedback.SetActive(false);
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUI.Box(new Rect(270, 10, 200, 80), "");
        GUI.Label(new Rect(275, 15, 190, 20), $"Spacebar Click: {enableSpacebarClick}");
        GUI.Label(new Rect(275, 35, 190, 20), $"Focused: {focusedObject?.name ?? "None"}");
        GUI.Label(new Rect(275, 55, 190, 20), "Press SPACE to click");
        
        if (GUI.Button(new Rect(275, 70, 80, 15), "Test Click"))
        {
            TriggerSpacebarClick();
        }
    }
}