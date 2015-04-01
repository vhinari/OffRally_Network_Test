using UnityEngine;
using System.Collections;

public class Enemy_01 : MonoBehaviour
{
	private Enemy_BulletLaunch eBullet;
	
	void Awake()
	{
		eBullet = new E_BP01 (this.gameObject);
	}

	public void ReleaseBullet()
	{
		eBullet.Launch ();
	}

	~Enemy_01()
	{
		System.GC.Collect ();
	}
}


