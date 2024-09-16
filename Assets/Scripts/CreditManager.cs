using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour 
{
	public GameObject m_ViejaPrefab;
	public GameObject m_GordoPrefab;

	void Awake() 
	{
		//DontDestroyOnLoad(transform.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Application.LoadLevel(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Application.LoadLevel(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Application.LoadLevel(2);
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}


	public void LoadBattleScene()
	{
		Application.LoadLevel(2);
	}


	public void InstantiateSprites()
	{
		m_ViejaPrefab.SetActive(true);
		m_GordoPrefab.SetActive(true);
	}
}
