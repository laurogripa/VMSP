using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class DefenseAutoStart : MonoBehaviour
{
	private bool _started;

	void Start()
	{
		if (_started) return;
		_started = true;
		StartCoroutine(AutoStart());
	}

	private IEnumerator AutoStart()
	{
		// wait a few frames to let scene objects spawn
		yield return null;
		yield return null;
		yield return null;

		StageManager manager = null;
		for (int i = 0; i < 120 && manager == null; i++)
		{
			manager = Object.FindFirstObjectByType<StageManager>();
			yield return null;
		}
		if (manager != null)
		{
			manager.NoIntro();
		}
	}
}
