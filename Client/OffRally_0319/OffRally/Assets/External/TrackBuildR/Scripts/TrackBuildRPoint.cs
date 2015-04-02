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

/// <summary>
/// Track BuildR Point holds the data concerning this point. The track is made up of a Bezier curve that is defined by these points.
/// It also holds a few precalculations to speed up track generation
/// </summary>

public class TrackBuildRPoint : MonoBehaviour
{
    public string pointName = "point";

    public Transform baseTransform;
    [SerializeField]
    private Vector3 _position;

    [SerializeField]
    private float _width = 15;//FIA Starting Grid regulation numberOfPoints
//    [SerializeField]
//    private float _cant = 0;//track tilt
    [SerializeField]
    private float _crownAngle = 0;//track camber
//    public TrackBuildRCurve curveA;
//    public TrackBuildRCurve curveB;

    //Bezier Control Points
    [SerializeField]
    private bool _splitControlPoints = false;
    [SerializeField]
    private Vector3 _forwardControlPoint;
    [SerializeField]
    private Vector3 _backwardControlPoint;

    //Boundary control points for borders
    [SerializeField]
    private Vector3 _leftTrackBoundary = Vector3.zero;
    [SerializeField]
    private Vector3 _leftForwardControlPoint;
    [SerializeField]
    private Vector3 _leftBackwardControlPoint;
    [SerializeField]
    private bool _leftSplitControlPoints = false;

    [SerializeField]
    private Vector3 _rightTrackBoundary = Vector3.zero;
    [SerializeField]
    private Vector3 _rightForwardControlPoint;
    [SerializeField]
    private Vector3 _rightBackwardControlPoint;
    [SerializeField]
    private bool _rightSplitControlPoints = false;

    //Internal stored calculations
    [SerializeField]
    private Vector3 _trackDirection = Vector3.forward;
    [SerializeField]
    private Quaternion _trackUpQ = Quaternion.LookRotation(Vector3.up);
    [SerializeField]
    private Vector3 _trackCross = Vector3.right;

    [SerializeField]
    private bool _isDirty = true;
    public bool shouldReRender = true;

    //Curve Variables
    public TrackBuildRPoint lastPoint;
    public TrackBuildRPoint nextPoint;

    public Vector3 center = Vector3.zero;
    public int storedPointSize = 0;
    public float arcLength = 0;
    [SerializeField]
    private bool _render = true;
    [SerializeField]
    private bool _renderBounds = true;
    [SerializeField]
    private bool _trackCollider = true;

    private bool _colliderSides = true;

    [SerializeField]
    private bool _extrudeTrack = false;

    [SerializeField]
    private float _extrudeLength = 1.5f;

    [SerializeField]
    private bool _extrudeTrackBottom = false;

    [SerializeField]
    private bool _extrudeCurveEnd = false;

    [SerializeField]
    private float _extrudeBevel = 1;

    [SerializeField]
    private bool _generateBumpers = true;

    public float[] storedArcLengths = null;
    public float[] storedArcLengthsFull = null;
    public int storedArcLengthArraySize = 750;
    public float[] normalisedT;
    [SerializeField]
    private float _boundaryHeight = 1.0f;
    /// ////
    public float[] midPointPerc;
    public float[] targetSize;
    public int[] prevNormIndex;
    public int[] nextNormIndex;

    public Vector3[] sampledPoints;
    public float[] sampledWidths;
    public float[] sampledCrowns;
    public Vector3[] sampledLeftBoundaryPoints;
    public Vector3[] sampledRightBoundaryPoints;
    public Vector3[] sampledTrackDirections;
    public Vector3[] sampledTrackUps;
    public Vector3[] sampledTrackCrosses;
    public float[] sampledAngles;
    public bool[] clipArrayLeft;
    public bool[] clipArrayRight;

    public GameObject holder = null;
    public DynamicMeshGenericMultiMaterialMesh dynamicTrackMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicBoundaryMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicOffroadMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicBumperMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicColliderMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicBottomMesh = new DynamicMeshGenericMultiMaterialMesh();

    //texture values
    public int trackTextureStyleIndex = 0;
    public int offroadTextureStyleIndex = 1;
    public int boundaryTextureStyleIndex = 2;
    public int bumperTextureStyleIndex = 3;
    public int bottomTextureStyleIndex = 0;

