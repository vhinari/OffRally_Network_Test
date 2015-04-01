using UnityEngine;
using System.Collections;

//사용 안 함.

public class E_MP01 : Enemy_Move
{
	private GameObject _Player;

	private Vector3 _Direction;
	private Vector3 _StartPoint;
	private Vector3 _EndPoint;

	public E_MP01(GameObject _Enemy)
		: base(_Enemy)
	{
		MoveSpeed = 2f;
	
		_Direction = Vector3.zero;
		_StartPoint = _Enemy.transform.position;
		_EndPoint = Vector3.zero;
	}

	public override void E_MovePattern ()
	{
		_Player = GameObject.FindWithTag ("Player");

		_Direction = _Player.transform.position - _Enemy.transform.position ;
		Vector3.Normalize (_Direction);
		_Enemy.transform.LookAt (_Direction);
	}
	
	public override void E_Movement ()
	{
		E_MovePattern ();
		_Enemy.rigidbody.velocity = _Direction * MoveSpeed;
	}
}