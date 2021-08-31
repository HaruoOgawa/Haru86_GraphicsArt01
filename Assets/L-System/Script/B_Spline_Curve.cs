using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Spline_Curve : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter filter;
    [SerializeField] Material b_spline_mat;
    [SerializeField] int meshCount=10;
    
    [SerializeField] List<Vector3> controlPoints=new List<Vector3>();
    void Start()
    {
      
    }

    void Update()
    {
        
    }

    void Cal_BSplineCurve(){
        int p=controlPoints.Count;
        //12
        int n=3;
        //3
        int m=p+n+1;
        //16

        float[] u=GetKnotVector(m,n);

        for(int i=0;i<p-1;i++){

        }

    }

    float[] GetKnotVector(int m,int n,float knotMin=0.0f,float knotMax=1.0f){
        List<float> knotVector=new List<float>();
        int knotN=n+1;
        //4

        for(int i=0;i<m;i++){
            if(i>=0&&i<knotN){
                knotVector.Add(knotMin);
            }else if(i>=knotN&&i<(m-knotN)){
                int knotWidth=m-knotN*2;
                float knotVal=(knotMax-knotMin)/(float)knotWidth;
                knotVal=knotVal*(float)(i-knotN+1);
                knotVector.Add(knotVal);
            }else if(i>=(m-knotN)&&i<m){
                knotVector.Add(knotMax);
            }
        }

        return knotVector.ToArray();
    }

    void GetBasisFunction(){

    }
    
}
