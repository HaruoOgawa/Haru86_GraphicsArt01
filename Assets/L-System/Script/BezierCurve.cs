using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    MeshFilter filter;
    MeshRenderer render;
    [SerializeField] Material material;
    [SerializeField] int mesh_segment=10;
    [SerializeField] Vector3[] bezierPoints;

    void Awake(){
        filter=this.gameObject.GetComponent<MeshFilter>();
        render=this.gameObject.GetComponent<MeshRenderer>();
        render.sharedMaterial=material;
    }
    void Start()
    {
        RenderBezierCurve();
    }

    void Update()
    {
        RenderBezierCurve();
    }

    void RenderBezierCurve(){
        Mesh bezierCurveMesh=new Mesh();
        
        List<Vector3> mesh_vertices=new List<Vector3>();
        List<int> mesh_indices=new List<int>();
         List<int> mesh_triangles=new List<int>();
        // List<Vector3> mesh_uv=new List<Vector3>();

        for(int i =0;i<mesh_segment;i++){
            float t=(float)i/(float)(mesh_segment-1);
           
            Vector3 p0=Vector3.Lerp(bezierPoints[0],bezierPoints[1],t);
            Vector3 p1=Vector3.Lerp(bezierPoints[1],bezierPoints[2],t);
            Vector3 p2=Vector3.Lerp(bezierPoints[2],bezierPoints[3],t);

            Vector3 q0=Vector3.Lerp(p0,p1,t);
            Vector3 q1=Vector3.Lerp(p1,p2,t);

            Vector3 h=Vector3.Lerp(q0,q1,t);

            mesh_vertices.Add(h);
           
        }

        for(int i=0;i<mesh_segment-2;i++){
            mesh_triangles.Add(0);
            mesh_triangles.Add(i+1);
            mesh_triangles.Add(i+2);
        }

        bezierCurveMesh.vertices=mesh_vertices.ToArray();
        bezierCurveMesh.triangles=mesh_triangles.ToArray();
        filter.sharedMesh=bezierCurveMesh;
    }
}




//       Mesh bezierCurveMesh=new Mesh();
        
    //     List<Vector3> mesh_vertices=new List<Vector3>();
    //     List<int> mesh_indices=new List<int>();
    //     // List<Vector3> mesh_triangles=new List<Vector3>();
    //     // List<Vector3> mesh_uv=new List<Vector3>();

    //     int indice=0;

    //     for(int i =0;i<mesh_segment;i++){
    //         float t=(float)i/(float)(mesh_segment-1);
            
    //        // Debug.Log("indice:"+indice+"/"+"t:"+t);
    //         //Debug.Log("t:"+t);
           
    //         Vector3 p0=Vector3.Lerp(bezierPoints[0],bezierPoints[1],t);
    //         Vector3 p1=Vector3.Lerp(bezierPoints[1],bezierPoints[2],t);
    //         Vector3 p2=Vector3.Lerp(bezierPoints[2],bezierPoints[3],t);

    //         Vector3 q0=Vector3.Lerp(p0,p1,t);
    //         Vector3 q1=Vector3.Lerp(p1,p2,t);

    //         Vector3 h=Vector3.Lerp(q0,q1,t);

    //         mesh_vertices.Add(h);
    //         mesh_indices.Add(indice);
    //         if(i<mesh_segment-1){
    //             mesh_indices.Add(indice+1);
    //         }
    //         indice++;
    //     }


    //     bezierCurveMesh.vertices=mesh_vertices.ToArray();
    //     bezierCurveMesh.SetIndices(mesh_indices.ToArray(),MeshTopology.Lines,0);
    //     filter.sharedMesh=bezierCurveMesh;