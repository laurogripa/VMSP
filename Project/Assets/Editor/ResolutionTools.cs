using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public static class ResolutionTools
{
    [MenuItem("Tools/Set Default Resolution 1920x1080")]
    public static void SetDefaultResolution()
    {
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        Debug.Log("Set default resolution to 1920x1080");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Set CanvasScaler 1920x1080")] 
    public static void SetCanvasScaler()
    {
        var scalers = Object.FindObjectsOfType<CanvasScaler>(true);
        foreach (var scaler in scalers)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            EditorUtility.SetDirty(scaler);
        }
        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Applied CanvasScaler 1920x1080 to all canvases in open scenes.");
    }
}
