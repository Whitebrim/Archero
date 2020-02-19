using UnityEngine;
using UnityEngine.AI;

public abstract class WalkingEnemy : Enemy
{
    float movingStateTimer = 0;
    NavMeshAgent agent;

    protected new void Awake()
    {
        base.Awake();
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        agent.speed = Speed;
    }

    protected void FixedUpdate()
    {
        switch (walkingState)
        {
            case MovingState.MOVING:
                if (Time.time - movingStateTimer >= movingTime)
                {
                    walkingState = MovingState.STAYING;
                    movingStateTimer = Time.time;
                }
                else
                {
                    agent.destination = player.componentCache.position;
                }
                break;
            case MovingState.STAYING:
                if(Time.time - movingStateTimer >= waitingTime)
                {
                    if (touchingPlayer != null)
                    {
                        if (touchingPlayer.TakeDamage(new DamageReport(damage * touchDamageMultiplier, this)))
                            touchingPlayer = null;
                        movingStateTimer = Time.time;
                    }
                    else
                    {
                        walkingState = MovingState.MOVING;
                        movingStateTimer = Time.time + Random.Range(0, randomTime);
                    }
                }
                else
                {
                    agent.destination = transform.position;
                }
                break;
            default:
                break;
        }
    }

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.tag == Tags.playerTag)
        {
            walkingState = MovingState.STAYING;
            movingStateTimer = Time.time;
        }
    }
}
