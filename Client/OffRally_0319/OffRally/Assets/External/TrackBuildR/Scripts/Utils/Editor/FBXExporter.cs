// BuildR
// Available on the Unity3D Asset Store
// Jasper Stocker http://support.jasperstocker.com
// Support contact email@jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

//This FBX Exporter isn't a fully functional one
//For the purposes of BuildR this will only export basic geometry
//There is no support for animation and rigging
//TESTED IN
//Unity3D, Maya

public class FBXExporter
{	
	private static Mesh exportMesh;
	private static ExportMaterial[] exportTextures;
	private static string targetFolder = "Assets/Buildr/Exported/";
	private static string targetName = "ExportedFBX";
	private static bool _copyTextures = true;
	
	//ID numbers to connect the various parts of the FBX data
	public static int geomIdent = 10000;
	public static int modelIdent = 20000;
	public static int textureIdent = 30000;
	public static int matieralIdent = 40000;
	
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
	
	private static void MeshToFile(string folder, string filename) 
    {
		using (StreamWriter sw = new StreamWriter(folder +"/" + filename + ".fbx")) 
		{
			sw.Write(MeshToString());
		}
    }
	
	private static string MeshToString() 
    {
        StringBuilder sb = new StringBuilder();
		
		//Header
		sb.AppendLine("; FBX 7.2.0 project file");
		sb.AppendLine("; Copyright (C) 1997-2010 Autodesk Inc. and/or its licensors.");
		sb.AppendLine("; All rights reserved.");
		sb.AppendLine("; ----------------------------------------------------");
		
		sb.AppendLine("FBXHeaderExtension:  {");
		sb.AppendLine("	FBXHeaderVersion: 1003");
		sb.AppendLine("	FBXVersion: 7200");
		sb.AppendLine("	Creator: \"Track BuildR v1.1 FBX Exporter V1\"");
		sb.AppendLine("}");
		
		sb.AppendLine("; Object definitions");
		sb.AppendLine(";------------------------------------------------------------------");
		
		sb.AppendLine("Definitions:  {");
		sb.AppendLine("	Version: 100");
		sb.AppendLine("	Count: 4");
		sb.AppendLine("	ObjectType: \"Geometry\" {");
		sb.AppendLine("		Count: 1");
		sb.AppendLine("		PropertyTemplate: \"FbxMesh\" {");
		sb.AppendLine("		}");
		sb.AppendLine("	}");
		sb.AppendLine("	ObjectType: \"Material\" {");
		sb.AppendLine("		Count: 1");
		sb.AppendLine("		PropertyTemplate: \"FbxSurfacePhong\" {");
		sb.AppendLine("		}");
		sb.AppendLine("	}");
		sb.AppendLine("	ObjectType: \"Texture\" {");
		sb.AppendLine("		Count: 1");
		sb.AppendLine("		PropertyTemplate: \"FbxFileTexture\" {");
		sb.AppendLine("		}");
		sb.AppendLine("	}");
		sb.AppendLine("	ObjectType: \"Model\" {");
		sb.AppendLine("		Count: 1");
		sb.AppendLine("		PropertyTemplate: \"FbxNode\" {");
		sb.AppendLine("		}");
		sb.AppendLine("	}");
		sb.AppendLine("}");
		
		sb.AppendLine("; Object properties");
		sb.AppendLine(";------------------------------------------------------------------");
		
		int numberOfSubmeshes = exportMesh.subMeshCount;
		
		sb.AppendLine("Objects:  {");
		for(int sm=0; sm<numberOfSubmeshes; sm++)
		{
			//recompile data lists by submesh
			int[] Tris = exportMesh.GetTriangles(sm);
			List<int> SMTris = new List<int>();
			List<Vector3> SMVerts = new List<Vector3>();
			List<Vector3> SMNorms = new List<Vector3>();
			List<Vector2> SMUVs = new List<Vector2>();
			int newIndex = 0;
			foreach(int index in Tris)
			{
				Vector3 vert = exportMesh.vertices[index];
				SMVerts.Add(vert);
				SMUVs.Add(exportMesh.uv[index]);
				SMNorms.Add(exportMesh.normals[index]);
				SMTris.Add(newIndex);
				newIndex++;
			}
			
		//GEOMETRY DATA
			int vertCount = SMVerts.Count;
			sb.AppendLine("	Geometry: "+(geomIdent+sm)+", \"Geometry::\", \"Mesh\" {");
			sb.AppendLine("		Vertices: *"+(vertCount*3)+" {");
			sb.Append("			a: ");
			
			for(int i=0;i<vertCount;i++)
	        {
				if(i>0)
					sb.Append(",");
	        	Vector3 wv = SMVerts[i];
	            sb.Append(string.Format("{0},{1},{2}",-wv.x*100,wv.y*100,wv.z*100));//invert the x and scale up by 100* as Unity FBX default is 0.01
	        }
			sb.AppendLine("");
			sb.AppendLine("		}  ");
			
			int triCount = SMTris.Count;
			sb.AppendLine("		PolygonVertexIndex: *"+triCount+" { ");
			sb.Append("			a:  ");
			for(int i=0;i<triCount;i+=3)
	        {
				if(i>0)
					 sb.Append(",");
				
				int a = SMTris[i];
				int b = SMTris[i+2];
				int c = -SMTris[i+1]-1;//oh look at that - isn't that an interesting way of denoting the end of a polygon...
				//NOTE: we reversed the x for we need to rewind the triangles
				
				sb.Append(string.Format("{0},{1},{2}",a,b,c));//
	        }
			sb.AppendLine("");
			sb.AppendLine("		} ");
			
			sb.AppendLine("		GeometryVersion: 124");
			sb.AppendLine("		LayerElementNormal: 0 {");
			sb.AppendLine("			Version: 101");
			sb.AppendLine("			Name: \"\"");
			sb.AppendLine("			MappingInformationType: \"ByPolygonVertex\"");
			sb.AppendLine("			ReferenceInformationType: \"Direct\"");
			sb.AppendLine("			Normals: *"+ (triCount*3) +" {");
			sb.Append("					a:  ");
			
			//map out the normals by polyvertex so we'll use the triangle array to get all the normal values
			//also - need to reverse the x value as we've x-flipped the model
			for(int i=0;i<triCount;i++)
	        {
				if(i>0)
					 sb.Append(",");
				Vector3 nrm = SMNorms[SMTris[i]];
	            sb.Append(string.Format("{0},{1},{2}",-nrm.x,nrm.y,nrm.z));
	        }
			sb.AppendLine("");
			sb.AppendLine("			} ");
			sb.AppendLine("		}");
			
			int UVCount = SMUVs.Count;
			sb.AppendLine("		LayerElementUV: 0 {");
			sb.AppendLine("			Version: 101");
			sb.AppendLine("			Name: \"map1\"");
			sb.AppendLine("			MappingInformationType: \"ByPolygonVertex\"");
			sb.AppendLine("			ReferenceInformationType: \"IndexToDirect\"");
			
			sb.AppendLine("			UV: *"+(UVCount*2)+" {");
			sb.Append("				a:");
			for(int i=0;i<UVCount;i++)
	        {
				if(i>0)
					 sb.Append(",");
				Vector2 smuv = SMUVs[i];
	            sb.Append(string.Format("{0},{1}",smuv.x,smuv.y));
	        }
			sb.AppendLine("");
			sb.AppendLine("			} ");
			
			sb.AppendLine("			UVIndex: *"+triCount+" {");
			sb.Append("				a:");
			for(int i=0;i<triCount;i+=3)
	        {
				if(i>0)
					 sb.Append(",");
				
				int a = SMTris[i];
				int b = SMTris[i+2];
				int c = SMTris[i+1];
				//rewind triangles because of x flip
				
				sb.Append(string.Format("{0},{1},{2}",a,b,c));//
			}
			sb.AppendLine("");
			
			sb.AppendLine("			} ");
			sb.AppendLine("		} ");
			
			sb.AppendLine("		LayerElementMaterial: 0 {");
			sb.AppendLine("			Version: 101");
			sb.AppendLine("			Name: \"\"");
			sb.AppendLine("			MappingInformationType: \"AllSame\"");
			sb.AppendLine("			ReferenceInformationType: \"IndexToDirect\"");
			sb.AppendLine("			Materials: *1 {");
			sb.AppendLine("				a: 0");
			sb.AppendLine("			} ");
			sb.AppendLine("		}");
			sb.AppendLine("		Layer: 0 {");
			sb.AppendLine("			Version: 100");
			sb.AppendLine("			LayerElement:  {");
			sb.AppendLine("				Type: \"LayerElementNormal\"");
			sb.AppendLine("				TypedIndex: 0");
			sb.AppendLine("			}");
			sb.AppendLine("			LayerElement:  {");
			sb.AppendLine("				Type: \"LayerElementMaterial\"");
			sb.AppendLine("				TypedIndex: 0");
			sb.AppendLine("			}");
			sb.AppendLine("			LayerElement:  {");
			sb.AppendLine("				Type: \"LayerElementUV\"");
			sb.AppendLine("				TypedIndex: 0");
			sb.AppendLine("			}");
			sb.AppendLine("		}");
			sb.AppendLine("	}");
		}
		
	//MODELS
		int count = 0;
		foreach( ExportMaterial eMat in exportTextures )
		{
			sb.AppendLine("	Model: "+(modelIdent+count)+", \"Model::"+targetName+":"+eMat.name+"\", \"Mesh\" {");
			sb.AppendLine("		Version: 232");
			sb.AppendLine("		Properties70:  {");
			sb.AppendLine("			P: \"RotationActive\", \"bool\", \"\", \"\",1");
			sb.AppendLine("			P: \"InheritType\", \"enum\", \"\", \"\",1");
			sb.AppendLine("			P: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("			P: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
//		    sb.AppendLine("			P: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",0.01,0.01,0.01");//Unity units default to meters
			if(!eMat.generated && eMat.material.HasProperty("_BumpMap"))
			{
				string bumpMapFalePath = AssetDatabase.GetAssetPath(eMat.material.GetTexture("_BumpMap"));
				sb.AppendLine("			P: \"NormalMap\", \"Enum\", \"A+U\",0, \""+bumpMapFalePath+"\"");
			}
			sb.AppendLine("		}");
			sb.AppendLine("		Shading: T");
			sb.AppendLine("		Culling: \"CullingOff\"");
			sb.AppendLine("	}");
			count++;
		}
		
	//MATERIALS
		count = 0;
		foreach( ExportMaterial eMat in exportTextures )
		{
			sb.AppendLine("	Material: "+(matieralIdent+count)+", \"Material::"+targetName+":"+eMat.name+"F\", \"\" {");
			sb.AppendLine("		Version: 102");
			sb.AppendLine("		ShadingModel: \"phong\"");
			sb.AppendLine("		Properties70:  {");
			if(eMat.generated)
			{
				sb.AppendLine("			P: \"DiffuseColor\", \"ColorRGB\", \"Color\", \" \",1,1,1");
				sb.AppendLine("			P: \"Diffuse\", \"Vector3D\", \"Vector\", \"\",1,1,1");
			}else{
				Color diffCol = eMat.material.color;
				sb.AppendLine("			P: \"Diffuse\", \"Vector3D\", \"Vector\", \"\","+diffCol.r+","+diffCol.g+","+diffCol.b);
				sb.AppendLine("			P: \"DiffuseColor\", \"ColorRGB\", \"Color\", \" \","+diffCol.r+","+diffCol.g+","+diffCol.b);
				
				if(eMat.material.HasProperty("_SpecColor"))
				{
					Color specCol = eMat.material.GetColor("_SpecColor");
					sb.AppendLine("			P: \"Specular\", \"Vector3D\", \"Vector\", \"\","+specCol.r+","+specCol.g+","+specCol.b);
					sb.AppendLine("			P: \"SpecularColor\", \"ColorRGB\", \"Color\", \" \","+specCol.r+","+specCol.g+","+specCol.b);
				}
				
				if(eMat.material.HasProperty("_Shininess"))
				{
					sb.AppendLine("			P: \"Shininess\", \"double\", \"Number\", \"\","+eMat.material.GetFloat("_Shininess"));
				}
			}
			sb.AppendLine("		}");
			sb.AppendLine("	}");
			count++;
		}
		
	//TEXTURES
	//If selected - export the textures to the export folder
	//Add the textures to the 
		count = 0;
		foreach( ExportMaterial eMat in exportTextures )
		{
			string destinationFile = eMat.filepath;
			string texturePath = destinationFile;
			if(_copyTextures)
			{
				int stripIndex = destinationFile.LastIndexOf('/');
   				if (stripIndex >= 0)
        			destinationFile = destinationFile.Substring(stripIndex + 1).Trim();
        		texturePath = destinationFile;//relative file path when using textures in the export folder
				string folder = targetFolder;
        		destinationFile = folder + destinationFile;
				if(!eMat.generated)
				{
					try
					{
						//Copy the source file
						//Debug.Log("Copying texture from " + kvp.Value.filepath + " to " + destinationFile);
						File.Copy(eMat.filepath, destinationFile, true);
					}
					catch
					{
						Debug.LogWarning("Could not copy texture! "+eMat.name);
					}
				}
			}
			
			sb.AppendLine("	Texture: "+(textureIdent+count)+", \"Texture::"+targetName+":"+eMat.name+"2F\", \"\" {");
			sb.AppendLine("		Type: \"TextureVideoClip\"");
			sb.AppendLine("		Version: 202");
			sb.AppendLine("		TextureName: \"Texture::"+targetName+":"+eMat.name+"2F\"");
			sb.AppendLine("		FileName: \""+destinationFile+"\"");
			sb.AppendLine("		RelativeFilename: \""+texturePath+"\"");
			sb.AppendLine("	}");
			count++;
		}
		sb.AppendLine("}");
		
	//CONNECTIONS
	//This part defines how all the data objects connect to make the model
		sb.AppendLine("; Object connections");
		sb.AppendLine(";------------------------------------------------------------------");
		
		sb.AppendLine("Connections:  {");
		
		int conCount = 0;
		foreach( ExportMaterial eMat in exportTextures )
		{
			sb.AppendLine("	;Model::"+targetName+":"+eMat.name+", Model::RootNode");
			sb.AppendLine("	C: \"OO\","+(modelIdent+conCount)+",0");
			sb.AppendLine("	");
			
			sb.AppendLine("	;Texture::"+targetName+":"+eMat.name+"2F, Material::"+targetName+":"+eMat.name+"2");
			sb.AppendLine("	C: \"OP\","+(textureIdent+conCount)+","+(matieralIdent+conCount)+", \"DiffuseColor\"");
			sb.AppendLine("	");
			
			sb.AppendLine("	;Geometry::, Model::"+targetName+":"+eMat.name+"");
			sb.AppendLine("	C: \"OO\","+(geomIdent+conCount)+","+(modelIdent+conCount)+"");
			sb.AppendLine("	");
			
			sb.AppendLine("	;Material::"+targetName+":"+eMat.name+"2, Model::"+targetName+":"+eMat.name+"");
			sb.AppendLine("	C: \"OO\","+(matieralIdent+conCount)+","+(modelIdent+conCount)+"");
			sb.AppendLine("	");
			conCount++;
		}
		sb.AppendLine("}");
 
        return sb.ToString();
    }
 
	private static bool CreateTargetFolder()
	{
		string newDirectory = targetFolder;
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