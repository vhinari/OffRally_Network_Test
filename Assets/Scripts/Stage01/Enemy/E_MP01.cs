using UnityEngine;
using System.Collections;

public class E_MP01 : Enemy_Move
{
	private GameObject E01;
	private GameObject _Player;
	private Vector3 _EndPoint;

	private const float MAX_ANGLE = 15f;

	public E_MP01(GameObject _Enemy)
		: base(_Enemy)
	{
		E01 = _Enemy;
		MoveSpeed = 2.5f;
		_EndPoint = Vector3.zero;

		Turn ();
	}

	public void Turn()
	{
		_Player = GameObject.FindWithTag ("Player");

		Vector3 Dir = _Player.transform.position - E01.transform.position;
		float cosValue = Vector3.Dot (Dir, E01.transform.up);

		if( cosValue >= MAX_ANGLE )
			E01.transform.Rotate (0, 0, MAX_ANGLE);
		else if( cosValue <= -MAX_ANGLE )
			E01.transform.Rotate (0, 0, -MAX_ANGLE);
		else
			E01.transform.Rotate (0, 0, cosValue);

		_EndPoint = new Vector3(Dir.x, Camera.main.rect.yMin - 1f, 0);
		//_WayPoint [0].transform.position = new Vector3 (Dir.x, Camera.main.rect.yMin - 1f, 0);
	}
	
	public override void E_Movement ()
	{
		//E01.transform.position = Vector3.Lerp (E01.transform.position, _WayPoint [0].transform.position, Time.deltaTime);
		E01.transform.position = Vector3.Lerp (E01.transform.position, _EndPoint, Time.deltaTime);

//		throw new System.NotImplementedException ();
	}
}

