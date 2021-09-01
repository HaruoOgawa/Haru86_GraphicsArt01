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

    enum MeshType{
        Lines,
        Point
    } 
    [SerializeField] MeshType meshType=MeshType.Lines;

    enum RenderFlag{
        surface,
        reverse
    }

    [SerializeField] RenderFlag renderFlag=RenderFlag.reverse;
    void Start()
    {
        Render_BSplineCurve();
    }

    void Update()
    {
         Render_BSplineCurve();
    }

    

    void Render_BSplineCurve(){
        Mesh B_Spline_Mesh=new Mesh();
        List<B_Spline_Data> data=new List<B_Spline_Data>();
        data.Clear();
        data=Cal_BSplineCurve();


        List<Vector3> pos=new List<Vector3>();
        pos.Clear();
        List<int> index=new List<int>();
        index.Clear();

        for(int i=0;i<data.Count;i++){
            Vector3 p=data[i].position;
            pos.Add(p);
            
             index.Add(i);

             if(renderFlag==RenderFlag.reverse){
                if(i<data.Count-1){
                    index.Add(i+1);
                }else{
                    index.Add(data.Count*2-1);
                }
             }else{
                 if(i<data.Count-1){
                    index.Add(i+1);
                }
             }
           
        }

        if(renderFlag==RenderFlag.reverse){
            for(int i=0;i<data.Count;i++){
                Vector3 p=data[i].position;
                p.y=-p.y;
                pos.Add(p);

                index.Add(data.Count+i);
                if(i<data.Count-1){
                index.Add(data.Count+i+1);
                }

            }
        }

       

     
        B_Spline_Mesh.vertices=pos.ToArray();
        
        if(meshType==MeshType.Lines){
            B_Spline_Mesh.SetIndices(index.ToArray(),MeshTopology.Lines,0);
        }else if(meshType==MeshType.Point){
            B_Spline_Mesh.SetIndices(index.ToArray(),MeshTopology.Points,0);
        }
        
        meshRenderer.material=b_spline_mat;
        filter.mesh=B_Spline_Mesh;
    }

    List<Vector3> ReverseCurvePos(List<Vector3> pos){
        List<Vector3> posWithReverse=pos;
        for(int i=0;i<pos.Count;i++){
            Vector3 p=pos[i];
            p.y=-p.y;
            posWithReverse.Add(p);
        }


        return posWithReverse;
    }

    List<B_Spline_Data> Cal_BSplineCurve(){
        int p=controlPoints.Count;
        int n=3;
        int m=p+n+1;
        
        float[] u=GetKnotVector(m,n);
        
        List<float> tDelta=new List<float>();
        int num=(int)(u[u.Length-1]/tWidth);
        for(int i=0;i<num;i++){
            tDelta.Add((float)(tWidth*i));
        }

        //このSを頂点座標として利用する //tDelta.Count
        List<B_Spline_Data> S=new List<B_Spline_Data>();
        for(int i=0;i<tDelta.Count;i++){
            S.Add(new B_Spline_Data(new Vector3(0,0,0),i));
        }
        
        S[0]=new B_Spline_Data(controlPoints[0],S[0].index);
       // S[S.Count-1]=new B_Spline_Data(controlPoints[0],S[S.Count-1].index);
      
        //各TにおけるBスプラインの値を求めている
        for(int i=1;i<tDelta.Count;i++){
            for(int j=0;j<p;j++){
                float b=GetBasisFunction(u,j,n,tDelta[i]);
                S[i]=new B_Spline_Data(S[i].position+controlPoints[j]*b,S[i].index);
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
                float knotVal=(knotMax-knotMin)/(float)(knotWidth+1);
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
