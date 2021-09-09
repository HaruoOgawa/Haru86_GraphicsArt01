using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphicsArt.GPUFlower.GPUFlower_Base;
using GraphicsArt.GPUFlower.GPUFlower_Flowers;

public class B_Spline_Curve_Debugger : MonoBehaviour
{
    [SerializeField] BSplineData bSplineData;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Material petal_mat;
    
    void Start()
    {
       // RenderTestPetal();
       RenderTestBSplineCurve();
    }

    void Update()
    {
       // RenderTestPetal();
       RenderTestBSplineCurve();
    }

    void RenderTestPetal(){
        GPUFlower_Base.BaseFlower_Data data=new GPUFlower_Base.BaseFlower_Data();
        data=GPUFlower_Base.Cal_BSpline_Surface(bSplineData.controlPoints,bSplineData.knotMin,bSplineData.knotMax,bSplineData.tWidth);


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

    void RenderTestBSplineCurve(){
        List<GPUFlower_Base.B_Spline_Data> data=new List<GPUFlower_Base.B_Spline_Data>();
        data=GPUFlower_Base.Cal_BSplineCurve(bSplineData.controlPoints,bSplineData.knotMin,bSplineData.knotMax,bSplineData.tWidth);

        Mesh testSplineMesh=new Mesh();
        List<Vector3> vertices=new List<Vector3>();
        List<int> indives=new List<int>();
        for(int i=0;i<data.Count;i++){
            vertices.Add(data[i].position);

            indives.Add(data[i].index);
            if(i<data.Count-1){
                indives.Add(data[i+1].index);
            }
        }

        testSplineMesh.vertices=vertices.ToArray();
        testSplineMesh.SetIndices(indives.ToArray(),MeshTopology.Lines,0);

        meshFilter.mesh=testSplineMesh;

    }

}
