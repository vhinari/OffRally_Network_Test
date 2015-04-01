using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private StageManager stageMgr;

	public GameObject PrefInit;
	public GameObject PrefPause;
	private static GameObject GUIInit;
	private static GameObject GUIPause;

	public enum GameState
	{
		Init,
		Play,
		Pause,
		End,
		Quit
	}
	private static GameState CurState = GameState.Init;
	
	public enum GameStage
	{
		ResetScene,
		Stage1,
		Stage2,
		Stage3,
		Stage4
	}
	private static int CurStage = (int)GameStage.Stage1;

	private static GameManager s_Instance = null;
	public static GameManager	instance
	{
		get
		{
			if(s_Instance == null)
				s_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
			//만약 s_Instance가 null이라면 GameManager 타입을 찾아 s_Instance에 대입.
			
			if(s_Instance == null)
			{
				GameObject obj = new GameObject("GameManager");
				s_Instance = obj.AddComponent(typeof(GameManager)) as GameManager;
				/*
				1. 만약 위에서의 if문을 거치고서도 s_Instance가 null이라면
				2. 임시 GameObject obj에 GameManager 찾아 대입.
				3. s_Instance에 obj( GameManager )의 컴포넌트를 대입.
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

		GUIInit = Instantiate((Object)PrefInit, new Vector3(0.5f, 0.5f, 0), Quaternion.identity) as GameObject;
		GUIPause = Instantiate((Object)PrefPause, new Vector3(0.5f, 0.5f, 0), Quaternion.identity) as GameObject;
	}

	void Update()
	{
		switch ((int)CurState) {
		case (int)GameState.Init:
		{
			GUIInit.SetActive ( true );
			InitStage (CurStage);
			GUIInit.SetActive ( false );

			CurState = GameState.Pause;
		}
		break;

		case (int)GameState.Play:
		{
			GUIPause.SetActive ( false );
			Time.timeScale = 1;

			stageMgr.RepeatMethod();
		}
		break;

		case (int)GameState.Pause:
		{
			GUIPause.SetActive ( true );
			Time.timeScale = 0;
		}
			break;

		case (int)GameState.End:
		{
			Time.timeScale = 0;
			stageMgr.ResetStage();
			Application.LoadLevel ("Stage_Reset");
			CurState = GameState.Init;
		}
			break;

		case (int)GameState.Quit:
		{
			Application.Quit ();
			System.GC.Collect ();
		}
			break;
		}
	}

	void LateUpdate()
	{
		if(GetStageChangeKey ())
			ChangeStage();
		ChangeStateKeyInput ();
	}
	
	void ChangeStage()
	{
		Application.LoadLevel (CurStage);
		CurState = GameState.Init;
	}

	void InitStage(int _curStage)
	{
		switch(_curStage)
		{
		case (int)GameStage.Stage1:{stageMgr = new _1stStageManager();}break;
		case (int)GameStage.Stage2:{stageMgr = new _2ndStageManager();}break;
		case (int)GameStage.Stage3:{stageMgr = new _3rdStageManager();}break;
		case (int)GameStage.Stage4:{stageMgr = new _4thStageManager();}break;
		}
	}

	bool GetStageChangeKey()
	{
		if (Input.GetKey (KeyCode.Z) && CurStage > (int)GameStage.Stage1)
		{
			--CurStage;
			return true;
		}
		else if(Input.GetKey (KeyCode.Z) && CurStage == (int)GameStage.Stage1)
		{
			CurStage = (int)GameStage.Stage4;
			return true;
		}
		
		if (Input.GetKey (KeyCode.X) && CurStage < (int)GameStage.Stage4) 
		{
			++CurStage;
			return true;
		}
		else if (Input.GetKey (KeyCode.X) && CurStage == (int)GameStage.Stage4) 
		{
			CurStage = (int)GameStage.Stage1;
			return true;
		}

		return false;
	}

	void ChangeState(GameState _curState)
	{
		switch ((int)_curState)
		{
		case (int)GameState.Init:	{CurState = GameState.Init;}	break;
		case (int)GameState.Play:	{CurState = GameState.Play;}	break;
		case (int)GameState.Pause:	{CurState = GameState.Pause;}	break;
		case (int)GameState.End:	{CurState = GameState.End;}		break;
		case (int)GameState.Quit:	{CurState = GameState.Quit;}	break;
		}
	}
	
	void ChangeStateKeyInput()
	{
		if (Input.GetKey (KeyCode.Return) && CurState != GameState.Init) 
			ChangeState(GameState.Play);

		if (Input.GetKey (KeyCode.Escape) && (CurState != GameState.Pause) && (CurState != GameState.Init))
			ChangeState(GameState.Pause);
	}
}
