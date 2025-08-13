using UnityEngine;
using Tobii.Gaming;

public class QuickBlinkTest : MonoBehaviour
{
    private GameObject focusedObject;
    
    void Update()
    {
        try
        {
            // Get current focused object
            GameObject newFocused = TobiiAPI.GetFocusedObject();
            
            if (newFocused != focusedObject)
            {
                if (focusedObject != null)
                    Debug.Log($"Stopped looking at: {focusedObject.name}");
                if (newFocused != null)
                    Debug.Log($"Now looking at: {newFocused.name}");
                focusedObject = newFocused;
            }
            
            // SPACE key to test clicking
            if (Input.GetKeyDown(KeyCode.Space) && focusedObject != null)
            {
                Debug.Log($"*** SPACE CLICK TEST on {focusedObject.name} ***");
                
                // Send mouse events
                focusedObject.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
                StartCoroutine(DelayedMouseUp());
                
                // Visual flash
                CreateFlash();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }
    
    System.Collections.IEnumerator DelayedMouseUp()
    {
        yield return new WaitForSeconds(0.1f);
        if (focusedObject != null)
        {
            focusedObject.SendMessage("OnMouseUp", SendMessageOptions.DontRequireReceiver);
            Debug.Log($"Sent OnMouseUp to {focusedObject.name}");
        }
    }
    
    void CreateFlash()
    {
        GameObject flash = new GameObject("Flash");
        Canvas canvas = flash.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        
        GameObject rect = new GameObject("Rect");
        rect.transform.SetParent(flash.transform);
        var image = rect.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.green;
        
        var rectTransform = rect.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 200);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        StartCoroutine(DestroyFlash(flash));
    }
    
    System.Collections.IEnumerator DestroyFlash(GameObject flash)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(flash);
    }
    
    void OnGUI()
    {
        GUI.Box(new Rect(10, 400, 300, 80), "");
        GUI.Label(new Rect(15, 405, 290, 20), "QUICK BLINK TEST");
        GUI.Label(new Rect(15, 425, 290, 20), $"Looking at: {focusedObject?.name ?? "Nothing"}");
        GUI.Label(new Rect(15, 445, 290, 20), "Press SPACE while looking at button");
        GUI.Label(new Rect(15, 465, 290, 20), "Watch console for click events");
    }
}