    public void Reset()
    {
        hideFlags = HideFlags.HideInInspector;
    }

    public Vector3 position
    {
        get
        {
            return baseTransform.rotation * _position;
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            if (_position != newValue)
                _isDirty = true;
            _position = newValue;
        }
    }
    public Vector3 worldPosition
    {
        get
        {
            return baseTransform.rotation * _position + baseTransform.position;
        }
        set
        {
            Vector3 newValue = value - baseTransform.position;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            if (_position != newValue)
            {
                _isDirty = true;
                _position = newValue;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 forwardControlPoint
    {
        get
        {
            return baseTransform.rotation * (_forwardControlPoint + _position);
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -_position;
            if (_forwardControlPoint != newValue)
            {
                _isDirty = true;
                _forwardControlPoint = newValue;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 backwardControlPoint
    {
        get
        {
            Vector3 controlPoint = (_splitControlPoints) ? _backwardControlPoint : -_forwardControlPoint;
            return baseTransform.rotation * (controlPoint + _position);
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -_position;
            if (_splitControlPoints)
            {
                if (backwardControlPoint != newValue)
                {
                    _isDirty = true;
                    _backwardControlPoint = newValue;
                    RecalculateStoredValues();
                }
            }
            else
            {
                if (_forwardControlPoint != -newValue)
                {
                    _isDirty = true;
                    _forwardControlPoint = -newValue;
                    RecalculateStoredValues();
                }
            }
        }
    }

    public bool splitControlPoints
    {
        get { return _splitControlPoints; }
        set
        {
            if (value != _splitControlPoints)
                _backwardControlPoint = -_forwardControlPoint;
            if (splitControlPoints != value)
            {
                _isDirty = true;
                _splitControlPoints = value;
                RecalculateStoredValues();
            }
        }
    }

    //LEFT BOUNDARY
    public Vector3 leftTrackBoundary
    {
        get
        {
            return baseTransform.rotation * (_leftTrackBoundary + _position - (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position - (trackCross * _width / 2));
            if (leftTrackBoundary != newValue)
            {
                _isDirty = true;
                _leftTrackBoundary = newValue;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 leftTrackBoundaryWorld
    {
        get
        {
            return baseTransform.rotation * (_leftTrackBoundary + _position - (trackCross * _width / 2) + baseTransform.position);
        }
        set
        {
            Vector3 newValue = value - baseTransform.position;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position - (trackCross * _width / 2));
            if (leftTrackBoundary != newValue)
            {
                _isDirty = true;
                _leftTrackBoundary = newValue;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 leftForwardControlPoint
    {
        get
        {
            return baseTransform.rotation * (_leftForwardControlPoint + _position + _leftTrackBoundary - (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + _leftTrackBoundary - (trackCross * _width / 2));
            if (_leftForwardControlPoint != newValue)
            {
                _isDirty = true;
                _leftForwardControlPoint = newValue;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 leftBackwardControlPoint
    {
        get
        {
            Vector3 controlPoint = (_leftSplitControlPoints) ? _leftBackwardControlPoint : -_leftForwardControlPoint;
            return baseTransform.rotation * (controlPoint + _position + _leftTrackBoundary - (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + _leftTrackBoundary - (trackCross * _width / 2));
            if (_leftSplitControlPoints)
            {
                if (_leftBackwardControlPoint != newValue)
                {
                    _isDirty = true;
                    _leftBackwardControlPoint = newValue;
                    RecalculateStoredValues();
                }
            }
            else
            {
                if (_leftForwardControlPoint != -newValue)
                {
                    _isDirty = true;
                    _leftForwardControlPoint = -newValue;
                    RecalculateStoredValues();
                }
            }
        }
    }

    public bool leftSplitControlPoints
    {
        get { return _leftSplitControlPoints; }
        set
        {
            if (value != _leftSplitControlPoints)
                _leftBackwardControlPoint = -_leftForwardControlPoint;
            if(_leftSplitControlPoints!=value)
            {
                _isDirty = true;
                _leftSplitControlPoints = value;
                RecalculateStoredValues();
            }
        }
    }


    //RIGHT BOUNDARY
    public Vector3 rightTrackBoundary
    {
        get
        {
            return baseTransform.rotation * (_rightTrackBoundary + _position + (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + (trackCross * _width / 2));
            if(_rightTrackBoundary!=newValue)
            {
                _isDirty = true;
                _rightTrackBoundary = newValue;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 rightTrackBoundaryWorld
    {
        get
        {
            return baseTransform.rotation * (_rightTrackBoundary + _position + (trackCross * _width / 2) + baseTransform.position);
        }
        set
        {
            Vector3 newValue = value - baseTransform.position;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + (trackCross * _width / 2));
            if (leftTrackBoundary != newValue)
            {
                _isDirty = true;
                _rightTrackBoundary = newValue;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 rightForwardControlPoint
    {
        get
        {
            return baseTransform.rotation * (_rightForwardControlPoint + _position + _rightTrackBoundary + (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + _rightTrackBoundary + (trackCross * _width / 2));
            if(_rightForwardControlPoint!=newValue)
            {
                _isDirty = true;
                _rightForwardControlPoint = newValue;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 rightBackwardControlPoint
    {
        get
        {
            Vector3 controlPoint = (_rightSplitControlPoints) ? _rightBackwardControlPoint : -_rightForwardControlPoint;
            return baseTransform.rotation * (controlPoint + _position + _rightTrackBoundary + (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + _rightTrackBoundary + (trackCross * _width / 2));
            if (_rightSplitControlPoints)
            {
                if(_rightBackwardControlPoint!=newValue)
                {
                    _isDirty = true;
                    _rightBackwardControlPoint = newValue;
                    RecalculateStoredValues();
                }
            }
            else
            {  if(_rightForwardControlPoint!=-newValue)
            {
                _isDirty = true;
                _rightForwardControlPoint = -newValue;
                RecalculateStoredValues();
            }}
        }
    }

    public bool rightSplitControlPoints
    {
        get { return _rightSplitControlPoints; }
        set
        {
            if(value != _rightSplitControlPoints)
            {
                _rightBackwardControlPoint = -_rightForwardControlPoint;
                RecalculateStoredValues();
            }
            if(_rightSplitControlPoints!=value)
            {
                _isDirty = true;
                _rightSplitControlPoints = value;
                RecalculateStoredValues();
            }
        }
    }

    public Vector3 trackDirection
    {
        get
        {
            return _trackDirection;
        }
//        set
//        {
//            if (value == Vector3.zero)
//                return;
//            _trackDirection = value.normalized;
//            Vector3 trackUpV = _trackUpQ * Vector3.forward;
//            _trackUpQ = Quaternion.LookRotation(trackUpV, _trackDirection);
//            _trackCross = Vector3.Cross(trackUpV, _trackDirection).normalized;
//        }
    }

    public float width
    {
        get
        {
            return _width;
        }
        set
        {
            if (_width != value)
                _isDirty = true;
            _width = value;
        }
    }
//
//    public float cant
//    {
//        get { return _cant; }
//        set
//        {
//            if(_cant != value)
//            {
//                isDirty = true;
//                _cant = value;
//                RecalculateTrackUpQ();
//                _trackCross = Vector3.Cross(trackUpQ * Vector3.forward, _trackDirection).normalized;
////                _cant = SplineMaths.ClampAngle(value);
//            }
////            Debug.Log("cant cahnged "+value);
//        }
//    }

    public float crownAngle
    {
        get { return _crownAngle; }
        set
        {
            if (_crownAngle != value)
            {
                _isDirty = true;
                _crownAngle = value;
            }
        }
    }

    public Vector3 trackUp
    {
        get { return _trackUpQ * Vector3.forward; }
    }

    public Quaternion trackUpQ
    {
        get
        {
            return _trackUpQ;
        }
        set
        {
            if(_trackUpQ != value)
            {

                _trackUpQ = value;
                RecalculateStoredValues();
//                Vector3 trackUpV = value * Vector3.forward;
//                _trackUpQ = Quaternion.LookRotation(trackUpV, _trackDirection);
//                _trackCross = Vector3.Cross(trackUpV, _trackDirection).normalized;
                _isDirty = true;
            }

        }
    }

    public Vector3 trackCross {get {return _trackCross;}}

    public void MatchBoundaryValues()
    {
        leftForwardControlPoint = forwardControlPoint+(leftTrackBoundary-worldPosition);
        rightForwardControlPoint = forwardControlPoint+(rightTrackBoundary-worldPosition);
    }

    public void RecalculateStoredValues()
    {
        _trackDirection = (_forwardControlPoint.normalized - _backwardControlPoint.normalized).normalized;
        if(_trackDirection == Vector3.zero)
            _trackDirection = (nextPoint.worldPosition - worldPosition).normalized;
        Vector3 trackUpV = _trackUpQ * Vector3.forward;
        _trackUpQ = Quaternion.LookRotation(trackUpV, _trackDirection);
        _trackCross = Vector3.Cross(trackUpV, _trackDirection).normalized;
        isDirty = true;
    }

//    public void RecalculateTrackUpQ()
//    {
//        float cantRad = _cant * Mathf.Deg2Rad;
//        _trackUpQ = Quaternion.LookRotation(Quaternion.LookRotation(_trackDirection) * new Vector3(Mathf.Sin(cantRad), Mathf.Cos(cantRad), 0).normalized);
//    }

    //Curve methods
    public bool curveIsDirty
    {
        get
        {
            if (nextPoint == null) return false;
            return _isDirty || nextPoint._isDirty;
        }
    }

    public bool curveShouldReRender
    {
        get
        {
            if (nextPoint == null) return false;
            return shouldReRender || nextPoint.shouldReRender;
        }
    }

    public bool render
    {
        get { return _render; }
        set
        {
            if (_render != value)
            {
                _render = value;
                SetReRender();
            }
        }
    }

    public bool renderBounds
    {
        get { return _renderBounds; }
        set
        {
            if (_renderBounds != value)
            {
                _renderBounds = value;
                SetReRender();
            }
        }
    }

    public bool trackCollider
    {
        get { return _trackCollider; }
        set
        {
            if (_trackCollider != value)
            {
                _trackCollider = value;
                SetReRender();
            }
        }
    }

    public bool extrudeTrack
    {
        get
        { return _extrudeTrack; }
        set
        {
            if (_extrudeTrack != value)
            {
                _extrudeTrack = value;
                SetReRender();
            }
        }
    }

    public float extrudeLength
    {
        get
        { return _extrudeLength; }
        set
        {
            _extrudeLength = value;
            SetReRender();
        }
    }

    public bool extrudeTrackBottom
    {
        get
        { return _extrudeTrackBottom; }
        set
        {
            if (_extrudeTrackBottom != value)
            {
                _extrudeTrackBottom = value;
                SetReRender();
            }
        }
    }


    public bool extrudeCurveEnd
    {
        get
        { return _extrudeCurveEnd; }
        set
        {
            if (_extrudeCurveEnd != value)
            {
                _extrudeCurveEnd = value;
                SetReRender();
            }
        }
    }

    public float extrudeBevel
    {
        get
        { return _extrudeBevel; }
        set
        {
            _extrudeBevel = Mathf.Clamp(value, 0, 2);
            SetReRender();
        }
    }

    public bool generateBumpers
    {
        get { return _generateBumpers; }
        set
        {
            _generateBumpers = value;
            SetReRender();
        }
    }

    public bool isDirty
    {
        get {return _isDirty;} 
        set
        {
            if(_isDirty != value)
            {
//                if(value)
//                    Debug.Log("isDirty "+pointName);
                _isDirty = value;
            }
        }
    }

    public float boundaryHeight
    {
        get {return _boundaryHeight;} 
        set
        {
            if(_boundaryHeight != value)
            {
                _boundaryHeight = value;
                SetReRender();
            }
        }
    }

    public bool colliderSides
    {
        get { return _colliderSides; }
        set
        {
            if (_colliderSides != value)
            {
                _colliderSides = value;
                SetReRender();
            }
        }
    }

    public void SetReRender()
    {
        shouldReRender = true;
        if(nextPoint)
            nextPoint.shouldReRender = true;
    }


    public virtual string ToXML()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<trackpoint>");
            sb.AppendLine("<pointName>" + pointName + "</pointName>");
            sb.AppendLine("<baseTransform>" + baseTransform.name + "</baseTransform>");
        sb.AppendLine(XMLVariableConverter.ToXML(_position, "position"));//  "<RENAME>" + _position + "<RENAME>");
        sb.AppendLine("<_width>" + _width + "</_width>");
        sb.AppendLine("<_crownAngle>" + _crownAngle + "</_crownAngle>");
        sb.AppendLine("<_splitControlPoints>" + _splitControlPoints + "</_splitControlPoints>");
        sb.AppendLine(XMLVariableConverter.ToXML(_forwardControlPoint, "_forwardControlPoint"));// + _forwardControlPoint + "<RENAME>");
        sb.AppendLine(XMLVariableConverter.ToXML(_backwardControlPoint, "_backwardControlPoint"));// + _backwardControlPoint + "<RENAME>");
        sb.AppendLine(XMLVariableConverter.ToXML(_leftTrackBoundary, "_leftTrackBoundary"));// _leftTrackBoundary + "<RENAME>");
        sb.AppendLine(XMLVariableConverter.ToXML(_leftForwardControlPoint, "_leftForwardControlPoint"));// _leftForwardControlPoint + "<RENAME>");
        sb.AppendLine(XMLVariableConverter.ToXML(_leftBackwardControlPoint, "_leftBackwardControlPoint"));// _leftBackwardControlPoint + "<RENAME>");
        sb.AppendLine("<_leftSplitControlPoints>" + _leftSplitControlPoints + "</_leftSplitControlPoints>");
        sb.AppendLine(XMLVariableConverter.ToXML(_rightTrackBoundary, "_rightTrackBoundary"));// _rightTrackBoundary + "<RENAME>");
        sb.AppendLine(XMLVariableConverter.ToXML(_rightForwardControlPoint, "_rightForwardControlPoint"));// _rightForwardControlPoint + "<RENAME>");
        sb.AppendLine(XMLVariableConverter.ToXML(_rightBackwardControlPoint, "_rightBackwardControlPoint"));// _rightBackwardControlPoint + "<RENAME>");
        sb.AppendLine("<_rightSplitControlPoints>" + _rightSplitControlPoints + "</_rightSplitControlPoints>");
        sb.AppendLine(XMLVariableConverter.ToXML(_trackDirection, "_trackDirection"));// _trackDirection + "<RENAME>");
        sb.AppendLine(XMLVariableConverter.ToXML(_trackUpQ, "_trackUpQ"));//+ _trackUpQ + "<RENAME>");
        sb.AppendLine(XMLVariableConverter.ToXML(_trackCross, "_trackCross"));// _trackCross + "<RENAME>");
        sb.AppendLine("<shouldReRender>" + shouldReRender + "</shouldReRender>");
            //Curve Variables
        sb.AppendLine("<lastPoint>" + lastPoint.pointName + "</lastPoint>");
        sb.AppendLine("<nextPoint>" + nextPoint.pointName + "</nextPoint>");
        sb.AppendLine("<arcLength>" + arcLength + "</arcLength>");
        sb.AppendLine("<_render>" + _render + "</_render>");
        sb.AppendLine("<_renderBounds>" + _renderBounds + "</_renderBounds>");
        sb.AppendLine("<_trackCollider>" + _trackCollider + "</_trackCollider>");
        sb.AppendLine("<_extrudeTrack>" + _extrudeTrack + "</_extrudeTrack>");
        sb.AppendLine("<_extrudeLength>" + _extrudeLength + "</_extrudeLength>");
        sb.AppendLine("<_extrudeTrackBottom>" + _extrudeTrackBottom + "</_extrudeTrackBottom>");
        sb.AppendLine("<_extrudeCurveEnd>" + _extrudeCurveEnd + "</_extrudeCurveEnd>");
        sb.AppendLine("<_extrudeBevel>" + _extrudeBevel + "</_extrudeBevel>");
        sb.AppendLine("<generateBumpers>" + generateBumpers + "</generateBumpers>");
        sb.AppendLine("<boundaryHeight>" + _boundaryHeight + "</boundaryHeight>");
        sb.AppendLine("<trackTextureStyleIndex>" + trackTextureStyleIndex + "</trackTextureStyleIndex>");
        sb.AppendLine("<offroadTextureStyleIndex>" + offroadTextureStyleIndex + "</offroadTextureStyleIndex>");
        sb.AppendLine("<boundaryTextureStyleIndex>" + boundaryTextureStyleIndex + "</boundaryTextureStyleIndex>");
        sb.AppendLine("<bumperTextureStyleIndex>" + bumperTextureStyleIndex + "</bumperTextureStyleIndex>");
        sb.AppendLine("<bottomTextureStyleIndex>" + bottomTextureStyleIndex + "</bottomTextureStyleIndex>");
        sb.AppendLine("</trackpoint>");
        return sb.ToString();
    }

    public virtual void FromXML(XmlNode node)
    {
        if (node["pointName"] != null)
            pointName = node["pointName"].FirstChild.Value;
//        baseTransform = GameObject.Find(node["baseTransform"].FirstChild.Value).transform;
        _position = XMLVariableConverter.FromXMLVector3(node["position"]);
        _width = float.Parse(node["_width"].FirstChild.Value);
        _crownAngle = float.Parse(node["_crownAngle"].FirstChild.Value);
        _splitControlPoints = bool.Parse(node["_splitControlPoints"].FirstChild.Value);
        _forwardControlPoint = XMLVariableConverter.FromXMLVector3(node["_forwardControlPoint"]);
        _backwardControlPoint = XMLVariableConverter.FromXMLVector3(node["_backwardControlPoint"]);
        _leftTrackBoundary = XMLVariableConverter.FromXMLVector3(node["_leftTrackBoundary"]);
        _leftForwardControlPoint = XMLVariableConverter.FromXMLVector3(node["_leftForwardControlPoint"]);
        _leftBackwardControlPoint = XMLVariableConverter.FromXMLVector3(node["_leftBackwardControlPoint"]);
        _leftSplitControlPoints = bool.Parse(node["_leftSplitControlPoints"].FirstChild.Value);
        _rightTrackBoundary = XMLVariableConverter.FromXMLVector3(node["_rightTrackBoundary"]);
        _rightForwardControlPoint = XMLVariableConverter.FromXMLVector3(node["_rightForwardControlPoint"]);
        _rightBackwardControlPoint = XMLVariableConverter.FromXMLVector3(node["_rightBackwardControlPoint"]);
        _leftSplitControlPoints = bool.Parse(node["_leftSplitControlPoints"].FirstChild.Value);
        _trackDirection = XMLVariableConverter.FromXMLVector3(node["_trackDirection"]);
        if (node["_trackUpQ"] != null)
            _trackUpQ = XMLVariableConverter.FromXMLQuaternion(node["_trackUpQ"]);
        _trackCross = XMLVariableConverter.FromXMLVector3(node["_trackCross"]);
        shouldReRender = bool.Parse(node["shouldReRender"].FirstChild.Value);
        //lastPoint and nextPoint are set in track class
        arcLength = float.Parse(node["arcLength"].FirstChild.Value);
        if(node["_render"] != null)
        {
            _render = bool.Parse(node["_render"].FirstChild.Value);
            _renderBounds = bool.Parse(node["_renderBounds"].FirstChild.Value);
            _trackCollider = bool.Parse(node["_trackCollider"].FirstChild.Value);
            _extrudeTrack = bool.Parse(node["_extrudeTrack"].FirstChild.Value);
            _extrudeLength = float.Parse(node["_extrudeLength"].FirstChild.Value);
            _extrudeTrackBottom = bool.Parse(node["_extrudeTrackBottom"].FirstChild.Value);
            _extrudeCurveEnd = bool.Parse(node["_extrudeCurveEnd"].FirstChild.Value);
            _extrudeBevel = float.Parse(node["_extrudeBevel"].FirstChild.Value);
            generateBumpers = bool.Parse(node["generateBumpers"].FirstChild.Value);
            _boundaryHeight = float.Parse(node["boundaryHeight"].FirstChild.Value);
        }
        trackTextureStyleIndex = int.Parse(node["trackTextureStyleIndex"].FirstChild.Value);
        offroadTextureStyleIndex = int.Parse(node["offroadTextureStyleIndex"].FirstChild.Value);
        boundaryTextureStyleIndex = int.Parse(node["boundaryTextureStyleIndex"].FirstChild.Value);
        bumperTextureStyleIndex = int.Parse(node["bumperTextureStyleIndex"].FirstChild.Value);
        if (node["bottomTextureStyleIndex"] != null) 
            bottomTextureStyleIndex = int.Parse(node["bottomTextureStyleIndex"].FirstChild.Value);
    }
}
