using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour 
{

	public GameObject m_PlayerCamera;
	private Camera m_PlayerCameraComponent;
	private Camera m_CameraComponent;
	private Animator m_animator;


	// Use this for initialization
	void Start () 
	{
		m_CameraComponent = gameObject.GetComponent<Camera>();
		m_PlayerCameraComponent = m_PlayerCamera.GetComponent<Camera>();
		m_animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			PlayCinematic();
		}
	}

	public void OnTravelFinished ()
	{
		m_CameraComponent.tag = "Untagged";
		m_CameraComponent.enabled = false;
		m_PlayerCameraComponent.enabled = true;
		m_PlayerCameraComponent.tag = "MainCamera";
	}

	public void PlayCinematic ()
	{
		m_CameraComponent.tag = "MainCamera";
		m_CameraComponent.enabled = true;
		m_PlayerCameraComponent.enabled = false;
		m_PlayerCameraComponent.tag = "Untagged";
		m_animator.Play("Traveling");
	}
}
