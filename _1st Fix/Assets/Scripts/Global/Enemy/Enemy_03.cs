using UnityEngine;
using System.Collections;

public class Enemy_03 : MonoBehaviour {

	private Enemy_BulletLaunch eBullet;
	private Enemy_Move eMove;	
	
	void Awake()
	{
		eBullet = new E_BP03 (this.gameObject);
		eMove 	= new E_MP03 (this.gameObject);
	}
	
	public void ReleaseBullet()
	{
		eBullet.Launch ();
	}
	
	public void ReleaseMove()
	{
		eMove.E_Movement ();
	}
	
	~Enemy_03()
	{
		System.GC.Collect ();
	}
}
