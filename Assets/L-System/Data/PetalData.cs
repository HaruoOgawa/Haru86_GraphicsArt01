using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MyScriptable/Create PetalData")]
public class PetalData : ScriptableObject
{
    public List<Vector3> controlPoints;
    public float knotMin;
    public float knotMax;
    public float tWidth;
}
