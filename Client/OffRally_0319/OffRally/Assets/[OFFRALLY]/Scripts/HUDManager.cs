using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class HUDManager : MonoBehaviour {

    public Stopwatch LapSW;
    public Stopwatch TrackSW;

    public enum GLOBAL_HUD { MATCHING, GAMESTART, LAP_TIME }
    public enum LOCAL_HUD { WRONG_DIRECTION, GUIDELINE, RESULT }

    public Canvas g_Canvas_GLOBAL_HUD;
    public Canvas g_Canvas_LOCAL_HUD;

    private static HUDManager _instance;
    public static HUDManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType(typeof(HUDManager)) as HUDManager;
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "UnityComp";
                    _instance = container.AddComponent(typeof(HUDManager)) as HUDManager;
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        g_Canvas_GLOBAL_HUD = gameObject.transform.FindChild("GlobalCanvas").GetComponent<Canvas>();
        g_Canvas_LOCAL_HUD = gameObject.transform.FindChild("LocalCanvas").GetComponent<Canvas>();

        g_Canvas_GLOBAL_HUD.enabled = false;
        g_Canvas_LOCAL_HUD.enabled = false;
    }

    public void EnableHUD<T>(LOCAL_HUD hud, T value)
    {
        g_Canvas_GLOBAL_HUD.enabled = true;
        g_Canvas_LOCAL_HUD.enabled = true;

        if (Application.loadedLevel == 1)
        {
            LapSW = new Stopwatch();
            TrackSW = new Stopwatch();

            LapSW.Start();
            TrackSW.Start();
        }                    

        switch(hud)
        {
            case LOCAL_HUD.WRONG_DIRECTION:
                {

                }break;

            case LOCAL_HUD.RESULT:
                {
                    transform.FindChild("LapTime").GetComponent<GUIText>().text = 
                        LapSW.Elapsed.Minutes.ToString() + " : " + 
                        LapSW.Elapsed.Seconds.ToString() + " : " + 
                        LapSW.Elapsed.Milliseconds.ToString();

                    transform.FindChild("TrackTime").GetComponent<GUIText>().text =
                        TrackSW.Elapsed.Minutes.ToString() + " : " +
                        TrackSW.Elapsed.Seconds.ToString() + " : " +
                        TrackSW.Elapsed.Milliseconds.ToString();
                }break;
        }

    }


    void LateUpdate()
    {
   
    }

    
}
