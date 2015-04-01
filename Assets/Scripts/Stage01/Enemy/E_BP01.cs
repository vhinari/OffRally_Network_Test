using UnityEngine;
using System.Collections;

public class E_BP01 : Enemy_BulletLaunch
{
	private GameObject E01;
	private GameObject _Player;

	public E_BP01( GameObject _Enemy )
		: base(_Enemy)
	{
		E01 = _Enemy;
		E01.AddComponent<Enemy_01> ();
		
		_BulletPref = Resources.Load ("bullet") as GameObject;
		ShootSpeed = 5f;
		
		_Player = GameObject.FindWithTag ("Player");
	}

	public override void Launch (Vector3 _BulletDirection)
	{
		GameObject bullet = (GameObject)GameObject.Instantiate (_BulletPref, E01.transform.position, Quaternion.identity);
		Object.Destroy (bullet, 2f);

		bullet.rigidbody.AddForce (_BulletDirection * ShootSpeed);

//		throw new System.NotImplementedException ();
	}

	public override void Pattern ()
	{
		Vector3 BulletDirection = E01.transform.position - _Player.transform.position;

		Launch(BulletDirection);
		                                    
//		throw new System.NotImplementedException ();
	}
}




