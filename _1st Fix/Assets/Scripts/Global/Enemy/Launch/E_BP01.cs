using UnityEngine;
using System.Collections;

public class E_BP01 : Enemy_BulletLaunch
{
	private GameObject _Player;

	public E_BP01( GameObject _enemy )
		: base(_enemy)
	{
		_Player		= GameObject.FindWithTag ("Player");
		ShootSpeed	= 3f;
	}

	public override void Launch ()
	{
		GameObject bullet = (GameObject)GameObject.Instantiate (_BulletPref, _Enemy.transform.position, Quaternion.identity);
		Object.Destroy (bullet, 10f);

		Pattern (ref bullet);

		bullet.rigidbody.velocity = _BulletDir[_BulletDir.Count-1];
	}

	public override void Pattern (ref GameObject _PatternBullet)
	{
		float Distance = Mathf.Sqrt(
			(_Player.transform.position.x - _PatternBullet.transform.position.x) * (_Player.transform.position.x - _PatternBullet.transform.position.x) +
			(_Player.transform.position.y - _PatternBullet.transform.position.y) * (_Player.transform.position.y - _PatternBullet.transform.position.y));
		_BulletDir.Add(new Vector3 ((_Player.transform.position.x - _PatternBullet.transform.position.x) / Distance * ShootSpeed,
		                            (_Player.transform.position.y - _PatternBullet.transform.position.y) / Distance * ShootSpeed));

		_PatternBullet.transform.LookAt (_BulletDir[_BulletDir.Count-1]);
	}

	~E_BP01()
	{
		System.GC.Collect ();
	}
}