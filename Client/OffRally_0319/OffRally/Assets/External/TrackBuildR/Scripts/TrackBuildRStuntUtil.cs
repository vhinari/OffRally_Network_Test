using UnityEngine;
using System.Collections;

public class TrackBuildRStuntUtil
{
    public static void AddLoop(TrackBuildRTrack track, int selectedPointIndex)
    {
        TrackBuildRPoint atPoint = track[selectedPointIndex];
        int atPointIndex = selectedPointIndex;
        float loopRadius = track.loopRadius;
        float trackWidth = atPoint.width;
        Vector3 trackDirection = atPoint.trackDirection.normalized;
        Vector3 trackUp = atPoint.trackUpQ * Vector3.forward;
        Vector3 trackCross = atPoint.trackCross;

        Vector3 entryPosition = atPoint.worldPosition + (trackCross * trackWidth * 0.6f);
        Vector3 loopAxis = Vector3.Cross(trackDirection, trackUp);
        Vector3 loopCenter = atPoint.worldPosition + Vector3.up * loopRadius;
        Quaternion loopAngle = Quaternion.FromToRotation(Vector3.right, trackDirection);
        float controlPointLength = loopRadius / (Mathf.PI);
        Vector3 controlPointLateral = -loopAxis;

        int numberOfPoints = 6;
        float arcPercent = 1.0f / numberOfPoints;
        TrackBuildRPoint[] loopPoints = new TrackBuildRPoint[7];
        for (int i = 0; i < numberOfPoints; i++)
        {
            float pointArcPercent = arcPercent * i;
            TrackBuildRPoint newTrackPoint = track.InsertPoint(atPointIndex + 1);
            float rad = Mathf.PI * 2 * (pointArcPercent + 0.5f);
            Vector3 pointLoopPosition = loopAngle * ((new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0)) * loopRadius);
            Vector3 lateral = Vector3.Lerp((trackCross * trackWidth * -0.6f), (trackCross * trackWidth * 0.6f), pointArcPercent);
            Vector3 pointPosition = (pointLoopPosition) + lateral;
            Vector3 pointDirection = Vector3.Cross(-pointLoopPosition, loopAxis).normalized;
            newTrackPoint.worldPosition = loopCenter + pointPosition;
            newTrackPoint.trackUpQ = Quaternion.LookRotation(-pointLoopPosition, pointDirection);
            newTrackPoint.forwardControlPoint = newTrackPoint.worldPosition + (pointDirection * controlPointLength) - controlPointLateral;
            loopPoints[i] = newTrackPoint;
        }
        atPoint.worldPosition = entryPosition;
        atPoint.trackUpQ = Quaternion.LookRotation(Vector3.up, atPoint.trackDirection);
        atPoint.forwardControlPoint = atPoint.worldPosition + (trackDirection * controlPointLength) + controlPointLateral;
        loopPoints[6] = atPoint;
        //                    _trackBuildR.pointMode = TrackBuildR.pointModes.transform;

