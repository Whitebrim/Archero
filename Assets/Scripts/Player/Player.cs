using UnityEngine;
using ThirteenPixels.Soda;

public class Player : Entity
{
	[SerializeField] GameEvent onPlayerDeath;
	[SerializeField] GlobalVector2 input;
	[SerializeField] Shooter shooter;
	[SerializeField] PlayerAimer aimer;
	[SerializeField][ReadOnly] long coins = 0;
	float lastShootTime;

	private void OnEnable()
	{
		input.onChange.AddResponse(CheckMovementState);
	}

	private void OnDisable()
	{
		input.onChange.RemoveResponse(CheckMovementState);
	}

	protected new void Awake()
	{
		base.Awake();
		if (shooter == null)
			shooter = GetComponentInChildren<Shooter>();
		if (aimer == null)
			aimer = GetComponentInChildren<PlayerAimer>();
	}

	protected override void Death(Entity killer)
	{
		onPlayerDeath.Raise();
		Debug.Log("Player is DEAD");
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Adds coins on Enemy death
	/// </summary>
	/// <param name="amount"></param>
	public void AddCoins(int amount)
	{
		coins += amount;
	}

	/// <summary>
	/// Called when Input Vector2 is changed
	/// </summary>
	/// <param name="direction"></param>
	private void CheckMovementState(Vector2 direction)
	{
		if(walkingState == MovingState.STAYING && direction != Vector2.zero)
			walkingState = MovingState.MOVING;
		else
		{
			if (walkingState == MovingState.MOVING && direction == Vector2.zero)
				walkingState = MovingState.STAYING;
		}
	}

	private void Update()
	{
		if (walkingState == MovingState.STAYING)
		{
			if (aimer.Target != null)
			{
				aimer.FollowTarget();
				if (Time.time - lastShootTime >= (1 / attackSpeed))
				{
					lastShootTime = Time.time;
					shooter.Shoot(new DamageReport(damage, this));
				}
			}
		}
	}
	private void FixedUpdate()
	{
		if (aimer.Target == null)
			aimer.Aim();
		else if (!aimer.IsVisible())
			aimer.ResetTarget();
	}
}
