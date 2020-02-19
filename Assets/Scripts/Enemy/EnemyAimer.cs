using UnityEngine;
using ThirteenPixels.Soda;
public class EnemyAimer : Aimer
{
	[SerializeField] GlobalTransform player;
	[SerializeField] GameEvent onPlayerDeath;
	private void OnEnable()
	{
		onPlayerDeath.onRaise.AddResponse(ResetTarget);
	}
	private void OnDisable()
	{
		onPlayerDeath.onRaise.RemoveResponse(ResetTarget);
	}

	/// <summary>
	/// Finds nearest target if it's visible
	/// </summary>
	/// <returns>True if target was found, else false</returns>
	public bool Aim()
	{
		bool success = false;
		minDistance = -1;
		success = IsVisible(player.componentCache);
		return success;
	}
}
