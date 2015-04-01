using UnityEngine;
using System.Collections;
using UnityEditor;

public enum WheelType { FWD, RWD };


[CustomEditor(typeof(CarControllerV2)), CanEditMultipleObjects]
public class RCCEditor : Editor {
	
	CarControllerV2 carScript;
	
	Texture2D wheelIcon;
	Texture2D steerIcon;
	Texture2D configIcon;
	Texture2D lightIcon;
	Texture2D mobileIcon;
	Texture2D soundIcon;
	Texture2D gaugeIcon;
	Texture2D smokeIcon;
	
	bool WheelSettings;
	bool SteerSettings;
	bool Configurations;
	bool LightSettings;
	bool SoundSettings;
	bool MobileSettings;
	bool DashBoardSettings;
	bool SmokeSettings;

	Keyframe lastKeyFrame;
	
	void Awake(){
		
		carScript = (CarControllerV2)target;
		
		wheelIcon = Resources.Load("WheelIcon", typeof(Texture2D)) as Texture2D;
		steerIcon = Resources.Load("SteerIcon", typeof(Texture2D)) as Texture2D;
		configIcon = Resources.Load("ConfigIcon", typeof(Texture2D)) as Texture2D;
		lightIcon = Resources.Load("LightIcon", typeof(Texture2D)) as Texture2D;
		mobileIcon = Resources.Load("MobileIcon", typeof(Texture2D)) as Texture2D;
		soundIcon = Resources.Load("SoundIcon", typeof(Texture2D)) as Texture2D;
		gaugeIcon = Resources.Load("GaugeIcon", typeof(Texture2D)) as Texture2D;
		smokeIcon = Resources.Load("SmokeIcon", typeof(Texture2D)) as Texture2D;
		
	}
	
	
	
	public override void OnInspectorGUI () {
		
		serializedObject.Update();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		EditorGUILayout.BeginHorizontal();
		
		if(WheelSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(wheelIcon))
			if(!WheelSettings)	WheelSettings = true;
		else WheelSettings = false;
		
		if(SteerSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(steerIcon))
			if(!SteerSettings)	SteerSettings = true;
		else SteerSettings = false;
		
		if(Configurations)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(configIcon))
			if(!Configurations)	Configurations = true;
		else Configurations = false;
		
		if(LightSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(lightIcon))
			if(!LightSettings)	LightSettings = true;
		else LightSettings = false;
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		
		if(SoundSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(soundIcon))
			if(!SoundSettings)	SoundSettings = true;
		else SoundSettings = false;
		
		if(MobileSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(mobileIcon))
			if(!MobileSettings)	MobileSettings = true;
		else MobileSettings = false;
		
		if(DashBoardSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(gaugeIcon))
			if(!DashBoardSettings)	DashBoardSettings = true;
		else DashBoardSettings = false;
		
		if(SmokeSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(smokeIcon))
			if(!SmokeSettings)	SmokeSettings = true;
		else SmokeSettings = false;
		
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.EndHorizontal();
		
		GUI.backgroundColor = Color.gray;
		
		if(MobileSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Mobile Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("mobileController"),false);

			if(carScript.mobileController){
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_mobileControllerType"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("useAccelerometerForSteer"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelControl"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("gyroTiltMultiplier"),false);

