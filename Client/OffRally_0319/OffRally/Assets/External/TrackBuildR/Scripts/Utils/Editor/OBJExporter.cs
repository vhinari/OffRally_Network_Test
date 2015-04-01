// BuildR
// Available on the Unity3D Asset Store
// Jasper Stocker http://support.jasperstocker.com
// Support contact email@jasperstocker.com
// Based on EditorObjExporter.cs by KeliHlodversson
// http://wiki.unity3d.com/index.php?title=ObjExporter
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class OBJExporter
{
	private static int vertexOffset = 0;
	private static int normalOffset = 0;
	private static int uvOffset = 0;
	
	private static Mesh exportMesh;
	private static ExportMaterial[] exportTextures;
	private static string targetFolder = "Assets/Buildr/Exported/";
	private static string targetName = "ExportedObj";
	private static bool _copyTextures = true;
	
	public static bool Export(string folder, string filename, Mesh mesh, ExportMaterial[] textures, bool copyTextures)
	{
		
		exportMesh = mesh;
		exportTextures = textures;
		targetFolder = folder;
		targetName = filename;
		_copyTextures = copyTextures;
		
		if(folder.Contains(" "))
		{
			EditorUtility.DisplayDialog("Filename Error","The filename can't contain spaces","I'm sorry");
			return false;
		}
		
		if(filename.Contains(" "))
		{
			EditorUtility.DisplayDialog("Filename Error","The filename can't contain spaces","I'm sorry");
			return false;
		}
		
		/*if (!CreateTargetFolder())
		{
			Debug.LogError("There was a problem with the destination folder");
    		return false;
		}*/
		
        MeshToFile(targetFolder, targetName);
		
		exportMesh = null;
		exportTextures = null;
		
		return  true;
	}
	
	private static string MeshToString(Dictionary<string, ExportMaterial> materialList) 
    {
        StringBuilder sb = new StringBuilder();
		
		sb.AppendLine("# Unity Asset Buildr OBJ Exporter\n http://buildr.jasperstocker.com");
		
        sb.Append("g ").Append(targetName).Append("\n");
		foreach(Vector3 lv in exportMesh.vertices) 
        {
        	Vector3 wv = lv;
 
        	//This is sort of ugly - inverting x-component since we're in
        	//a different coordinate system than "everyone" is "used to".
            sb.Append(string.Format("v {0} {1} {2}\n",-wv.x,wv.y,wv.z));
        }
        sb.Append("\n");
 
        foreach(Vector3 lv in exportMesh.normals) 
        {
        	Vector3 wv = lv;
 
            sb.Append(string.Format("vn {0} {1} {2}\n",-wv.x,wv.y,wv.z));
        }
        sb.Append("\n");
 
        foreach(Vector3 v in exportMesh.uv) 
        {
            sb.Append(string.Format("vt {0} {1}\n",v.x,v.y));
        }
		
        for (int t=0; t < exportMesh.subMeshCount; t ++) {
            sb.Append("\n");
            if(t < exportTextures.Length - 1)
            {
                sb.Append("g ").Append(exportTextures[t].name).Append("\n");
                sb.Append("usemtl ").Append(exportTextures[t].name).Append("\n");
                sb.Append("usemap ").Append(exportTextures[t].name).Append("\n");
            }
            else
            {
                sb.Append("g ").Append(exportTextures[exportTextures.Length - 1].name).Append("\n");
                sb.Append("usemtl ").Append(exportTextures[exportTextures.Length - 1].name).Append("\n");
                sb.Append("usemap ").Append(exportTextures[exportTextures.Length - 1].name).Append("\n");
            }
 
            //See if this material is already in the materiallist.
            try
       		{
          		ExportMaterial objMaterial = exportTextures[t]; 
          		materialList.Add(objMaterial.name, objMaterial);
        	}
        	catch (ArgumentException)
        	{
            	//Already in the dictionary
				//Debug.LogError("Already in the dictionary");
        	}
 
 
            int[] triangles = exportMesh.GetTriangles(t);
            for (int i=0;i<triangles.Length;i+=3) 
            {
            	//Because we inverted the x-component, we also needed to alter the triangle winding.
                sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", 
                    triangles[i]+1 + vertexOffset, triangles[i+1]+1 + normalOffset, triangles[i+2]+1 + uvOffset));
            }
        }
 
        vertexOffset += exportMesh.vertices.Length;
        normalOffset += exportMesh.normals.Length;
        uvOffset += exportMesh.uv.Length;
 
        return sb.ToString();
    }

	private static void Clear()
    {
    	vertexOffset = 0;
    	normalOffset = 0;
    	uvOffset = 0;
    }
 
   	private static Dictionary<string, ExportMaterial> PrepareFileWrite()
   	{
   		Clear();
    	return new Dictionary<string, ExportMaterial>();
   	}
 
   	private static void MaterialsToFile(Dictionary<string, ExportMaterial> materialList, string folder, string filename)
   	{
   		using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".mtl")) 
        {
        	foreach( KeyValuePair<string, ExportMaterial> kvp in materialList )
        	{
        		sw.Write("\n");
        		sw.Write("newmtl {0}\n", kvp.Key);
        		sw.Write("Ka  1.0 1.0 1.0\n");
				sw.Write("Kd  1.0 1.0 1.0\n");
				sw.Write("Ks  1.0 1.0 1.0\n");
				sw.Write("Tf  1.0 1.0 1.0\n");
				sw.Write("d  1.0\n");
				sw.Write("Ns  1.0\n");
				sw.Write("Ni  1.0\n");
				sw.Write("illum 2\n");
				
				string destinationFile = kvp.Value.filepath;
				string texturePath = destinationFile;
				if(_copyTextures)
				{
					int stripIndex = destinationFile.LastIndexOf('/');
	   				if (stripIndex >= 0)
	        			destinationFile = destinationFile.Substring(stripIndex + 1).Trim();
	        		texturePath = destinationFile;//relative file path when using textures in the export folder
	        		destinationFile = folder + destinationFile;
					if(!kvp.Value.generated)
					{
						try
						{
							//Copy the source file
							//Debug.Log("Copying texture from " + kvp.Value.filepath + " to " + destinationFile);
							File.Copy(kvp.Value.filepath, destinationFile, true);
						}
						catch
						{
							Debug.LogWarning("Could not copy texture!");
						}
					}
				}
				sw.Write("map_Kd {0}", texturePath);
 
				sw.Write("\n\n\n");
        	}
        }
   	}
 
    private static void MeshToFile(string folder, string filename) 
    {
    	Dictionary<string, ExportMaterial> materialList = PrepareFileWrite();
 
        using (StreamWriter sw = new StreamWriter(folder +"/" + filename + ".obj")) 
        {
        	sw.Write("mtllib ./" + filename + ".mtl\n");
            sw.Write(MeshToString(materialList));
        }
 
        MaterialsToFile(materialList, folder, filename);
    }
 
	private static bool CreateTargetFolder()
	{
		string newDirectory = targetFolder;//targetFolder+targetName+"/";
		//Debug.Log("Create "+newDirectory);
		if(System.IO.Directory.Exists(newDirectory))
		{
			if(EditorUtility.DisplayDialog("File directory exists", "Are you sure you want to overwrite the contents of this file?", "Cancel", "Overwrite"))
			{
				return false;
			}
		}
				
		try
		{
			System.IO.Directory.CreateDirectory(newDirectory);
		}
		catch
		{
			EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
			return false;
		}
	
		return true;
	}
}
