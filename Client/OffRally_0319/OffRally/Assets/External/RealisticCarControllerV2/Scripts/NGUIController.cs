using UnityEngine;
using System.Collections;

public class NGUIController : MonoBehaviour {

	public float input;
	public float time = 5f;
	private bool pressing;

	void Start () {
	
	}

	void OnPress (bool isPressed)
	{
		if(isPressed)
			pressing = true;
		else
			pressing = false;
	}

	void Update(){

		if(pressing)
			input += Time.deltaTime * time;
		else
			input -= Time.deltaTime * time;

		if(input < 0f)
			input = 0f;
		if(input > 1f)
			input = 1f;

	}

}
