using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]

public class CarControllerV2 : MonoBehaviour
{

    //Mobile Controller, 모바일
    public bool mobileController = false;

    public enum MobileGUIType { NGUIController, UnityGUIController };
    public MobileGUIType _mobileControllerType;
    [HideInInspector]
    public bool NGUIController = false, UnityGUIController = true;

    public bool useAccelerometerForSteer = false, steeringWheelControl = false;
    private Vector3 defBrakePedalPosition;
    public bool demoGUI = false;
    public bool dashBoard = false;
    private bool andHandBrake = false;
    public float gyroTiltMultiplier = 2f;

    public NGUIController gasPedal, brakePedal, leftArrow, rightArrow, handBrakeGui;
    public GUITexture gas, brake, left, right, handbrake;

    //Wheel colliders of the vehicle.
    public WheelCollider Wheel_FL;
    public WheelCollider Wheel_FR;
    public WheelCollider Wheel_RL;
    public WheelCollider Wheel_RR;

    // Wheel transforms of the vehicle.
    public Transform FrontLeftWheelT;
    public Transform FrontRightWheelT;
    public Transform RearLeftWheelT;
    public Transform RearRightWheelT;

    public WheelCollider[] ExtraRearWheels;
    public Transform[] ExtraRearWheelsT;

    // Driver Steering Wheel.
    public Transform SteeringWheel;

    // Set wheel drive of the vehicle. If you are using rwd, you have to be careful with your rear wheel collider
    // settings and com of the vehicle. Otherwise, vehicle will behave like a toy. ***My advice is use fwd always***
    // 휠타입 변경 가능하다 기본은 fwd 타입을 사용한다. 
    public enum WheelType { FWD, RWD };
    public WheelType _wheelTypeChoise;
    private bool rwd = false, fwd = true;

    //Center of mass. 중심질량.
    public Transform COM;

    // Drift Configurations //드리프트 구성.
    public int steeringAssistanceDivider = 5;
    private float driftAngle;//드리프트 각도.
    private float stabilizerAssistance = 500f;//안정하게 드리프트 할때 필요한 변수.

    //Vehicle Mecanim 자동차 초기값
    public bool canControl = true;
    public bool canBurnout = true;
    public bool driftMode = false;
    public bool autoReverse = true;
    private bool canGoReverseNow = false;
    public float gearShiftRate = 10.0f;
    public int CurrentGear;
    public AnimationCurve EngineTorqueCurve;//커브
    private float[] GearRatio;
    public float EngineTorque = 600.0f;
    public float MaxEngineRPM = 6000.0f;
    public float MinEngineRPM = 1000.0f;
    public float SteerAngle = 20.0f;
    public float HighSpeedSteerAngle = 10.0f;
    public float HighSpeedSteerAngleAtSpeed = 80.0f;
    [HideInInspector]
    public float Speed;
    public float Brake = 200.0f;
    public float handbrakeStiffness = 0.025f;
    public float maxSpeed = 180.0f;
    public bool useDifferantial = true;
    private float differantialRatioRight;
    private float differantialRatioLeft;
    private float differantialDifference;

    // Each wheel transform's rotation value. 바귀의 각각의 회전 값을 변환 하는 변수.
    private float RotationValueFL, RotationValueFR, RotationValueRL, RotationValueRR;
    private float[] RotationValueExtra;

    //Misc.
    private float defSteerAngle;
    private float StiffnessRear;
    private float StiffnessFront;
    private bool reversing = false;
    private bool centerSteer = false;
    private bool headLightsOn = false;
    private float acceleration = 0f;
    private float lastVelocity = 0f;
    private float gearTimeMultiplier;

    //Audio.
    private GameObject skidAudio;
    public AudioClip skidClip;
    private GameObject crashAudio;
    public AudioClip[] crashClips;
    private GameObject engineAudio;
    public AudioClip engineClip;
    
    //Collision Force Limit. 충돌 강제 제한
    private int collisionForceLimit = 5;

    //Inputs.
    private float EngineRPM;
    [HideInInspector]
    public float motorInput;
    [HideInInspector]
    public float steerInput;

    //DashBoard.
    public Texture2D speedOMeter;
    public Texture2D speedOMeterNeedle;
    public Texture2D kiloMeter;
    public Texture2D kiloMeterNeedle;
    private float needleRotation = 0.0f;
    private float kMHneedleRotation = 0.0f;
    private float smoothedNeedleRotation = 0.0f;
    public Font dashBoardFont;
    public float guiWidth = 0.0f;
    public float guiHeight = 0.0f;

    //Smokes.
    public GameObject WheelSlipPrefab;
    private List<GameObject> WheelParticles = new List<GameObject>();
    public ParticleEmitter normalExhaustGas;
    public ParticleEmitter heavyExhaustGas;

    //Sideways Frictions. 커브 마찰력 
    private WheelFrictionCurve RearLeftFriction;
    private WheelFrictionCurve RearRightFriction;
    private WheelFrictionCurve FrontLeftFriction;
    private WheelFrictionCurve FrontRightFriction;
    private WheelFrictionCurve RearLeftFrictionForward;
    private WheelFrictionCurve RearRightFrictionForward;

    //Chassis Simulation. 자동차 몸체 
    public GameObject chassis; //자동차 모델링 넣기
    public float chassisVerticalLean = 3.0f;//세로축 자동차 기울기
    public float chassisHorizontalLean = 3.0f;//가로축 자동차 기울기
    private float horizontalLean = 0.0f;
    private float verticalLean = 0.0f;

    //Lights.
    public Light[] HeadLights;
    public Light[] BrakeLights;
    public Light[] ReverseLights;

    //Steering Wheel Controller 스트어링 휠 콜린더.
    public float steeringWheelMaximumSteerAngle = 180f;
    public float steeringWheelGuiScale = 256f;
    public float steeringWheelXOffset = 30;
    public float steeringWheelYOffset = 30;
    public Vector2 steeringWheelPivotPos = Vector2.zero;
    public float steeringWheelResetPosSpeed = 200f;
    public Texture2D steeringWheelTexture;
    private float steeringWheelsteerAngle;
    private bool steeringWheelIsTouching;
    private Rect steeringWheelTextureRect;
    private Vector2 steeringWheelWheelCenter;
    private float steeringWheelOldAngle;
    private int touchId = -1;
    private Vector2 touchPos;

    private Transform dynamicCOM;
    private float vehicleSizeX;
    private float vehicleSizeY;
    private float vehicleSizeZ;


    void Start()
    {

        SetWheelFrictions();
        SoundsInitialize();
        WheelTypeInit();
        GearInit();
        MobileGUI();
        SteeringWheelInit();
        if (WheelSlipPrefab)
            SmokeInit();

        Time.fixedDeltaTime = .03f; //물리 시간을 조작해서 조금더 안정적이게 물리 효과를 나타 나게 한다.
        GetComponent<Rigidbody>().centerOfMass = new Vector3(COM.localPosition.x * transform.localScale.x, COM.localPosition.y * transform.localScale.y, COM.localPosition.z * transform.localScale.z);
        //무게 중심 초기화 값(이걸 조정해서 자동차가 쉽게 날라가는걸 수정 할수 있다).
        GetComponent<Rigidbody>().maxAngularVelocity = 3.5f;
        guiWidth = Screen.width / 2 - 200;

        dynamicCOM = new GameObject("Dynamic Com").transform;
        dynamicCOM.parent = transform;//부모의 위치값을 받는다.

        if (mobileController)//모바일용
        {
            if (NGUIController)
                defBrakePedalPosition = brakePedal.transform.position;
            if (UnityGUIController)
                defBrakePedalPosition = brake.transform.position;
        }

        Renderer[] r = GetComponentsInChildren<Renderer>();
        if (r.Length > 0)
        {
            Array.Sort(r, delegate(Renderer r1, Renderer r2)
            {
                return r2.bounds.size.magnitude.CompareTo(r1.bounds.size.magnitude);
            });
            vehicleSizeX = r[0].bounds.size.x;
            vehicleSizeY = r[0].bounds.size.y;
            vehicleSizeZ = r[0].bounds.size.z;
        }

    }

