using UnityEngine;
using System.Collections;

public class E_BP02 : Enemy_BulletLaunch {

	private float theta;
	private float angle;

	public E_BP02( GameObject _enemy )
		: base(_enemy)
	{
		ShootSpeed = 10f;
		//theta = Random.Range (5f, 15f);
		theta = 15f;
		angle = 0;
	}
	
	public override void Launch ()
	{
		GameObject bullet = (GameObject)GameObject.Instantiate (_BulletPref, _Enemy.transform.position, Quaternion.identity);
		Object.Destroy (bullet, 10f);
		
		Pattern (ref bullet);
		
		bullet.rigidbody.velocity = _BulletDir[_BulletDir.Count-1] * ShootSpeed;
	}
	
	public override void Pattern (ref GameObject _PatternBullet)
	{
		//if (angle >= 360) --theta;
		//if (theta <= 0) theta = Random.Range (5f, 15f);

		_BulletDir.Add(new Vector3 (Mathf.Cos (Mathf.PI / 90 * angle), Mathf.Sin (Mathf.PI / 90 * angle), 0));
		_PatternBullet.transform.LookAt (_BulletDir[_BulletDir.Count-1]);
		angle += theta;
	}
}
