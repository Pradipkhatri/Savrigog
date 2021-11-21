using UnityEngine;

public interface IDamageable
{
	void TakeDamage(int DamageType, int Damagerate, bool SwordContact);
}