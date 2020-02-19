using UnityEngine;

public abstract class Aimer : MonoBehaviour
{
	[SerializeField] [ReadOnly] private Transform target; //Ссылка на цель, если она есть - иначе null
	[SerializeField] LayerMask obstacleMask = 256; // Obstacles
	protected float minDistance;
	GameObject owner;
	public Transform Target
	{
		get { return target; }
		protected set { target = value; }
	}

	private void Awake()
	{
		if (owner == null)
			owner = transform.parent.gameObject;
	}

	/// <summary>
	/// Raycasts target and checks if Ryacast hitted Obstacles
	/// </summary>
	/// <param name="target"></param>
	/// <returns>True if it's visible, else false</returns>
	protected bool IsVisible(Transform target)
	{
		Vector3 dirToTarget = (target.position - transform.position).normalized;
		float dstToTarget = Vector3.Distance(transform.position, target.position);
		if (minDistance == -1 || minDistance > dstToTarget)
		{
			if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
			{
				minDistance = dstToTarget;
				this.target = target;
				return true;
			}
		}
		return false;
	}

	public bool IsVisible()
	{
		Vector3 dirToTarget = (target.position - transform.position).normalized;
		float dstToTarget = Vector3.Distance(transform.position, target.position);
		return !Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask);
	}

	public void ResetTarget()
	{
		target = null;
	}

	public void FollowTarget()
	{
		owner.transform.LookAt(target);
	}
}