				EditorGUILayout.Space();
				GUI.color = Color.cyan;
				EditorGUILayout.LabelField("NGUI Elements");
				GUI.color = Color.white;
				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(serializedObject.FindProperty("gasPedal"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("brakePedal"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("handBrakeGui"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("leftArrow"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("rightArrow"),false);

				EditorGUILayout.Space();
				GUI.color = Color.cyan;
				EditorGUILayout.LabelField("Unity GUI Elements");
				GUI.color = Color.white;
				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(serializedObject.FindProperty("gas"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("brake"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrake"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("left"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("right"),false);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelMaximumSteerAngle"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelXOffset"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelYOffset"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelPivotPos"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelResetPosSpeed"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelTexture"),false);

			}
			
		}
		
		if(WheelSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Wheel Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			carScript.Wheel_FL = (WheelCollider)EditorGUILayout.ObjectField("Front Left Wheel Collider", carScript.Wheel_FL, typeof(WheelCollider), true);
			carScript.Wheel_FR = (WheelCollider)EditorGUILayout.ObjectField("Front Right Wheel Collider", carScript.Wheel_FR, typeof(WheelCollider), true);
			carScript.Wheel_RL = (WheelCollider)EditorGUILayout.ObjectField("Rear Left Wheel Collider", carScript.Wheel_RL, typeof(WheelCollider), true);
			carScript.Wheel_RR = (WheelCollider)EditorGUILayout.ObjectField("Rear Right Wheel Collider", carScript.Wheel_RR, typeof(WheelCollider), true);
			EditorGUILayout.Space();
			carScript.FrontLeftWheelT = (Transform)EditorGUILayout.ObjectField("Front Left Wheel Transform", carScript.FrontLeftWheelT, typeof(Transform), true);
			carScript.FrontRightWheelT = (Transform)EditorGUILayout.ObjectField("Front Right Wheel Transform", carScript.FrontRightWheelT, typeof(Transform), true);
			carScript.RearLeftWheelT = (Transform)EditorGUILayout.ObjectField("Rear Left Wheel Transform", carScript.RearLeftWheelT, typeof(Transform), true);
			carScript.RearRightWheelT = (Transform)EditorGUILayout.ObjectField("Rear Right Wheel Transform", carScript.RearRightWheelT, typeof(Transform), true);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ExtraRearWheels"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ExtraRearWheelsT"), true);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("SteeringWheel"), false);
			
		}
		
		if(SteerSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Steer Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			carScript.SteerAngle = EditorGUILayout.Slider("Steer Angle", carScript.SteerAngle, 5, 45);
			carScript.HighSpeedSteerAngle = EditorGUILayout.Slider("Steer Angle At High Speed", carScript.HighSpeedSteerAngle, 5, carScript.SteerAngle);
			carScript.HighSpeedSteerAngleAtSpeed = EditorGUILayout.Slider("High Speed for Steer Angle", carScript.HighSpeedSteerAngleAtSpeed, 0, carScript.maxSpeed);
			EditorGUILayout.Space();
			
		}
		
		if(Configurations){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Configurations");
			GUI.color = Color.white;
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("canControl"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("canBurnout"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("driftMode"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoReverse"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_wheelTypeChoise"));
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("COM"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("gearShiftRate"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EngineTorqueCurve"), false);

			lastKeyFrame.time = carScript.maxSpeed;
			lastKeyFrame.value = .3f;

			if(carScript.EngineTorqueCurve != null){
				if(carScript.EngineTorqueCurve.keys.Length > 0)
					carScript.EngineTorqueCurve.MoveKey(carScript.EngineTorqueCurve.length-1, lastKeyFrame);
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("EngineTorque"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpeed"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("useDifferantial"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxEngineRPM"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("MinEngineRPM"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Brake"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrakeStiffness"), false);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("chassis"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("chassisVerticalLean"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("chassisHorizontalLean"), false);
			
		}
		
		if(SoundSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Sound Settings");
			GUI.color = Color.white;

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("engineClip"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("skidClip"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("crashClips"), true);
			EditorGUILayout.Space();

			
		}
		
		if(DashBoardSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("DashBoard Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("demoGUI"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dashBoard"), false);
			
			if(carScript.dashBoard){
				EditorGUILayout.PropertyField(serializedObject.FindProperty("speedOMeter"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("speedOMeterNeedle"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("kiloMeter"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("kiloMeterNeedle"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("dashBoardFont"), false);
			}
			
		}
		
		if(SmokeSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Smoke Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("WheelSlipPrefab"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("normalExhaustGas"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("heavyExhaustGas"), false);
			
		}
		
		if(LightSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Light Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("HeadLights"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("BrakeLights"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ReverseLights"), true);
			
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		if(GUI.changed)
			EditorUtility.SetDirty(carScript);
		
		serializedObject.ApplyModifiedProperties();
		
	}
	
}
