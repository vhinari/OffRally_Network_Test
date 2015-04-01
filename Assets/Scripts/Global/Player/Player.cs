using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float		MoveSpeed;

	public GameObject	BulletPref;
	private float		BulletForce = 700f;

	void Awake()
	{
		MoveSpeed = 10f;
		BulletPref = Resources.Load ("bullet") as GameObject;
	}

	void Update()
	{
		if (Input.GetKey (KeyCode.Space))
			Shoot ();

		Move();
	}
	
	public void Shoot()
	{
		GameObject Bullet = (GameObject)GameObject.Instantiate (BulletPref, transform.position, Quaternion.identity);
		Object.Destroy (Bullet, 2f);

		Bullet.rigidbody.WakeUp();
		Bullet.rigidbody.AddForce(0, BulletForce, 0);
	}
	
	public void Move()
	{
		float HorInput = Input.GetAxisRaw ("Horizontal") * Time.deltaTime;
		float VerInput = Input.GetAxisRaw ("Vertical") * Time.deltaTime;
		
		transform.Translate (HorInput * MoveSpeed, 0, 0);
		transform.Translate (0, VerInput * MoveSpeed, 0);
	}
	
	~Player()
	{
		System.GC.Collect ();
	}
}

