using System.Collections.Generic;
using UnityEngine;
using ThirteenPixels.Soda;
public class EnemyHandler : MonoBehaviour
{
	[SerializeField] GameEvent onLevelPassed;
	[SerializeField] private List<Enemy> enemies;

	public List<Enemy> Enemies
	{
		get { return enemies; }
		private set { enemies = value; }
	}

	public void AddEnemy(Enemy enemy)
	{
		enemies.Add(enemy);
	}

	/// <summary>
	/// Removes enemy from enemies list, if list is empty raises onLevelPassed SODA event
	/// </summary>
	/// <param name="enemy"></param>
	public void RemoveEnemy(Enemy enemy)
	{
		enemies.Remove(enemy);
		if (enemies.Count == 0)
			onLevelPassed.Raise();
	}
}
