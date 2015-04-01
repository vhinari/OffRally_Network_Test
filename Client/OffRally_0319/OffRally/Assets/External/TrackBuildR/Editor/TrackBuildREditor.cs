// Track BuildR
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://support.jasperstocker.com
// For support contact email@jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TrackBuildR))]
public class TrackBuildREditor : Editor
{
    private const float LINE_RESOLUTION = 0.005f;
    private const float HANDLE_SCALE = 0.1f;

    private TrackBuildR _trackBuildR;
    private TrackBuildRTrack _track;
    private float _handleSize;
    [SerializeField]
    private int selectedPoint = 0;//selected track point

    [SerializeField]
    private int selectedCurveIndex = 0;//selected curve

    public static Texture2D[] _stageToolbarTexturesA;
    public static Texture2D[] _stageToolbarTexturesB;
    public static string[] trackModeString = new[] { "track", "boundary", "bumpers", "textures", "terrain", "stunt", "diagram", "options", "export" };
    public static string[] pointModeString = new[] { "transform", "control points", "track up","track point" };
    public static string[] boundaryModeString = new[] { "boundary transform", "boundary control points" };
    public static string[] pitModeString = new[] { "edit pit lane", "set start point", "set end point" };
    public const int numberOfMenuOptionsA = 5;
    public const int numberOfMenuOptionsB = 4;

    void OnEnable()
    {
        if (target != null)
        {
            _trackBuildR = (TrackBuildR)target;
            _track = _trackBuildR.track;
        }

        _stageToolbarTexturesA = new Texture2D[numberOfMenuOptionsA];
        _stageToolbarTexturesA[0] = (Texture2D)Resources.Load("GUI/track");
        _stageToolbarTexturesA[1] = (Texture2D)Resources.Load("GUI/boundary");
        _stageToolbarTexturesA[2] = (Texture2D)Resources.Load("GUI/bumpers");
        _stageToolbarTexturesA[3] = (Texture2D)Resources.Load("GUI/textures");
        _stageToolbarTexturesA[4] = (Texture2D)Resources.Load("GUI/terrain");
        _stageToolbarTexturesB = new Texture2D[numberOfMenuOptionsB];
        _stageToolbarTexturesB[0] = (Texture2D)Resources.Load("GUI/stunt");
        _stageToolbarTexturesB[1] = (Texture2D)Resources.Load("GUI/diagram");
        _stageToolbarTexturesB[2] = (Texture2D)Resources.Load("GUI/options");
        _stageToolbarTexturesB[3] = (Texture2D)Resources.Load("GUI/export");

        //Preview Camera
        if (_trackBuildR.trackEditorPreview != null)
            DestroyImmediate(_trackBuildR.trackEditorPreview);
        if (!EditorApplication.isPlaying && SystemInfo.supportsRenderTextures)
        {
            _trackBuildR.trackEditorPreview = new GameObject("Track Preview Cam");
            _trackBuildR.trackEditorPreview.hideFlags = HideFlags.HideAndDontSave;
            _trackBuildR.trackEditorPreview.AddComponent<Camera>();
            _trackBuildR.trackEditorPreview.camera.fieldOfView = 80;
            //Retreive camera settings from the main camera
            Camera[] cams = Camera.allCameras;
            bool sceneHasCamera = cams.Length > 0;
            Camera sceneCamera = null;
            Skybox sceneCameraSkybox = null;
            if (Camera.main)
            {
                sceneCamera = Camera.main;
            }
            else if (sceneHasCamera)
            {
                sceneCamera = cams[0];
            }

            if (sceneCamera != null)
                if (sceneCameraSkybox == null)
                    sceneCameraSkybox = sceneCamera.GetComponent<Skybox>();
            if (sceneCamera != null)
            {
                _trackBuildR.trackEditorPreview.camera.backgroundColor = sceneCamera.backgroundColor;
                if (sceneCameraSkybox != null)
                    _trackBuildR.trackEditorPreview.AddComponent<Skybox>().material = sceneCameraSkybox.material;
                else
                    if (RenderSettings.skybox != null)
                        _trackBuildR.trackEditorPreview.AddComponent<Skybox>().material = RenderSettings.skybox;
            }
        }
    }

    void OnDisable()
    {
        CleanUp();
    }

    void OnDestroy()
    {
        CleanUp();
    }

    private void CleanUp()
    {
        TrackBuildREditorInspector.CleanUp();
        DestroyImmediate(_trackBuildR.trackEditorPreview);
    }

