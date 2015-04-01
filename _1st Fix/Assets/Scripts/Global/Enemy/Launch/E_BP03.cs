using UnityEngine;
using System.Collections;

public class E_BP03 : Enemy_BulletLaunch {

	private float	theta;
	private float	rad;
	private float	rad_step;
	private int		Ammo;
	private Vector3 Default_Vec;

	public E_BP03( GameObject _enemy )
		: base(_enemy)
	{
		ShootSpeed	= 10f;
		Default_Vec = -_Enemy.transform.up;
		theta 		= 22.5f;
		Ammo 		= Random.Range (15, 20);
		rad_step	= Mathf.PI * 2f / (float)Ammo;

		if (Ammo % 2 == 0)
			rad = -Ammo / 2 * rad_step;
		else
			rad = ((float)-Ammo / 2f + 0.5f) * rad_step;
	}
	
	public override void Launch ()
	{
		GameObject[] bullet = new GameObject[Ammo];

		for (int iCount = 0; iCount < Ammo; iCount++, rad += rad_step) 
		{
			bullet [iCount] = (GameObject)GameObject.Instantiate (_BulletPref, _Enemy.transform.position, Quaternion.identity);
			Pattern (ref bullet[iCount]);
		}

		for (int iCount = 0; iCount < Ammo; iCount++) 
		{
			bullet [iCount].rigidbody.velocity = _BulletDir[iCount] * ShootSpeed;
			Object.Destroy (bullet [iCount], 2f);
		}
	}
	
	public override void Pattern (ref GameObject _PatternBullet)
	{
		float cos = Mathf.Cos (rad), sin = Mathf.Sin (rad);
		_BulletDir.Add (new Vector3 (Default_Vec.x * cos - Default_Vec.y * sin, Default_Vec.x * sin - Default_Vec.y * cos, 0));
		_PatternBullet.transform.LookAt (_BulletDir[_BulletDir.Count-1]);
	}
}
