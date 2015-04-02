using UnityEngine;
using UnityEditor;
using System.IO;

public class TrackBuildRExport
{
    private static Transform CURRENT_TRANSFORM;
    private const string COLLIDER_SUFFIX = "_Collider";

    private const string PROGRESSBAR_TEXT = "Exporting Track";
    private static string FILE_EXTENTION = ".obj";

    private const string ROOT_FOLDER = "Assets/TrackBuildR/Exported/";

    public static void InspectorGUI(TrackBuildR track)
    {
        TrackBuildRTrack trackData = track.track;

        const int guiWidth = 400;
        const int textWidth = 348;
        const int toggleWidth = 25;
        const int helpWidth = 20;

        CURRENT_TRANSFORM = track.transform;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filename", GUILayout.Width(225));
        track.exportFilename = EditorGUILayout.TextField(track.exportFilename, GUILayout.Width(175));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filetype", GUILayout.Width(350));
        track.fileType = (TrackBuildR.fileTypes)EditorGUILayout.EnumPopup(track.fileType, GUILayout.Width(50));
        switch (track.fileType)
        {
            case TrackBuildR.fileTypes.Obj:
                FILE_EXTENTION = ".obj";
                break;
            case TrackBuildR.fileTypes.Fbx:
                FILE_EXTENTION = ".fbx";
                break;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Copy Textures into Export Folder", GUILayout.Width(textWidth));
        track.copyTexturesIntoExportFolder = EditorGUILayout.Toggle(track.copyTexturesIntoExportFolder, GUILayout.Width(toggleWidth));
        if (GUILayout.Button("?", GUILayout.Width(helpWidth)))
        {
            string helpTitle = "Help - Copy Textures into Export Folder";
            string helpBody = "Check this box if you want to copy the textures you are using into the export folder." +
                "\nThis is useful if you plan to use the exported model elsewhere. Having the model and the textures in one folder will allow you to move this model with ease.";
            EditorUtility.DisplayDialog(helpTitle, helpBody, "close");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Export Collider");
//        track.exportSimpleCollider = EditorGUILayout.Toggle(track.exportSimpleCollider, GUILayout.Width(toggleWidth));
        track.exportCollider = EditorGUILayout.Toggle(track.exportCollider, GUILayout.Width(toggleWidth));
        if (GUILayout.Button("?", GUILayout.Width(helpWidth)))
        {
            string helpTitle = "Help - Export Collider Mesh";
            string helpBody = "Check this box if you wish to generate a trackCollider mesh for your model." +
                "\nThis will generate a mesh to be used with colliders.";
            EditorUtility.DisplayDialog(helpTitle, helpBody, "close");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Export as Prefab", GUILayout.Width(textWidth));
        track.createPrefabOnExport = EditorGUILayout.Toggle(track.createPrefabOnExport, GUILayout.Width(toggleWidth));
        if (GUILayout.Button("?", GUILayout.Width(helpWidth)))
        {
            string helpTitle = "Help - Export as Prefab";
            string helpBody = "Select this if you wish to create a prefab of your model." +
                "\nThis is recommended if you're exporting a trackCollider so they will get packaged together.";
            EditorUtility.DisplayDialog(helpTitle, helpBody, "close");
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Export with tangents", GUILayout.Width(textWidth));
        track.includeTangents = EditorGUILayout.Toggle(track.includeTangents, GUILayout.Width(toggleWidth));
        if (GUILayout.Button("?", GUILayout.Width(helpWidth)))
        {
            string helpTitle = "Help - with tangents";
            string helpBody = "Export the models with calculated tangents." + 
                "\nSome shaders require tangents to be calculated on the model." + 
                "\nUnity will do this automatically on all imported meshes so it's not neccessary here." + 
                "/nBut you might want them if you're taking them to another program.";
            EditorUtility.DisplayDialog(helpTitle, helpBody, "close");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        bool usingSubstances = false;
        int numberOfTextures = trackData.numberOfTextures;
        for(int i = 0; i < numberOfTextures; i++)
        {
            TrackBuildRTexture texture = trackData.Texture(i);
            if(texture.type == TrackBuildRTexture.Types.Substance)
            {
                usingSubstances = true;
                break;
            }
        }
        if (usingSubstances)
        {
            EditorGUILayout.HelpBox("Model uses Substance textures." +
                "\nExporting model to " + track.fileType + " will lose references to this texture and it will be rendered white.",
                MessageType.Warning);
        }

        if (GUILayout.Button("Export", GUILayout.Width(guiWidth), GUILayout.Height(40)))
        {
            ExportModel(track);
        }

        EditorGUILayout.Space();
        if(GUILayout.Button("Export to XML"))
        {
            string defaultName = track.name;
            defaultName.Replace(" ", "_");
            string filepath = EditorUtility.SaveFilePanel("Export Track BuildR Track to XML", "Assets/TrackBuildR", defaultName, "xml");

            if (filepath != "")
            {
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    sw.Write(track.ToXML());//write out contents of data to XML
                }
            }
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Import from XML"))
        {
            string xmlpath = EditorUtility.OpenFilePanel("Import Track BuildR Track from XML", "Assets/TrackBuildR/", "xml");
            if (xmlpath != "")
                track.FromXML(xmlpath);
        }
        if (GUILayout.Button("Import from KML"))
        {
            string xmlpath = EditorUtility.OpenFilePanel("Import Google Earth KML", "Assets/TrackBuildR/", "kml");
            if (xmlpath != "")
                track.FromKML(xmlpath);
        }

        CURRENT_TRANSFORM = null;
    }

    private static void ExportModel(TrackBuildR track)
    {
        GameObject baseObject = new GameObject(track.exportFilename);
        baseObject.transform.position = CURRENT_TRANSFORM.position;
        baseObject.transform.rotation = CURRENT_TRANSFORM.rotation;
        EditorUtility.DisplayCancelableProgressBar(PROGRESSBAR_TEXT, "", 0.0f);
        track.ForceFullRecalculation();
        EditorUtility.DisplayCancelableProgressBar(PROGRESSBAR_TEXT, "", 0.1f);
        try
        {
            TrackBuildRTrack trackData = track.track;

            //check overwrites...
            string newDirectory = ROOT_FOLDER + track.exportFilename;
            if(!CreateFolder(newDirectory))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            EditorUtility.DisplayCancelableProgressBar(PROGRESSBAR_TEXT, "", 0.15f);

            int numberOfCurves = trackData.numberOfCurves;
            float exportProgress = 0.75f / (numberOfCurves*6.0f);
            ExportMaterial[] exportMaterials = new ExportMaterial[1];
            ExportMaterial exportTexture = new ExportMaterial();

            string[] dynNames = new []{"track","bumper","boundary","bottom","offread","trackCollider"};
            for(int c = 0; c < numberOfCurves; c++)
            {
                TrackBuildRPoint curve = trackData[c];

                int numberOfDynMeshes = 6;
                DynamicMeshGenericMultiMaterialMesh[] dynMeshes = new DynamicMeshGenericMultiMaterialMesh[6];
                dynMeshes[0] = curve.dynamicTrackMesh;
                dynMeshes[1] = curve.dynamicBumperMesh;
                dynMeshes[2] = curve.dynamicBoundaryMesh;
                dynMeshes[3] = curve.dynamicBottomMesh;
                dynMeshes[4] = curve.dynamicOffroadMesh;
                dynMeshes[5] = curve.dynamicColliderMesh;

                int[] textureIndeices = new int[] { curve.trackTextureStyleIndex ,curve.bumperTextureStyleIndex, curve.boundaryTextureStyleIndex, curve.bottomTextureStyleIndex, curve.offroadTextureStyleIndex, 0};

                for(int d = 0; d < numberOfDynMeshes; d++)
                {
                    if(EditorUtility.DisplayCancelableProgressBar(PROGRESSBAR_TEXT, "Exporting Track Curve " + c + " " + dynNames[d], 0.15f + exportProgress * (c * 6 + d)))
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                    DynamicMeshGenericMultiMaterialMesh exportDynMesh = dynMeshes[d];
                    if(track.includeTangents || exportDynMesh.isEmpty)
                        exportDynMesh.Build(track.includeTangents);//rebuild with tangents

                    TrackBuildRTexture texture = trackData.Texture(textureIndeices[d]);
                    exportTexture.name = texture.customName;
                    exportTexture.material = texture.material;
                    exportTexture.generated = false;
                    exportTexture.filepath = texture.filePath;
                    exportMaterials[0] = exportTexture;

                    int meshCount = exportDynMesh.meshCount;
                    for (int i = 0; i < meshCount; i++)
                    {
                        Mesh exportMesh = exportDynMesh[i].mesh;
                        MeshUtility.Optimize(exportMesh);
                        string filenameSuffix = trackModelName(dynNames[d], c, (meshCount > 1) ? i : -1);// "trackCurve" + c + ((meshCount > 1) ? "_" + i.ToString() : "");
                        string filename = track.exportFilename + filenameSuffix;
                        Export(filename, ROOT_FOLDER + track.exportFilename + "/", track, exportMesh, exportMaterials);

                        if(track.createPrefabOnExport)
                        {
                            AssetDatabase.Refresh();//ensure the database is up to date...

                            string modelFilePath = ROOT_FOLDER + track.exportFilename + "/" + filename + FILE_EXTENTION;
                            if(d < numberOfDynMeshes - 1)
                            {
                                GameObject newModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadMainAssetAtPath(modelFilePath));
                                newModel.name = filename;
                                newModel.transform.parent = baseObject.transform;
                                newModel.transform.localPosition = Vector3.zero;
                                newModel.transform.localRotation = Quaternion.identity;
                            }
                            else
                            {
                                GameObject colliderObject = new GameObject("trackCollider");
                                colliderObject.AddComponent<MeshCollider>().sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(modelFilePath, typeof(Mesh));
                                colliderObject.transform.parent = baseObject.transform;
                                colliderObject.transform.localPosition = Vector3.zero;
                                colliderObject.transform.localRotation = Quaternion.identity;
                            }
                        }
                    }

                }
            }
            if(track.createPrefabOnExport)
            {
                string prefabPath = ROOT_FOLDER + track.exportFilename + "/" + track.exportFilename + ".prefab";
                Object prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if(prefab == null)
                    prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
                PrefabUtility.ReplacePrefab(baseObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
            }

            EditorUtility.DisplayCancelableProgressBar(PROGRESSBAR_TEXT, "", 0.70f);

            AssetDatabase.Refresh();//ensure the database is up to date...
        }
        catch(System.Exception e)
        {
            Debug.LogError("BuildR Export Error: "+e);
            EditorUtility.ClearProgressBar();
        }
        Object.DestroyImmediate(baseObject);
        EditorUtility.ClearProgressBar();
        EditorUtility.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }

    private static void ExportCollider(TrackBuildR data)
    {
        DynamicMeshGenericMultiMaterialMesh COL_MESH = new DynamicMeshGenericMultiMaterialMesh();
//        COL_MESH.subMeshCount = data.textures.Count;
//        BuildrBuildingCollider.Build(COL_MESH, data);
//        COL_MESH.CollapseSubmeshes();
        COL_MESH.Build(false);

        ExportMaterial[] exportTextures = new ExportMaterial[1];
        ExportMaterial newTexture = new ExportMaterial();
        newTexture.name = "blank";
        newTexture.filepath = "";
        newTexture.generated = true;
        exportTextures[0] = newTexture;

        int numberOfColliderMeshes = COL_MESH.meshCount;
        for (int i = 0; i < numberOfColliderMeshes; i++)
        {
            MeshUtility.Optimize(COL_MESH[i].mesh);
            string ColliderSuffixIndex = ((numberOfColliderMeshes > 1) ? "_" + i : "");
            string ColliderFileName = data.exportFilename + COLLIDER_SUFFIX + ColliderSuffixIndex;
            string ColliderFolder = ROOT_FOLDER + data.exportFilename + "/";
            Export(ColliderFileName, ColliderFolder, data, COL_MESH[i].mesh, exportTextures);
        }

        //string newDirectory = rootFolder+track.exportFilename;
        //if(!CreateFolder(newDirectory))
        //	return;
//        ExportMaterial[] exportTextures = new ExportMaterial[1];
//        ExportMaterial newTexture = new ExportMaterial();
//        newTexture.customName = "";
//        newTexture.filepath = "";
//        newTexture.generated = true;
//        exportTextures[0] = newTexture;
//        Export(track.exportFilename + COLLIDER_SUFFIX, ROOT_FOLDER + track.exportFilename + "/", track, EXPORT_MESH, exportTextures);
//
//        COL_MESH = null;
//        EXPORT_MESH = null;
    }

    private static void Export(string filename, string folder, TrackBuildR data, Mesh exportMesh, ExportMaterial[] exportTextures)
    {
        Debug.Log("Export "+filename+" "+folder);
        switch (data.fileType)
        {
            case TrackBuildR.fileTypes.Obj:
                OBJExporter.Export(folder, filename, exportMesh, exportTextures, data.copyTexturesIntoExportFolder);
                break;
            case TrackBuildR.fileTypes.Fbx:
                FBXExporter.Export(folder, filename, exportMesh, exportTextures, data.copyTexturesIntoExportFolder);
                break;
        }
    }

    private static bool CreateFolder(string newDirectory)
    {
        if (Directory.Exists(newDirectory))
        {
            if (EditorUtility.DisplayDialog("File directory exists", "Are you sure you want to overwrite the contents of this file?", "Cancel", "Overwrite"))
            {
                return false;
            }
        }

        try
        {
            Directory.CreateDirectory(newDirectory);
        }
        catch
        {
            EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
            return false;
        }

        return true;
    }

    private static string trackModelName(string type, int curve, int mesh)
    {
        return "_" + type + "_" + curve + ((mesh > -1) ? "_" + mesh.ToString() : "");
    }
}