using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

    //public GameObject FadeOut;

    public void Change(int scene)
    {
        Application.LoadLevel(scene);
    }

    //void Update ()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;
    //        Debug.Log(this.gameObject.name);

    //        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
    //        {
    //            if (hit.transform.tag.Equals("StartButton"))
    //            {
    //                FadeOut.SetActive(false);
    //            }
    //        }
    //    }
    //}
}
