using UnityEngine;
using System.Collections;

public abstract class Enemy_Move : MonoBehaviour 
{
	protected float MoveSpeed;
	protected GameObject _Enemy;
	
	public Enemy_Move(GameObject _Enemy)
	{
		_Enemy = Resources.Load ("Enemy") as GameObject;
	}
	public abstract void E_Movement();
}
