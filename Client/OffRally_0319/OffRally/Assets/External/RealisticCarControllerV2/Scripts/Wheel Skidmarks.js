#pragma strict

var vehicle : GameObject;
var startSlipValue : float = 5;
private var skidmarks : Skidmarks = null;
private var lastSkidmark : int = -1;
private var wheel_col : WheelCollider;

function Start ()
	{
	wheel_col = GetComponent(WheelCollider);
	if(FindObjectOfType(Skidmarks))
	{
		skidmarks = FindObjectOfType(Skidmarks);
	}
	else
		Debug.Log("No skidmarks object found. Skidmarks will not be drawn. Drag ''SkidmarksManager'' from Prefabs folder, and drop on to your existing scene...");
	}

function FixedUpdate ()
	{
	if(skidmarks){
		var GroundHit : WheelHit;
		wheel_col.GetGroundHit( GroundHit );
		var wheelSlipAmountSideways = Mathf.Abs( GroundHit.sidewaysSlip );

		if ( wheelSlipAmountSideways > startSlipValue)
		{

		var skidPoint : Vector3 = GroundHit.point + 2*(vehicle.rigidbody.velocity) * Time.deltaTime;

		lastSkidmark = skidmarks.AddSkidMark(skidPoint, GroundHit.normal, wheelSlipAmountSideways/25, lastSkidmark);	
		}
		else
		{
		lastSkidmark = -1;
		}
		
	}
	}