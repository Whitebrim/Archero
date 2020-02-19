using UnityEngine;

public abstract class Shell : MonoBehaviour
{
	protected const float timeToDisappear = 2f;
	protected enum FlyingState
	{
		FLYING,
		STUCK
	}

	[SerializeField] protected float speed = 1;
	[SerializeField] protected Rigidbody rigidbody;
	protected DamageReport damageReport;
	protected Shooter shooter;
	protected FlyingState flyingState = FlyingState.FLYING;
	protected float stuckTime; // Time, that represents Time.time when shell stuck in wall

	private void Awake()
	{
		if (rigidbody == null)
			rigidbody = GetComponent<Rigidbody>();
	}

	/// <summary>
	/// Called when this shell is shot
	/// </summary>
	/// <param name="shooter"></param>
	/// <param name="damageReport"></param>
	public void Shoot(Shooter shooter, DamageReport damageReport)
	{
		this.damageReport = damageReport;
		this.shooter = shooter;
		rigidbody.velocity = transform.forward * speed;
	}

	protected void OnTriggerEnter(Collider other)
	{
		string tag = other.tag;
		switch (tag)
		{
			default:
			case Tags.obstacleTag:
				OnObstacleCollision(other.transform);
				break;
			case Tags.enemyTag:
				OnEnemyCollision(other.GetComponent<Entity>());
				break;
			case Tags.playerTag:
				OnPlayerCollision(other.GetComponent<Entity>());
				break;
		}
	}
	protected abstract void OnObstacleCollision(Transform obstacle);
	protected abstract void OnEnemyCollision(Entity entity);
	protected abstract void OnPlayerCollision(Entity entity);
}
