using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Spline_Curve : MonoBehaviour
{
    struct B_Spline_Data{
        public Vector3 position;
        public int index;

        public B_Spline_Data(Vector3 p,int i){
            this.position=p;
            this.index=i;
        }
    }

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter filter;
    [SerializeField] Material b_spline_mat;
    [SerializeField] int t=10;
    
    [SerializeField] List<Vector3> controlPoints=new List<Vector3>();
    [SerializeField] float knotMin=0.0f;
    [SerializeField] float knotMax=1.0f;
    [SerializeField] float tWidth=0.01f;
    void Start()
    {
        Mesh B_Spline_Mesh=new Mesh();
        B_Spline_Data[] data=Cal_BSplineCurve();
        Vector3[] pos=new Vector3[t];
        int[] index=new int[t];
        for(int i=0;i<t;i++){
            pos[i]=data[i].position;
            index[i]=data[i].index;
        }
        B_Spline_Mesh.vertices=pos;
        B_Spline_Mesh.SetIndices(index,MeshTopology.Lines,0);
        
        meshRenderer.material=b_spline_mat;
        filter.mesh=B_Spline_Mesh;
    }

    void Update()
    {
        
    }

    B_Spline_Data[] Cal_BSplineCurve(){
        int p=controlPoints.Count;
        //12
        int n=3;
        //3
        int m=p+n+1;
        //16

        float[] u=GetKnotVector(m,n);
        List<float> tDelta=new List<float>();
        int num=(int)(u[u.Length-1]/tWidth);
        for(int i=0;i<num;i++){
            tDelta.Add((float)(tWidth*i));
        }

        //このSを頂点座標として利用する
        B_Spline_Data[] S=new B_Spline_Data[tDelta.Count];
        for(int i=0;i<tDelta.Count;i++){
            S[i]=new B_Spline_Data(new Vector3(0,0,0),i);
        }
        
        S[0].position=controlPoints[0];

        //各TにおけるBスプラインの値を求めている
        for(int i=1;i<tDelta.Count;i++){
            for(int j=0;j<p;j++){
                float b=GetBasisFunction(u,j,n,tDelta[i]);
                S[i].position=S[i].position+controlPoints[j]*b;
            }
        }
        return S;

    }

    float[] GetKnotVector(int m,int n){
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

    float GetBasisFunction(float[] u,int j,int k,float t){
        float w1=0.0f;
        float w2=0.0f;

        if(k==0){
            if(u[j]<t&&t<=u[j+1]){
                return 1.0f;
            }else{
                return 0.0f;
            }
        }else{
            if(u[j+k+1]-u[j+1]!=0.0f){
                w1=GetBasisFunction(u,j+1,k-1,t)*(u[j+k+1]-t)/(u[j+k+1]-u[j+1]);
            }

            if((u[j+k]-u[j])!=0.0f){
                w2=GetBasisFunction(u,j,k-1,t)*(t-u[j])/(u[j+k]-u[j]);
            }
            
            return w1+w2;
        }
       
    }
    
}
