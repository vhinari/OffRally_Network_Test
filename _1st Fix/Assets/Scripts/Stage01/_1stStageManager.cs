using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _1stStageManager : StageManager {
	
	private const float Enemy_xPos = 10f;
	private const float Enemy_yPos = 6f;

	private const float RepeatMin = 0.3f;
	private const float RepeatMax = 0.5f;

	public _1stStageManager() 
	{
		Enemy_DefaultPos = new Vector3[8];
		//8방향 - 시계 방향으로 생성.
		Enemy_DefaultPos[0] = new Vector3 (Camera.main.rect.xMin/2 + Enemy_xPos/2, Camera.main.rect.yMax + Enemy_yPos, 0);//위 - 왼쪽.
		Enemy_DefaultPos[1] = new Vector3 (Camera.main.rect.xMax/2 - Enemy_xPos/2, Camera.main.rect.yMax + Enemy_yPos, 0);//위 - 오른쪽.
		Enemy_DefaultPos[2] = new Vector3 (Camera.main.rect.xMax + Enemy_xPos, Camera.main.rect.yMax/2 - Enemy_yPos/2, 0);//왼쪽 - 위쪽.
		Enemy_DefaultPos[3] = new Vector3 (Camera.main.rect.xMax + Enemy_xPos, Camera.main.rect.yMax/2 + Enemy_yPos/2, 0);//왼쪽 - 아래쪽.
		Enemy_DefaultPos[4] = new Vector3 (Camera.main.rect.xMin/2 - Enemy_xPos/2, Camera.main.rect.yMin - Enemy_yPos, 0);//아래 - 오른쪽.
		Enemy_DefaultPos[5] = new Vector3 (Camera.main.rect.xMax/2 + Enemy_xPos/2, Camera.main.rect.yMin - Enemy_yPos, 0);//아래 - 왼쪽.
		Enemy_DefaultPos[6] = new Vector3 (Camera.main.rect.xMin - Enemy_xPos, Camera.main.rect.yMax/2 - Enemy_yPos/2, 0);//오른쪽 - 아래쪽.
		Enemy_DefaultPos[7] = new Vector3 (Camera.main.rect.xMin - Enemy_xPos, Camera.main.rect.yMax/2 + Enemy_yPos/2, 0);//오른쪽 - 위쪽.

		_Enemy = new GameObject[8];
		
		for (int iCount = 0; iCount < _Enemy.Length; iCount++)
		{
			_Enemy[iCount] = Instantiate(_EnemyPref, Enemy_DefaultPos[iCount], Quaternion.identity) as GameObject;
			_Enemy[iCount].AddComponent<Enemy_01> ();	
		}

		timeLeft = 1;
	}
	
	public override void RepeatMethod()
	{
		if (Time.timeScale == 1) 
		{
			timeLeft -= Time.deltaTime;

			if(timeLeft < 0)
			{		
				for(int iCount = 0; iCount < _Enemy.Length; iCount++)
					_Enemy[iCount].SendMessage ("ReleaseBullet");

				timeLeft = Random.Range(RepeatMin, RepeatMax);
			}
		}
	}

	public override void ResetStage ()
	{
		Destroy (_Player);

		for(int idx = 0; idx < _Enemy.Length; idx++)
			Destroy (_Enemy[idx]);

		System.GC.Collect ();
	}

	~_1stStageManager()
	{
		System.GC.Collect ();
	}
}
