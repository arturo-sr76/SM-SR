using UnityEngine;
using System.Collections;

public class Lifebar : Fillbar 
{
	protected override float GetAmountToFill ()
	{
		CharacterBattle target;
		if (m_IsPlayers)
		{
			target = m_BattleManager.m_PlayerScript;
		}
		else
		{
			target = m_BattleManager.m_EnemyScript;
		}
		if ( target.m_MaxHp != 0)
		{
			return (float)target.m_CurrentHp / (float)target.m_MaxHp;
		}
		return 0;
	}
}