    void SetWheelFrictions()//휠 셋팅 
    {

        RearLeftFriction = Wheel_RL.sidewaysFriction;
        RearRightFriction = Wheel_RR.sidewaysFriction;
        FrontLeftFriction = Wheel_FL.sidewaysFriction;
        FrontRightFriction = Wheel_FR.sidewaysFriction;

        RearLeftFrictionForward = Wheel_RL.forwardFriction;
        RearRightFrictionForward = Wheel_RR.forwardFriction;

        RotationValueExtra = new float[ExtraRearWheels.Length];

        StiffnessRear = Wheel_RL.sidewaysFriction.stiffness;//힘과 미끄러짐의 비율 
        StiffnessFront = Wheel_FL.sidewaysFriction.stiffness;

        defSteerAngle = SteerAngle;

    }

    void SoundsInitialize()
    {

        engineAudio = new GameObject("EngineSound");
        engineAudio.transform.position = transform.position;
        engineAudio.transform.rotation = transform.rotation;
        engineAudio.transform.parent = transform;
        engineAudio.AddComponent<AudioSource>();
        engineAudio.GetComponent<AudioSource>().minDistance = 5;
        engineAudio.GetComponent<AudioSource>().volume = 0;
        engineAudio.GetComponent<AudioSource>().clip = engineClip;
        engineAudio.GetComponent<AudioSource>().loop = true;
        engineAudio.GetComponent<AudioSource>().Play();

        skidAudio = new GameObject("SkidSound");
        skidAudio.transform.position = transform.position;
        skidAudio.transform.rotation = transform.rotation;
        skidAudio.transform.parent = transform;
        skidAudio.AddComponent<AudioSource>();
        skidAudio.GetComponent<AudioSource>().minDistance = 10;
        skidAudio.GetComponent<AudioSource>().volume = 0;
        skidAudio.GetComponent<AudioSource>().clip = skidClip;
        skidAudio.GetComponent<AudioSource>().loop = true;
        skidAudio.GetComponent<AudioSource>().Play();

        crashAudio = new GameObject("CrashSound");
        crashAudio.transform.position = transform.position;
        crashAudio.transform.rotation = transform.rotation;
        crashAudio.transform.parent = transform;
        crashAudio.AddComponent<AudioSource>();
        crashAudio.GetComponent<AudioSource>().minDistance = 10;

    }//사운드 초기화 값 셋팅

    void WheelTypeInit()//휠에 맞는 타입 지정 
    {

        switch (_wheelTypeChoise)
        {
            case WheelType.FWD:
                fwd = true;
                rwd = false;
                break;
            case WheelType.RWD:
                fwd = false;
                rwd = true;
                break;
        }

        switch (_mobileControllerType)
        {
            case MobileGUIType.NGUIController:
                NGUIController = true;
                UnityGUIController = false;
                break;
            case MobileGUIType.UnityGUIController:
                NGUIController = false;
                UnityGUIController = true;
                break;
        }

    }

    void GearInit()//기어 셋팅
    {

        GearRatio = new float[EngineTorqueCurve.length];

        for (int i = 0; i < EngineTorqueCurve.length; i++)
        {

            GearRatio[i] = EngineTorqueCurve.keys[i].value;

        }

    }

    void Differantial()//차동?.자동차 휠에 맞는 움직임?
    {

        if (useDifferantial)
        {

            if (fwd)
            {
                differantialDifference = Mathf.Clamp(Mathf.Abs(Wheel_FR.rpm) - Mathf.Abs(Wheel_FL.rpm), -50f, 50f);
                differantialRatioRight = Mathf.Lerp(0f, 1f, ((((Mathf.Abs(Wheel_FR.rpm) + Mathf.Abs(Wheel_FL.rpm)) + 10 / 2) + differantialDifference) / (Mathf.Abs(Wheel_FR.rpm) + Mathf.Abs(Wheel_FL.rpm))));
                differantialRatioLeft = Mathf.Lerp(0f, 1f, ((((Mathf.Abs(Wheel_FR.rpm) + Mathf.Abs(Wheel_FL.rpm)) + 10 / 2) - differantialDifference) / (Mathf.Abs(Wheel_FR.rpm) + Mathf.Abs(Wheel_FL.rpm))));
            }
            if (rwd)
            {
                differantialDifference = Mathf.Clamp(Mathf.Abs(Wheel_RR.rpm) - Mathf.Abs(Wheel_RL.rpm), -50f, 50f);
                differantialRatioRight = Mathf.Lerp(0f, 1f, ((((Mathf.Abs(Wheel_RR.rpm) + Mathf.Abs(Wheel_RL.rpm)) + 10 / 2) + differantialDifference) / (Mathf.Abs(Wheel_RR.rpm) + Mathf.Abs(Wheel_RL.rpm))));
                differantialRatioLeft = Mathf.Lerp(0f, 1f, ((((Mathf.Abs(Wheel_RR.rpm) + Mathf.Abs(Wheel_RL.rpm)) + 10 / 2) - differantialDifference) / (Mathf.Abs(Wheel_RR.rpm) + Mathf.Abs(Wheel_RL.rpm))));
            }

        }
        else
        {

            differantialRatioRight = 1;
            differantialRatioLeft = 1;

        }

    }

    void MobileGUI()//모바일
    {

        if (!mobileController)
        {
            if (gasPedal)
                gasPedal.gameObject.SetActive(false);
            if (brakePedal)
                brakePedal.gameObject.SetActive(false);
            if (leftArrow)
                leftArrow.gameObject.SetActive(false);
            if (rightArrow)
                rightArrow.gameObject.SetActive(false);
            if (handBrakeGui)
                handBrakeGui.gameObject.SetActive(false);
            if (gas)
                gas.gameObject.SetActive(false);
            if (brake)
                brake.gameObject.SetActive(false);
            if (left)
                left.gameObject.SetActive(false);
            if (right)
                right.gameObject.SetActive(false);
            if (handbrake)
                handbrake.gameObject.SetActive(false);
        }

    }//모바일용 GUI 

    void SteeringWheelInit()//스트어링 셋팅 
    {

        steeringWheelGuiScale = (Screen.width * 1.0f) / 2.7f;
        steeringWheelIsTouching = false;
        steeringWheelTextureRect = new Rect(steeringWheelXOffset + (steeringWheelGuiScale / Screen.width), -steeringWheelYOffset + (Screen.height - (steeringWheelGuiScale)), steeringWheelGuiScale, steeringWheelGuiScale);
        steeringWheelWheelCenter = new Vector2(steeringWheelTextureRect.x + steeringWheelTextureRect.width * 0.5f, Screen.height - steeringWheelTextureRect.y - steeringWheelTextureRect.height * 0.5f);
        steeringWheelsteerAngle = 0f;

    }

    void SmokeInit()//스모트 셋팅
    {

        Instantiate(WheelSlipPrefab, Wheel_FR.transform.position, transform.rotation);
        Instantiate(WheelSlipPrefab, Wheel_FL.transform.position, transform.rotation);
        Instantiate(WheelSlipPrefab, Wheel_RR.transform.position, transform.rotation);
        Instantiate(WheelSlipPrefab, Wheel_RL.transform.position, transform.rotation);

        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (go.name == "WheelSlip(Clone)")
                WheelParticles.Add(go);
        }

        WheelParticles[0].transform.position = Wheel_FR.transform.position;
        WheelParticles[1].transform.position = Wheel_FL.transform.position;
        WheelParticles[2].transform.position = Wheel_RR.transform.position;
        WheelParticles[3].transform.position = Wheel_RL.transform.position;

