using UnityEngine;
using System.Collections;

public class Enemy_02 : MonoBehaviour {

	private Enemy_BulletLaunch eBullet;
	private Enemy_Move eMove;	

	void Awake()
	{
		eBullet = new E_BP02 (this.gameObject);
		eMove 	= new E_MP02 (this.gameObject);
	}
	
	public void ReleaseBullet()
	{
		eBullet.Launch ();
	}

	public void ReleaseMove()
	{
		eMove.E_Movement ();
	}
	
	~Enemy_02()
	{
		System.GC.Collect ();
	}
}
