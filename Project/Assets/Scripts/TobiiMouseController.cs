using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class TobiiMouseController : MonoBehaviour
{
    [Header("Eye Tracking Settings")]
    public bool enableEyeTracking = true;
    public bool showGazeCursor = true;
    
    [Header("Gaze Cursor")]
    public GameObject gazeCursorPrefab;
    
    private GameObject gazeCursor;
    private Camera mainCamera;
    private GameObject lastFocusedObject;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
            
        // Create gaze cursor if enabled and prefab is provided
        if (showGazeCursor && gazeCursorPrefab != null)
        {
            gazeCursor = Instantiate(gazeCursorPrefab);
        }
    }
    
    private void Update()
    {
        // Only process eye tracking on desktop platforms
        if (!enableEyeTracking || 
            Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (gazeCursor != null)
                gazeCursor.SetActive(false);
            return;
        }
        
        ProcessEyeTracking();
    }
    
    private void ProcessEyeTracking()
    {
        try
        {
            // Get gaze point and convert to screen coordinates
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            if (gazePoint.IsRecent())
            {
                Vector2 gazePosition = gazePoint.Screen;
                UpdateGazeCursor(gazePosition);
                
                // Convert screen position to world position for mouse events
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(gazePosition.x, gazePosition.y, mainCamera.nearClipPlane));
                
                // Handle focused object
                GameObject focusedObject = TobiiAPI.GetFocusedObject();
                HandleFocusedObject(focusedObject);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Tobii API error: {e.Message}");
        }
    }
    
    private void UpdateGazeCursor(Vector2 gazePosition)
    {
        if (gazeCursor != null)
        {
            gazeCursor.SetActive(true);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(gazePosition.x, gazePosition.y, 10f));
            gazeCursor.transform.position = worldPos;
        }
    }
    
    private void HandleFocusedObject(GameObject focusedObject)
    {
        // Handle mouse enter/exit events
        if (focusedObject != lastFocusedObject)
        {
            // Mouse exit previous object
            if (lastFocusedObject != null)
            {
                SendMouseEvent(lastFocusedObject, "OnMouseExit");
            }
            
            // Mouse enter new object
            if (focusedObject != null)
            {
                SendMouseEvent(focusedObject, "OnMouseEnter");
            }
            
            lastFocusedObject = focusedObject;
        }
    }
    
    private IEnumerator DelayedMouseUp(GameObject target)
    {
        yield return new WaitForSeconds(0.1f);
        SendMouseEvent(target, "OnMouseUp");
    }
    
    private void SendMouseEvent(GameObject target, string eventMethod)
    {
        if (target != null)
        {
            target.SendMessage(eventMethod, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    public void EnableEyeTracking(bool enable)
    {
        enableEyeTracking = enable;
    }
}