        for (int i = 0; i < numberOfPoints + 1; i++)
        {
            loopPoints[i].extrudeTrack = true;
            loopPoints[i].extrudeTrackBottom = true;
            loopPoints[i].extrudeLength = 0.5f;
            loopPoints[i].RecalculateStoredValues();
        }
    }

    public static void AddJump(TrackBuildRTrack track, int selectedPoint)
    {
        TrackBuildRPoint atPoint = track[selectedPoint];
        TrackBuildRPoint lastPoint = track.GetPoint(selectedPoint - 1);
        TrackBuildRPoint nextPoint = track.GetPoint(selectedPoint + 1);

        float trackPartDistance = lastPoint.arcLength + atPoint.arcLength;
        float jumpDistance = Mathf.Min(trackPartDistance * 0.333f, track.maxJumpLength);

        Vector3 jumpDirection = atPoint.trackDirection;
        Vector3 jumpMiddle = atPoint.worldPosition;
        Vector3 startCross = atPoint.trackCross;
        float trackWidth = atPoint.width * 0.5f;
        Quaternion trackUp = atPoint.trackUpQ;

        Vector3 jumpHeight = trackUp * (Vector3.forward * track.jumpHeight);
        Vector3 jumpStartPosition = jumpMiddle - jumpDirection * (jumpDistance * 0.33f);
        Vector3 jumpEndPosition = jumpMiddle + jumpDirection * (jumpDistance * 0.33f);

        lastPoint.extrudeTrack = true;
        lastPoint.extrudeLength = track.jumpHeight;
        lastPoint.extrudeCurveEnd = true;

        atPoint.Reset();
        atPoint.worldPosition = jumpStartPosition + jumpHeight;
        atPoint.forwardControlPoint = jumpDirection * (jumpDistance * 0.5f) + jumpStartPosition + jumpHeight * 2;
        atPoint.trackUpQ = trackUp;
        atPoint.render = false;

        TrackBuildRPoint jumpEnd = track.InsertPoint(selectedPoint + 1);
        jumpEnd.worldPosition = jumpEndPosition + jumpHeight;
        jumpEnd.forwardControlPoint = jumpDirection * (jumpDistance * 0.5f) + jumpEndPosition;
        jumpEnd.trackUpQ = trackUp;
        jumpEnd.extrudeTrack = true;
        jumpEnd.extrudeLength = track.jumpHeight;
        jumpEnd.extrudeCurveEnd = true;

        atPoint.RecalculateStoredValues();
        jumpEnd.RecalculateStoredValues();
    }


    public static void AddJumpTwist(TrackBuildRTrack track, int selectedPoint)
    {
        TrackBuildRPoint atPoint = track[selectedPoint];
        TrackBuildRPoint lastPoint = track.GetPoint(selectedPoint - 1);
        TrackBuildRPoint nextPoint = track.GetPoint(selectedPoint + 1);

        float trackPartDistance = lastPoint.arcLength + atPoint.arcLength;
        float jumpDistance = Mathf.Min(trackPartDistance * 0.333f, track.maxJumpLength);

        float trackWidth = atPoint.width * 0.5f;
        Vector3 startCross = atPoint.trackCross;
        Vector3 jumpDirection = atPoint.trackDirection;
        Vector3 jumpMiddle = atPoint.worldPosition;
        Quaternion atPointUpQ = atPoint.trackUpQ;
        Quaternion trackUpJump = Quaternion.AngleAxis(track.twistAngle, -jumpDirection);
        Quaternion trackCrossExit = trackUpJump * (atPointUpQ);
        Quaternion trackCrossEntry = Quaternion.Inverse(trackUpJump) * (atPointUpQ);
        Vector3 jumpLateral = startCross * track.twistAngle / 33.3f;

        Vector3 jumpHeight = atPointUpQ * (Vector3.forward * track.jumpHeight);
        Vector3 jumpStartPosition = jumpMiddle - jumpDirection * (jumpDistance * 0.33f) + jumpHeight - jumpLateral;
        Vector3 jumpEndPosition = jumpMiddle + jumpDirection * (jumpDistance * 0.33f) + jumpHeight + jumpLateral;

        lastPoint.extrudeTrack = true;
        lastPoint.extrudeLength = track.jumpHeight;
        lastPoint.extrudeCurveEnd = true;
        lastPoint.extrudeTrackBottom = true;

        atPoint.Reset();
        atPoint.worldPosition = jumpStartPosition;
        atPoint.forwardControlPoint = jumpDirection * (jumpDistance * 0.5f) + jumpStartPosition + jumpHeight;
        atPoint.trackUpQ = trackCrossExit;
        atPoint.render = false;

        TrackBuildRPoint jumpEnd = track.InsertPoint(selectedPoint + 1);
        jumpEnd.worldPosition = jumpEndPosition;
        jumpEnd.forwardControlPoint = jumpDirection * (jumpDistance * 0.5f) + jumpEndPosition - jumpHeight;
        jumpEnd.trackUpQ = trackCrossEntry;
        jumpEnd.extrudeTrack = true;
        jumpEnd.extrudeLength = track.jumpHeight;
        jumpEnd.extrudeCurveEnd = true;
        jumpEnd.extrudeTrackBottom = true;

        atPoint.RecalculateStoredValues();
        jumpEnd.RecalculateStoredValues();
    }
    /// <summary>
    /// unfinished!
    /// </summary>
    /// <param name="track"></param>
    /// <param name="selectedPoint"></param>
    public static void AddTwist(TrackBuildRTrack track, int selectedPoint)
    {
        TrackBuildRPoint atPoint = track[selectedPoint];
        TrackBuildRPoint lastPoint = track.GetPoint(selectedPoint - 1);

        float twistDistance = Mathf.Min((lastPoint.arcLength + atPoint.arcLength) * 0.333f, track.maxJumpLength);
        Vector3 twistDirection = atPoint.trackDirection;
        Vector3 twistMiddle = atPoint.worldPosition;
        Vector3 twistUp = atPoint.trackUp;
        Vector3 twistAxis = Vector3.Cross(twistDirection, twistUp);
        float twistRadius = track.twistRadius;
        Vector3 twistStartPosition = -twistDirection * (twistDistance * 0.33f);
        Vector3 twistEndPosition = twistDirection * (twistDistance * 0.33f);
        Vector3 twistCentreHeight = twistUp * twistRadius;
        Quaternion twistAngle = Quaternion.LookRotation(twistDirection, twistUp);
        Vector3 twistCenter = atPoint.worldPosition + Vector3.up * twistRadius;
        float controlPointLength = twistRadius / (Mathf.PI);

        int numberOfPoints = track.twistPoints;
        float arcPercent = 1.0f / numberOfPoints;
        TrackBuildRPoint[] loopPoints = new TrackBuildRPoint[numberOfPoints+1];
        for (int i = 0; i < numberOfPoints; i++)
        {
            float pointArcPercent = arcPercent * i;
            float radA = Mathf.PI * 2 * (pointArcPercent + 0.5f);
            Vector3 pointLoopPosition = twistAngle * ((new Vector3(Mathf.Sin(radA), Mathf.Cos(radA), 0)) * twistRadius);
            float smoothI = pointArcPercent * pointArcPercent * (3.0f - 2.0f * pointArcPercent);
            Vector3 lateral = Vector3.Lerp(twistStartPosition, twistEndPosition, pointArcPercent + (pointArcPercent - smoothI));
            Vector3 pointPosition = (pointLoopPosition) + lateral;
            Vector3 pointDirection = Vector3.Cross(-pointLoopPosition, twistAxis).normalized;

            TrackBuildRPoint newTrackPoint = track.InsertPoint(selectedPoint + 1 + i);
            newTrackPoint.worldPosition = twistCenter + pointPosition;
            newTrackPoint.trackUpQ = Quaternion.LookRotation(-pointLoopPosition, pointDirection);
            newTrackPoint.forwardControlPoint = newTrackPoint.worldPosition + (pointDirection * controlPointLength);
            loopPoints[i] = newTrackPoint;
        }
        atPoint.worldPosition += twistStartPosition;
        atPoint.trackUpQ = Quaternion.LookRotation(Vector3.up, atPoint.trackDirection);
        atPoint.forwardControlPoint = atPoint.worldPosition + (twistDirection * controlPointLength) - twistAxis;
        loopPoints[6] = atPoint;
        //                    _trackBuildR.pointMode = TrackBuildR.pointModes.transform;

        for (int i = 0; i < numberOfPoints + 1; i++)
        {
            loopPoints[i].extrudeTrack = true;
            loopPoints[i].extrudeTrackBottom = true;
            loopPoints[i].extrudeLength = 0.5f;
            loopPoints[i].RecalculateStoredValues();
        }
    }
}
