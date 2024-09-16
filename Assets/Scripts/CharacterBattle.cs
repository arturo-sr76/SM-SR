using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum AttackType
{
	Damage,
	Heal
}

[System.Serializable]
public struct Attack
{
	public string id;
	public AttackType type;
	public int amount;
	
	public Attack(string pId, AttackType pType, int pAmount)
	{
		id = pId;
		type = pType;
		amount = pAmount;
	}
}

[System.Serializable]
public class CharacterBattle : MonoBehaviour 
{
	public int m_MaxHp;
	public int m_CurrentHp;
	public bool m_isDead;
	public Attack[] m_AttackList;
	protected bool m_IsPlayer = false;
	public GameObject m_BattleManager;
	protected BattleManager m_BattleScript;


	void Awake()
	{
		m_BattleScript = m_BattleManager.GetComponent<BattleManager>();
	}

	// Use this for initialization
	void Start () 
	{
		m_isDead = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void Attack (string pId)
	{
		Attack attack = GetAttack (pId);
		if (attack.type == AttackType.Damage)
		{
			if (m_IsPlayer)
			{
				m_BattleScript.m_Enemy.SendMessage("TakeDamage", attack.amount);
			}
			else
			{
				m_BattleScript.m_Player.SendMessage("TakeDamage", attack.amount);
			}
			//TakeDamage (attack.amount);
			print ("Attack " + pId + " deals " + attack.amount + " damage.");
		}
		if (attack.type == AttackType.Heal)
		{
			HealDamage(attack.amount);
			print ("Attack " + pId + " heals " + attack.amount + " damage.");
		}

		m_BattleScript.PlayAnimation(pId);
	}


	public Attack GetAttack (string pId)
	{
		foreach (Attack attack in m_AttackList)
		{
			if (attack.id == pId)
			{
				return attack;
			}
		}
		return new Attack(pId, AttackType.Damage, 0);
	}
	
	
	public void TakeDamage (int pAmount)
	{
		m_CurrentHp -= pAmount;
		if (m_CurrentHp <= 0)
		{
			m_CurrentHp = 0;
			m_isDead = true;
		}
	}
	
	
	public void HealDamage (int pAmount)
	{
		m_CurrentHp += pAmount;
		if (m_CurrentHp > m_MaxHp)
		{
			m_CurrentHp = m_MaxHp;
		}
	}

}
