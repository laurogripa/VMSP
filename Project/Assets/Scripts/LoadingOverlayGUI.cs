using UnityEngine;

[RequireComponent(typeof(AsyncSceneLoader))]
public class LoadingOverlayGUI : MonoBehaviour
{
	[SerializeField] private string loadingText = "Loading stage2...";
	[SerializeField] private Rect progressRect = new Rect(0.1f, 0.85f, 0.8f, 0.05f);
	[SerializeField] private Color barBg = new Color(0f, 0f, 0f, 0.6f);
	[SerializeField] private Color barFg = new Color(1f, 1f, 1f, 0.9f);
	[SerializeField] private int fontSize = 24;

	private AsyncSceneLoader loader;

	private void Awake()
	{
		loader = GetComponent<AsyncSceneLoader>();
	}

	private void OnGUI()
	{
		if (loader == null) return;

		float w = Screen.width;
		float h = Screen.height;

		Rect bar = new Rect(progressRect.x * w, progressRect.y * h, progressRect.width * w, progressRect.height * h);
		// Background
		Color prev = GUI.color;
		GUI.color = barBg;
		GUI.Box(bar, GUIContent.none);

		// Foreground fill
		GUI.color = barFg;
		Rect fill = new Rect(bar.x + 2f, bar.y + 2f, Mathf.Max(0f, (bar.width - 4f) * Mathf.Clamp01(loader.Progress01)), bar.height - 4f);
		GUI.Box(fill, GUIContent.none);

		// Text centered
		GUI.color = Color.white;
		var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = fontSize, wordWrap = false };
		GUI.Label(new Rect(0, bar.y - 40f, w, 30f), loadingText, style);
		GUI.color = prev;
	}
}
