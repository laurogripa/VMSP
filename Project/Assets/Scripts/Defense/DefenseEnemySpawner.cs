using System.Collections;
using UnityEngine;

public class DefenseEnemySpawner : MonoBehaviour
{
	[SerializeField] private GameObject spawnPrefab;
	[SerializeField] private float spawnRadius = 5f;
	[SerializeField] private float spawnIntervalSeconds = 1.25f;
	[SerializeField] private int spawnPerWave = 2;
	[SerializeField] private int maxSimultaneousEnemies = 30;
	[SerializeField] private float chaserSpeed = 0.4f;

	private Transform player;
	private GameObject sceneTemplate;

	public void SetTemplate(GameObject template)
	{
		sceneTemplate = template;
	}

	void Start()
	{
		player = GameObject.Find("Player")?.transform ?? GameObject.Find("Player(Clone)")?.transform;
		StartCoroutine(SpawnLoop());
	}

	private IEnumerator SpawnLoop()
	{
		var wait = new WaitForSeconds(spawnIntervalSeconds);
		while (true)
		{
			yield return wait;
			if (player == null)
			{
				player = GameObject.Find("Player")?.transform ?? GameObject.Find("Player(Clone)")?.transform;
				if (player == null) continue;
			}
			int alive = CountAlive();
			for (int i = 0; i < spawnPerWave && alive < maxSimultaneousEnemies; i++)
			{
				var template = GetTemplate();
				if (template == null) break;
				Vector2 dir = Random.insideUnitCircle.normalized;
				Vector3 pos = player.position + new Vector3(dir.x, dir.y, 0f) * spawnRadius;
				var enemy = Instantiate(template, pos, Quaternion.identity);
				enemy.SetActive(true);
				PrepareSpawnedEnemy(enemy);
				alive++;
			}
		}
	}

	private GameObject GetTemplate()
	{
		if (sceneTemplate != null) return sceneTemplate;
		if (spawnPrefab != null) return spawnPrefab;
		var anyEnemy = FindObjectOfType<EnemyBehavior>();
		if (anyEnemy != null)
		{
			sceneTemplate = anyEnemy.gameObject;
		}
		return sceneTemplate;
	}

	private void PrepareSpawnedEnemy(GameObject enemy)
	{
		AttachToGameElements(enemy.transform);
		var eb = enemy.GetComponent<EnemyBehavior>();
		if (eb != null) eb.enabled = false;
		var chaser = enemy.GetComponent<ChaserEnemy>();
		if (chaser == null) chaser = enemy.AddComponent<ChaserEnemy>();
		chaser.moveSpeed = chaserSpeed;
		enemy.tag = "Enemy";
		var col = enemy.GetComponent<Collider2D>();
		if (col != null) col.isTrigger = true;
		var sr = enemy.GetComponent<SpriteRenderer>();
		if (sr != null)
		{
			Color c = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.8f, 1f);
			sr.color = c;
		}
	}

	private void AttachToGameElements(Transform t)
	{
		var ge = GameObject.Find("GameElements") ?? GameObject.Find("GameElements(Clone)");
		if (ge != null) t.SetParent(ge.transform);
	}

	private int CountAlive()
	{
#if UNITY_2023_1_OR_NEWER
		return FindObjectsByType<ChaserEnemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
#else
		return FindObjectsOfType<ChaserEnemy>().Length;
#endif
	}
}
