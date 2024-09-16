using UnityEngine;
using System.Collections;

public class AnimationTest : MonoBehaviour 
{
	Animator animator;
	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			animator.Play("Atropello");
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			animator.Play("Aplastamiento");
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			animator.Play("AtropelloPie");
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			animator.Play("Bolso");
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			animator.Play("Gato");
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			animator.Play("Tetas");
		}

	}
}
