using UnityEngine;

public class ChaserEnemy : MonoBehaviour
{
	public float moveSpeed = 0.6f;
	private Transform target;
	private Collider2D enemyCollider;
	private Vector3 baseScale;
	private float chargeTime;
	private const float chargeThreshold = 0.5f;
	private bool hasExploded;

	void Awake()
	{
		target = GameObject.Find("Player")?.transform ?? GameObject.Find("Player(Clone)")?.transform;
		enemyCollider = GetComponent<Collider2D>();
		if (enemyCollider == null)
		{
			enemyCollider = gameObject.AddComponent<CircleCollider2D>();
			((CircleCollider2D)enemyCollider).isTrigger = true;
		}
		baseScale = transform.localScale;
	}

	void Update()
	{
		if (BackAction.onPause) return;
		if (target == null)
		{
			target = GameObject.Find("Player")?.transform ?? GameObject.Find("Player(Clone)")?.transform;
			if (target == null) return;
		}
		Vector3 direction = (target.position - transform.position).normalized;
		transform.position += direction * moveSpeed * Time.deltaTime;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		float t = Mathf.Clamp01(chargeTime / chargeThreshold);
		float jitter = Mathf.Sin(Time.time * 40f) * 5f * t;
		transform.rotation = Quaternion.AngleAxis(angle + 180f + jitter, Vector3.forward);

		// Hover charge using mouse cursor (desktop)
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
		Vector3 mouseWorld = Camera.main != null ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : transform.position;
		bool hovered = enemyCollider != null && enemyCollider.OverlapPoint(new Vector2(mouseWorld.x, mouseWorld.y));
		chargeTime = Mathf.Clamp(chargeTime + (hovered ? Time.deltaTime : -Time.deltaTime), 0f, chargeThreshold);
		transform.localScale = baseScale * (1f + 0.15f * t);
		if (!hasExploded && chargeTime >= chargeThreshold)
		{
			Explode();
			return;
		}
#endif
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (hasExploded) return;
		if (other != null && other.CompareTag("Player"))
		{
			Explode();
		}
	}

	public void Explode(bool countAsKill = true)
	{
		if (hasExploded) return;
		hasExploded = true;
		// Use player's standard explosion particle system
		var playerGo = GameObject.Find("Player") ?? GameObject.Find("Player(Clone)");
		if (playerGo != null)
		{
			var playerPs = playerGo.GetComponentInChildren<ParticleSystem>(true);
			if (playerPs != null)
			{
				var psInstance = Object.Instantiate(playerPs, transform.position, Quaternion.identity);
				psInstance.gameObject.SetActive(true);
				psInstance.Play();
				var main = psInstance.main;
				float ttl = main.duration + main.startLifetimeMultiplier + 0.5f;
				Object.Destroy(psInstance.gameObject, ttl);
			}
		}
		if (countAsKill)
		{
			var manager = GameObject.FindObjectOfType<EnemyManager>();
			if (manager != null)
			{
				manager.RegisterKill();
			}
		}
		Object.Destroy(gameObject);
	}
}
