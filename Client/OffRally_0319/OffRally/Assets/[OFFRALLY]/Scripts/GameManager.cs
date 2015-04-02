using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

public enum GameState { INTRO, MAIN_MENU, GAME }

    public delegate void OnStateChangeHandler();

    protected GameManager() { }
    private static GameManager instance = null;
    public event OnStateChangeHandler OnStateChange;
    public  GameState gameState { get; private set; }

    public static GameManager Instance
    {
        get {
            if (GameManager.instance == null)
            {
                DontDestroyOnLoad(GameManager.instance);
                GameManager.instance = new GameManager();
            }
            return GameManager.instance;
        }

    }

    public void SetGameState(GameState state){
        this.gameState = state;
        OnStateChange();
    }

    public void OnApplicationQuit(){
        GameManager.instance = null;
    }
}
