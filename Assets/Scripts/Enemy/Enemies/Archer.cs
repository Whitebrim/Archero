using UnityEngine;

public class Archer : WalkingEnemy
{
	[SerializeField] Shooter shooter;
	[SerializeField] EnemyAimer aimer;
	float lastShootTime;

	protected override void Death(Entity killer)
	{
		shooter.Dispose();
		base.Death(killer);
	}

	protected new void Awake()
	{
		base.Awake();
		if (shooter == null)
			shooter = GetComponentInChildren<Shooter>();
		if (aimer == null)
			aimer = GetComponentInChildren<EnemyAimer>();
	}

	protected void Update()
	{
		if (aimer.Target != null)
		{
			walkingState = MovingState.STAYING;
			aimer.FollowTarget();
			if (Time.time - lastShootTime >= (1 / attackSpeed))
			{
				lastShootTime = Time.time;
				shooter.Shoot(new DamageReport(damage, this));
			}
		}
	}
	protected new void FixedUpdate()
	{
		base.FixedUpdate();
		if(walkingState == MovingState.STAYING)
		{
			if (aimer.Target == null)
				aimer.Aim();
			else if (!aimer.IsVisible())
				aimer.ResetTarget();
		}
		
	}
}
