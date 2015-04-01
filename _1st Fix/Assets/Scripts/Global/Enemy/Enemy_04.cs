using UnityEngine;
using System.Collections;

public class Enemy_04 : MonoBehaviour {

	private Enemy_BulletLaunch eBullet;
	
	void Awake()
	{
		eBullet = new E_BP04 (this.gameObject);
	}
	
	public void ReleaseBullet()
	{
		eBullet.Launch ();
	}

	~Enemy_04()
	{
		System.GC.Collect ();
	}
}
