using UnityEngine;
using System.Collections;

public class PlayButton : MonoBehaviour {

    public GameObject MMMOn;
    public GameObject CanvasOn;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.Log(this.gameObject.name);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag.Equals("PlayButton"))
                {
                    MMMOn.SetActive(true);
                    CanvasOn.SetActive(true);
                }
            }
        }
	}
}