        WheelParticles[0].transform.parent = Wheel_FR.transform;
        WheelParticles[1].transform.parent = Wheel_FL.transform;
        WheelParticles[2].transform.parent = Wheel_RR.transform;
        WheelParticles[3].transform.parent = Wheel_RL.transform;

    }

    void Update()
    {

        WheelAlign();

        if (canControl)//내가 컨트롤 하면 라이팅 
        {
            Lights();

            if (mobileController)//모바일 확인 
            {
                MobileSteeringInputs();
                if (NGUIController)
                    NGUIControlling();
                if (UnityGUIController)
                    UnityGUIControlling();
                if (steeringWheelControl)
                    SteeringWheelControlling();
            }

            else
            {
                KeyboardControlling();//키 컨트롤
            }

            if (chassis)//차몸통
              Chassis();
        }

    }

    void FixedUpdate()
    {

        ShiftGears();
        SkidAudio();//오디오
        Braking();//브레이크
        Differantial();//휠에 대한 움직임을 나타 낸다 휠타입에 맞게.

        if (WheelSlipPrefab)//스모크
            SmokeInstantiateRate();

        if (canControl)
        {
            Engine();//자동차 엔진
        }


    }

    void Engine()
    {

        //Engine Curve
        if (EngineTorqueCurve.keys.Length >= 2)
        {
            if (CurrentGear == EngineTorqueCurve.length - 2)
                gearTimeMultiplier =  (((-EngineTorqueCurve[CurrentGear].time / gearShiftRate) / (maxSpeed * 3)) + 1f);
            else
                gearTimeMultiplier = ((-EngineTorqueCurve[CurrentGear].time / (maxSpeed * 3)) + 1f);
        }
        else
        {
            gearTimeMultiplier = 1;
            Debug.Log("You DID NOT CREATE any engine torque curve keys!, Please create 1 key at least...");
        }

        //Speed
        Speed = GetComponent<Rigidbody>().velocity.magnitude * 3.0f;//벡터를 float 형으로 변환할때 사용 한다. 벡터의 길이로 변환 
        //Stabilizer안정장치.
        GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * ((Mathf.Abs(motorInput) * steerInput) * (stabilizerAssistance * 5f)));

        //Acceleration Calculation.가속계산.
        acceleration = 0f;
        acceleration = (transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z;

        //Drag Limit.차의 속도에 따른 마찰력 증가.
        if (Speed < 100)
            GetComponent<Rigidbody>().drag = Mathf.Clamp((acceleration / 30), 0f, 1f);
        else
            GetComponent<Rigidbody>().drag = .04f;

        //Steer Limit.
        SteerAngle = Mathf.Lerp(defSteerAngle, HighSpeedSteerAngle, (Speed / HighSpeedSteerAngleAtSpeed));
//        print(SteerAngle);

        //Engine RPM.
        if (EngineTorqueCurve.keys.Length >= 2)
        {
            EngineRPM = ((((Mathf.Abs((Wheel_FR.rpm * gearShiftRate * Mathf.Clamp01(motorInput)) + (Wheel_FL.rpm * gearShiftRate * motorInput)) / 2) * (GearRatio[CurrentGear])) * gearTimeMultiplier) + MinEngineRPM);
        }
        else
        {
            EngineRPM = ((((Mathf.Abs((Wheel_FR.rpm * gearShiftRate * Mathf.Clamp01(motorInput)) + (Wheel_FL.rpm * gearShiftRate * motorInput)) / 2)) * gearTimeMultiplier) + MinEngineRPM);
        }

        //Reversing Bool.
        if (motorInput <= 0 && Wheel_RL.rpm < 20 && canGoReverseNow)
        {
            reversing = true;
        }
        else
        {
            reversing = false;
        }

        if (autoReverse)
        {
            canGoReverseNow = true;
        }
        else
        {
            if (motorInput == 0 && Speed < 5)
                canGoReverseNow = true;
            else if (motorInput < 0 && Wheel_RR.rpm > 5)
                canGoReverseNow = false;
        }

        //Engine Audio Volume.
        engineAudio.GetComponent<AudioSource>().volume = Mathf.Lerp(engineAudio.GetComponent<AudioSource>().volume, Mathf.Clamp(motorInput, .15f, 1f), Time.deltaTime * 5);

        if (Speed < 40 && !reversing && canBurnout)
        {
            engineAudio.GetComponent<AudioSource>().pitch = Mathf.Lerp(engineAudio.GetComponent<AudioSource>().pitch, Mathf.Clamp(motorInput * 2, 1f, 2f), Time.deltaTime * 5);
            skidAudio.GetComponent<AudioSource>().volume = Mathf.Lerp(skidAudio.GetComponent<AudioSource>().volume, Mathf.Clamp(motorInput, 0f, 1f), Time.deltaTime * 5);
        }
        else if (Speed > 5)
        {
            engineAudio.GetComponent<AudioSource>().pitch = Mathf.Lerp(engineAudio.GetComponent<AudioSource>().pitch, Mathf.Lerp(1f, 2f, (EngineRPM - MinEngineRPM / 1.5f) / (MaxEngineRPM + MinEngineRPM)), Time.deltaTime * 5);
        }
        else
        {
            engineAudio.GetComponent<AudioSource>().pitch = Mathf.Lerp(engineAudio.GetComponent<AudioSource>().pitch, Mathf.Clamp(motorInput * 2, 1f, 2f), Time.deltaTime * 5);
        }

        //Applying Torque.휠에 맞게 속도 및 모터 조절 한다
        if (rwd)
        {

            if (Speed > maxSpeed)
            {
                Wheel_RL.motorTorque = 0;
                Wheel_RR.motorTorque = 0;
            }
            else if (!reversing)
            {
                Wheel_RL.motorTorque = EngineTorque * Mathf.Clamp(motorInput * differantialRatioLeft, 0f, 1f) * EngineTorqueCurve.Evaluate(Speed);
                Wheel_RR.motorTorque = EngineTorque * Mathf.Clamp(motorInput * differantialRatioRight, 0f, 1f) * EngineTorqueCurve.Evaluate(Speed);
            }
            if (reversing)
            {
                if (Speed < 30)
                {
                    Wheel_RL.motorTorque = (EngineTorque * motorInput) / 3;
                    Wheel_RR.motorTorque = (EngineTorque * motorInput) / 3;
                }
                else
                {
                    Wheel_RL.motorTorque = 0;
                    Wheel_RR.motorTorque = 0;
                }
            }

        }

        if (fwd)
        {

            if (Speed > maxSpeed)
            {
                Wheel_FL.motorTorque = 0;
                Wheel_FR.motorTorque = 0;
            }
            else if (!reversing)
            {
                Wheel_FL.motorTorque = EngineTorque * Mathf.Clamp(motorInput * differantialRatioLeft, 0f, 1f) * EngineTorqueCurve.Evaluate(Speed);
                Wheel_FR.motorTorque = EngineTorque * Mathf.Clamp(motorInput * differantialRatioRight, 0f, 1f) * EngineTorqueCurve.Evaluate(Speed);
            }
            if (reversing)
            {
                if (Speed < 30)
                {
                    Wheel_FL.motorTorque = (EngineTorque * motorInput) / 3;
                    Wheel_FR.motorTorque = (EngineTorque * motorInput) / 3;
                }
                else
                {
                    Wheel_FL.motorTorque = 0;
                    Wheel_FR.motorTorque = 0;
                }
            }
        }

    }

    void MobileSteeringInputs()//모바일 전용 스트러일 인풋 (볼필요 없음).
    {

        if (useAccelerometerForSteer)
        {

            steerInput = Input.acceleration.x * gyroTiltMultiplier;
            //Accelerometer Inputs.
            if (!driftMode)
            {
                Wheel_FL.steerAngle = Mathf.Clamp((SteerAngle * steerInput), -SteerAngle, SteerAngle);
                Wheel_FR.steerAngle = Mathf.Clamp((SteerAngle * steerInput), -SteerAngle, SteerAngle);
            }
            else
            {
                Wheel_FL.steerAngle = Mathf.Clamp((SteerAngle * steerInput), -SteerAngle, SteerAngle) + (driftAngle / steeringAssistanceDivider);
                Wheel_FR.steerAngle = Mathf.Clamp((SteerAngle * steerInput), -SteerAngle, SteerAngle) + (driftAngle / steeringAssistanceDivider);
            }

        }
        else
        {

            if (!steeringWheelControl)
            {
                //TouchScreen Inputs.
                if (!driftMode)
                {
                    Wheel_FL.steerAngle = Mathf.Clamp((SteerAngle * steerInput), -SteerAngle, SteerAngle);
                    Wheel_FR.steerAngle = Mathf.Clamp((SteerAngle * steerInput), -SteerAngle, SteerAngle);
                }
                else
                {
                    Wheel_FL.steerAngle = Mathf.Clamp((SteerAngle * steerInput), -SteerAngle, SteerAngle) + (driftAngle / steeringAssistanceDivider);
                    Wheel_FR.steerAngle = Mathf.Clamp((SteerAngle * steerInput), -SteerAngle, SteerAngle) + (driftAngle / steeringAssistanceDivider);
                }

            }
            else
            {
                //SteeringWheel Inputs.
                if (!driftMode)
                {
                    Wheel_FL.steerAngle = (SteerAngle * (-steeringWheelsteerAngle / steeringWheelMaximumSteerAngle));
                    Wheel_FR.steerAngle = (SteerAngle * (-steeringWheelsteerAngle / steeringWheelMaximumSteerAngle));
                }
                else
                {
                    Wheel_FL.steerAngle = (SteerAngle * (-steeringWheelsteerAngle / steeringWheelMaximumSteerAngle)) + (driftAngle / steeringAssistanceDivider);
                    Wheel_FR.steerAngle = (SteerAngle * (-steeringWheelsteerAngle / steeringWheelMaximumSteerAngle)) + (driftAngle / steeringAssistanceDivider);
                }

            }

        }

    }

    void SteeringWheelControlling()//모바일용?
    {

        if (steeringWheelIsTouching)
        {

            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == touchId)
                {
                    touchPos = touch.position;

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        steeringWheelIsTouching = false;
                    }
                }
            }

            float newSteerAngle = Vector2.Angle(Vector2.up, touchPos - steeringWheelWheelCenter);

            if (Vector2.Distance(touchPos, steeringWheelWheelCenter) > 20f)
            {
                if (touchPos.x > steeringWheelWheelCenter.x)
                    steeringWheelsteerAngle -= newSteerAngle - steeringWheelOldAngle;
                else
                    steeringWheelsteerAngle += newSteerAngle - steeringWheelOldAngle;
            }

            if (steeringWheelsteerAngle > steeringWheelMaximumSteerAngle)
                steeringWheelsteerAngle = steeringWheelMaximumSteerAngle;
            else if (steeringWheelsteerAngle < -steeringWheelMaximumSteerAngle)
                steeringWheelsteerAngle = -steeringWheelMaximumSteerAngle;

            steeringWheelOldAngle = newSteerAngle;
        }
        else
        {

            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (steeringWheelTextureRect.Contains(new Vector2(touch.position.x, Screen.height - touch.position.y)))
                    {
                        steeringWheelIsTouching = true;
                        steeringWheelOldAngle = Vector2.Angle(Vector2.up, touch.position - steeringWheelWheelCenter);
                        touchId = touch.fingerId;
                    }
                }
            }

            if (!Mathf.Approximately(0f, steeringWheelsteerAngle))
            {
                float deltaAngle = steeringWheelResetPosSpeed * Time.deltaTime;

                if (Mathf.Abs(deltaAngle) > Mathf.Abs(steeringWheelsteerAngle))
                {
                    steeringWheelsteerAngle = 0f;
                    return;
                }

                if (steeringWheelsteerAngle > 0f)
                    steeringWheelsteerAngle -= deltaAngle;
                else
                    steeringWheelsteerAngle += deltaAngle;
            }
        }

    }

    void NGUIControlling()//NGUI 컨트롤 
    {

        motorInput = gasPedal.input + (-brakePedal.input);

        if (!useAccelerometerForSteer && !steeringWheelControl)
            steerInput = rightArrow.input + (-leftArrow.input);

        if (handBrakeGui.input > .1f)
            andHandBrake = true;
        else
            andHandBrake = false;

    }

    void UnityGUIControlling()//모바일용
    {

        if (centerSteer && !steeringWheelControl && !useAccelerometerForSteer)
        {

            if (steerInput * 10 < -1)
                steerInput += Time.deltaTime * 5f;
            if (steerInput * 10 > 1)
                steerInput -= Time.deltaTime * 5f;

            if (steerInput * 10 > -2 && steerInput * 10 < 2)
                steerInput = 0;

        }

        for (int i = 0; i < Input.touchCount; i++)
        {

            if (Input.GetTouch(i).phase != TouchPhase.Ended && gas.HitTest(Input.GetTouch(i).position))
            {
                motorInput = Mathf.Lerp(motorInput, 1, Time.deltaTime * 10);
                gas.color = new Color(.5f, .5f, .5f, 1f);
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Ended && gas.HitTest(Input.GetTouch(i).position))
            {
                motorInput = 0;
                gas.color = new Color(.5f, .5f, .5f, .35f);
            }

            if (Input.GetTouch(i).phase != TouchPhase.Ended && brake.HitTest(Input.GetTouch(i).position))
            {
                motorInput = Mathf.Lerp(motorInput, -1, Time.deltaTime * 10);
                brake.color = new Color(.5f, .5f, .5f, 1f);
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Ended && brake.HitTest(Input.GetTouch(i).position))
            {
                motorInput = 0;
                brake.color = new Color(.5f, .5f, .5f, .35f);
            }

            if (!useAccelerometerForSteer && !steeringWheelControl && Input.GetTouch(i).phase != TouchPhase.Ended && left.HitTest(Input.GetTouch(i).position))
            {
                if (Mathf.Abs(steerInput) < 1)
                {
                    if (steerInput > 0)
                        steerInput -= Time.deltaTime * 5f;
                    else
                        steerInput -= Time.deltaTime * 2.5f;
                }
                centerSteer = false;
                left.color = new Color(.5f, .5f, .5f, 1f);
            }

            else if (!useAccelerometerForSteer && !steeringWheelControl && Input.GetTouch(i).phase == TouchPhase.Ended && left.HitTest(Input.GetTouch(i).position))
            {
                centerSteer = true;
                left.color = new Color(.5f, .5f, .5f, .35f);
            }

            if (!useAccelerometerForSteer && !steeringWheelControl && Input.GetTouch(i).phase != TouchPhase.Ended && right.HitTest(Input.GetTouch(i).position))
            {
                if (Mathf.Abs(steerInput) < 1)
                {
                    if (steerInput < 0)
                        steerInput += Time.deltaTime * 5f;
                    else
                        steerInput += Time.deltaTime * 2.5f;
                }
                centerSteer = false;
                right.color = new Color(.5f, .5f, .5f, 1f);
            }

            else if (!useAccelerometerForSteer && !steeringWheelControl && Input.GetTouch(i).phase == TouchPhase.Ended && right.HitTest(Input.GetTouch(i).position))
            {
                centerSteer = true;
                right.color = new Color(.5f, .5f, .5f, .35f);
            }

            if (Input.GetTouch(i).phase != TouchPhase.Ended && handbrake.HitTest(Input.GetTouch(i).position))
            {
                andHandBrake = true;
                handbrake.color = new Color(.5f, .5f, .5f, 1f);
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Ended && handbrake.HitTest(Input.GetTouch(i).position))
            {
                andHandBrake = false;
                handbrake.color = new Color(.5f, .5f, .5f, .35f);
            }

        }

    }

    void KeyboardControlling()//자동차 input
    {

        motorInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        //Keyboard Inputs.
        Wheel_FL.steerAngle = (SteerAngle * steerInput) + (driftAngle / steeringAssistanceDivider);
        Wheel_FR.steerAngle = (SteerAngle * steerInput) + (driftAngle / steeringAssistanceDivider);
      
    }

    void ShiftGears()//스피드 마다 기어가 바뀐다.
    {

        for (int i = 0; i < EngineTorqueCurve.length; i++)
        {

            if (EngineTorqueCurve.Evaluate(Speed) < EngineTorqueCurve.keys[i].value)
                CurrentGear = i;

        }

    }

    void WheelAlign()
    {

        RaycastHit hit;//히트 감지
        WheelHit CorrespondingGroundHit;//휠이 땅에 있는지 감지 

        float test;
        float test2;

        //Front Left Wheel Transform.
        Vector3 ColliderCenterPointFL = Wheel_FL.transform.TransformPoint(Wheel_FL.center);
        
        Wheel_FL.GetGroundHit(out CorrespondingGroundHit);

        if (Physics.Raycast(ColliderCenterPointFL, -Wheel_FL.transform.up, out hit, (Wheel_FL.suspensionDistance + Wheel_FL.radius) * transform.localScale.y))
        {
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("CarCollider"))
            {
                test = FrontLeftWheelT.transform.position.y;
                FrontLeftWheelT.transform.position = hit.point + (Wheel_FL.transform.up * Wheel_FL.radius) * transform.localScale.y;
               //바퀴가 공중으로 올라 갔을때 
                test2 = test - FrontLeftWheelT.transform.position.y;
                if (test2 < -0.1)
                {
                    print("test");
                }
                //test2 = test - FrontLeftWheelT.transform.position.y;
                //if (test2 > 0.1)
                //{
                //    print("test");
                //}
                float extension = (-Wheel_FL.transform.InverseTransformPoint(CorrespondingGroundHit.point).y - Wheel_FL.radius) / Wheel_FL.suspensionDistance;

                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point + Wheel_FL.transform.up * (CorrespondingGroundHit.force / 8000),
                    extension <= 0.0 ? Color.magenta : Color.white);
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - Wheel_FL.transform.forward * CorrespondingGroundHit.forwardSlip, Color.green);
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - Wheel_FL.transform.right * CorrespondingGroundHit.sidewaysSlip, Color.red);
            }
        }
        else
        {
            FrontLeftWheelT.transform.position = ColliderCenterPointFL - (Wheel_FL.transform.up * Wheel_FL.suspensionDistance) * transform.localScale.y;
        }

        if (fwd && Speed < 20 && motorInput > 0 && canBurnout)
            RotationValueFL += Wheel_FL.rpm * (12) * Time.deltaTime;
        else
            RotationValueFL += Wheel_FL.rpm * (6) * Time.deltaTime;
        
        FrontLeftWheelT.transform.rotation = Wheel_FL.transform.rotation * Quaternion.Euler(RotationValueFL, Wheel_FL.steerAngle + (driftAngle / steeringAssistanceDivider), Wheel_FL.transform.rotation.z);
      
        //Front Right Wheel Transform.
        Vector3 ColliderCenterPointFR = Wheel_FR.transform.TransformPoint(Wheel_FR.center);
        Wheel_FR.GetGroundHit(out CorrespondingGroundHit);

        if (Physics.Raycast(ColliderCenterPointFR, -Wheel_FR.transform.up, out hit, (Wheel_FR.suspensionDistance + Wheel_FR.radius) * transform.localScale.y))
        {
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("CarCollider"))
            {
                FrontRightWheelT.transform.position = hit.point + (Wheel_FR.transform.up * Wheel_FR.radius) * transform.localScale.y;
                float extension = (-Wheel_FR.transform.InverseTransformPoint(CorrespondingGroundHit.point).y - Wheel_FR.radius) / Wheel_FR.suspensionDistance;
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point + Wheel_FR.transform.up * (CorrespondingGroundHit.force / 8000), extension <= 0.0 ? Color.magenta : Color.white);
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - Wheel_FR.transform.forward * CorrespondingGroundHit.forwardSlip, Color.green);
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - Wheel_FR.transform.right * CorrespondingGroundHit.sidewaysSlip, Color.red);
            }
        }
        else
        {
            FrontRightWheelT.transform.position = ColliderCenterPointFR - (Wheel_FR.transform.up * Wheel_FR.suspensionDistance) * transform.localScale.y;
        }

        if (fwd && Speed < 20 && motorInput > 0 && canBurnout)
            RotationValueFR += Wheel_FR.rpm * (12) * Time.deltaTime;
        else
            RotationValueFR += Wheel_FR.rpm * (6) * Time.deltaTime;

        FrontRightWheelT.transform.rotation = Wheel_FR.transform.rotation * Quaternion.Euler(RotationValueFR, Wheel_FR.steerAngle + (driftAngle / steeringAssistanceDivider), Wheel_FR.transform.rotation.z);


        //Rear Left Wheel Transform.
        Vector3 ColliderCenterPointRL = Wheel_RL.transform.TransformPoint(Wheel_RL.center);
        Wheel_RL.GetGroundHit(out CorrespondingGroundHit);

        if (Physics.Raycast(ColliderCenterPointRL, -Wheel_RL.transform.up, out hit, (Wheel_RL.suspensionDistance + Wheel_RL.radius) * transform.localScale.y))
        {
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("CarCollider"))
            {
                RearLeftWheelT.transform.position = hit.point + (Wheel_RL.transform.up * Wheel_RL.radius) * transform.localScale.y;
                float extension = (-Wheel_RL.transform.InverseTransformPoint(CorrespondingGroundHit.point).y - Wheel_RL.radius) / Wheel_RL.suspensionDistance;
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point + Wheel_RL.transform.up * (CorrespondingGroundHit.force / 8000), extension <= 0.0 ? Color.magenta : Color.white);
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - Wheel_RL.transform.forward * CorrespondingGroundHit.forwardSlip, Color.green);
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - Wheel_RL.transform.right * CorrespondingGroundHit.sidewaysSlip, Color.red);
            }
        }
        else
        {
            RearLeftWheelT.transform.position = ColliderCenterPointRL - (Wheel_RL.transform.up * Wheel_RL.suspensionDistance) * transform.localScale.y;
        }
        RearLeftWheelT.transform.rotation = Wheel_RL.transform.rotation * Quaternion.Euler(RotationValueRL, 0, Wheel_RL.transform.rotation.z);

        if (rwd && Speed < 20 && motorInput > 0 && canBurnout)
            RotationValueRL += Wheel_RL.rpm * (12) * Time.deltaTime;
        else
            RotationValueRL += Wheel_RL.rpm * (6) * Time.deltaTime;


        //Rear Right Wheel Transform.
        Vector3 ColliderCenterPointRR = Wheel_RR.transform.TransformPoint(Wheel_RR.center);
        Wheel_RR.GetGroundHit(out CorrespondingGroundHit);

        if (Physics.Raycast(ColliderCenterPointRR, -Wheel_RR.transform.up, out hit, (Wheel_RR.suspensionDistance + Wheel_RR.radius) * transform.localScale.y))
        {
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("CarCollider"))
            {
                RearRightWheelT.transform.position = hit.point + (Wheel_RR.transform.up * Wheel_RR.radius) * transform.localScale.y;
                float extension = (-Wheel_RR.transform.InverseTransformPoint(CorrespondingGroundHit.point).y - Wheel_RR.radius) / Wheel_RR.suspensionDistance;
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point + Wheel_RR.transform.up * (CorrespondingGroundHit.force / 8000), extension <= 0.0 ? Color.magenta : Color.white);
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - Wheel_RR.transform.forward * CorrespondingGroundHit.forwardSlip, Color.green);
                Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - Wheel_RR.transform.right * CorrespondingGroundHit.sidewaysSlip, Color.red);
            }
        }
        else
        {
            RearRightWheelT.transform.position = ColliderCenterPointRR - (Wheel_RR.transform.up * Wheel_RR.suspensionDistance) * transform.localScale.y;
        }
        RearRightWheelT.transform.rotation = Wheel_RR.transform.rotation * Quaternion.Euler(RotationValueRR, 0, Wheel_RR.transform.rotation.z);

        if (rwd && Speed < 20 && motorInput > 0 && canBurnout)
            RotationValueRR += Wheel_RR.rpm * (12) * Time.deltaTime;
        else
            RotationValueRR += Wheel_RR.rpm * (6) * Time.deltaTime;

        if (ExtraRearWheels.Length > 0)
        {

            for (int i = 0; i < ExtraRearWheels.Length; i++)
            {

                Vector3 ColliderCenterPointExtra = ExtraRearWheels[i].transform.TransformPoint(ExtraRearWheels[i].center);

                if (Physics.Raycast(ColliderCenterPointExtra, -ExtraRearWheels[i].transform.up, out hit, (ExtraRearWheels[i].suspensionDistance + ExtraRearWheels[i].radius) * transform.localScale.y))
                {
                    ExtraRearWheelsT[i].transform.position = hit.point + (ExtraRearWheels[i].transform.up * ExtraRearWheels[i].radius) * transform.localScale.y;
                }
                else
                {
                    ExtraRearWheelsT[i].transform.position = ColliderCenterPointExtra - (ExtraRearWheels[i].transform.up * ExtraRearWheels[i].suspensionDistance) * transform.localScale.y;
                    ExtraRearWheels[i].brakeTorque = Brake / 10;
                }
                ExtraRearWheelsT[i].transform.rotation = ExtraRearWheels[i].transform.rotation * Quaternion.Euler(RotationValueExtra[i], 0, ExtraRearWheels[i].transform.rotation.z);
                RotationValueExtra[i] += ExtraRearWheels[i].rpm * (6) * Time.deltaTime;
                ExtraRearWheels[i].GetGroundHit(out CorrespondingGroundHit);

            }

        }

        //Drift Angle Calculation.
        WheelHit CorrespondingGroundHit5;
        Wheel_RR.GetGroundHit(out CorrespondingGroundHit5);
        driftAngle = Mathf.Lerp(driftAngle, (Mathf.Clamp(CorrespondingGroundHit5.sidewaysSlip, -35, 35)), Time.deltaTime * 2);

        //Driver SteeringWheel.
        if (SteeringWheel)
            SteeringWheel.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, (Wheel_FL.steerAngle + (driftAngle / steeringAssistanceDivider)) * -6);

    }

    void DynamicWheelStiffness()
    {

        if (!driftMode)
        {
            RearLeftFriction.stiffness = Mathf.Lerp(RearLeftFriction.stiffness, StiffnessRear, Time.deltaTime * 2);
            RearRightFriction.stiffness = Mathf.Lerp(RearRightFriction.stiffness, StiffnessRear, Time.deltaTime * 2);
            FrontLeftFriction.stiffness = Mathf.Lerp(FrontLeftFriction.stiffness, StiffnessFront, Time.deltaTime * 2);
            FrontRightFriction.stiffness = Mathf.Lerp(FrontRightFriction.stiffness, StiffnessFront, Time.deltaTime * 2);
            Wheel_FL.sidewaysFriction = FrontRightFriction;
            Wheel_FR.sidewaysFriction = FrontLeftFriction;
            Wheel_RL.sidewaysFriction = RearLeftFriction;
            Wheel_RR.sidewaysFriction = RearRightFriction;
          
        }
        else
        {
            RearLeftFriction.stiffness = Mathf.Lerp(RearLeftFriction.stiffness, .035f, Time.deltaTime * 2);
            RearRightFriction.stiffness = Mathf.Lerp(RearRightFriction.stiffness, .035f, Time.deltaTime * 2);
            FrontLeftFriction.stiffness = Mathf.Lerp(FrontLeftFriction.stiffness, .045f, Time.deltaTime * 2);
            FrontRightFriction.stiffness = Mathf.Lerp(FrontRightFriction.stiffness, .045f, Time.deltaTime * 2);
            Wheel_FL.sidewaysFriction = FrontRightFriction;
            Wheel_FR.sidewaysFriction = FrontLeftFriction;
            Wheel_RL.sidewaysFriction = RearLeftFriction;
            Wheel_RR.sidewaysFriction = RearRightFriction;
            print("bb");
        }

    }

    void Braking()
    {

        //HandBrake
        if (Input.GetButton("Jump") || andHandBrake)
        {

            Wheel_RL.brakeTorque = Brake;
            Wheel_RR.brakeTorque = Brake;

            if (Speed > 5)
            {
                RearLeftFriction.stiffness = Mathf.Lerp(RearLeftFriction.stiffness, handbrakeStiffness, Time.deltaTime * 10);
                RearRightFriction.stiffness = Mathf.Lerp(RearRightFriction.stiffness, handbrakeStiffness, Time.deltaTime * 10);

                Wheel_RL.sidewaysFriction = RearLeftFriction;
                Wheel_RR.sidewaysFriction = RearRightFriction;
            }
            else
            {
                DynamicWheelStiffness();
            }

            //Normal Brake
        }
        else
        {

            DynamicWheelStiffness();

            if (motorInput == 0)
            {
                Wheel_RL.brakeTorque = Brake / 10f;
                Wheel_RR.brakeTorque = Brake / 10f;
                Wheel_FL.brakeTorque = Brake / 10f;
                Wheel_FR.brakeTorque = Brake / 10f;
            }
            else if (motorInput < 0 && !reversing)
            {
                Wheel_FL.brakeTorque = Brake * (Mathf.Abs(motorInput));
                Wheel_FR.brakeTorque = Brake * (Mathf.Abs(motorInput));
                Wheel_RL.brakeTorque = Brake * (Mathf.Abs(motorInput / 2));
                Wheel_RR.brakeTorque = Brake * (Mathf.Abs(motorInput / 2));
 
            }
            else
            {
                Wheel_RL.brakeTorque = 0;
                Wheel_RR.brakeTorque = 0;
                Wheel_FL.brakeTorque = 0;
                Wheel_FR.brakeTorque = 0;
            }

        }

    }

    void OnGUI()
    {

        GUI.skin.font = dashBoardFont;
        GUI.skin.label.fontSize = 12;
        GUI.skin.box.fontSize = 12;
        Matrix4x4 orgRotation = GUI.matrix;

        if (canControl)
        {

            if (useAccelerometerForSteer)
            {
                if (NGUIController)
                {
                    leftArrow.gameObject.SetActive(false);
                    rightArrow.gameObject.SetActive(false);
                    handBrakeGui.gameObject.SetActive(true);
                    brakePedal.transform.position = leftArrow.transform.position;
                }
                if (UnityGUIController)
                {
                    left.gameObject.SetActive(false);
                    right.gameObject.SetActive(false);
                    handbrake.gameObject.SetActive(true);
                    brake.transform.position = left.transform.position;
                }
                steeringWheelControl = false;
            }
            else if (mobileController)
            {
                if (NGUIController)
                {
                    leftArrow.gameObject.SetActive(true);
                    rightArrow.gameObject.SetActive(true);
                    handBrakeGui.gameObject.SetActive(true);
                    brakePedal.transform.position = defBrakePedalPosition;
                }
                if (UnityGUIController)
                {
                    left.gameObject.SetActive(true);
                    right.gameObject.SetActive(true);
                    handbrake.gameObject.SetActive(true);
                    brake.transform.position = defBrakePedalPosition;
                }
            }

            if (mobileController)
            {
                if (NGUIController)
                {
                    gasPedal.gameObject.SetActive(true);
                    brakePedal.gameObject.SetActive(true);
                    handBrakeGui.gameObject.SetActive(true);
                }
                if (UnityGUIController)
                {
                    gas.gameObject.SetActive(true);
                    brake.gameObject.SetActive(true);
                    handbrake.gameObject.SetActive(true);
                }
            }

            if (steeringWheelControl && mobileController)
            {
                if (NGUIController)
                {
                    leftArrow.gameObject.SetActive(false);
                    rightArrow.gameObject.SetActive(false);
                }
                if (UnityGUIController)
                {
                    left.gameObject.SetActive(false);
                    right.gameObject.SetActive(false);
                }
                GUIUtility.RotateAroundPivot(-steeringWheelsteerAngle, steeringWheelTextureRect.center + steeringWheelPivotPos);
                GUI.DrawTexture(steeringWheelTextureRect, steeringWheelTexture);
                GUI.matrix = orgRotation;
            }

            if (demoGUI)
            {

                GUI.backgroundColor = Color.black;

                GUI.Box(new Rect(Screen.width - 410 - guiWidth, 10 + guiHeight, 400, 220), "");
                GUI.Label(new Rect(Screen.width - 400 - guiWidth, 10 + guiHeight, 400, 150), "Engine RPM : " + Mathf.CeilToInt(EngineRPM));
                GUI.Label(new Rect(Screen.width - 400 - guiWidth, 30 + guiHeight, 400, 150), "Speed : " + Mathf.CeilToInt(Speed));
                GUI.Label(new Rect(Screen.width - 400 - guiWidth, 190 + guiHeight, 400, 150), "Horizontal Tilt : " + Input.acceleration.x);
                GUI.Label(new Rect(Screen.width - 400 - guiWidth, 210 + guiHeight, 400, 150), "Vertical Tilt : " + Input.acceleration.y);
                if (fwd)
                {
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 50 + guiHeight, 400, 150), "Left Wheel RPM : " + Mathf.CeilToInt(Wheel_FL.rpm));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 70 + guiHeight, 400, 150), "Right Wheel RPM : " + Mathf.CeilToInt(Wheel_FR.rpm));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 90 + guiHeight, 400, 150), "Left Wheel Torque : " + Mathf.CeilToInt(Wheel_FL.motorTorque));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 110 + guiHeight, 400, 150), "Right Wheel Torque : " + Mathf.CeilToInt(Wheel_FR.motorTorque));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 130 + guiHeight, 400, 150), "Left Wheel Brake : " + Mathf.CeilToInt(Wheel_FL.brakeTorque));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 150 + guiHeight, 400, 150), "Right Wheel Brake : " + Mathf.CeilToInt(Wheel_FR.brakeTorque));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 170 + guiHeight, 400, 150), "Steer Angle : " + Mathf.CeilToInt(Wheel_FL.steerAngle));
                }
                if (rwd)
                {
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 50 + guiHeight, 400, 150), "Left Wheel RPM : " + Mathf.CeilToInt(Wheel_RL.rpm));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 70 + guiHeight, 400, 150), "Right Wheel RPM : " + Mathf.CeilToInt(Wheel_RR.rpm));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 90 + guiHeight, 400, 150), "Left Wheel Torque : " + Mathf.CeilToInt(Wheel_RL.motorTorque));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 110 + guiHeight, 400, 150), "Right Wheel Torque : " + Mathf.CeilToInt(Wheel_RR.motorTorque));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 130 + guiHeight, 400, 150), "Left Wheel Brake : " + Mathf.CeilToInt(Wheel_RL.brakeTorque));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 150 + guiHeight, 400, 150), "Right Wheel Brake : " + Mathf.CeilToInt(Wheel_RR.brakeTorque));
                    GUI.Label(new Rect(Screen.width - 400 - guiWidth, 170 + guiHeight, 400, 150), "Steer Angle : " + Mathf.CeilToInt(Wheel_FL.steerAngle));
                }

                GUI.backgroundColor = Color.blue;
                GUI.Button(new Rect(Screen.width - 30 - guiWidth, 165 + guiHeight, 10, Mathf.Clamp((-motorInput * 100), -100, 0)), "");
                if (mobileController)
                {
                    if (GUI.Button(new Rect(Screen.width - 275, 200, 250, 50), "Use Accelerometer \n For Steer"))
                    {
                        if (useAccelerometerForSteer)
                            useAccelerometerForSteer = false;
                        else useAccelerometerForSteer = true;
                    }

                    if (GUI.Button(new Rect(Screen.width - 275, 275, 250, 50), "Use Steering Wheel"))
                    {
                        if (steeringWheelControl)
                            steeringWheelControl = false;
                        else steeringWheelControl = true;
                    }
                }

                GUI.backgroundColor = Color.red;
                GUI.Button(new Rect(Screen.width - 45 - guiWidth, 165 + guiHeight, 10, Mathf.Clamp((motorInput * 100), -100, 0)), "");

            }

            if (dashBoard)
            {

                GUI.skin.label.fontSize = 18;

                needleRotation = Mathf.Lerp(0, 180, (EngineRPM - MinEngineRPM / 1.5f) / (MaxEngineRPM + MinEngineRPM));
                kMHneedleRotation = Mathf.Lerp(-18, 210, Speed / 240);
                smoothedNeedleRotation = Mathf.Lerp(smoothedNeedleRotation, needleRotation, Time.deltaTime * 2);

                //if (Wheel_FL.rpm > -10)
                //    GUI.Label(new Rect(Screen.width - 90, 150, 40, 40), "" + (CurrentGear + 1));
                //else
                //    GUI.Label(new Rect(Screen.width - 90, 150, 40, 40), "R");

                GUI.DrawTexture(new Rect(Screen.width - 270, 50, 256, 128), speedOMeter);
                //GUI.Label(new Rect(Screen.width - 225, 150, 100, 40), "" + Mathf.CeilToInt(EngineRPM));
                GUI.Label(new Rect(84, 116, 100, 40), "" + Mathf.CeilToInt(Speed));

                GUI.DrawTexture(new Rect(10, 10, 180, 180), kiloMeter);

                GUIUtility.RotateAroundPivot(smoothedNeedleRotation, new Vector2(Screen.width - 142, 178));
                GUI.DrawTexture(new Rect(Screen.width - 270, 50, 256, 256), speedOMeterNeedle);

                GUI.matrix = orgRotation;
                GUIUtility.RotateAroundPivot(kMHneedleRotation, new Vector2(100, 100));
                GUI.DrawTexture(new Rect(45, 40, 110, 130), kiloMeterNeedle);
                GUI.matrix = orgRotation;
            }

        }

    }

    void SmokeInstantiateRate()
    {

        WheelHit CorrespondingGroundHit0;
        WheelHit CorrespondingGroundHit1;
        WheelHit CorrespondingGroundHit2;
        WheelHit CorrespondingGroundHit3;

        if (WheelParticles.Count > 0)
        {

            if (Speed > 40)
            {

                Wheel_FR.GetGroundHit(out CorrespondingGroundHit0);
                if (Mathf.Abs(CorrespondingGroundHit0.sidewaysSlip) > 15f || Mathf.Abs(CorrespondingGroundHit0.forwardSlip) > 4f)
                {
                    WheelParticles[0].GetComponent<ParticleEmitter>().emit = true;
                }
                else
                {
                    WheelParticles[0].GetComponent<ParticleEmitter>().emit = false;
                }

                Wheel_FL.GetGroundHit(out CorrespondingGroundHit1);
                if (Mathf.Abs(CorrespondingGroundHit1.sidewaysSlip) > 15f || Mathf.Abs(CorrespondingGroundHit1.forwardSlip) > 4f)
                {
                    WheelParticles[1].GetComponent<ParticleEmitter>().emit = true;
                }
                else
                {
                    WheelParticles[1].GetComponent<ParticleEmitter>().emit = false;
                }

                Wheel_RR.GetGroundHit(out CorrespondingGroundHit2);
                if (Mathf.Abs(CorrespondingGroundHit2.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit2.forwardSlip) > 4f)
                {
                    WheelParticles[2].GetComponent<ParticleEmitter>().emit = true;
                }
                else
                {
                    WheelParticles[2].GetComponent<ParticleEmitter>().emit = false;
                }

                Wheel_RL.GetGroundHit(out CorrespondingGroundHit3);
                if (Mathf.Abs(CorrespondingGroundHit3.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit3.forwardSlip) > 4f)
                {
                    WheelParticles[3].GetComponent<ParticleEmitter>().emit = true;
                }
                else
                {
                    WheelParticles[3].GetComponent<ParticleEmitter>().emit = false;
                }

            }
            else if (canBurnout)
            {

                Wheel_FR.GetGroundHit(out CorrespondingGroundHit0);
                if (Mathf.Abs(CorrespondingGroundHit0.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit0.forwardSlip) > 4f)
                    WheelParticles[0].GetComponent<ParticleEmitter>().emit = true;
                else WheelParticles[0].GetComponent<ParticleEmitter>().emit = false;

                Wheel_FL.GetGroundHit(out CorrespondingGroundHit1);
                if (Mathf.Abs(CorrespondingGroundHit1.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit1.forwardSlip) > 4f)
                    WheelParticles[1].GetComponent<ParticleEmitter>().emit = true;
                else WheelParticles[1].GetComponent<ParticleEmitter>().emit = false;

                Wheel_RR.GetGroundHit(out CorrespondingGroundHit2);
                if (Mathf.Abs(CorrespondingGroundHit2.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit2.forwardSlip) > 4f)
                    WheelParticles[2].GetComponent<ParticleEmitter>().emit = true;
                else WheelParticles[2].GetComponent<ParticleEmitter>().emit = false;

                Wheel_RL.GetGroundHit(out CorrespondingGroundHit3);
                if (Mathf.Abs(CorrespondingGroundHit3.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit3.forwardSlip) > 4f)
                    WheelParticles[3].GetComponent<ParticleEmitter>().emit = true;
                else WheelParticles[3].GetComponent<ParticleEmitter>().emit = false;

            }
            else
            {

                Wheel_FR.GetGroundHit(out CorrespondingGroundHit0);
                if (Mathf.Abs(CorrespondingGroundHit0.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit0.forwardSlip) > 4f)
                    WheelParticles[0].GetComponent<ParticleEmitter>().emit = true;
                else WheelParticles[0].GetComponent<ParticleEmitter>().emit = false;

                Wheel_FL.GetGroundHit(out CorrespondingGroundHit1);
                if (Mathf.Abs(CorrespondingGroundHit1.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit1.forwardSlip) > 4f)
                    WheelParticles[1].GetComponent<ParticleEmitter>().emit = true;
                else WheelParticles[1].GetComponent<ParticleEmitter>().emit = false;

                Wheel_RR.GetGroundHit(out CorrespondingGroundHit2);
                if (Mathf.Abs(CorrespondingGroundHit2.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit2.forwardSlip) > 4f)
                    WheelParticles[2].GetComponent<ParticleEmitter>().emit = true;
                else WheelParticles[2].GetComponent<ParticleEmitter>().emit = false;

                Wheel_RL.GetGroundHit(out CorrespondingGroundHit3);
                if (Mathf.Abs(CorrespondingGroundHit3.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit3.forwardSlip) > 4f)
                    WheelParticles[3].GetComponent<ParticleEmitter>().emit = true;
                else WheelParticles[3].GetComponent<ParticleEmitter>().emit = false;

            }

        }

        if (normalExhaustGas && canControl)
        {
            if (Speed < 20)
                normalExhaustGas.emit = true;
            else normalExhaustGas.emit = false;
        }

        if (heavyExhaustGas && canControl)
        {
            if (Speed < 10 && motorInput > 0)
                heavyExhaustGas.emit = true;
            else heavyExhaustGas.emit = false;
        }

        if (!canControl)
        {
            heavyExhaustGas.emit = false;
            normalExhaustGas.emit = false;
        }

    }

    void SkidAudio()
    {

        WheelHit CorrespondingGroundHit;
        Wheel_FR.GetGroundHit(out CorrespondingGroundHit);

        if (Mathf.Abs(CorrespondingGroundHit.sidewaysSlip) > 5f || Mathf.Abs(CorrespondingGroundHit.forwardSlip) > 4f)
            skidAudio.GetComponent<AudioSource>().volume = Mathf.Abs(CorrespondingGroundHit.sidewaysSlip) / 10 + Mathf.Abs(CorrespondingGroundHit.forwardSlip) / 10;
        else
            skidAudio.GetComponent<AudioSource>().volume -= Time.deltaTime;

    }

    void OnCollisionEnter(Collision collision)
    {


        if (collision.contacts.Length > 0)
        {

            if (collision.relativeVelocity.magnitude > collisionForceLimit && crashClips.Length > 0)
            {
                if (collision.contacts[0].thisCollider.gameObject.layer != LayerMask.NameToLayer("Wheel") && collision.gameObject.layer != LayerMask.NameToLayer("Road"))
                {
                    crashAudio.GetComponent<AudioSource>().clip = crashClips[UnityEngine.Random.Range(0, crashClips.Length)];
                    crashAudio.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(1f, 1.2f);
                    crashAudio.GetComponent<AudioSource>().Play();
                }
            }

        }

    }

    void Chassis()//차대 기울기
    {
        //좌우 기울기
        verticalLean = Mathf.Clamp(Mathf.Lerp(verticalLean, GetComponent<Rigidbody>().angularVelocity.x * chassisVerticalLean, Time.deltaTime * 5), -3.0f, 3.0f);

        if (!reversing)//역방향 일때 ex) 위아래 기울기
            horizontalLean = Mathf.Clamp(Mathf.Lerp(horizontalLean, GetComponent<Rigidbody>().angularVelocity.y * chassisHorizontalLean, Time.deltaTime * 4), -5.0f, 5.0f);

        Quaternion target = Quaternion.Euler(verticalLean, chassis.transform.localRotation.y + (GetComponent<Rigidbody>().angularVelocity.z), horizontalLean);
        chassis.transform.localRotation = target;


        
        dynamicCOM.localPosition = new Vector3(Mathf.Clamp((transform.InverseTransformDirection(rigidbody.angularVelocity).y), (-vehicleSizeX / 15), (vehicleSizeX / 15)) + COM.localPosition.x, -Mathf.Abs(Mathf.Clamp((transform.InverseTransformDirection(rigidbody.angularVelocity).z * 2), (-vehicleSizeY / 15), (vehicleSizeY / 15))) + COM.localPosition.y, Mathf.Clamp((transform.InverseTransformDirection(rigidbody.angularVelocity).x), (-vehicleSizeZ / 15), (vehicleSizeZ / 15)) + COM.localPosition.z);
        dynamicCOM.rotation = transform.rotation;
       // rigidbody.centerOfMass = new Vector3((dynamicCOM.localPosition.x) * transform.localScale.x, (dynamicCOM.localPosition.y) * transform.localScale.y, (dynamicCOM.localPosition.z) * transform.localScale.z);
     
    }

    void Lights()
    {

        float lightInput;
        lightInput = Mathf.Clamp(-motorInput * 2, 0.0f, 1.0f);

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (headLightsOn)
                headLightsOn = false;
            else headLightsOn = true;
        }

        for (int i = 0; i < BrakeLights.Length; i++)
        {

            if (!reversing)
                BrakeLights[i].intensity = lightInput;
            else
                BrakeLights[i].intensity = 0f;

        }

        for (int i = 0; i < HeadLights.Length; i++)
        {

            if (headLightsOn)
                HeadLights[i].enabled = true;
            else
                HeadLights[i].enabled = false;

        }

        for (int i = 0; i < ReverseLights.Length; i++)
        {

            if (!reversing)
                ReverseLights[i].intensity = Mathf.Lerp(ReverseLights[i].intensity, 0.0f, Time.deltaTime * 5);
            else
                ReverseLights[i].intensity = lightInput;

        }

    }

}