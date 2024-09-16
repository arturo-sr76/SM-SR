using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleGlow : MonoBehaviour {

	Text text;
	Color color;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		color = text.color;
	}
	
	// Update is called once per frame
	void Update () {
		color.a = Mathf.Abs((float)Mathf.Sin(Time.time));
		text.color = color;
	print(color.a);
	}
}
