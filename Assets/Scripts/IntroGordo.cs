using UnityEngine;
using System.Collections;

public class IntroGordo : MonoBehaviour {

	Animator m_Animator;

	// Use this for initialization
	void Start () {
		m_Animator = GetComponent<Animator>();
		m_Animator.Play("Andando");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
