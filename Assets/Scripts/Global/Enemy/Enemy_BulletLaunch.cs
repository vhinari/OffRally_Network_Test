using UnityEngine;
using System.Collections;

public abstract class Enemy_BulletLaunch : MonoBehaviour {

	protected float ShootSpeed = 30f;
	protected GameObject _BulletPref;

	public Enemy_BulletLaunch(GameObject _Enemy)
	{
		_Enemy = Resources.Load ("Enemy") as GameObject;
	}
	public abstract void Launch(Vector3 _BulletDirection);
	public abstract void Pattern();
}
