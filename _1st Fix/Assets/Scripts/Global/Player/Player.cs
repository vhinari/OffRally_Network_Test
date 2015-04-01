using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private const float SharpMoving = 5f;
	private float		MoveSpeed;

	public GameObject	BulletPref;
	private float		BulletForce = 700f;
	private bool		CanShoot = true;

	void Awake()
	{
		MoveSpeed = SharpMoving * 2f;
		BulletPref = Resources.Load ("bullet") as GameObject;
	}

	void Update()
	{
		if (Input.GetKey (KeyCode.Space) && CanShoot)
			Shoot ();

		Move();
	}

	IEnumerator Reload()
	{
		yield return new WaitForSeconds (0.3f);
		CanShoot = true;
		StopCoroutine ("Reload");
	}
	
	public void Shoot()
	{
		GameObject Bullet = (GameObject)GameObject.Instantiate (BulletPref, transform.position, Quaternion.identity);
		Object.Destroy (Bullet, 2f);

		Bullet.rigidbody.WakeUp();
		Bullet.rigidbody.AddForce(0, BulletForce, 0);

		CanShoot = false;

		StartCoroutine ("Reload");
	}
	
	public void Move()
	{
		float HorInput = Input.GetAxisRaw ("Horizontal") * Time.deltaTime;
		float VerInput = Input.GetAxisRaw ("Vertical") * Time.deltaTime;

		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			MoveSpeed = SharpMoving;
		else
			MoveSpeed = SharpMoving * 2f;

		transform.Translate (HorInput * MoveSpeed, 0, 0);
		transform.Translate (0, VerInput * MoveSpeed, 0);
	}

	void OnTriggerEnter(Collider Obj)
	{
		if (Obj.CompareTag ("EnemyBullet"))
		{
			gameObject.SetActive (false);
			GameManager.instance.SendMessage("ChangeState", GameManager.GameState.End);
		}
	}

	
	~Player()
	{
		System.GC.Collect ();
	}
}

