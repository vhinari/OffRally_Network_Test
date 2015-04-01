using UnityEngine;
using System;
using System.Collections;

public class CarChange : MonoBehaviour {
	
	private GameObject[] objects;
	private int activeObjectIdx;
	public float cameraDistance = 15f;
	
	private GameObject activeObject;
	private float size;
	private bool selectScreen = true;
	
	public Vector3 cameraOffset = new Vector3(0.0f, 1.0f, 0.0f);

	void Start () {

		objects = GameObject.FindGameObjectsWithTag("Player");
		
		Array.Sort(objects, delegate(GameObject go1, GameObject go2) {
			return go1.transform.position.x.CompareTo(go2.transform.position.x);
		});
		
		SetActiveObject(objects[activeObjectIdx]);

	}

	void Update () {

		if(selectScreen)
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, objects[activeObjectIdx].transform.position + (-Camera.main.transform.forward * size) + cameraOffset, Time.deltaTime * 5f);

	}
	
	void OnGUI()
	{

		if(selectScreen){

			GUIStyle centeredStyle = GUI.skin.GetStyle("Button");
			centeredStyle.alignment = TextAnchor.MiddleCenter;

			// Next
			if( GUI.Button(new Rect(Screen.width/2 + 65, Screen.height-100, 120, 50), "Next") )
			{
				activeObjectIdx++;
				if( activeObjectIdx >= objects.Length )
					activeObjectIdx = 0;
				
				SetActiveObject(objects[activeObjectIdx]);
			}	
			
			// Previous
			if( GUI.Button(new Rect(Screen.width / 2 - 185, Screen.height-100, 120, 50), "Previous") )
			{
				activeObjectIdx--;
				if( activeObjectIdx < 0 )
					activeObjectIdx = objects.Length - 1;
				
				SetActiveObject(objects[activeObjectIdx]);
			}

			// Select Car
			if( GUI.Button(new Rect(Screen.width / 2 - 60, Screen.height-100, 120, 50), "Select") )
			{
				selectScreen = false;
				objects[activeObjectIdx].GetComponent<CarControllerV2>().canControl = true;
				GetComponent<CamManager>().enabled = true;
				GetComponent<CamManager>().target = objects[activeObjectIdx];
				GetComponent<CamManager>().dist = size;
			}

		}else{

			if( GUI.Button(new Rect(Screen.width - 270, 350, 240, 50), "Select Screen") )
			{
				selectScreen = true;
				objects[activeObjectIdx].GetComponent<CarControllerV2>().canControl = false;
				GetComponent<CamManager>().enabled = false;
				GetComponent<SmoothFollow>().enabled = false;
				GetComponent<MouseOrbit>().enabled = false;
				Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, 330, Camera.main.transform.rotation.z);
			}

		}

	}
	
	void SetActiveObject(GameObject go) {
		
		size = cameraDistance;
		
		Renderer[] r = objects[activeObjectIdx].GetComponentsInChildren<Renderer>();
		if( r.Length > 0 )
		{
			Array.Sort(r, delegate(Renderer r1, Renderer r2) {
				return r2.bounds.size.magnitude.CompareTo(r1.bounds.size.magnitude);
			});
			size = r[0].bounds.size.magnitude;
		}
	}

}
