using UnityEngine;
using System.Collections;

public class Timebar : Fillbar 
{
	protected override float GetAmountToFill ()
	{
		float time = 0;
		if (m_IsPlayers)
		{
			time = (float)m_BattleManager.m_PlayerTime / (float)m_BattleManager.m_PlayerCooldownTime;
		}
		else
		{
			time = (float)m_BattleManager.m_EnemyTime / (float)m_BattleManager.m_EnemyCooldownTime;
		}
		return time;
	}
}
