using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    GameManager GM;

    void Awake()
    {
        GM = GameManager.Instance;
        GM.OnStateChange += HandleOnStateChange;
    }

    public void HandleOnStateChange()
    {
        Debug.Log("OnStateChange!");
    }


    public void OnGUI()
    {
        //menu layout
        GUI.BeginGroup(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 800));
        GUI.Box(new Rect(0, 0, 100, 200), "Menu");
        if (GUI.Button(new Rect(10, 40, 80, 30), "Start"))
        {
            StartGame();
        }
        if (GUI.Button(new Rect(10, 160, 80, 30), "Quit"))
        {
            Quit();
        }
        GUI.EndGroup();
    }


    public void StartGame()
    {
        //start game scene
        GM.SetGameState(GameManager.GameState.GAME);
        Debug.Log(GM.gameState);
    }


    public void Quit()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
