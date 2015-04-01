using UnityEngine;
using System.Collections;

public class Enemy_01 : MonoBehaviour
{
	private Enemy_BulletLaunch eBullet;
	private Enemy_Move eMove;

	void Awake()
	{
		eBullet = new E_BP01(this.gameObject);
		eMove = new E_MP01 (this.gameObject);

		transform.position = new Vector3 (Camera.main.rect.xMax / 2, Camera.main.rect.yMax + 1f, 0);
		transform.Rotate (0, 0, 180f);
	}

	void Update()
	{
		eMove.E_Movement ();
		eBullet.Pattern ();
	}

	void OnTriggerEnter(Collider Obj)
	{
		if (Obj.CompareTag ("Bullet"))
			gameObject.SetActive (false);
	}

	~Enemy_01()
	{
		System.GC.Collect ();
	}
}


