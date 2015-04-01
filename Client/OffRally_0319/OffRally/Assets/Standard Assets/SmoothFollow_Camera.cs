using UnityEngine;
using System.Collections;

public class SmoothFollow_Camera : MonoBehaviour {


    public Transform target;
    public float distance = 10.0f;
    public float height = 5.0f;

    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
    void LateUpdate()
    {
        if (!target)
            return;

        //현재 회전 각도를 계산
        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Y 축 주위 회전을 감쇠
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // 높이
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // 회전으로 각도를 변환
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        //
        transform.position = target.position;

        transform.position -= currentRotation * Vector3.forward * distance;


        // Set the height of the camera
        transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
        
        //transform.position.y += pos;
        // Always look at the target
        transform.LookAt(target);
	}
}
