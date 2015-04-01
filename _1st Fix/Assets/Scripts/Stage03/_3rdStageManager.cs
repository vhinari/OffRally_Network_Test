using UnityEngine;
using System.Collections;

public class _3rdStageManager : StageManager {

	private const float yMax = -4.5f;

	private const float RepeatMin = 0.5f;
	private const float RepeatMax = 2.0f;
	
	public _3rdStageManager() 
	{
		float Enemy_yPos = 6f;
		float Enemy_xPos = 10f;

		Enemy_DefaultPos = new Vector3[8];
		Enemy_DefaultPos[0] = new Vector3 (Camera.main.rect.xMin - Enemy_xPos, Camera.main.rect.height + Enemy_yPos, 0);
		Enemy_DefaultPos[1] = new Vector3 (Camera.main.rect.xMax + Enemy_xPos, Camera.main.rect.height + Enemy_yPos, 0);
		Enemy_DefaultPos[2] = Enemy_DefaultPos [0];
		Enemy_DefaultPos[3] = Enemy_DefaultPos [1];
		Enemy_DefaultPos[4] = Enemy_DefaultPos [0];
		Enemy_DefaultPos[5] = Enemy_DefaultPos [1];
		Enemy_DefaultPos[6] = Enemy_DefaultPos [0];
		Enemy_DefaultPos[7] = Enemy_DefaultPos [1];

		_Enemy = new GameObject[8];
		
		for (int iCount = 0; iCount < _Enemy.Length; iCount++)
		{
			_Enemy[iCount] = Instantiate(_EnemyPref, Enemy_DefaultPos[iCount], Quaternion.identity) as GameObject;
			_Enemy[iCount].AddComponent<Enemy_03> ();	
		}
		
		timeLeft = 1;
	}
	
	public override void RepeatMethod()
	{
		if (Time.timeScale == 1) 
		{
			for(int iCount = 0; iCount < _Enemy.Length; iCount++)
			{
				_Enemy[iCount].SendMessage ("ReleaseMove");

				if(_Enemy[iCount].transform.position.y <= yMax)
					_Enemy[iCount].transform.position = Enemy_DefaultPos[iCount];
			}
		
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
	
	~_3rdStageManager()
	{
		System.GC.Collect ();
	}
}
