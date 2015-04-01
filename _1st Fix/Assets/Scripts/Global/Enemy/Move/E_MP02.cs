using UnityEngine;
using System.Collections;

public class E_MP02 : Enemy_Move {

	private Vector3 _Direction = Vector3.zero;
	private float xMin;
	private float xMax;
	float xOffset = 0;

	public E_MP02(GameObject _Enemy)
		: base(_Enemy)
	{
		MoveSpeed = 2f;

		xMin = -6f;
		xMax = 7f;
	}

	public override void E_MovePattern ()
	{
		if (_Enemy.transform.position.x >= xMax)xOffset = -1f;
		else if (_Enemy.transform.position.x <= xMin)xOffset = 1f;

		_Direction = new Vector3 (xOffset, 0, 0);
	
		Vector3.Normalize (_Direction);
		_Enemy.transform.LookAt (_Direction);
	}

	public override void E_Movement ()
	{
		E_MovePattern ();
		_Enemy.rigidbody.velocity = _Direction * MoveSpeed;
	}
}
