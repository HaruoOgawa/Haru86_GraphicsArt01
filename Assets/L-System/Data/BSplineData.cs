using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MyScriptable/Create B-Spline Curve Data")]
public class BSplineData : ScriptableObject
{
    public List<Vector3> controlPoints;
    public float knotMin;
    public float knotMax;
    public float tWidth;
}
