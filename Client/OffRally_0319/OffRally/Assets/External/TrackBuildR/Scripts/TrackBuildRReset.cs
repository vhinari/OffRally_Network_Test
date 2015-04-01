using UnityEngine;

/// <summary>
/// This script allows the player to press the R button to reset their vehicle onto the track
/// It will also automatically reset the player if they deviate from the track too much
/// </summary>
public class TrackBuildRReset : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private TrackBuildRTrack track;

    [SerializeField]
    private BoxCollider[] noTrackingAreas;//defined box colliders that will define areas that the target can't reset in

    [SerializeField]
    private float resetHeight = 0.5f;//height off track for reset to take place

    [SerializeField]
    private float maxDistance = 10.0f;//maximum distance allowed when recarding target position

    [SerializeField]
    private float resetDistance = 20.0f;//distance from track that will cause an automatic reset

    //Private variables
    private Rigidbody targetRigidBody;
    private float nearestPercent;
    private Vector3 nearestPostion;
    private Vector3 nearestDirection;
    private bool insideIgnore;

    void Start()
    {
        targetRigidBody = target.rigidbody;
        UpdateCalculations(true);
    }

    void OnEnable()
    {
        InvokeRepeating("UpdateCalculations", 1.0f, 1.0f);
    }

    void OnDisable()
    {
        CancelInvoke("UpdateCalculations");
    }

	void Update () 
    {
        if(Input.GetKeyDown(KeyCode.R))
            ResetTarget();
	}

    private void UpdateCalculations()
    {
        UpdateCalculations(false);
    }

    private void UpdateCalculations(bool initalCalculation)
    {
        if(target == null)
            return;

        Vector3 targetPosition = target.position;
        foreach (BoxCollider area in noTrackingAreas)
        {
            if(area.bounds.Contains(targetPosition))
            {
                insideIgnore = true;
                return;
            }
        }

        if(insideIgnore)
        {
            insideIgnore = false;
            initalCalculation = true;
        }

        float trackLength = track.trackLength;
        float trackSearchPoints = trackLength / 10.0f;
        float trackSearchIncrement = 1.0f / trackSearchPoints;
        float nearestSqrDistance = Mathf.Infinity;
        float newNearestPercent = 0;
        Vector3 newNearestPosition = targetPosition;
        for (float i = 0; i < 1.0f; i += trackSearchIncrement)
        {
            Vector3 serachPoint = track.GetTrackPosition(i);
            Vector3 searchDifference = serachPoint - targetPosition;
            float searchSqrDist = Vector3.SqrMagnitude(searchDifference);
            if (searchSqrDist < nearestSqrDistance)
            {
                nearestSqrDistance = searchSqrDist;
                newNearestPercent = i;
                newNearestPosition = serachPoint;
            }
        }

        float calculatedDistance = Vector3.Distance(targetPosition, newNearestPosition);
        if(calculatedDistance > maxDistance && !initalCalculation)
        {
            return;
        }

        float percentAdvance = Mathf.Abs(nearestPercent - newNearestPercent);
        if (percentAdvance > 0.1f && percentAdvance < 0.9f && !initalCalculation)
        {
            ResetTarget();
            return;
        }

        float distanceFromTrack = Vector3.Distance(targetPosition, newNearestPosition);
        if (distanceFromTrack > resetDistance)
        {
            ResetTarget();
            return;
        }

        nearestPercent = newNearestPercent;
        nearestPostion = newNearestPosition;
        nearestDirection = track.GetTrackDirection(nearestPercent);
    }

    private void ResetTarget()
    {
        if(target!= null)
        {
            target.position = nearestPostion + Vector3.up * resetHeight;
            target.rotation = Quaternion.LookRotation(nearestDirection);
            if(targetRigidBody != null)
            {
                targetRigidBody.velocity = Vector3.zero;
                targetRigidBody.angularVelocity = Vector3.zero;
            }
            UpdateCalculations(true);
        }
    }
}
