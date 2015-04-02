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
using System.Collections.Generic;

public class TrackBuildREditorInspector
{
    [SerializeField]
    private static int selectedTexture;

    [SerializeField]
    private static RenderTexture pointPreviewTexture = null;
    private static float aspect = 1.7777f;
    private static Vector3 previewCameraHeight = new Vector3(0, 1.8f, 0);
    private static int previewResolution = 800;
    private static int pointTrackSelection = 0;
    private static string[] pointTrackNames = new []{"Selected Point", "Track Wide"};

    public static void OnInspectorGUI(TrackBuildR _trackBuildR, int selectedPointIndex, int selectedCurveIndex)
    {
        TrackBuildRTrack _track = _trackBuildR.track;

        if (TrackBuildREditor._stageToolbarTexturesA == null)
            return;

        GUILayout.BeginVertical(GUILayout.Width(400));

        //Track Preview Window
        EditorGUILayout.Space();
        RenderPreview(_trackBuildR);
        EditorGUILayout.LabelField("Track Lap Length approx. " + (_track.trackLength / 1000).ToString("F2") + "km / " + (_track.trackLength / 1609.34f).ToString("F2") + " miles");

        EditorGUILayout.LabelField("Track Polycount: "+_track.lastPolycount);

        int currentTrackMode = (int)_trackBuildR.mode;
        int currentTrackModeA = currentTrackMode < 5 ? currentTrackMode : -1;
        int currentTrackModeB = currentTrackMode > 4 ? currentTrackMode - 5 : -1;
        GUIContent[] guiContentA = new GUIContent[TrackBuildREditor.numberOfMenuOptionsA];
        GUIContent[] guiContentB = new GUIContent[TrackBuildREditor.numberOfMenuOptionsB];
        for (int i = 0; i < TrackBuildREditor.numberOfMenuOptionsA; i++)
            guiContentA[i] = new GUIContent(TrackBuildREditor._stageToolbarTexturesA[i], TrackBuildREditor.trackModeString[i]);
        for (int i = 0; i < TrackBuildREditor.numberOfMenuOptionsB; i++)
            guiContentB[i] = new GUIContent(TrackBuildREditor._stageToolbarTexturesB[i], TrackBuildREditor.trackModeString[i]);
        int newTrackModeA = GUILayout.Toolbar(currentTrackModeA, guiContentA, GUILayout.Width(400), GUILayout.Height(50));
        int newTrackModeB = GUILayout.Toolbar(currentTrackModeB, guiContentB, GUILayout.Width(400), GUILayout.Height(50));
        if (newTrackModeA != currentTrackModeA)
        {
            _trackBuildR.mode = (TrackBuildR.modes)newTrackModeA;
            GUI.changed = true;
        }
        if (newTrackModeB != currentTrackModeB)
        {
            _trackBuildR.mode = (TrackBuildR.modes)newTrackModeB + 5;
            GUI.changed = true;
        }

        if (_track.numberOfTextures == 0)
            EditorGUILayout.HelpBox("There are no textures defined. Track will not render until this is done", MessageType.Error);

        TrackBuildRPoint point = null;
        if (_track.realNumberOfPoints > 0)
        {
            point = _trackBuildR.track[selectedPointIndex];
        }

        if(!_track.render)
            EditorGUILayout.HelpBox("Track rendering is disabled", MessageType.Warning);

        switch(_trackBuildR.mode)
        {
            case TrackBuildR.modes.track:

                EditorGUILayout.Space();
                Title("Track", TrackBuildRColours.GREEN);

                bool trackloop = EditorGUILayout.Toggle("Is Looped", _track.loop);
                if(_track.loop != trackloop)
                {
                    Undo.RecordObject(_track, "Toggled Loop");
                    _track.loop = trackloop;
                }

                EditorGUILayout.BeginHorizontal();
                if(_trackBuildR.pointMode != TrackBuildR.pointModes.add)
                {
                    if(GUILayout.Button("Add New Point"))
                        _trackBuildR.pointMode = TrackBuildR.pointModes.add;
                }
                else
                {
                    if(GUILayout.Button("Cancel Add New Point"))
                        _trackBuildR.pointMode = TrackBuildR.pointModes.transform;
                }

                EditorGUI.BeginDisabledGroup(_track.realNumberOfPoints < 3);
                if(_trackBuildR.pointMode != TrackBuildR.pointModes.remove)
                {
                    if(GUILayout.Button("Remove Point"))
                        _trackBuildR.pointMode = TrackBuildR.pointModes.remove;
                }
                else
                {
                    if(GUILayout.Button("Cancel Remove Point"))
                        _trackBuildR.pointMode = TrackBuildR.pointModes.transform;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                if(!_track.drawMode)
                {
                    EditorGUILayout.BeginHorizontal();
                    if(GUILayout.Button("Layout Track Points"))
                    {
                        if(EditorUtility.DisplayDialog("Discard Current Track?", "Do you wish to discard the current track layout?", "Yup", "Nope"))
                        {
                            _track.Clear();
                            _track.drawMode = true;
                        }
                    }
                    if(GUILayout.Button("?", GUILayout.Width(35)))
                    {
                        EditorUtility.DisplayDialog("Layout Track", "This allows you to click place points to define your track. It will erase the current track layout and start anew. Ideally used with a defined diagram to help you plot out the track", "Ok - got it!");

                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    if(GUILayout.Button("Stop Layout Track"))
                    {
                        _track.drawMode = false;
                    }
                    EditorGUILayout.EndVertical();
                    return;
                }

                float meshResolution = _track.meshResolution;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Track Mesh Resolution", GUILayout.Width(140));
                meshResolution = EditorGUILayout.Slider(meshResolution, 0.9f, 20.0f);
                EditorGUILayout.LabelField("metres", GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
                if(meshResolution != _track.meshResolution)
                {
                    _track.SetTrackDirty();
                    _track.meshResolution = meshResolution;
                }

                if(_track.realNumberOfPoints == 0)
                {
                    EditorGUILayout.HelpBox("There are no track points defined, add nextNormIndex track point to begin", MessageType.Warning);
                    EditorGUILayout.EndVertical();
                    return;
                }


                EditorGUILayout.Space();
                Title("Track Point", TrackBuildRColours.RED);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Point " + (selectedPointIndex + 1) + " selected");
                if(GUILayout.Button("Goto Point"))
                    GotoScenePoint(point.position);
                EditorGUILayout.EndHorizontal();


                int currentMode = (int)_trackBuildR.pointMode;
                int newStage = GUILayout.Toolbar(currentMode, TrackBuildREditor.pointModeString);
                if(newStage != currentMode)
                {
                    _trackBuildR.pointMode = (TrackBuildR.pointModes)newStage;
                    GUI.changed = true;
                }

                switch(_trackBuildR.pointMode)
                {
                    case TrackBuildR.pointModes.transform:
                        Vector3 pointposition = EditorGUILayout.Vector3Field("Point Position", point.position);
                        if(pointposition != point.position)
                        {
                            Undo.RecordObject(point, "Position Modified");
                            point.position = pointposition;
                        }
                        break;

                    case TrackBuildR.pointModes.controlpoint:
                        bool pointsplitControlPoints = EditorGUILayout.Toggle("Split Control Points", point.splitControlPoints);
                        if(pointsplitControlPoints != point.splitControlPoints)
                        {
                            Undo.RecordObject(point, "Split Points Toggled");
                            point.splitControlPoints = pointsplitControlPoints;
                        }
                        Vector3 pointforwardControlPoint = EditorGUILayout.Vector3Field("Control Point Position", point.forwardControlPoint);
                        if(pointforwardControlPoint != point.forwardControlPoint)
                        {
                            Undo.RecordObject(point, "Forward Control Point Changed");
                            point.forwardControlPoint = pointforwardControlPoint;
                        }
                        break;

                    case TrackBuildR.pointModes.trackup:

                        //nothing to show - yet...

                        break;

                    case TrackBuildR.pointModes.trackpoint:


                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Point Crown");
                        float pointcrownAngle = EditorGUILayout.Slider(point.crownAngle, -45, 45);
                        if(pointcrownAngle != point.crownAngle)
                        {
                            point.isDirty = true;
                            Undo.RecordObject(point, "Crown Modified");
                            point.crownAngle = pointcrownAngle;
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Point Width", GUILayout.Width(250));
                        float pointwidth = EditorGUILayout.FloatField(point.width);
                        if(pointwidth != point.width)
                        {
                            point.isDirty = true;
                            Undo.RecordObject(point, "Width Modified");
                            point.width = pointwidth;
                        }
                        EditorGUILayout.LabelField("metres", GUILayout.Width(75));
                        EditorGUILayout.EndHorizontal();
                        break;
                }
                break;

            case TrackBuildR.modes.boundary:

                EditorGUILayout.Space();
                Title("Track Boundary", TrackBuildRColours.GREEN);
                //Track Based Boundary Options
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Split Boundary from Track");
                bool trackdisconnectBoundary = EditorGUILayout.Toggle(_track.disconnectBoundary, GUILayout.Width(25));
                if(trackdisconnectBoundary != _track.disconnectBoundary)
                {
                    Undo.RecordObject(_track, "disconnect boundary");
                    _track.disconnectBoundary = trackdisconnectBoundary;
                    GUI.changed = true;
                    _track.ReRenderTrack();
                }

                if(GUILayout.Button("Reset Boundary Points"))
                {
                    Undo.RecordObject(_track, "reset boundary");
                    for(int i = 0; i < _track.numberOfPoints; i++)
                    {
                        _track[i].MatchBoundaryValues();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render Both Sides of Boundary");
                bool renderBothSides = EditorGUILayout.Toggle(_track.renderBoundaryWallReverse, GUILayout.Width(50));
                if(_track.renderBoundaryWallReverse != renderBothSides)
                {
                    Undo.RecordObject(_track, "render reverse boundary");
                    _track.renderBoundaryWallReverse = renderBothSides;
                    GUI.changed = true;
                    _track.ReRenderTrack();
                }
                EditorGUILayout.EndHorizontal();

                float newTrackColliderHeight = EditorGUILayout.FloatField("Track Collider Height", _track.trackColliderWallHeight);
                if(newTrackColliderHeight != _track.trackColliderWallHeight)
                {
                    Undo.RecordObject(_track, "trackCollider height");
                    _track.trackColliderWallHeight = newTrackColliderHeight;
                    _track.ReRenderTrack();
                    GUI.changed = true;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Track Collider Should Have Roof");
                bool newRoofCooliderValue = EditorGUILayout.Toggle(_track.includeColliderRoof, GUILayout.Width(50));
                if(newRoofCooliderValue != _track.includeColliderRoof)
                {
                    Undo.RecordObject(_track, "trackCollider roof");
                    _track.includeColliderRoof = newRoofCooliderValue;
                    _track.ReRenderTrack();
                    GUI.changed = true;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                Title("Point Boundary", TrackBuildRColours.RED);
                //Selected Point Boundary Options
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Point " + (selectedPointIndex + 1) + " selected");
                if(GUILayout.Button("Goto Point"))
                    GotoScenePoint(point.position);
                EditorGUILayout.EndHorizontal();

                int currentBoundaryMode = (int)_trackBuildR.boundaryMode;
                int newBoundaryMode = GUILayout.Toolbar(currentBoundaryMode, TrackBuildREditor.boundaryModeString);
                if(newBoundaryMode != currentBoundaryMode)
                {
                    _trackBuildR.boundaryMode = (TrackBuildR.boundaryModes)newBoundaryMode;
                    GUI.changed = true;
                }

                if(_track.realNumberOfPoints > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Split Boundary Control Points");
                    bool pointSplitControlPoints = EditorGUILayout.Toggle(point.leftSplitControlPoints, GUILayout.Width(50));
                    if(point.leftSplitControlPoints != pointSplitControlPoints)
                    {
                        Undo.RecordObject(point, "split boundary");
                        point.leftSplitControlPoints = pointSplitControlPoints;
                        point.rightSplitControlPoints = pointSplitControlPoints;
                        GUI.changed = true;
                        _track.SetTrackDirty();
                    }
                    EditorGUILayout.EndHorizontal();

                }

                EditorGUILayout.HelpBox("It is suggested that you disable the trackCollider bounding box when working on Track BuildR\nYou can do this by clicking on 'gizmos' above the scene view and deselecting 'Mesh Collider'", MessageType.Info);
                break;

            case TrackBuildR.modes.bumpers:

                EditorGUILayout.Space();
                Title("Track Bumpers", TrackBuildRColours.RED);
                bool _tracktrackBumpers = EditorGUILayout.Toggle("Enable", _track.trackBumpers);
                if(_track.trackBumpers != _tracktrackBumpers)
                {
                    Undo.RecordObject(_track, "bumpers");
                    if(_tracktrackBumpers == false)
                        for(int i = 0; i < _track.numberOfCurves; i++)
                            _track[i].generateBumpers = false;
                    _track.trackBumpers = _tracktrackBumpers;
                }
                EditorGUI.BeginDisabledGroup(!_track.trackBumpers);
                float bumperWidth = EditorGUILayout.Slider("Width", _track.bumperWidth, 0.1f, 2.0f);
                if(bumperWidth != _track.bumperWidth)
                {
                    Undo.RecordObject(_track, "bumper width");
                    GUI.changed = true;
                    _track.bumperWidth = bumperWidth;
                }
                float bumperHeight = EditorGUILayout.Slider("Height", _track.bumperHeight, 0.01f, 0.2f);
                if(bumperHeight != _track.bumperHeight)
                {
                    Undo.RecordObject(_track, "bumper height");
                    GUI.changed = true;
                    _track.bumperHeight = bumperHeight;
                }
                float bumperAngleThresold = EditorGUILayout.Slider("Threshold Angle", _track.bumperAngleThresold, 0.005f, 1.5f);
                if(bumperAngleThresold != _track.bumperAngleThresold)
                {
                    Undo.RecordObject(_track, "bumper threshold");
                    GUI.changed = true;
                    _track.bumperAngleThresold = bumperAngleThresold;
                }
                if(GUI.changed)//change on mouse up
                {
                    _track.ReRenderTrack();
                }
                EditorGUI.EndDisabledGroup();
                break;

            case TrackBuildR.modes.textures:

                EditorGUILayout.Space();
                Title("Render Properties", TrackBuildRColours.BLUE);

                pointTrackSelection = GUILayout.Toolbar(pointTrackSelection, pointTrackNames);

                TrackBuildRPoint[] selectedCurves;
                EditorGUILayout.BeginHorizontal();
                switch(pointTrackSelection)
                {
                    default:
                        selectedCurves = new TrackBuildRPoint[]{_track[selectedCurveIndex]};
                        Undo.RecordObjects(selectedCurves, "Curve Details Modified");

                        EditorGUILayout.LabelField("Selected Curve: " + selectedCurves[0].pointName);
                        if(GUILayout.Button("Goto Curve"))
                            GotoScenePoint(selectedCurves[0].center);
                        break;

                    case 1:
                        selectedCurves = _track.points;
                        Undo.RecordObjects(selectedCurves, "Curve Details Modified");
                        break;
                }
                EditorGUILayout.EndHorizontal();

                TrackBuildRPoint selectedCurve = selectedCurves[0];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render Curve");
                bool render = EditorGUILayout.Toggle(selectedCurve.render);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render Boundaries");
                bool renderBounds = EditorGUILayout.Toggle(selectedCurve.renderBounds);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Boundary Height");
                float boundaryHeight = EditorGUILayout.Slider(selectedCurve.boundaryHeight, 0, 10);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Colliders");
                bool trackCollider = EditorGUILayout.Toggle(selectedCurve.trackCollider);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Side Colliders");
                bool colliderSides = EditorGUILayout.Toggle(selectedCurve.colliderSides);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Extrude Track");
                bool extrudeTrack = EditorGUILayout.Toggle(selectedCurve.extrudeTrack);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render Track Bottom");
                bool extrudeTrackBottom = EditorGUILayout.Toggle(selectedCurve.extrudeTrackBottom);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render Curve End");
                bool extrudeCurveEnd = EditorGUILayout.Toggle(selectedCurve.extrudeCurveEnd);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Extrude Length");
                float extrudeLength = EditorGUILayout.Slider(selectedCurve.extrudeLength, 0.1f, 25.0f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Extrude Bevel");
                float extrudeBevel = EditorGUILayout.Slider(selectedCurve.extrudeBevel, 0, 2);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Generate Bumpers");
                bool generateBumpers = EditorGUILayout.Toggle(selectedCurve.generateBumpers);
                EditorGUILayout.EndHorizontal();
                
                if (selectedCurve.render != render)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.render = render;
                if (selectedCurve.renderBounds != renderBounds)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.renderBounds = renderBounds;
                if (selectedCurve.boundaryHeight != boundaryHeight)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.boundaryHeight = boundaryHeight;
                if (selectedCurve.trackCollider != trackCollider)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.trackCollider = trackCollider;
                if (selectedCurve.extrudeTrack != extrudeTrack)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.extrudeTrack = extrudeTrack;
                if (selectedCurve.extrudeTrackBottom != extrudeTrackBottom)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.extrudeTrackBottom = extrudeTrackBottom;
                if (selectedCurve.extrudeCurveEnd != extrudeCurveEnd)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.extrudeCurveEnd = extrudeCurveEnd;
                if (selectedCurve.extrudeLength != extrudeLength)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.extrudeLength = extrudeLength;
                if (selectedCurve.extrudeBevel != extrudeBevel)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.extrudeBevel = extrudeBevel;
                if (selectedCurve.generateBumpers != generateBumpers)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.generateBumpers = generateBumpers;
                if (selectedCurve.colliderSides != colliderSides)
                    foreach (TrackBuildRPoint selectedCurveArray in selectedCurves)
                        selectedCurveArray.colliderSides = colliderSides;

                TrackBuildRTexture[] textures = _track.GetTexturesArray();
                int numberOfTextures = textures.Length;
                string[] textureNames = new string[numberOfTextures];
                for(int t = 0; t < numberOfTextures; t++)
                    textureNames[t] = textures[t].customName;
                if(numberOfTextures > 0)
                {
                    int trackTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.trackTextureStyleIndex, "Track Texture");
                    if(trackTextureStyleIndex != selectedCurve.trackTextureStyleIndex)
                    {
                        selectedCurve.trackTextureStyleIndex = trackTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                    int offroadTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.offroadTextureStyleIndex, "Offroad Texture");
                    if(offroadTextureStyleIndex != selectedCurve.offroadTextureStyleIndex)
                    {
                        selectedCurve.offroadTextureStyleIndex = offroadTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                    int boundaryTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.boundaryTextureStyleIndex, "Boundary Texture");
                    if(boundaryTextureStyleIndex != selectedCurve.boundaryTextureStyleIndex)
                    {
                        selectedCurve.boundaryTextureStyleIndex = boundaryTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                    int bumperTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.bumperTextureStyleIndex, "Bumper Texture");
                    if(bumperTextureStyleIndex != selectedCurve.bumperTextureStyleIndex)
                    {
                        selectedCurve.bumperTextureStyleIndex = bumperTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                    int extrudeTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.bottomTextureStyleIndex, "Track Extrude Texture");
                    if(extrudeTextureStyleIndex != selectedCurve.bottomTextureStyleIndex)
                    {
                        selectedCurve.bottomTextureStyleIndex = extrudeTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                }

                EditorGUILayout.Space();
                Title("Track Texture Library", TrackBuildRColours.RED);

                if(GUILayout.Button("Add New"))
                {
                    _track.AddTexture();
                    numberOfTextures++;
                    selectedTexture = numberOfTextures - 1;
                }
                if(numberOfTextures == 0)
                {
                    EditorGUILayout.HelpBox("There are no textures to show", MessageType.Info);
                    return;
                }

                if(numberOfTextures > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Texture", GUILayout.Width(75));

                    selectedTexture = EditorGUILayout.Popup(selectedTexture, textureNames);

                    TrackBuildRTexture trackBuildRTexture = _track.Texture(selectedTexture);

                    if(GUILayout.Button("Remove Texture"))
                    {
                        _track.RemoveTexture(trackBuildRTexture);
                        numberOfTextures--;
                        selectedTexture = 0;
                        trackBuildRTexture = _track.Texture(selectedTexture);
                        return;
                    }
                    EditorGUILayout.EndHorizontal();

                    if(TextureGUI(ref trackBuildRTexture))
                    {
                        _track.ReRenderTrack();
                    }
                }
                break;


            case TrackBuildR.modes.terrain:

                Title("Terrain", TrackBuildRColours.RED);

                EditorGUILayout.HelpBox("I'd love to hear feedback on this new feature.\nWhat works? What doesn't.\nLet me know!", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Terrain Mode", GUILayout.Width(90));
                EditorGUI.BeginDisabledGroup(_trackBuildR.terrainMode == TrackBuildR.terrainModes.mergeTerrain);
                if(GUILayout.Button("Merge Terrain"))
                    _trackBuildR.terrainMode = TrackBuildR.terrainModes.mergeTerrain;
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(_trackBuildR.terrainMode == TrackBuildR.terrainModes.conformTrack);
                if(GUILayout.Button("Conform Track"))
                    _trackBuildR.terrainMode = TrackBuildR.terrainModes.conformTrack;
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                switch(_trackBuildR.terrainMode)
                {
                    case TrackBuildR.terrainModes.mergeTerrain:

                        EditorGUILayout.BeginVertical("box");
                        Title("Terrain Merge", TrackBuildRColours.RED);
                        EditorGUILayout.LabelField("Selected Terrain");
                        EditorGUILayout.BeginHorizontal();
                        _track.mergeTerrain = (Terrain)EditorGUILayout.ObjectField(_track.mergeTerrain, typeof(Terrain), true);
                        if(GUILayout.Button("Find Terrain"))
                            _track.mergeTerrain = GameObject.FindObjectOfType<Terrain>();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Terrain Merge Width");
                        _track.terrainMergeWidth = EditorGUILayout.Slider(_track.terrainMergeWidth, 0, 100);
                        EditorGUILayout.EndVertical();
                
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Terrain Match Accuracy");
                        _track.terrainAccuracy = EditorGUILayout.Slider(_track.terrainAccuracy, 0, 5);
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Terrain Match Margin");
                        _track.terrainMergeMargin = EditorGUILayout.Slider(_track.terrainMergeMargin, 1, 5);
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Terrain Merge Curve");
                        _track.mergeCurve = EditorGUILayout.CurveField(_track.mergeCurve, GUILayout.Height(75));
                        EditorGUILayout.EndVertical();

                        if (_track.mergeTerrain == null)
                            EditorGUILayout.HelpBox("You have not selected a terrain in the scene", MessageType.Error);

                        if (_track.disconnectBoundary)
                            EditorGUILayout.HelpBox("Terrain Merge doesn't fully support tracks that have boundaries split", MessageType.Error);

                        EditorGUI.BeginDisabledGroup(_track.mergeTerrain == null);
                        if(GUILayout.Button("Merge Terrain", GUILayout.Height(50)))
                        {
                            if(_track.disconnectBoundary)
                            {
                                if (EditorUtility.DisplayDialog("Terrain Merge Warning", "Terrain Merge doesn't fully support tracks that have boundaries split", "Continue", "Cancel"))
                                {
                                    Undo.RecordObject(_track.mergeTerrain.terrainData, "Merge Terrain");
                                    TrackBuildRTerrain.MergeTerrain(_track, _track.mergeTerrain);
                                }
                            }
                            else
                            {
                                Undo.RecordObject(_track.mergeTerrain.terrainData, "Merge Terrain");
                                TrackBuildRTerrain.MergeTerrain(_track, _track.mergeTerrain);
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndVertical();
                        break;

                    case TrackBuildR.terrainModes.conformTrack:

                        EditorGUILayout.BeginVertical("box");
                        Title("Conform Track to Terrain", TrackBuildRColours.RED);
                        EditorGUILayout.LabelField("Selected Terrain");
                        EditorGUILayout.BeginHorizontal();
                        _track.mergeTerrain = (Terrain)EditorGUILayout.ObjectField(_track.mergeTerrain, typeof(Terrain), true);
                        if(GUILayout.Button("Find Terrain"))
                            _track.mergeTerrain = GameObject.FindObjectOfType<Terrain>();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Track Conform Accuracy");
                        _track.conformAccuracy = EditorGUILayout.Slider(_track.conformAccuracy, 0, 25);
                        EditorGUILayout.EndVertical();

                        if (_track.mergeTerrain == null)
                            EditorGUILayout.HelpBox("You have not selected a terrain in the scene", MessageType.Error);

                        EditorGUI.BeginDisabledGroup(_track.mergeTerrain == null);
                        if(GUILayout.Button("Conform Track", GUILayout.Height(50)))
                        {
                            Undo.RecordObject(_track, "Conform Track");
                            TrackBuildRTerrain.ConformTrack(_track, _track.mergeTerrain);
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndVertical();

                        break;
                }

                break;

            case TrackBuildR.modes.stunt:
                
                EditorGUILayout.Space();
                Title("Stunt", TrackBuildRColours.RED);

                if(_track.realNumberOfPoints == 0)
                {
                    EditorGUILayout.HelpBox("No Track to Apply Parts to",MessageType.Error);
                    return;
                }

                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField("Stunt Part Type");
                _trackBuildR.stuntMode = (TrackBuildR.stuntModes)EditorGUILayout.EnumPopup(_trackBuildR.stuntMode, GUILayout.Width(160), GUILayout.Height(30));
                EditorGUILayout.EndHorizontal();

                switch(_trackBuildR.stuntMode)
                {
                    case TrackBuildR.stuntModes.loop:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Loop Radius");
                        _track.loopRadius = EditorGUILayout.Slider(_track.loopRadius, 10, 100);
                        EditorGUILayout.EndHorizontal();
                        if(GUILayout.Button("Add Loop da Loop"))
                        {
                            Undo.RecordObject(_track, "Add Loop");
                            TrackBuildRStuntUtil.AddLoop(_track, selectedPointIndex);
                            GUI.changed = true;
                        }
                        break;

                    case TrackBuildR.stuntModes.jump:


                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Jump Height Radius");
                        _track.jumpHeight = EditorGUILayout.Slider(_track.jumpHeight, 0, 50);
                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Maximum Jump Length");
                        _track.maxJumpLength = EditorGUILayout.Slider(_track.maxJumpLength, 1, 100);
                        EditorGUILayout.EndHorizontal();

                        if (GUILayout.Button("Add Jump to Point"))
                        {
                            //                    Undo.RecordObject(_track, "Add Jump");
                            TrackBuildRStuntUtil.AddJump(_track, selectedPointIndex);
                            GUI.changed = true;
                        }

                        break;
                    case TrackBuildR.stuntModes.jumptwist:

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Jump Height Radius");
                        _track.jumpHeight = EditorGUILayout.Slider(_track.jumpHeight, 0, 50);
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Maximum Jump Length");
                        _track.maxJumpLength = EditorGUILayout.Slider(_track.maxJumpLength, 1, 100);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Jump Twist Angle");
                        _track.twistAngle = EditorGUILayout.Slider(_track.twistAngle, -90, 90);
                        EditorGUILayout.EndHorizontal();

                        if (GUILayout.Button("Add Jump Twist to Point"))
                        {
                            //                    Undo.RecordObject(_track, "Add Jump");
                            TrackBuildRStuntUtil.AddJumpTwist(_track, selectedPointIndex);
                            GUI.changed = true;
                        }

                        break;

//                        case TrackBuildR.stuntModes.twist:
//
//                         EditorGUILayout.BeginHorizontal();
//                        EditorGUILayout.LabelField("Twist Radius");
//                        _track.twistRadius = EditorGUILayout.Slider(_track.twistRadius, 0, 50);
//                        EditorGUILayout.EndHorizontal();
//
//                        
//                        EditorGUILayout.BeginHorizontal();
//                        EditorGUILayout.LabelField("Maximum Twist Length");
//                        _track.maxJumpLength = EditorGUILayout.Slider(_track.maxJumpLength, 1, 100);
//                        EditorGUILayout.EndHorizontal();
//
//                        if(GUILayout.Button("Add Twist to Point"))
//                        {
//                            //                    Undo.RecordObject(_track, "Add Jump");
//                            TrackBuildRStuntUtil.AddTwist(_track, selectedPointIndex);
//                            GUI.changed = true;
//                        }
//
//                        break;
                }




                break;

            case TrackBuildR.modes.diagram:

                _track.CheckDiagram();

                EditorGUILayout.Space();
                Title("Diagram Image", TrackBuildRColours.RED);

                _track.showDiagram = EditorGUILayout.Toggle("Show Diagram", _track.showDiagram);
                _track.diagramGO.renderer.enabled = _track.showDiagram;

                EditorGUILayout.BeginHorizontal();
                if(_track.diagramMaterial.mainTexture != null)
                {
                    float height = _track.diagramMaterial.mainTexture.height * (200.0f / _track.diagramMaterial.mainTexture.width);
                    GUILayout.Label(_track.diagramMaterial.mainTexture, GUILayout.Width(200), GUILayout.Height(height));
                }
                EditorGUILayout.BeginVertical();
                if(GUILayout.Button("Load Diagram"))
                {
                    string newDiagramFilepath = EditorUtility.OpenFilePanel("Load Track Diagram", "/", "");
                    if(newDiagramFilepath != _track.diagramFilepath)
                    {
                        _track.diagramFilepath = newDiagramFilepath;
                        WWW www = new WWW("file:///" + newDiagramFilepath);
                        Texture2D newTexture = new Texture2D(100, 100);
                        www.LoadImageIntoTexture(newTexture);
                        _track.diagramMaterial.mainTexture = newTexture;

                        _track.diagramGO.transform.localScale = new Vector3(newTexture.width, 0, newTexture.height);
                        _track.showDiagram = true;
                    }
                }
                if(GUILayout.Button("Clear"))
                {
                    _track.diagramFilepath = "";
                    _track.diagramMaterial.mainTexture = null;
                    _track.showDiagram = false;
                }


                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Diagram Scale", GUILayout.Width(100));
                float newScale = EditorGUILayout.FloatField(_track.scale, GUILayout.Width(40));
                if(_track.scale != newScale)
                {
                    _track.scale = newScale;
                    UpdateDiagram(_track);
                }
                EditorGUILayout.LabelField("metres", GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if(_track.assignedPoints == 0)
                {
                    if(GUILayout.Button("Draw Scale"))
                    {
                        _track.assignedPoints = 1;
                    }
                    if(GUILayout.Button("?", GUILayout.Width(25)))
                    {
                        EditorUtility.DisplayDialog("Draw Scale", "Once you load a diagram, use this to define the start and end of the diagram scale (I do hope your diagram has a scale...)", "ok");
                    }
                }
                else
                {
                    if(GUILayout.Button("Cancel Draw Scale"))
                    {
                        _track.assignedPoints = 0;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField(_track.diagramFilepath);


                break;

            case TrackBuildR.modes.options:

                EditorGUILayout.Space();
                Title("Generation Options", TrackBuildRColours.RED);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render Track");
                _track.render = EditorGUILayout.Toggle(_track.render, GUILayout.Width(15));
                EditorGUILayout.EndHorizontal();

                //Toggle showing the wireframe when we have selected the model.
                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                EditorGUILayout.LabelField("Show Wireframe");
                _track.showWireframe = EditorGUILayout.Toggle(_track.showWireframe, GUILayout.Width(15));
                EditorGUILayout.EndHorizontal();

                //Tangent calculation
                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                EditorGUI.BeginDisabledGroup(_trackBuildR.tangentsGenerated);
                if(GUILayout.Button("Build Tangents", GUILayout.Height(38)))
                {
                    Undo.RecordObject(_trackBuildR, "Build Tangents");
                    _trackBuildR.GenerateTangents();
                    GUI.changed = false;
                }
                EditorGUI.EndDisabledGroup();
                if(!_trackBuildR.tangentsGenerated)
                    EditorGUILayout.HelpBox("The model doesn't have tangents", MessageType.Warning);
                EditorGUILayout.EndHorizontal();

                //Lightmap rendering
                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                EditorGUI.BeginDisabledGroup(_trackBuildR.lightmapGenerated);
                if(GUILayout.Button("Build Lightmap UVs", GUILayout.Height(38)))
                {
                    Undo.RecordObject(_trackBuildR, "Build Lightmap UVs");
                    _trackBuildR.GenerateSecondaryUVSet();
                    GUI.changed = false;
                }
                EditorGUI.EndDisabledGroup();
                if(!_trackBuildR.lightmapGenerated)
                    EditorGUILayout.HelpBox("The model doesn't have lightmap UVs", MessageType.Warning);
                EditorGUILayout.EndHorizontal();

                //Mesh Optimisation
                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                EditorGUI.BeginDisabledGroup(_trackBuildR.optimised);
                if(GUILayout.Button("Optimise Mesh For Runtime", GUILayout.Height(38)))
                {
                    Undo.RecordObject(_trackBuildR, "Optimise Meshes");
                    _trackBuildR.OptimseMeshes();
                    GUI.changed = false;
                }
                EditorGUI.EndDisabledGroup();
                if(!_trackBuildR.optimised)
                    EditorGUILayout.HelpBox("The model is currently fully optimised for runtime", MessageType.Warning);
                EditorGUILayout.EndHorizontal();

                if(GUILayout.Button("Force Full Rebuild of Track"))
                    _trackBuildR.ForceFullRecalculation();

                break;

            case TrackBuildR.modes.export:
                TrackBuildRExport.InspectorGUI(_trackBuildR);
                break;
        }

        GUILayout.EndVertical();
    }

    /// <summary>
    /// A GUI stub that displays the coloured titles in the inspector for Track BuildR
    /// </summary>
    /// <param customName="titleString">The title to display</param>
    /// <param customName="colour">Colour of the background</param>
    private static void Title(string titleString, Color colour)
    {
        //TITLE
        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.fixedHeight = 60;
        title.fixedWidth = 400;
        title.alignment = TextAnchor.UpperCenter;
        title.fontStyle = FontStyle.Bold;
        title.normal.textColor = Color.white;
        EditorGUILayout.LabelField(titleString, title);
        Texture2D facadeTexture = new Texture2D(1, 1);
        facadeTexture.SetPixel(0, 0, colour);
        facadeTexture.Apply();
        Rect sqrPos = new Rect(0, 0, 0, 0);
        if (Event.current.type == EventType.Repaint)
            sqrPos = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(sqrPos, facadeTexture);
        EditorGUI.LabelField(sqrPos, titleString, title);
    }

    private static void RenderPreview(TrackBuildR _trackBuildR)
    {
        if (!SystemInfo.supportsRenderTextures)
            return;

        if (EditorApplication.isPlaying)
            return;

        TrackBuildRTrack _track = _trackBuildR.track;
        if (_track.realNumberOfPoints < 2)
            return;

        if (pointPreviewTexture == null)
            pointPreviewTexture = new RenderTexture(previewResolution, Mathf.RoundToInt(previewResolution / aspect), 24, RenderTextureFormat.RGB565);

        float previewpercent = (_trackBuildR.previewPercentage + _trackBuildR.previewStartPoint) % 1.0f;
        if(!_trackBuildR.previewForward) previewpercent = 1.0f - previewpercent;

        _track.diagramGO.renderer.enabled = false;
        GameObject trackEditorPreview = _trackBuildR.trackEditorPreview;
        Vector3 trackUp = _track.GetTrackUp(previewpercent) * Vector3.forward;
        Vector3 trackDirection = _track.GetTrackDirection(previewpercent);
        if (!_trackBuildR.previewForward) trackDirection = -trackDirection;
        trackEditorPreview.transform.position = _track.GetTrackPosition(previewpercent) + (previewCameraHeight.y * trackUp) + _trackBuildR.transform.position;
        trackEditorPreview.transform.rotation = Quaternion.LookRotation(trackDirection, trackUp);
        trackEditorPreview.camera.targetTexture = pointPreviewTexture;
        trackEditorPreview.camera.Render();
        trackEditorPreview.camera.targetTexture = null;
        _track.diagramGO.renderer.enabled = _track.showDiagram;

        GUILayout.Label(pointPreviewTexture, GUILayout.Width(400), GUILayout.Height(225));

        EditorGUILayout.BeginHorizontal();
        string trackForwardText = _trackBuildR.previewForward ? "Track Preivew Direction Forward >>" : "<< Track Preivew Direction Backward";
        if(GUILayout.Button(trackForwardText))
            _trackBuildR.previewForward = !_trackBuildR.previewForward;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Preview Track Percentage");
        EditorGUILayout.BeginHorizontal();
        _trackBuildR.previewPercentage = EditorGUILayout.Slider(_trackBuildR.previewPercentage, 0, 1);
        EditorGUILayout.LabelField("0-1", GUILayout.Width(25));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Preview Track Start Point");
        EditorGUILayout.BeginHorizontal();
        _trackBuildR.previewStartPoint = EditorGUILayout.Slider(_trackBuildR.previewStartPoint, 0, 1);
        EditorGUILayout.LabelField("0-1", GUILayout.Width(25));
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// The Texture display GUI
    /// </summary>
    /// <param customName="texture"></param>
    /// <returns>True if this texture was modified</returns>
    private static bool TextureGUI(ref TrackBuildRTexture texture)
    {
        if(texture.material == null)
            texture.material = new Material(Shader.Find("Diffuse"));

        bool isModified = false;
        string textureName = texture.customName;
        textureName = EditorGUILayout.TextField("Name", textureName);
        if (texture.customName != textureName)
        {
            texture.customName = textureName;
        }

        
        texture.type = (TrackBuildRTexture.Types)EditorGUILayout.EnumPopup("Type", texture.type);

        //Shader Time
        Shader[] tempshaders = (Shader[])Resources.FindObjectsOfTypeAll(typeof(Shader));
        List<string> shaderNames = new List<string>(ShaderProperties.NAMES);
        foreach (Shader shader in tempshaders)
        {
            if (!string.IsNullOrEmpty(shader.name) && !shader.name.StartsWith("__") && !shader.name.Contains("hidden"))
                shaderNames.Add(shader.name);
        }
        int selectedShaderIndex = shaderNames.IndexOf(texture.material.shader.name);
        int newSelectedShaderIndex = EditorGUILayout.Popup("Shader", selectedShaderIndex, shaderNames.ToArray());
        if (selectedShaderIndex != newSelectedShaderIndex)
        {
            texture.material.shader = Shader.Find(shaderNames[newSelectedShaderIndex]);
        }

        switch(texture.type)
        {
            case TrackBuildRTexture.Types.Basic:
                Shader selectedShader = texture.material.shader;
                int propertyCount = ShaderUtil.GetPropertyCount(selectedShader);

                for (int s = 0; s < propertyCount; s++)
                {
                    ShaderUtil.ShaderPropertyType propertyTpe = ShaderUtil.GetPropertyType(selectedShader, s);
                    string shaderPropertyName = ShaderUtil.GetPropertyName(selectedShader, s);
                    switch (propertyTpe)
                    {
                        case ShaderUtil.ShaderPropertyType.TexEnv:
                            Texture shaderTexture = texture.material.GetTexture(shaderPropertyName);
                            Texture newShaderTexture = (Texture)EditorGUILayout.ObjectField(shaderPropertyName, shaderTexture, typeof(Texture), false);
                            if (shaderTexture != newShaderTexture)
                            {
                                texture.material.SetTexture(shaderPropertyName, newShaderTexture);
                            }
                            break;

                        case ShaderUtil.ShaderPropertyType.Color:
                            Color shaderColor = texture.material.GetColor(shaderPropertyName);
                            Color newShaderColor = EditorGUILayout.ColorField(shaderPropertyName, shaderColor);
                            if (shaderColor != newShaderColor)
                            {
                                texture.material.SetColor(shaderPropertyName, newShaderColor);
                            }
                            break;

                        case ShaderUtil.ShaderPropertyType.Float:
                            float shaderFloat = texture.material.GetFloat(shaderPropertyName);
                            float newShaderFloat = EditorGUILayout.FloatField(shaderPropertyName, shaderFloat);
                            if (shaderFloat != newShaderFloat)
                            {
                                texture.material.SetFloat(shaderPropertyName, newShaderFloat);
                            }
                            break;

                        case ShaderUtil.ShaderPropertyType.Range:
                            float shaderRange = texture.material.GetFloat(shaderPropertyName);
                            float rangeMin = ShaderUtil.GetRangeLimits(selectedShader, s, 1);
                            float rangeMax = ShaderUtil.GetRangeLimits(selectedShader, s, 2);
                            float newShaderRange = EditorGUILayout.Slider(shaderPropertyName, shaderRange, rangeMin, rangeMax);
                            if (shaderRange != newShaderRange)
                            {
                                texture.material.SetFloat(shaderPropertyName, newShaderRange);
                            }
                            break;

                        case ShaderUtil.ShaderPropertyType.Vector:
                            Vector3 shaderVector = texture.material.GetVector(shaderPropertyName);
                            Vector3 newShaderVector = EditorGUILayout.Vector3Field(shaderPropertyName, shaderVector);
                            if (shaderVector != newShaderVector)
                            {
                                texture.material.SetVector(shaderPropertyName, newShaderVector);
                            }
                            break;
                    }
                }

                if(texture.texture == null)
                    return isModified;

                bool textureflipped = EditorGUILayout.Toggle("Flip Clockwise", texture.flipped);
                if(textureflipped != texture.flipped)
                {
                    isModified = true;
                    texture.flipped = textureflipped;
                }

                Vector2 textureUnitSize = texture.textureUnitSize;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("trackTexture width", GUILayout.Width(75));//, GUILayout.Width(42));
                float textureUnitSizex = EditorGUILayout.FloatField(texture.textureUnitSize.x, GUILayout.Width(25));
                if(textureUnitSizex != textureUnitSize.x)
                {
                    isModified = true;
                    textureUnitSize.x = textureUnitSizex;
                }
                EditorGUILayout.LabelField("metres", GUILayout.Width(40));//, GUILayout.Width(42));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("trackTexture height", GUILayout.Width(75));//, GUILayout.Width(42));
                float textureUnitSizey = EditorGUILayout.FloatField(texture.textureUnitSize.y, GUILayout.Width(25));
                if(textureUnitSizey != textureUnitSize.y)
                {
                    isModified = true;
                    textureUnitSize.y = textureUnitSizey;
                }
                EditorGUILayout.LabelField("metres", GUILayout.Width(40));
                EditorGUILayout.EndHorizontal();
                texture.textureUnitSize = textureUnitSize;

                const int previewTextureUnitSize = 120;
                const int previewPadding = 25;

                EditorGUILayout.LabelField("1 Metre Squared", GUILayout.Width(previewTextureUnitSize));
                GUILayout.Space(previewPadding);
                EditorGUILayout.Space();

                Rect texturePreviewPostion = new Rect(0, 0, 0, 0);
                if(Event.current.type == EventType.Repaint)
                    texturePreviewPostion = GUILayoutUtility.GetLastRect();

                Rect previewRect = new Rect(texturePreviewPostion.x, texturePreviewPostion.y, previewTextureUnitSize, previewTextureUnitSize);
                Rect sourceRect = new Rect(0, 0, (1.0f / textureUnitSize.x), (1.0f / textureUnitSize.y));

                Graphics.DrawTexture(previewRect, texture.texture, sourceRect, 0, 0, 0, 0);
                GUILayout.Space(previewTextureUnitSize);
            break;

            case TrackBuildRTexture.Types.Substance:

                texture.proceduralMaterial = (ProceduralMaterial)EditorGUILayout.ObjectField("Procedural Material", texture.proceduralMaterial, typeof(ProceduralMaterial), false);

                if (texture.proceduralMaterial != null)
                {
                    ProceduralMaterial pMat = texture.proceduralMaterial;
                    GUILayout.Label(pMat.GetGeneratedTexture(pMat.mainTexture.name), GUILayout.Width(400));
                }
                else
                    EditorGUILayout.HelpBox("There is no substance material set.", MessageType.Error);
            break;

            case TrackBuildRTexture.Types.User:
                texture.userMaterial = (Material)EditorGUILayout.ObjectField("User Material", texture.userMaterial, typeof(Material), false);

                if (texture.userMaterial != null)
                {
                    Material mat = texture.userMaterial;
                    GUILayout.Label(mat.mainTexture, GUILayout.Width(400));
                }
                else
                    EditorGUILayout.HelpBox("There is no substance material set.", MessageType.Error);
            break;
        }
        if (isModified)
            GUI.changed = true;

        return isModified;
    }

    /// <summary>
    /// Deals with modifing the diagram used in track building
    /// </summary>
    private static void UpdateDiagram(TrackBuildRTrack _track)
    {
        //        Texture texture = _track.diagramMaterial.mainTexture;
        float scaleSize = Vector3.Distance(_track.scalePointB, _track.scalePointA);
        float diagramScale = _track.scale / scaleSize;
        _track.diagramGO.transform.localScale *= diagramScale;
        _track.scalePointA *= diagramScale;
        _track.scalePointB *= diagramScale;
    }

    /// <summary>
    /// A stub of GUI for selecting the texture for a specific part of the track on a specific curve
    /// IE. The track texture, the wall texture, etc...
    /// </summary>
    /// <param customName="_track"></param>
    /// <param customName="textureIndex"></param>
    /// <param customName="label"></param>
    /// <returns></returns>
    private static int CurveTextureSelector(TrackBuildRTrack _track, int textureIndex, string label)
    {
        TrackBuildRTexture[] textures = _track.GetTexturesArray();
        int numberOfTextures = textures.Length;
        string[] textureNames = new string[numberOfTextures];
        for (int t = 0; t < numberOfTextures; t++)
            textureNames[t] = textures[t].customName;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(label);
        textureIndex = EditorGUILayout.Popup(textureIndex, textureNames);
        TrackBuildRTexture tbrTexture = _track.Texture(textureIndex);
        EditorGUILayout.EndVertical();
        GUILayout.Label(tbrTexture.texture, GUILayout.Width(50), GUILayout.Height(50));
        EditorGUILayout.EndHorizontal();

        return textureIndex;
    }

    /// <summary>
    /// Called to ensure we're not leaking stuff into the Editor
    /// </summary>
    public static void CleanUp()
    {
        if (pointPreviewTexture)
        {
            pointPreviewTexture.DiscardContents();
            pointPreviewTexture.Release();
            Object.DestroyImmediate(pointPreviewTexture);
        }
    }
    
    /// <summary>
    /// A little hacking of the Unity Editor to allow us to focus on an arbitrary point in 3D Space
    /// We're replicating pressing the F button in scene view to focus on the selected object
    /// Here we can focus on a 3D point
    /// </summary>
    /// <param customName="position">The 3D point we want to focus on</param>
    private static void GotoScenePoint(Vector3 position)
    {
        Object[] intialFocus = Selection.objects;
        GameObject tempFocusView = new GameObject("Temp Focus View");
        tempFocusView.transform.position = position;
        Selection.objects = new Object[] { tempFocusView };
        SceneView.lastActiveSceneView.FrameSelected();
        Selection.objects = intialFocus;
        Object.DestroyImmediate(tempFocusView);
    }
}
