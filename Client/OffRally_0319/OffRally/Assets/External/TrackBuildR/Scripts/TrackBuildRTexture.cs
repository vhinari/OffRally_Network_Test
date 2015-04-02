// Track BuildR
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://support.jasperstocker.com
// For support contact email@jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Text;
using System.Xml;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TrackBuildRTexture : MonoBehaviour
{

    public enum Types
    {
        Basic,
        User,
        Substance
    }

    public string customName = "new trackTexture";
    public bool tiled = true;
    public bool patterned = false;
    public bool flipped = false;
    public Types type = Types.Basic;
    [SerializeField]
    private Vector2 _tileUnitUV = Vector2.one;//the UV coords of the end of nextNormIndex pattern in the trackTexture - used to match up textures to geometry
    [SerializeField]
    private Vector2 _textureUnitSize = Vector2.one;//the world numberOfPoints of the trackTexture - default 1m x 1m
    public int tiledX = 1;//the amount of times the trackTexture should be repeated along the x axis
    public int tiledY = 1;//the amount of times the trackTexture should be repeated along the y axis
    public Vector2 maxUVTile = Vector2.zero;//used for trackTexture atlasing
    public Vector2 minWorldUnits = Vector2.zero;//also used for atlasing
    public Vector2 maxWorldUnits = Vector2.zero;//also used for atlasing
    public Material material;
    [SerializeField]
    private ProceduralMaterial _proceduralMaterial;
    [SerializeField]
    private Material _userMaterial;

    public bool isDirty = false;

    void Reset()
    {
        hideFlags = HideFlags.HideInInspector;
        if(material==null)
            material = new Material(Shader.Find("Diffuse"));
    }


    public TrackBuildRTexture Duplicate()
    {
        return Duplicate(customName + " copy");
    }

    public TrackBuildRTexture Duplicate(string newName)
    {
        TrackBuildRTexture newTexture = gameObject.AddComponent<TrackBuildRTexture>();
        newTexture.customName = "Track Texture";

        newTexture.tiled = true;
        newTexture.patterned = false;
        newTexture.tileUnitUV = _tileUnitUV;
        newTexture.textureUnitSize = _textureUnitSize;
        newTexture.tiledX = tiledX;
        newTexture.tiledY = tiledY;
        newTexture.maxUVTile = maxUVTile;
        newTexture.material = new Material(material);
        newTexture.proceduralMaterial = _proceduralMaterial;
        newTexture.userMaterial = userMaterial;

        return newTexture;
    }

    public Texture texture
    {
        get
        {
            switch(type)
            {
                default:
                    if (material.mainTexture == null)
                        return null;
                    return material.mainTexture;

                case Types.Substance:
                    if (_proceduralMaterial == null)
                        return null;
                    if (_proceduralMaterial.mainTexture == null)
                        return null;
                    return _proceduralMaterial.mainTexture;

                case Types.User:
                    if (_userMaterial == null)
                        return null;
                    if (_userMaterial.mainTexture == null)
                        return null;
                    return _userMaterial.mainTexture;
            }
        }

        set
        {
            if (value == null)
                return;
            if(value != texture)
            {
                switch(type)
                {
                    case Types.Basic:
                        material.mainTexture = value;
                        break;

                    case Types.Substance:
                        _proceduralMaterial.mainTexture = value;
                        break;

                    case Types.User:
                        _userMaterial.mainTexture = value;
                        break;
                }
                isDirty = true;
            }
        }
    }

    public bool isSubstance
    {
        get
        {
            return type == Types.Substance && _proceduralMaterial != null;
        }
    }

    public bool isUSer
    {
        get
        {
            return type == Types.User && _userMaterial != null;
        }
    }

    public ProceduralMaterial proceduralMaterial
    {
        get { return _proceduralMaterial; }
        set
        {
            if(value != proceduralMaterial)
            {
                _proceduralMaterial = value;
                isDirty = true;
                if (value == null)
                    return;
                _proceduralMaterial.isReadable = true;
            }
        }
    }

    public Material userMaterial
    {
        get
        {
            return _userMaterial;
        } 
        set
        {
            if(value != userMaterial)
            {
                _userMaterial = value;
                isDirty = true;
            }
        }
    }

    public Vector2 tileUnitUV
    {
        get { return _tileUnitUV; }
        set { _tileUnitUV = value; }
    }

    public Vector2 textureUnitSize
    {
        get { return _textureUnitSize; }
        set { _textureUnitSize = value; }
    }

    public void CheckMaxUV(Vector2 checkUV)
    {
        if (checkUV.x > maxUVTile.x)
        {
            maxUVTile.x = checkUV.x;
        }
        if (checkUV.y > maxUVTile.y)
        {
            maxUVTile.y = checkUV.y;
        }
    }

    public void MaxWorldUnitsFromUVs(Vector2 uv)
    {
        float xsize = uv.x * _textureUnitSize.x;
        float ysize = uv.y * _textureUnitSize.y;
        if (xsize > maxWorldUnits.x)
        {
            maxWorldUnits.x = xsize;
        }
        if (ysize > maxWorldUnits.y)
        {
            maxWorldUnits.y = ysize;
        }
    }

    public Material GetMaterial()
    {
        switch(type)
        {
            default:
                return material;

            case Types.Substance:
                if(isSubstance)
                    return proceduralMaterial;
                else
                    return material;

            case Types.User:
                if (isUSer)
                    return userMaterial;
                else
                    return material;
        }
    }

#if UNITY_EDITOR
    public string filePath
    {
        get
        {
            switch(type)
            {
                default:
                    return AssetDatabase.GetAssetPath(texture);

                case Types.Substance:
                    if(isSubstance)
                        return AssetDatabase.GetAssetPath(proceduralMaterial);
                    else
                        return AssetDatabase.GetAssetPath(texture);

                case Types.User:
                    if (isUSer)
                        return AssetDatabase.GetAssetPath(userMaterial);
                    else
                        return AssetDatabase.GetAssetPath(texture);
            }
        }
    }

    public virtual string ToXML()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<texture>");
        sb.AppendLine("<customName>" + customName + "</customName>");
        sb.AppendLine("<tiled>" + tiled + "</tiled>");
        sb.AppendLine("<patterned>" + patterned + "</patterned>");
        sb.AppendLine("<flipped>" + flipped + "</flipped>");
        sb.AppendLine("<type>" + type + "</type>");
        sb.AppendLine(XMLVariableConverter.ToXML(_tileUnitUV, "_tileUnitUV"));
        sb.AppendLine(XMLVariableConverter.ToXML(_textureUnitSize, "_textureUnitSize"));
        sb.AppendLine("<tiledX>" + tiledX + "</tiledX>");
        sb.AppendLine("<tiledY>" + tiledY + "</tiledY>");
        sb.AppendLine(XMLVariableConverter.ToXML(maxUVTile, "maxUVTile"));
        sb.AppendLine(XMLVariableConverter.ToXML(minWorldUnits, "minWorldUnits"));
        sb.AppendLine(XMLVariableConverter.ToXML(maxWorldUnits, "maxWorldUnits"));
        sb.AppendLine("<_proceduralMaterial>" + AssetDatabase.GetAssetPath(_proceduralMaterial) + "</_proceduralMaterial>");
        sb.AppendLine("<_userMaterial>" + AssetDatabase.GetAssetPath(_userMaterial) + "</_userMaterial>");
        sb.AppendLine("<texture>" + AssetDatabase.GetAssetPath(material.mainTexture) + "</texture>");
        sb.AppendLine(XMLVariableConverter.ToXML(material.color, "color"));
        sb.AppendLine("</texture>");
        return sb.ToString();
    }

    public virtual void FromXML(XmlNode node)
    {
        material = new Material(Shader.Find("Diffuse"));
        if (node["customName"]!=null)
            customName = node["customName"].FirstChild.Value;
        tiled = bool.Parse(node["tiled"].FirstChild.Value);
        patterned = bool.Parse(node["patterned"].FirstChild.Value);
        flipped = bool.Parse(node["flipped"].FirstChild.Value);
        if (node["type"] != null)
            type = (Types)System.Enum.Parse(typeof(Types), node["type"].FirstChild.Value);
        _tileUnitUV = XMLVariableConverter.FromXMLVector2(node["_tileUnitUV"]);
        _textureUnitSize = XMLVariableConverter.FromXMLVector2(node["_textureUnitSize"]);
        tiledX = int.Parse(node["tiledX"].FirstChild.Value);
        tiledY = int.Parse(node["tiledY"].FirstChild.Value);
        maxUVTile = XMLVariableConverter.FromXMLVector2(node["maxUVTile"]);
        minWorldUnits = XMLVariableConverter.FromXMLVector2(node["minWorldUnits"]);
        maxWorldUnits = XMLVariableConverter.FromXMLVector2(node["maxWorldUnits"]);

        ProceduralMaterial loadedProcedualMaterial = null;
        if(node["_proceduralMaterial"] != null)
        {
            if(node["_proceduralMaterial"].HasChildNodes)
                loadedProcedualMaterial = (ProceduralMaterial)AssetDatabase.LoadAssetAtPath(node["_proceduralMaterial"].FirstChild.Value, typeof(ProceduralMaterial));
        }
        if (loadedProcedualMaterial != null)
            _proceduralMaterial = loadedProcedualMaterial;

        Material loadedMaterial = null;
        if (node["_userMaterial"] != null)
        {
            if (node["_userMaterial"].HasChildNodes)
                loadedMaterial = (Material)AssetDatabase.LoadAssetAtPath(node["_userMaterial"].FirstChild.Value, typeof(Material));
        }
        if (loadedMaterial != null)
            _userMaterial = loadedMaterial;


        if (node["texture"].HasChildNodes)
            material.mainTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(node["texture"].FirstChild.Value, typeof(Texture2D));

        if(node["color"].HasChildNodes)
            material.color = XMLVariableConverter.FromXMLtoColour(node["color"]);

    }
#endif
}
