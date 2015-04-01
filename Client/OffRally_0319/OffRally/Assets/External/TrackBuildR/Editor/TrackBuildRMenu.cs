// Track BuildR
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://trackbuildr.jasperstocker.com
// For support contact email@jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using UnityEditor;

public class TrackBuildRMenu : EditorWindow
{

    [MenuItem("GameObject/Create New BuildR Track", false, 0)]
    public static void CreateNewBuilding()
    {
        GameObject newTrack = new GameObject("New Track");
        Undo.RegisterCreatedObjectUndo(newTrack, "Created New Track BuildR Track");
        TrackBuildR trackBuildR = newTrack.AddComponent<TrackBuildR>();
        trackBuildR.Init();
        //Focus on new track
        Selection.objects = new Object[] { newTrack };
        SceneView.lastActiveSceneView.FrameSelected();
    }
}