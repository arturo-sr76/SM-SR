using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Fillbar : MonoBehaviour 
{
	public BattleManager m_BattleManager;
	public bool m_IsPlayers;
	private Image m_image;
	private float m_AmountToFill;
	
	
	void Awake()
	{
		m_image = GetComponent<Image>();
	}
	

	// Update is called once per frame
	void Update () 
	{
		m_AmountToFill = GetAmountToFill();
		m_image.fillAmount = m_AmountToFill;
	}

	protected abstract float GetAmountToFill ();
}
