using UnityEngine;
using System.Collections;

public class Checker : MonoBehaviour
{
    public int m_CheckerID;

    private const float m_fRepeatTime = 1f;
    private Transform Target;//차량 Transform

    void Awake()
    {
        Target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    //객체(현재 Chekcker) 활성화시 차량의 정방향 체크하는 함수를 연속으로 호출
    void OnEnable()
    {
        InvokeRepeating("CheckCarDirection", 0, m_fRepeatTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Car Collider"))
            transform.parent.GetComponent<LapCheck>().SendMessage("CheckerEnabler", m_CheckerID);
    }

    //객체(현재 Chekcker) 비활성화시 Invoke 비활성화
    void OnDisable()
    {
        CancelInvoke("CheckCarDirection");
    }

    //정방향 Check 함수 다음 Checker 기준 90도 이상 틀어지면 
    void CheckCarDirection()
    {
        Vector3 TargetToVec = Target.position - transform.position;
        TargetToVec.Normalize();
        if (Vector3.Dot(TargetToVec, -Target.forward) < 0f || Vector3.Dot(TargetToVec, -Target.forward) > 1f)
        {
            Debug.Log("Wrong");
        }
    }

    
}
