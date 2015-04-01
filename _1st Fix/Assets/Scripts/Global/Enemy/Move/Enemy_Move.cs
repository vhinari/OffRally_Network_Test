using UnityEngine;
using System.Collections;

public abstract class Enemy_Move : MonoBehaviour 
{
	protected float MoveSpeed;
	protected GameObject _Enemy;
	
	public Enemy_Move(GameObject _Enemy)
	{
		this._Enemy = _Enemy;
	}
	public abstract void E_Movement();
	public abstract void E_MovePattern();
}
