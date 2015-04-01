using UnityEngine;
using System.Collections;

public class MainMenumove : MonoBehaviour {

    public GameObject Play;
    public GameObject Developer;
    public GameObject Option;
    public GameObject Exit;

    //IEnumerator WaitMove();

	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("start", 0, Time.deltaTime);
        InvokeRepeating("developer", 0.3f, Time.deltaTime);
        InvokeRepeating("option", 0.6f, Time.deltaTime);
        InvokeRepeating("exit", 0.9f, Time.deltaTime);
	}

    void start()
    {
        Play.transform.Translate(new Vector3(-0.1f, 0, 0));
        if (Play.transform.position.x <= -10)
            CancelInvoke("start");
    }

    void developer()
    {
        Developer.transform.Translate(new Vector3(-0.1f, 0, 0));
        if (Developer.transform.position.x <= -10)
            CancelInvoke("developer");
    }        

    void option()
    {
        Option.transform.Translate(new Vector3(-0.1f, 0, 0));
        if (Option.transform.position.x <= -10)
            CancelInvoke("option");
    }
    
    void exit()
    {
        Exit.transform.Translate(new Vector3(-0.1f, 0, 0));
        if (Exit.transform.position.x <= -10)
            CancelInvoke("exit");
    }
}
