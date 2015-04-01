using UnityEngine;
using System.Collections;

public class Button_Start : MonoBehaviour {

	void OnMouseDown()
	{
		Application.LoadLevel ("InGame_Stage01");
	}
}
