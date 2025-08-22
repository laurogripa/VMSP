using UnityEngine;

public class ChaserEnemy : MonoBehaviour
{
	public float moveSpeed = 0.6f;
	private Transform target;

	void Awake()
	{
		target = GameObject.Find("Player")?.transform ?? GameObject.Find("Player(Clone)")?.transform;
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
		transform.rotation = Quaternion.AngleAxis(angle + 180f, Vector3.forward);
	}
}
