using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public enum GameState
	{
		Ready,
		Play,
		Pause,
		End
	}

	GameState CurState = GameState.Ready;
	
	public enum GamaStage
	{
		Stage1 = 101,
		Stage2,
		Stage3,
		Stage4
	}
	
	private static GameManager s_Instance = null;
	public static GameManager	instance
	{
		get
		{
			if(s_Instance == null)
				s_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
			//만약 s_Instance가 null이라면 CollisionManager 타입을 찾아 s_Instance에 대입.
			
			if(s_Instance == null)
			{
				GameObject obj = new GameObject("GameManager");
				s_Instance = obj.AddComponent(typeof(GameManager)) as GameManager;
				Debug.Log("Could not locate an GameManager object. GameManager was Generated Automaticly.");
				/*
				1. 만약 위에서의 if문을 거치고서도 s_Instance가 null이라면
				2. 임시 GameObject obj에 GameManager 찾아 대입.
				3. s_Instance에 obj( GameManager )의 컴포넌트를 대입.
				4. 디버그. 
			 */
			}
			return s_Instance;
		}
	}
	
	void Awake()
	{
		if(s_Instance == null)
		{
			s_Instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			if(this != s_Instance)
				Destroy(this.gameObject);
		}
	}

	void Update()
	{
		switch ((int)CurState)
		{
		case (int)GameState.Ready:
			break;

		case (int)GameState.Play:
			break;
		case (int)GameState.Pause:
			break;

		case (int)GameState.End:
			Application.Quit();
			System.GC.Collect();
			break;
		}
	}

	void ChangeState(GameState _State)
	{
		CurState = _State;
	}

	void ChangeStage(GamaStage _Stage)
	{
		string nextStage = "";

		switch ((int)_Stage)
		{
		case (int)GamaStage.Stage1: nextStage = "InGame_Stage01;"; break;
		case (int)GamaStage.Stage2: nextStage = "InGame_Stage02;"; break;
		case (int)GamaStage.Stage3: nextStage = "InGame_Stage03;"; break;
		case (int)GamaStage.Stage4: nextStage = "InGame_Stage04;"; break;
		}

		Application.LoadLevel (nextStage);
	}
}
