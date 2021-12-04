using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MyScriptable/Create StemDataScriptable")]
public class StemDataScriptable : ScriptableObject
{
    public List<Vector3> baseControlPoints;
    public int ySegment;
    public int circleSegment=6;
    public bool isLSystem=false;
}
