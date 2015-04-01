using UnityEngine;
using System.Collections;

public class TrackBuildRCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target = null;

    [SerializeField]
    private float distance = 4.5f;

    [SerializeField]
    private float height = 2.0f;

    [SerializeField]
    private Vector3 modifyAngle = Vector3.zero;

    void LateUpdate()
    {
        if(target != null)
        {
            Vector3 targetPos = target.position - target.forward * distance + target.up * height;
            Vector3 targetDirection = target.position - targetPos;
            Quaternion targetRot = Quaternion.LookRotation(targetDirection, target.up);
            Quaternion modifyAngleQ = Quaternion.Euler(modifyAngle);

            transform.position = targetPos;// Vector3.Lerp(transform.position, targetPos, 0.5f);
            transform.rotation = targetRot * modifyAngleQ;
        }
    }
}