    /// <summary>
    /// This function renders and controls the layout track function in the menu
    /// Users can click place track points into the scene for a quick and easy creation of a track
    /// </summary>
    private void DrawTrack()
    {
        SceneView.focusedWindow.wantsMouseMove = true;
        _handleSize = HandleUtility.GetHandleSize(_trackBuildR.transform.position) * 0.1f;
        Plane buildingPlane = new Plane(Vector3.up, _trackBuildR.transform.forward);
        float distance;
        Vector3 mousePlanePoint = Vector3.zero;
        Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
        if (buildingPlane.Raycast(mouseRay, out distance))
            mousePlanePoint = mouseRay.GetPoint(distance);

        int numberOfPoints = _track.realNumberOfPoints;
        for (int i = 0; i < numberOfPoints; i++)
        {
            TrackBuildRPoint thisPoint = _track[i];
            Vector3 thisPosition = thisPoint.worldPosition;
            Vector3 lastPosition = (i > 0) ? _track[i - 1].worldPosition : thisPosition;
            Vector3 nextPosition = (i < numberOfPoints - 1) ? _track[i + 1].worldPosition : thisPosition;

            Vector3 backwardTrack = thisPosition - lastPosition;
            Vector3 forwardTrack = nextPosition - thisPosition;
            Vector3 controlDirection = (backwardTrack + forwardTrack).normalized;
            float controlMagnitude = (backwardTrack.magnitude + forwardTrack.magnitude) * 0.333f;

            thisPoint.forwardControlPoint = (controlDirection * controlMagnitude) + thisPosition;
        }

        //draw track outline
        int numberOfCurves = _track.numberOfCurves;
        Vector3 position = _trackBuildR.transform.position;
        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRPoint curve = _track[i];
            int curvePoints = curve.storedPointSize;
            bool lastPoint = i == numberOfCurves - 1;

            Handles.color = (lastPoint && _track.loop) ? TrackBuildRColours.RED : TrackBuildRColours.GREEN;

            for (int p = 1; p < curvePoints; p++)
            {
                int indexA = p - 1;
                int indexB = p;

                Handles.DrawLine(curve.sampledPoints[indexA] + position, curve.sampledPoints[indexB] + position);
                Vector3 trackCrossWidth = curve.sampledTrackCrosses[indexA] * (curve.sampledWidths[indexA] * 0.5f);
                Handles.DrawLine(curve.sampledPoints[indexA] + trackCrossWidth + position, curve.sampledPoints[indexB] + trackCrossWidth + position);
                Handles.DrawLine(curve.sampledPoints[indexA] - trackCrossWidth + position, curve.sampledPoints[indexB] - trackCrossWidth + position);
            }
        }

        Handles.color = Color.green;
        if (Handles.Button(mousePlanePoint, Quaternion.identity, _handleSize, _handleSize, Handles.DotCap))
        {
            TrackBuildRPoint newTrackPoint = _track.gameObject.AddComponent<TrackBuildRPoint>();//CreateInstance<TrackBuildRPoint>();
            newTrackPoint.baseTransform = _trackBuildR.transform;
            newTrackPoint.position = mousePlanePoint;
            _track.AddPoint(newTrackPoint);
        }

        if (_track.realNumberOfPoints > 0)
        {
            TrackBuildRPoint pointOne = _track[0];
            Handles.Label(pointOne.worldPosition, "Loop Track");
            if (Handles.Button(pointOne.worldPosition, Quaternion.identity, _handleSize, _handleSize, Handles.DotCap))
            {
                _track.drawMode = false;
                _trackBuildR.UpdateRender();
            }
        }

