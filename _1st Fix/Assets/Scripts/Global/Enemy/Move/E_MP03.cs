using UnityEngine;
using System.Collections;

public class E_MP03 : Enemy_Move {

	private Vector3 _Direction = Vector3.zero;
	private float _DirxPos;
	private float _DiryPos;

	public E_MP03(GameObject _Enemy)
		: base(_Enemy)
	{
		MoveSpeed = 10f;
		_DirxPos = -_Enemy.transform.position.x;
		_DiryPos = -6f;

	}
	
	public override void E_MovePattern ()
	{
		_Direction = new Vector3 (_DirxPos, _DiryPos, 0);	
		Vector3.Normalize (_Direction);
		_Enemy.transform.LookAt (_Direction);
	}
	
	public override void E_Movement ()
	{
		E_MovePattern ();
		_Enemy.rigidbody.velocity = _Direction * MoveSpeed * Time.deltaTime;
	}
}
