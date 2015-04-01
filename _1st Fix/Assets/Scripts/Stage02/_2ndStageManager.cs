using UnityEngine;
using System.Collections;

public class _2ndStageManager : StageManager {
	
	private const float Enemy_yPos = 4f;

	public _2ndStageManager() 
	{
		Vector3[] Enemy_DefaultPos = new Vector3[2];
		float xDiv = Camera.main.rect.center.x;

		Enemy_DefaultPos[0] = new Vector3 (Camera.main.rect.xMax / 2 - xDiv * 13, Camera.main.rect.height + Enemy_yPos, 0);
		Enemy_DefaultPos[1] = new Vector3 (Camera.main.rect.xMax / 2 + xDiv * 13, Camera.main.rect.height - Enemy_yPos, 0);

		_Enemy = new GameObject[2];
		
		for (int iCount = 0; iCount < _Enemy.Length; iCount++)
		{
			_Enemy[iCount] = Instantiate(_EnemyPref, Enemy_DefaultPos[iCount], Quaternion.identity) as GameObject;
			_Enemy[iCount].AddComponent<Enemy_02> ();	
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
				{
					_Enemy[iCount].SendMessage ("ReleaseBullet");
					_Enemy[iCount].SendMessage ("ReleaseMove");
				}
				
				timeLeft = 0.1f;
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
	
	~_2ndStageManager()
	{
		System.GC.Collect ();
	}
}
