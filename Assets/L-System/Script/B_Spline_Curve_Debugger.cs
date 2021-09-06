using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphicsArt.GPUFlower.GPUFlower_Base;

public class B_Spline_Curve_Debugger : MonoBehaviour
{
    [SerializeField] PetalData petalData;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Material petal_mat;
    
    void Start()
    {
        RenderTestPetal();
    }

    void Update()
    {
        RenderTestPetal();
    }

    void RenderTestPetal(){
        GPUFlower_Base.BaseFlower_Data data=new GPUFlower_Base.BaseFlower_Data();
        data=GPUFlower_Base.Cal_BSpline_Surface(petalData.controlPoints,petalData.knotMin,petalData.knotMax,petalData.tWidth);

        Mesh petalMesh=new Mesh();
        petalMesh.vertices=data.vertices.ToArray();
        petalMesh.triangles=data.triangles.ToArray();
        petalMesh.normals=data.normals.ToArray();
        petalMesh.RecalculateNormals();

        meshFilter.mesh=petalMesh;
        meshRenderer.material=petal_mat;
    }

}
