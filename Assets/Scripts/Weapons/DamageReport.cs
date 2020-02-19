[System.Serializable]
public struct DamageReport
{
	public float damage;
	public Entity attacker;

	/// <summary>
	/// Damage info
	/// </summary>
	/// <param name="damage">Amount of damage</param>
	/// <param name="attacker">Reference to attacker</param>
	public DamageReport(float damage, Entity attacker)
	{
		this.damage = damage;
		this.attacker = attacker;
	}
}
