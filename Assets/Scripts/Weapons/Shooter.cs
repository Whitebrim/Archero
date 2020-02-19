using UnityEngine;
using System.Collections.Generic;

public class Shooter : MonoBehaviour
{
	[SerializeField] GameObject shellPrefab;
	[SerializeField] int poolSize = 20;
	Queue<GameObject> pool;

    private void Awake()
	{
		pool = new Queue<GameObject>(poolSize);
		ResizePool(poolSize);
	}

    #region Pool

	public void ResizePool(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			var obj = Instantiate(shellPrefab);
			obj.SetActive(false);
			pool.Enqueue(obj);
		}
	}

	public void Dispose()
	{
		while (pool.Count > 0)
		{
			Destroy(pool.Dequeue());
		}
	}
    #endregion

	/// <summary>
	/// Calls when shell needs to go back to the pool
	/// </summary>
	/// <param name="shell"></param>
    public void DeactivateShell(GameObject shell)
	{
		shell.SetActive(false);
		shell.GetComponent<Collider>().enabled = true;
		try
		{
			shell.transform.position = transform.position;
			pool.Enqueue(shell);
		}
		catch (System.Exception e)
		{
			//Debug.Log("Shooted of this shell is already dead");
			Destroy(shell);
		}
	}

	/// <summary>
	/// Shoots one prefab Shell with given DamageReport forward
	/// </summary>
	/// <param name="damageReport">damage info</param>
	public void Shoot(DamageReport damageReport)
	{
		var newShell = pool.Dequeue();
		newShell.transform.position = transform.position;
		newShell.transform.rotation = transform.rotation;
		newShell.SetActive(true);
		newShell.GetComponent<Shell>().Shoot(this, damageReport);
		if (pool.Count <= 1)
			ResizePool(poolSize);
	}
}
