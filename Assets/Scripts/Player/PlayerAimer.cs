using UnityEngine;

public class PlayerAimer : Aimer
{
	[SerializeField] GlobalEnemyHandler enemyHandler;

	/// <summary>
	/// Finds nearest target if it's visible
	/// </summary>
	/// <returns>True if target was found, else false</returns>
	public bool Aim()
	{
		bool success = false;
		minDistance = -1;
		foreach (var enemies in enemyHandler.componentCache.Enemies)
		{
			success = IsVisible(enemies.transform);
		}
		return success;
	}
}
