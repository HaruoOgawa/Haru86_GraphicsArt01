using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Spline_Curve : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter filter;
    [SerializeField] Material b_spline_mat;
    [SerializeField] int meshCount=10;
    [SerializeField] Vector3 p1=new Vector3(0,0,0);
    [SerializeField] Vector3 p2=new Vector3(0,0,0);
    [SerializeField] Vector3 p3=new Vector3(0,0,0);
    void Start()
    {
        Mesh bSpline_mesh=new Mesh();
        
        List<Vector3> bspline_points=new List<Vector3>();
        List<int> bspline_index=new List<int>();

        for(int i=0;i<meshCount;i++){
            Vector3 point=new Vector3(0,0,0);
            float t=(int)(i/meshCount-1);
            
            point=CalBSpline(p1,p2,p3,t);
            bspline_points.Add(point);
            bspline_index.Add(i);
        }

        bSpline_mesh.vertices=bspline_points.ToArray();
        bSpline_mesh.SetIndices(bspline_index.ToArray(),MeshTopology.Lines,0);

        meshRenderer.material=b_spline_mat;
        filter.mesh=bSpline_mesh;
    }

    void Update()
    {
        
    }

    Vector3 CalBSpline(Vector2 p1,Vector2 p2,Vector2 p3,float t){
        float x=BSplineX(p1.x,p2.x,p3.x,t);
        float y=BSplineY(p1.y,p2.y,p3.y,t);
        return new Vector3(x,y,0);
    }

    float BSplineX(float x1,float x2,float x3,float t){
        return Mathf.Pow((1-t),2)*x1+2.0f*(1-t)*x2+Mathf.Pow(t,2)*x3;
    }

    float BSplineY(float y1,float y2,float y3,float t){
         return Mathf.Pow((1-t),2)*y1+2.0f*(1-t)*y2+Mathf.Pow(t,2)*y3;
    }
}
