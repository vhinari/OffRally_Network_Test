using UnityEngine;
using System.Collections;

public class E_BP04 : Enemy_BulletLaunch {

	private GameObject	_Player;
	private GameObject _BigBullet;
	
	private float		timeLeft;
	private bool		isLaunched;

	private float		rad;
	private float		rad_step;
	private int			Ammo;
	private Vector3 	Default_Vec;
	
	public E_BP04( GameObject _enemy )
		: base(_enemy)
	{
		_Player		= GameObject.FindWithTag ("Player");
		ShootSpeed	= 1f;
		timeLeft	= 2f;
		isLaunched	= false;

		Default_Vec = -_Enemy.transform.up;
		Ammo 		= Random.Range(8, 12);
		rad_step	= Mathf.PI * 2f / (float)Ammo;
		
		if (Ammo % 2 == 0)
			rad = -Ammo / 2 * rad_step;
		else
			rad = ((float)-Ammo / 2f + 0.5f) * rad_step;
	}
	
	public override void Launch ()
	{
		if (isLaunched == false) 
		{
			_BigBullet = (GameObject)GameObject.Instantiate (_BulletPref, _Enemy.transform.position, Quaternion.identity);
			
			Pattern (ref _BigBullet);

			_BigBullet.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
			_BigBullet.rigidbody.velocity = _BulletDir [_BulletDir.Count - 1];
			isLaunched	= true;
		}

		if(isLaunched)
			timeLeft -= Time.deltaTime;
		
		if(timeLeft < 0)
		{	
			GameObject[] bullet = new GameObject[Ammo];
			
			for (int iCount = 1; iCount < Ammo; iCount++, rad += rad_step) 
			{
				bullet [iCount] = (GameObject)GameObject.Instantiate (_BulletPref, _BigBullet.transform.position, Quaternion.identity);
				Pattern (ref bullet[iCount]);
			}
			
			_BigBullet.SetActive(false);
			ShootSpeed = 3f;
			
			for (int iCount = 1; iCount < Ammo; iCount++) 
			{
				bullet [iCount].rigidbody.velocity = _BulletDir[iCount] * ShootSpeed;
				Object.Destroy (bullet [iCount], 2f);
			}
			
			isLaunched	= false;
			timeLeft	= 2f;
			_BigBullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			Destroy(_BigBullet);
		}

	}

	public override void Pattern (ref GameObject _PatternBullet)
	{
		if (isLaunched == false)
		{
			float Distance = Mathf.Sqrt ((_Player.transform.position.x - _PatternBullet.transform.position.x) * (_Player.transform.position.x - _PatternBullet.transform.position.x) +
										 (_Player.transform.position.y - _PatternBullet.transform.position.y) * (_Player.transform.position.y - _PatternBullet.transform.position.y));
			_BulletDir.Add (new Vector3 ((_Player.transform.position.x - _PatternBullet.transform.position.x) / Distance * ShootSpeed,
		    		                    (_Player.transform.position.y - _PatternBullet.transform.position.y) / Distance * ShootSpeed));
			_PatternBullet.transform.LookAt (_BulletDir [_BulletDir.Count - 1]);
		}
		else
		{
			float cos = Mathf.Cos (rad), sin = Mathf.Sin (rad);
			_BulletDir.Add (new Vector3 (Default_Vec.x * cos - Default_Vec.y * sin, Default_Vec.x * sin - Default_Vec.y * cos, 0));
			_PatternBullet.transform.LookAt (_BulletDir[_BulletDir.Count - 1]);
		}
	}

	~E_BP04()
	{
		System.GC.Collect ();
	}
}
