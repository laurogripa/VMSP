using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : MonoBehaviour
{
	[SerializeField] private string sceneName = "Genius";
	[SerializeField] private bool autoActivateWhenReady = true;
	[SerializeField] private float minShowSeconds = 1f;
	[SerializeField] private bool startLoadingOnAwake = true;

	public float Progress01 { get; private set; }
	private AsyncOperation loadOperation;
	private float startTime;
	private bool isLoading = false;

	private void Start()
	{
		if (startLoadingOnAwake)
		{
			StartLoading();
		}
	}

	public void StartLoading()
	{
		if (isLoading) return;
		isLoading = true;
		startTime = Time.unscaledTime;
		StartCoroutine(BeginLoad());
	}

	private System.Collections.IEnumerator BeginLoad()
	{
		if (string.IsNullOrEmpty(sceneName)) yield break;
		loadOperation = SceneManager.LoadSceneAsync(sceneName);
		if (loadOperation == null) yield break;
		loadOperation.allowSceneActivation = false;

		while (!loadOperation.isDone)
		{
			Progress01 = Mathf.Clamp01(loadOperation.progress / 0.9f);
			bool ready = Progress01 >= 1f;
			bool minTimeElapsed = (Time.unscaledTime - startTime) >= minShowSeconds;

			if (ready && autoActivateWhenReady && minTimeElapsed)
			{
				loadOperation.allowSceneActivation = true;
			}
			yield return null;
		}
	}

	public void ActivateNow()
	{
		if (loadOperation != null)
		{
			loadOperation.allowSceneActivation = true;
		}
	}
}
