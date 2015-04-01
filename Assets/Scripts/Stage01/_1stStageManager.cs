using UnityEngine;
using System.Collections;

public class _1stStageManager : MonoBehaviour {

	private GameObject _Player;
	private Vector3 Player_DefaultPos;

	private GameObject _Enemy01;
	private Vector3 Enemy_DefaultPos;

	void Awake()
	{
		Player_DefaultPos = new Vector3 (Camera.main.rect.xMax / 2, Camera.main.rect.yMax / 2 - 1f, 0);
		_Player = Resources.Load ("Player") as GameObject;
		_Player = (GameObject)GameObject.Instantiate (_Player, Player_DefaultPos, Quaternion.identity);
		_Player.AddComponent<Player> ();

		Enemy_DefaultPos = new Vector3 (Camera.main.rect.xMax / 2, Camera.main.rect.yMax + 5f, 0);
		_Enemy01 = Resources.Load ("Enemy") as GameObject;
		_Enemy01 = (GameObject)GameObject.Instantiate (_Enemy01, Enemy_DefaultPos, Quaternion.identity);
		_Enemy01.AddComponent<Enemy_01> ();

		Object.Destroy (_Enemy01, 10f);

		InvokeRepeating ("SpawnEnemy", 2f, 2f);
	}

	void SpawnEnemy()
	{
		GameObject _enemy = (GameObject)GameObject.Instantiate (_Enemy01, Enemy_DefaultPos, Quaternion.identity);
		_enemy.AddComponent<Enemy_01> ();

		Object.Destroy (_enemy, 10f);
	}
}
