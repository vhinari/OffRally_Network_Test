using UnityEngine;
using System.Collections;

public class CamManager: MonoBehaviour {

    private SmoothFollow_Camera cameraScript;
	private MouseOrbit orbitScript;

	public float dist = 10.0f;
	public int cameraChangeCount = 0;
	public GameObject target;
    public Vector3 pos;
	void Start ()
    {

        cameraScript = GetComponent<SmoothFollow_Camera>();
		orbitScript = GetComponent<MouseOrbit>(); //모바일?
        
	}

	void Update ()
    {

        //target_pos = target.transform;
        //target_pos.position += pos;

        cameraScript.target = target.transform;//타겟 위치 잡기
		cameraScript.distance = dist; //거리 
        cameraScript.height = pos.y;// 높이

		orbitScript.target = target.transform;
		orbitScript.distance = dist;


		if(Input.GetKeyDown(KeyCode.C)){
			cameraChangeCount++;
			if(cameraChangeCount == 3)
				cameraChangeCount = 0;
		}
	
		switch(cameraChangeCount)
        {
		case 0:
			orbitScript.enabled = false;
			cameraScript.enabled = true;
			break;
		case 1:
			orbitScript.enabled = true;
			cameraScript.enabled = false;
			break;
		case 2:
			orbitScript.enabled = false;
			cameraScript.enabled = false;
			break;
		}

	}

}
