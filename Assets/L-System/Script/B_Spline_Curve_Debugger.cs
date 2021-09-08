using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphicsArt.GPUFlower.GPUFlower_Base;
using GraphicsArt.GPUFlower.GPUFlower_Flowers;

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

        GPUFlower_Flowers.Multi_Flower_Data multi_Flower_Data=new GPUFlower_Flowers.Multi_Flower_Data();
        multi_Flower_Data=GPUFlower_Flowers.RenderMultiFlower(data,new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0));

        Mesh petalMesh=new Mesh();
        petalMesh.vertices=multi_Flower_Data.vertices.ToArray();
        petalMesh.triangles=multi_Flower_Data.triangles.ToArray();
        petalMesh.normals=multi_Flower_Data.normals.ToArray();
        petalMesh.RecalculateNormals();

        meshFilter.mesh=petalMesh;
        meshRenderer.material=petal_mat;

       
    }

}
