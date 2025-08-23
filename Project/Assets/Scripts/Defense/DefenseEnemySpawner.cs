using System.Collections;
using UnityEngine;

public class DefenseEnemySpawner : MonoBehaviour
{
	[SerializeField] private GameObject spawnPrefab;
	[SerializeField] private float spawnRadius = 5f;
	[SerializeField] private float spawnIntervalSeconds = 1.25f;
	[SerializeField] private int spawnPerWave = 2;
	[SerializeField] private int maxSimultaneousEnemies = 30;
	[SerializeField] private float baseChaserSpeed = 0.42f; // Reduced by 30%
	[SerializeField] private float speedIncreasePerKill = 0.02f; // 2% per kill

	private Transform player;
	private GameObject sceneTemplate;
	private GameObject runtimeTemplate;
	private int enemiesKilled = 0;
	private float currentChaserSpeed;
	
	public static DefenseEnemySpawner Instance { get; private set; }

	public void SetTemplate(GameObject template)
	{
		sceneTemplate = template;
	}

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		player = GameObject.Find("Player")?.transform ?? GameObject.Find("Player(Clone)")?.transform;
		currentChaserSpeed = baseChaserSpeed;
		StartCoroutine(SpawnLoop());
	}

	public void OnEnemyKilled()
	{
		enemiesKilled++;
		currentChaserSpeed = baseChaserSpeed * (1f + (enemiesKilled * speedIncreasePerKill));
	}

	private IEnumerator SpawnLoop()
	{
		var wait = new WaitForSeconds(spawnIntervalSeconds);
		while (true)
		{
			if (BackAction.onGameOver) yield break;
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
			return sceneTemplate;
		}
		
		if (runtimeTemplate == null)
		{
			runtimeTemplate = CreateRuntimeEnemyTemplate();
		}
		return runtimeTemplate;
	}

	private GameObject CreateRuntimeEnemyTemplate()
	{
		var template = new GameObject("RuntimeEnemyTemplate");
		template.SetActive(false);
		
		var sr = template.AddComponent<SpriteRenderer>();
		var playerSR = player?.GetComponent<SpriteRenderer>();
		if (playerSR != null && playerSR.sprite != null)
		{
			sr.sprite = playerSR.sprite;
		}
		else
		{
			var planeSprite = Resources.Load<Sprite>("Defense/plane");
			sr.sprite = planeSprite;
		}
		
		sr.color = Color.red;
		var col = template.AddComponent<CircleCollider2D>();
		col.isTrigger = true;
		col.radius = 0.6f; // Increased tolerance
		template.transform.localScale = Vector3.one * 0.5f;
		
		return template;
	}

	private void PrepareSpawnedEnemy(GameObject enemy)
	{
		AttachToGameElements(enemy.transform);
		var eb = enemy.GetComponent<EnemyBehavior>();
		if (eb != null) eb.enabled = false;
		var chaser = enemy.GetComponent<ChaserEnemy>();
		if (chaser == null) chaser = enemy.AddComponent<ChaserEnemy>();
		chaser.moveSpeed = currentChaserSpeed;
		enemy.tag = "Enemy";
		var col = enemy.GetComponent<CircleCollider2D>();
		if (col != null) 
		{
			col.isTrigger = true;
			col.radius = 0.6f; // Increased tolerance
		}
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