        if (GUI.changed)
        {
            UpdateGui();
        }
    }

    void OnSceneGUI()
    {
        if (_track.drawMode)
        {
            DrawTrack();
            return;
        }

        if (SceneView.focusedWindow != null)
            SceneView.focusedWindow.wantsMouseMove = false;
        Vector3 position = _trackBuildR.transform.position;
        Camera sceneCamera = Camera.current;
        _handleSize = HandleUtility.GetHandleSize(_trackBuildR.transform.position) * 0.1f;

        int realNumberOfPoints = _track.realNumberOfPoints;

        Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
        Quaternion mouseLookDirection = Quaternion.LookRotation(-mouseRay.direction);
        int numberOfCurves = _track.numberOfCurves;

        switch (_trackBuildR.mode)
        {
            case TrackBuildR.modes.track:
                Handles.color = TrackBuildRColours.GREEN;
                switch (_trackBuildR.pointMode)
                {
                    case TrackBuildR.pointModes.add:

                        if (SceneView.focusedWindow != null)
                            SceneView.focusedWindow.wantsMouseMove = true;

                        if (Event.current.type == EventType.MouseMove)
                            Repaint();

                        Handles.color = TrackBuildRColours.GREY;
                        for(int i = 0; i < _track.realNumberOfPoints; i++)
                        {
                            Vector3 pointPos = _track[i].worldPosition;
                            float handleSize = HandleUtility.GetHandleSize(pointPos);
                            Handles.DotCap(0, pointPos, Quaternion.identity, handleSize*0.05f);
                        }

                        Handles.color = TrackBuildRColours.GREEN;
                        float mousePercentage = NearestmMousePercentage();// _track.GetNearestPoint(mousePlanePoint);
                        Vector3 mouseTrackPoint = _track.GetTrackPosition(mousePercentage) + position;
                        Handles.Label(mouseTrackPoint, "Add New Track Point");
                        float newPointHandleSize = HandleUtility.GetHandleSize(mouseTrackPoint) * HANDLE_SCALE;
                        if (Handles.Button(mouseTrackPoint, mouseLookDirection, newPointHandleSize, newPointHandleSize, Handles.DotCap))
                        {
                            int newPointIndex = _track.GetLastPointIndex(mousePercentage);
                            TrackBuildRPoint newPoint = _track.InsertPoint(newPointIndex + 1);
                            newPoint.worldPosition = mouseTrackPoint;
                            newPoint.width = _track.GetTrackWidth(mousePercentage);
                            newPoint.crownAngle = _track.GetTrackCrownAngle(mousePercentage);
                            selectedPoint = newPointIndex + 1;
                            GUI.changed = true;
                            _trackBuildR.pointMode = TrackBuildR.pointModes.transform;
                        }
                        break;

                    case TrackBuildR.pointModes.remove:

                        if (SceneView.focusedWindow != null)
                            SceneView.focusedWindow.wantsMouseMove = true;

                        Handles.color = TrackBuildRColours.RED;
                        for (int i = 0; i < realNumberOfPoints; i++)
                        {
                            TrackBuildRPoint point = _track[i];

                            float pointHandleSize = HandleUtility.GetHandleSize(point.worldPosition) * HANDLE_SCALE;
                            Handles.Label(point.worldPosition, "Remove Track Point");
                            if (Handles.Button(point.worldPosition, mouseLookDirection, pointHandleSize, pointHandleSize, Handles.DotCap))
                            {
                                _track.RemovePoint(point);
                                GUI.changed = true;
                                _trackBuildR.pointMode = TrackBuildR.pointModes.transform;
                            }
                        }

                        break;

                    default:

                        SceneGUIPointBased();
                        break;

                }

                //draw track outline
                for (int i = 0; i < numberOfCurves; i++)
                {
                    TrackBuildRPoint curve = _track[i];
                    if(curve==null)
                        continue;
                    int curvePoints = curve.storedPointSize;

                    float dotPA = Vector3.Dot(sceneCamera.transform.forward, curve.worldPosition - sceneCamera.transform.position);
                    float dotPB = Vector3.Dot(sceneCamera.transform.forward, curve.nextPoint.worldPosition - sceneCamera.transform.position);

                    if (dotPA < 0 && dotPB < 0)
                        continue;

                    float curveDistance = Vector3.Distance(sceneCamera.transform.position, curve.center);
                    int pointJump = Mathf.Max((int)(curveDistance / 20.0f), 1);
                    Color trackOutline = curve.render ? TrackBuildRColours.GREEN : TrackBuildRColours.RED;
                    Color trackOutlineA = trackOutline;
                    trackOutlineA.a = 0.5f;
                    for (int p = pointJump; p < curvePoints; p += pointJump)
                    {
                        int indexA = p - pointJump;
                        int indexB = p;

                        if (p + pointJump > curvePoints - 1)
                            indexB = curvePoints - 1;

                        Handles.color = trackOutlineA;
                        Handles.DrawLine(curve.sampledPoints[indexA] + position, curve.sampledPoints[indexB] + position);
                        Handles.color = trackOutline;
                        Vector3 trackCrossWidth = curve.sampledTrackCrosses[indexA] * (curve.sampledWidths[indexA] * 0.5f);
                        Handles.DrawLine(curve.sampledPoints[indexA] + trackCrossWidth + position, curve.sampledPoints[indexB] + trackCrossWidth + position);
                        Handles.DrawLine(curve.sampledPoints[indexA] - trackCrossWidth + position, curve.sampledPoints[indexB] - trackCrossWidth + position);
                    }
                }
                break;

            case TrackBuildR.modes.boundary:

                //draw boundary outline
                for (int i = 0; i < numberOfCurves; i++)
                {
                    TrackBuildRPoint curve = _track[i];
                    int curvePoints = curve.storedPointSize;

                    float dotPA = Vector3.Dot(sceneCamera.transform.forward, curve.worldPosition - sceneCamera.transform.position);
                    float dotPB = Vector3.Dot(sceneCamera.transform.forward, curve.nextPoint.worldPosition - sceneCamera.transform.position);

                    if (dotPA < 0 && dotPB < 0)
                        continue;

                    float curveDistance = Vector3.Distance(sceneCamera.transform.position, curve.center);
                    int pointJump = Mathf.Max((int)(curveDistance / 20.0f), 1);
                    for (int p = pointJump; p < curvePoints; p += pointJump)
                    {
                        int indexA = p - pointJump;
                        int indexB = p;

                        if (p + pointJump > curvePoints - 1)
                            indexB = curvePoints - 1;

                        if (_track.disconnectBoundary)
                        {
                            Handles.color = TrackBuildRColours.BLUE;
                            Handles.DrawLine(curve.sampledLeftBoundaryPoints[indexA] + position, curve.sampledLeftBoundaryPoints[indexB] + position);
                            Handles.color = TrackBuildRColours.RED;
                            Handles.DrawLine(curve.sampledRightBoundaryPoints[indexA] + position, curve.sampledRightBoundaryPoints[indexB] + position);
                        }
                        else
                        {

                            Vector3 trackCrossWidth = curve.sampledTrackCrosses[indexA] * (curve.sampledWidths[indexA] * 0.5f);
                            Handles.color = TrackBuildRColours.BLUE;
                            Handles.DrawLine(curve.sampledPoints[indexA] + trackCrossWidth + position, curve.sampledPoints[indexB] + trackCrossWidth + position);
                            Handles.color = TrackBuildRColours.RED;
                            Handles.DrawLine(curve.sampledPoints[indexA] - trackCrossWidth + position, curve.sampledPoints[indexB] - trackCrossWidth + position);
                        }
                    }
                }
                SceneGUIPointBased();
                break;

            case TrackBuildR.modes.textures:

                for (int i = 0; i < numberOfCurves; i++)
                {
                    TrackBuildRPoint thisCurve = _track[i];

                    float pointHandleSize = HandleUtility.GetHandleSize(thisCurve.center) * HANDLE_SCALE;
                    Handles.color = (i == selectedCurveIndex) ? TrackBuildRColours.RED : TrackBuildRColours.BLUE;
                    if (Handles.Button(thisCurve.center, Quaternion.identity, pointHandleSize, pointHandleSize, Handles.DotCap))
                    {
                        selectedCurveIndex = i;
                        GUIUtility.hotControl = 0;
                        GUIUtility.keyboardControl = 0;
                        GUI.changed = true;
                    }
                }


                Handles.color = TrackBuildRColours.RED;
                TrackBuildRPoint selectedCurve = _track[selectedCurveIndex];
                int numberOfSelectedCurvePoints = selectedCurve.storedPointSize;
                for (int i = 0; i < numberOfSelectedCurvePoints - 1; i++)
                {
                    Vector3 leftPointA = selectedCurve.sampledLeftBoundaryPoints[i];
                    Vector3 leftPointB = selectedCurve.sampledLeftBoundaryPoints[i + 1];
                    Vector3 rightPointA = selectedCurve.sampledRightBoundaryPoints[i];
                    Vector3 rightPointB = selectedCurve.sampledRightBoundaryPoints[i + 1];

                    Handles.DrawLine(leftPointA, leftPointB);
                    Handles.DrawLine(rightPointA, rightPointB);

                    if (i == 0)
                        Handles.DrawLine(leftPointA, rightPointA);
                    if (i == numberOfSelectedCurvePoints - 2)
                        Handles.DrawLine(leftPointB, rightPointB);
                }

                break;

            case TrackBuildR.modes.terrain:

                //nothing

                break;

            case TrackBuildR.modes.stunt:

                SceneGUIPointBased();

                TrackBuildRPoint atPoint = _track[selectedPoint];
                TrackBuildRPoint lastPoint = _track.GetPoint(selectedPoint - 1);
                TrackBuildRPoint nextPoint = _track.GetPoint(selectedPoint + 1);
                float trackWidth;
                Vector3 startCross;
                Vector3 p0, p1, p2, p3, p4, p5, p6, p7;
                switch(_trackBuildR.stuntMode)
                {
                    case TrackBuildR.stuntModes.loop:
                        
                        atPoint = _track[selectedPoint];
                        trackWidth = atPoint.width;
                        
                        float loopRadius = _track.loopRadius;
                        Vector3 loopPosition = atPoint.worldPosition;
                        Vector3 trackDirection = atPoint.trackDirection.normalized;
                        Vector3 trackup = atPoint.trackUpQ * Vector3.forward;
                        Vector3 trackCross = atPoint.trackCross;
                        Vector3 loopCentreHeight = loopRadius * trackup;
                        Quaternion loopAngle = Quaternion.FromToRotation(Vector3.right, trackDirection);
                        for(float i = 0; i < 0.99f; i += 0.01f )
                        {
                            float radA = Mathf.PI * 2 * (i+0.5f);
                            float radB = Mathf.PI * 2 * (i+0.51f);
                            Vector3 pointLoopPositionA = loopAngle * ((new Vector3(Mathf.Sin(radA), Mathf.Cos(radA), 0)) * loopRadius);
                            Vector3 pointLoopPositionB = loopAngle * ((new Vector3(Mathf.Sin(radB), Mathf.Cos(radB), 0)) * loopRadius);
                            Vector3 lateral = Vector3.Lerp((trackCross * trackWidth * -0.6f), (trackCross * trackWidth * 0.6f), i);
                            Vector3 pointPositionA = (pointLoopPositionA) + lateral + loopPosition + loopCentreHeight;
                            Vector3 pointPositionB = (pointLoopPositionB) + lateral + loopPosition + loopCentreHeight;
                            Handles.DrawLine(pointPositionA, pointPositionB);
                        }
                        break;

                    case TrackBuildR.stuntModes.jump:

                        atPoint = _track[selectedPoint];
                        lastPoint = _track.GetPoint(selectedPoint - 1);
                        nextPoint = _track.GetPoint(selectedPoint + 1);

                        float trackPartDistance = lastPoint.arcLength + atPoint.arcLength;
                        float jumpDistance = Mathf.Min(trackPartDistance * 0.333f, _track.maxJumpLength);

                        Vector3 jumpDirection = atPoint.trackDirection;
                        Vector3 jumpMiddle = atPoint.worldPosition;
                        startCross = atPoint.trackCross;
                        trackWidth = atPoint.width*0.5f;
                        Quaternion trackUp = atPoint.trackUpQ;

                        Vector3 jumpHeight = trackUp * (Vector3.forward * _track.jumpHeight);
                        Vector3 jumpStartPosition = jumpMiddle - jumpDirection * (jumpDistance * 0.33f);
                        Vector3 jumpEndPosition = jumpMiddle + jumpDirection * (jumpDistance * 0.33f);

                        p0 = lastPoint.worldPosition + trackWidth * startCross;
                        p1 = lastPoint.worldPosition - trackWidth * startCross;
                        p2 = jumpStartPosition + trackWidth * startCross + jumpHeight;
                        p3 = jumpStartPosition - trackWidth * startCross + jumpHeight;

                        p4 = jumpEndPosition + trackWidth * startCross + jumpHeight;
                        p5 = jumpEndPosition - trackWidth * startCross + jumpHeight;
                        p6 = nextPoint.worldPosition + trackWidth * startCross;
                        p7 = nextPoint.worldPosition - trackWidth * startCross;

                        Handles.DrawLine(p0, p2);
                        Handles.DrawLine(p1, p3);
                        Handles.DrawLine(p0, p1);
                        Handles.DrawLine(p2, p3);
                        Handles.DrawLine(p2, p2 - jumpHeight);
                        Handles.DrawLine(p3, p3 - jumpHeight);
                        Handles.DrawLine(p0, p2 - jumpHeight);
                        Handles.DrawLine(p1, p3 - jumpHeight);

                        Handles.DrawLine(p4, p6);
                        Handles.DrawLine(p5, p7);
                        Handles.DrawLine(p4, p5);
                        Handles.DrawLine(p6, p7);
                        Handles.DrawLine(p4, p4 - jumpHeight);
                        Handles.DrawLine(p5, p5 - jumpHeight);
                        Handles.DrawLine(p6, p4 - jumpHeight);
                        Handles.DrawLine(p7, p5 - jumpHeight);
                        
                        break;

//                        case TrackBuildR.stuntModes.twist:
//                        
//                        atPoint = _track[selectedPoint];
//                        lastPoint = _track.GetPoint(selectedPoint - 1);
//
//                        float twistDistance = Mathf.Min((lastPoint.arcLength + atPoint.arcLength) * 0.333f, _track.maxJumpLength);
//
//                        Vector3 twistDirection = atPoint.trackDirection;
//                        Vector3 twistMiddle = atPoint.worldPosition;
//                        Vector3 twistUp = atPoint.trackUp;
//                        float twistRadius = _track.twistRadius;
//                        Vector3 twistStartPosition = twistMiddle - twistDirection * (twistDistance * 0.33f);
//                        Vector3 twistEndPosition = twistMiddle + twistDirection * (twistDistance * 0.33f);
//                        Vector3 twistCentreHeight = twistUp * twistRadius;
//                        Quaternion twistAngle = Quaternion.LookRotation(twistDirection, twistUp);
//                        for(float i = 0; i < 0.99f; i += 0.01f )
//                        {
//                            float radA = Mathf.PI * 2 * (i+0.5f);
//                            float radB = Mathf.PI * 2 * (i+0.51f);
//                            Vector3 pointLoopPositionA = twistAngle * ((new Vector3(Mathf.Sin(radA), Mathf.Cos(radA), 0)) * twistRadius);
//                            Vector3 pointLoopPositionB = twistAngle * ((new Vector3(Mathf.Sin(radB), Mathf.Cos(radB), 0)) * twistRadius);
//                            float smoothI = i * i * (3.0f - 2.0f * i);
//                            Vector3 lateral = Vector3.Lerp(twistStartPosition, twistEndPosition, i + (i-smoothI));
//                            Vector3 pointPositionA = (pointLoopPositionA) + lateral + twistCentreHeight;
//                            Vector3 pointPositionB = (pointLoopPositionB) + lateral + twistCentreHeight;
//                            Handles.DrawLine(pointPositionA, pointPositionB);
//                        }
//
//                        break;

                    case TrackBuildR.stuntModes.jumptwist:

                        atPoint = _track[selectedPoint];
                        lastPoint = _track.GetPoint(selectedPoint - 1);
                        nextPoint = _track.GetPoint(selectedPoint + 1);

                        float trackTPartDistance = lastPoint.arcLength + atPoint.arcLength;
                        float jumpTDistance = Mathf.Min(trackTPartDistance * 0.333f, _track.maxJumpLength);

                        trackWidth = atPoint.width * 0.5f;
                        startCross = atPoint.trackCross;
                        Vector3 jumpTDirection = atPoint.trackDirection;
                        Vector3 jumpTMiddle = atPoint.worldPosition;
                        Quaternion atPointUpQ = atPoint.trackUpQ;
                        Quaternion trackUpJump = Quaternion.AngleAxis(_track.twistAngle, -jumpTDirection);
                        Vector3 trackCrossExit = trackUpJump * startCross;
                        Vector3 trackCrossEntry = Quaternion.Inverse(trackUpJump) * startCross;
                        Vector3 jumpLateral = startCross * _track.twistAngle / 33.3f;

                        Vector3 jumpTHeight = atPointUpQ * (Vector3.forward * _track.jumpHeight);
                        Vector3 jumpTStartPosition = jumpTMiddle - jumpTDirection * (jumpTDistance * 0.33f) + jumpTHeight - jumpLateral;
                        Vector3 jumpTEndPosition = jumpTMiddle + jumpTDirection * (jumpTDistance * 0.33f) + jumpTHeight + jumpLateral;
                        
                        p0 = lastPoint.worldPosition + trackWidth * startCross;
                        p1 = lastPoint.worldPosition - trackWidth * startCross;
                        p2 = jumpTStartPosition + trackWidth * trackCrossExit;
                        p3 = jumpTStartPosition - trackWidth * trackCrossExit;

                        p4 = jumpTEndPosition + trackWidth * trackCrossEntry;
                        p5 = jumpTEndPosition - trackWidth * trackCrossEntry;
                        p6 = nextPoint.worldPosition + trackWidth * startCross;
                        p7 = nextPoint.worldPosition - trackWidth * startCross;

                        Handles.DrawLine(p0, p2);
                        Handles.DrawLine(p1, p3);
                        Handles.DrawLine(p0, p1);
                        Handles.DrawLine(p2, p3);
//                        Handles.DrawLine(p2, p2 - jumpTHeight);
//                        Handles.DrawLine(p3, p3 - jumpTHeight);
//                        Handles.DrawLine(p0, p2 - jumpTHeight);
//                        Handles.DrawLine(p1, p3 - jumpTHeight);

                        Handles.DrawLine(p4, p6);
                        Handles.DrawLine(p5, p7);
                        Handles.DrawLine(p4, p5);
                        Handles.DrawLine(p6, p7);
//                        Handles.DrawLine(p4, p4 - jumpTHeight);
//                        Handles.DrawLine(p5, p5 - jumpTHeight);
//                        Handles.DrawLine(p6, p4 - jumpTHeight);
//                        Handles.DrawLine(p7, p5 - jumpTHeight);

                        break;
                }

                break;

            case TrackBuildR.modes.diagram:
                if (SceneView.focusedWindow != null)
                    SceneView.focusedWindow.wantsMouseMove = true;
                if (!_track.showDiagram)
                    break;
                Plane diagramPlane = new Plane(Vector3.up, position);
                float diagramDistance;
                float crossSize = _handleSize * 10;

                switch (_trackBuildR.track.assignedPoints)
                {
                    case 0://display the diagram scale points

                        Vector3 diagramPointA = _track.scalePointA;
                        Vector3 diagramPointB = _track.scalePointB;

                        if (diagramPointA != Vector3.zero || diagramPointB != Vector3.zero)
                        {

                            Handles.color = TrackBuildRColours.BLUE;
                            Handles.DrawLine(diagramPointA, diagramPointA + Vector3.left * crossSize);
                            Handles.DrawLine(diagramPointA, diagramPointA + Vector3.right * crossSize);
                            Handles.DrawLine(diagramPointA, diagramPointA + Vector3.forward * crossSize);
                            Handles.DrawLine(diagramPointA, diagramPointA + Vector3.back * crossSize);

                            Handles.color = TrackBuildRColours.GREEN;
                            Handles.DrawLine(diagramPointB, diagramPointB + Vector3.left * crossSize);
                            Handles.DrawLine(diagramPointB, diagramPointB + Vector3.right * crossSize);
                            Handles.DrawLine(diagramPointB, diagramPointB + Vector3.forward * crossSize);
                            Handles.DrawLine(diagramPointB, diagramPointB + Vector3.back * crossSize);

                            Handles.color = TrackBuildRColours.RED;
                            Handles.DrawLine(diagramPointA, diagramPointB);
                        }
                        break;

                    case 1://place the first of two scale points to define the diagram scale size

                        Ray diagramRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
                        if (diagramPlane.Raycast(diagramRay, out diagramDistance))
                        {
                            Vector3 diagramPlanePoint = diagramRay.GetPoint(diagramDistance);

                            Handles.color = TrackBuildRColours.BLUE;
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.left * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.right * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.forward * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.back * crossSize);

                            Handles.color = new Color(0, 0, 0, 0);
                            if (Handles.Button(diagramPlanePoint, Quaternion.identity, crossSize, crossSize, Handles.DotCap))
                            {
                                _track.scalePointA = diagramPlanePoint;
                                _track.assignedPoints = 2;
                            }
                        }

                        break;

                    case 2://place the second of two scale points to define the diagram scale

                        Vector3 diagramPoint1 = _track.scalePointA;
                        Handles.color = TrackBuildRColours.BLUE;
                        Handles.DrawLine(diagramPoint1, diagramPoint1 + Vector3.left * crossSize);
                        Handles.DrawLine(diagramPoint1, diagramPoint1 + Vector3.right * crossSize);
                        Handles.DrawLine(diagramPoint1, diagramPoint1 + Vector3.forward * crossSize);
                        Handles.DrawLine(diagramPoint1, diagramPoint1 + Vector3.back * crossSize);

                        Ray diagramRayB = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
                        if (diagramPlane.Raycast(diagramRayB, out diagramDistance))
                        {
                            Vector3 diagramPlanePoint = diagramRayB.GetPoint(diagramDistance);

                            Handles.color = TrackBuildRColours.RED;
                            Handles.DrawLine(diagramPlanePoint, diagramPoint1);

                            Handles.color = TrackBuildRColours.GREEN;
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.left * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.right * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.forward * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.back * crossSize);

                            Handles.color = new Color(0, 0, 0, 0);
                            if (Handles.Button(diagramPlanePoint, Quaternion.identity, crossSize, crossSize, Handles.DotCap))
                            {
                                _track.scalePointB = diagramPlanePoint;
                                _track.assignedPoints = 0;
                                //wUpdateDiagram();
                            }
                        }
                        break;
                }
                break;

        }


        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    //                    Debug.Log("UndoRedoPerformed");
                                        _trackBuildR.ForceFullRecalculation();
                    GUI.changed = true;
                    return;
            }
        }

        if (GUI.changed)
        {
            UpdateGui();
        }
    }

    private void SceneGUIPointBased()
    {
        Vector3 position = _trackBuildR.transform.position;
        Camera sceneCamera = Camera.current;
        _handleSize = HandleUtility.GetHandleSize(_trackBuildR.transform.position) * 0.1f;
        int realNumberOfPoints = _track.realNumberOfPoints;
        Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
        for (int i = 0; i < realNumberOfPoints; i++)
        {
            TrackBuildRPoint point = _track[i];
            if (Vector3.Dot(sceneCamera.transform.forward, point.worldPosition - sceneCamera.transform.position) < 0)
                continue;

            Handles.Label(point.worldPosition, "point " + (i + 1));
            float pointHandleSize = HandleUtility.GetHandleSize(point.worldPosition) * HANDLE_SCALE;
            Handles.color = (i == selectedPoint) ? TrackBuildRColours.RED : TrackBuildRColours.GREEN;
            if (Handles.Button(point.worldPosition, Quaternion.identity, pointHandleSize, pointHandleSize, Handles.DotCap))
            {
                selectedPoint = i;
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
                GUI.changed = true;
                point.isDirty = true;
            }

            if (i == selectedPoint)
            {
                switch (_trackBuildR.mode)
                {
                    case TrackBuildR.modes.track:

                        switch (_trackBuildR.pointMode)
                        {
                            case TrackBuildR.pointModes.transform:
                                Vector3 currentPosition = point.worldPosition;
                                currentPosition = Handles.DoPositionHandle(currentPosition, Quaternion.identity);
                                if (currentPosition != point.worldPosition)
                                {
                                    Undo.RecordObject(point, "Point Changed");
                                    point.isDirty = true;
                                    point.worldPosition = currentPosition;
                                }

                                //greyed out control points for user ease
                                Handles.color = TrackBuildRColours.DARK_GREY;
                                Handles.DrawLine(point.worldPosition, point.forwardControlPoint + position);
                                Handles.DrawLine(point.worldPosition, point.backwardControlPoint + position);
                                if (Handles.Button(point.backwardControlPoint + position, Quaternion.identity, pointHandleSize, pointHandleSize, Handles.DotCap))
                                    _trackBuildR.pointMode = TrackBuildR.pointModes.controlpoint;
                                if (Handles.Button(point.forwardControlPoint + position, Quaternion.identity, pointHandleSize, pointHandleSize, Handles.DotCap))
                                    _trackBuildR.pointMode = TrackBuildR.pointModes.controlpoint;
                                break;

                            case TrackBuildR.pointModes.controlpoint:
                                //render reverse first so forward renders on top
                                Handles.DrawLine(point.worldPosition, point.backwardControlPoint + position);
                                point.backwardControlPoint = Handles.DoPositionHandle(point.backwardControlPoint + position, Quaternion.identity) - position;
                                if (Vector3.Dot(mouseRay.direction, point.worldPosition - mouseRay.origin) > 0)
                                    Handles.Label(point.backwardControlPoint, "point " + (i + 1) + " reverse control point");

                                if (Vector3.Dot(mouseRay.direction, point.worldPosition - mouseRay.origin) > 0)
                                    Handles.Label(point.forwardControlPoint, "point " + (i + 1) + " control point");
                                Handles.color = TrackBuildRColours.RED;
                                Handles.DrawLine(point.worldPosition, point.forwardControlPoint + position);
                                point.forwardControlPoint = Handles.DoPositionHandle(point.forwardControlPoint + position, Quaternion.identity) - position;

                                break;

                            case TrackBuildR.pointModes.trackup:

                                Undo.RecordObject(point, "Point Changed");
                                point.trackUpQ = Handles.RotationHandle(point.trackUpQ, point.worldPosition);

                                Handles.color = TrackBuildRColours.BLUE;
                                Handles.ArrowCap(0, point.worldPosition, point.trackUpQ, pointHandleSize * 10);
                                Handles.Label(point.worldPosition + point.trackUpQ * Vector3.forward * pointHandleSize * 15, "Up Vector");
                                Handles.color = TrackBuildRColours.GREEN;
                                Handles.ArrowCap(0, point.worldPosition, Quaternion.LookRotation(point.trackDirection), pointHandleSize * 10);
                                Handles.Label(point.worldPosition + point.trackDirection * pointHandleSize * 15, "Direction Vector");
                                
                                Handles.color = TrackBuildRColours.RED;
                                Quaternion quatForward = Quaternion.LookRotation(point.trackUpQ * Vector3.up);
                                Handles.ArrowCap(0, point.worldPosition, quatForward, pointHandleSize * 10);
                                Handles.Label(point.worldPosition + Quaternion.LookRotation(point.trackUpQ * Vector3.up) * Vector3.forward * pointHandleSize * 15, "Up Forward Vector");

                                Handles.color = TrackBuildRColours.PURPLE;
                                Quaternion quatCross = Quaternion.LookRotation(point.trackCross);
                                Handles.ArrowCap(0, point.worldPosition, quatCross, pointHandleSize * 10);
                                Handles.Label(point.worldPosition + point.trackCross * pointHandleSize * 15, "Cross Vector");
                                break;

                            case TrackBuildR.pointModes.trackpoint:

                                //Track Width
                                Handles.color = TrackBuildRColours.RED;
                                float pointWidth = point.width / 2;
                                Vector3 sliderPos = Handles.Slider(point.worldPosition + point.trackCross * pointWidth, point.trackCross);
                                float pointwidth = Vector3.Distance(sliderPos, point.worldPosition) * 2;
                                if (pointwidth != point.width)
                                {
                                    Undo.RecordObject(point, "Point Changed");
                                    point.isDirty = true;
                                    point.width = pointwidth;
                                }

                                //Crown
                                Handles.color = TrackBuildRColours.GREEN;
                                Vector3 crownPosition = point.worldPosition + point.trackUp * point.crownAngle;
                                Vector3 newCrownPosition = Handles.Slider(crownPosition, point.trackUp);
                                Vector3 crownDifference = newCrownPosition - point.worldPosition;
                                if (crownDifference.sqrMagnitude != 0)//Crown Modified
                                {
                                    Undo.RecordObject(point, "Point Changed");
                                    point.isDirty = true;
                                    point.crownAngle = Vector3.Project(crownDifference, point.trackUp).magnitude * Mathf.Sign(Vector3.Dot(crownDifference, point.trackUp));
                                }

                                break;
                        }
                        break;


                    case TrackBuildR.modes.boundary:

                        if (_track.disconnectBoundary)
                        {
                            if (Vector3.Dot(mouseRay.direction, point.worldPosition - mouseRay.origin) > 0)
                            {
                                Handles.color = TrackBuildRColours.RED;
                                Handles.Label(point.leftTrackBoundaryWorld, "point " + (i + 1) + " left track boundary");
                                Handles.Label(point.rightTrackBoundaryWorld, "point " + (i + 1) + " right track boundary");
                                Handles.DrawLine(point.worldPosition, point.leftTrackBoundaryWorld);
                                Handles.DrawLine(point.worldPosition, point.rightTrackBoundaryWorld);
                            }
                            switch (_trackBuildR.boundaryMode)
                            {
                                case TrackBuildR.boundaryModes.transform:
                                    Undo.RecordObject(point, "Point Changed");
                                    point.leftTrackBoundaryWorld = Handles.DoPositionHandle(point.leftTrackBoundaryWorld, Quaternion.identity);
                                    point.rightTrackBoundaryWorld = Handles.DoPositionHandle(point.rightTrackBoundaryWorld, Quaternion.identity);
                                    break;

                                case TrackBuildR.boundaryModes.controlpoint:
                                    Undo.RecordObject(point, "Point Changed");
                                    Handles.color = TrackBuildRColours.RED;
                                    Handles.DrawLine(point.leftTrackBoundaryWorld, point.leftForwardControlPoint + position);
                                    point.leftForwardControlPoint = Handles.DoPositionHandle(point.leftForwardControlPoint + position, Quaternion.identity) - position;

                                    Handles.DrawLine(point.leftTrackBoundaryWorld, point.leftBackwardControlPoint + position);
                                    point.leftBackwardControlPoint = Handles.DoPositionHandle(point.leftBackwardControlPoint + position, Quaternion.identity) - position;

                                    Handles.DrawLine(point.rightTrackBoundaryWorld, point.rightForwardControlPoint + position);
                                    point.rightForwardControlPoint = Handles.DoPositionHandle(point.rightForwardControlPoint + position, Quaternion.identity) - position;

                                    Handles.DrawLine(point.rightTrackBoundaryWorld, point.rightBackwardControlPoint + position);
                                    point.rightBackwardControlPoint = Handles.DoPositionHandle(point.rightBackwardControlPoint + position, Quaternion.identity) - position;
                                    break;

                            }
                        }
                        break;

                    case TrackBuildR.modes.stunt:

                        break;
                }
            }
        }
    }

    /// <summary>
    /// Get the nearest point on the track curve to the  mouse position
    /// We essentailly project the track onto a 2D plane that is the editor camera and then find a point on that
    /// </summary>
    /// <returns>A percentage of the nearest point on the track curve to the nerest metre</returns>
    private float NearestmMousePercentage()
    {
        Camera cam = Camera.current;
        float screenHeight = cam.pixelHeight;
        Vector2 mousePos = Event.current.mousePosition;
        mousePos.y = screenHeight - mousePos.y;
        int numberOfSearchPoints = 600;
        Vector3 position = _trackBuildR.transform.position;

        Vector2 zeropoint = cam.WorldToScreenPoint(_track.GetTrackPosition(0) + position);
        float nearestPointSqrMag = Vector2.SqrMagnitude(zeropoint - mousePos);
        float nearestT = 0;
        float nearestPointSqrMagB = Vector2.SqrMagnitude(zeropoint - mousePos);
        float nearestTb = 0;

        for (int i = 1; i < numberOfSearchPoints; i++)
        {
            float t = i / (float)numberOfSearchPoints;
            Vector2 point = cam.WorldToScreenPoint(_track.GetTrackPosition(t) + position);
            float thisPointMag = Vector2.SqrMagnitude(point - mousePos);
            if (thisPointMag < nearestPointSqrMag)
            {
                nearestPointSqrMagB = nearestPointSqrMag;
                nearestTb = nearestT;

                nearestT = t;
                nearestPointSqrMag = thisPointMag;
            }
            else
            {
                if (thisPointMag < nearestPointSqrMagB)
                {
                    nearestTb = t;
                    nearestPointSqrMagB = thisPointMag;
                }
            }
        }
        float pointADist = Mathf.Sqrt(nearestPointSqrMag);
        float pointBDist = Mathf.Sqrt(nearestPointSqrMagB);
        float lerpvalue = pointADist / (pointADist + pointBDist);
        return Mathf.Lerp(nearestT, nearestTb, lerpvalue);
    }

    public override void OnInspectorGUI()
    {
        TrackBuildREditorInspector.OnInspectorGUI(_trackBuildR, selectedPoint, selectedCurveIndex);

        if (GUI.changed)
        {
            UpdateGui();
        }
    }

    /// <summary>
    /// Handle GUI changes and repaint requests
    /// </summary>
    private void UpdateGui()
    {
        Repaint();
        HandleUtility.Repaint();
        SceneView.RepaintAll();
        _trackBuildR.UpdateRender();
        EditorUtility.SetDirty(_trackBuildR);
        EditorUtility.SetDirty(_track);
    }
}
