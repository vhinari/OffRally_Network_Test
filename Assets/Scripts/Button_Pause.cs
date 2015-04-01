using UnityEngine;
using System.Collections;

public class Button_Pause : MonoBehaviour {

	private int OnPause = 0;

	void OnMouseDown()
	{
		OnPause++;

		if((OnPause % 2) == 0)
			GameManager.instance.SendMessage ("ChangeState", GameManager.GameState.Play);
		else
			GameManager.instance.SendMessage ("ChangeState", GameManager.GameState.Pause);
	}

}
