using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyBattle : CharacterBattle 
{

	// Use this for initialization
	void Start () 
	{
		m_IsPlayer = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


	public void ChooseAttack()
	{
		int i = Random.Range(0, m_AttackList.Length);
		Attack(m_AttackList[i].id);
		if (m_AttackList[i].id == "Galleta")
		{
			m_BattleScript.ShowGalleta();
		}
	}
}
