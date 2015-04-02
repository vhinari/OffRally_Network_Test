using UnityEngine;
using System.Collections;

public class SceneChange : MonoBehaviour
{
    public Color color;
    public Color color2;
    float time;

    // Use this for initialization
    void Start()
    {
        color = gameObject.GetComponent<GUITexture>().color;
        color2 = new Color(0, 0, 0, 0);
        time = 0.0f;
    }


    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<GUITexture>().color = Color.Lerp(color, color2, Time.time / 3.0f);
    }
}
