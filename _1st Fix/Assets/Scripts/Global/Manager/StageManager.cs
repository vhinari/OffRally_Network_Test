using UnityEngine;
using System.Collections;

public abstract class StageManager : MonoBehaviour {

	protected static GameObject _PlayerPref = Resources.Load ("Player") as GameObject;
	protected static GameObject _Player;
	protected Vector3 Player_DefaultPos;

	protected static GameObject _EnemyPref = Resources.Load ("Enemy") as GameObject;
	protected GameObject[] _Enemy;
	protected Vector3[] Enemy_DefaultPos;

	protected float timeLeft;

	public StageManager()
	{
		Vector3 Player_DefaultPos = new Vector3 (Camera.main.rect.center.x, Camera.main.rect.yMax / 2 - 1f, 0);
		_Player = (GameObject)GameObject.Instantiate (_PlayerPref, Player_DefaultPos, Quaternion.identity);
		_Player.AddComponent<Player> ();
	}
	
	public abstract void RepeatMethod();
	public abstract void ResetStage();

	~StageManager()
	{
		System.GC.Collect ();
	}
}
