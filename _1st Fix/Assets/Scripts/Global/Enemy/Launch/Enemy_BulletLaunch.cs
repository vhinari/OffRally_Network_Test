using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy_BulletLaunch : MonoBehaviour {

	protected float ShootSpeed;
	protected GameObject _BulletPref;
	
	protected GameObject _Enemy;
	protected Vector3 _BulletDirection;
	protected List<Vector3> _BulletDir;

	public Enemy_BulletLaunch(GameObject _Enemy)
	{
		this._Enemy = _Enemy;
		_BulletPref = Resources.Load ("bullet") as GameObject;
		_BulletDir = new List<Vector3> ();
	}

	public abstract void Launch();
	public abstract void Pattern(ref GameObject _PatternBullet);

	~Enemy_BulletLaunch()
	{
		System.GC.Collect ();
	}
}
