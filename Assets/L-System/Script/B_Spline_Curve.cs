using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Spline_Curve : MonoBehaviour
{

    #region BaseFlowerField
    
    struct B_Spline_Data{
        public Vector3 position;
        public int index;

        public B_Spline_Data(Vector3 p,int i){
            this.position=p;
            this.index=i;
        }
    }
    struct BaseFlower_Data{
        public List<Vector3> vertices;
        public List<int> indices;
    }

     enum MeshType{
        Lines,
        Point,
        triangles
    } 

    enum RenderFlag{
        surface,
        reverse
    }

    BaseFlower_Data baseFlower_Data;
  
    [Header("BaseFlowerModeling")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter filter;
    [SerializeField] Material b_spline_mat;
    [SerializeField] int t=10;
    
    [SerializeField] List<Vector3> controlPoints=new List<Vector3>();
    [SerializeField] float knotMin=0.0f;
    [SerializeField] float knotMax=1.0f;
    [SerializeField] float tWidth=0.01f;
    [SerializeField] MeshType meshType=MeshType.Lines;
    [SerializeField] RenderFlag renderFlag=RenderFlag.reverse;
    

    #endregion

    #region MultiFlowerField
    enum FlowerType{
        BaseFlower,
        MultiFlower
    }

    List<Vector3> FibonacciPosition=new List<Vector3>();

    [Space(1)]
    [Header("MultiFlower")]
    [SerializeField] int N=1;

    #endregion

    void Start()
    {
        Init();
        CalFibonacciPosition();
        RenderMultiFlower();
    }

    void Update()
    {
        //  Debug.Log("baseFlower_Data.vertices.Count:"+baseFlower_Data.vertices.Count);
        // Debug.Log("baseFlower_Data.indices.Count:"+baseFlower_Data.indices.Count);
        //RenderMultiFlower();
    }

    Vector3 rot(Vector3 pos){
        
        return new Vector3(0,0,0);
    }
    //BaseFlower//////////////////////////////////////////////////////////////////////////
    #region BaseFlower
    void Init(){
        baseFlower_Data=new BaseFlower_Data();
        baseFlower_Data.vertices=new List<Vector3>();
        baseFlower_Data.indices=new List<int>();

        Mesh B_Spline_Mesh=new Mesh();
        List<B_Spline_Data> data=new List<B_Spline_Data>();
        data.Clear();
        data=Cal_BSplineCurve();


        List<Vector3> pos=new List<Vector3>();
        pos.Clear();
        List<int> index=new List<int>();
        index.Clear();
        List<int> triangles=new List<int>();
        triangles.Clear();

        for(int i=0;i<data.Count;i++){
            Vector3 p=data[i].position;
            p=Quaternion.Euler(0,0,90)*p;
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
                p=Quaternion.Euler(0,0,90)*p;
                pos.Add(p);

                index.Add(data.Count+i);
                if(i<data.Count-1){
                    index.Add(data.Count+i+1); 
                }

            }
        }

        int posCount=pos.Count-2;
        int rightCount=(posCount)/2;
        int leftCount=posCount-rightCount;

        //rightTriangles 
        for(int i=0;i<rightCount;i++){
            triangles.Add(i);
            triangles.Add(i+1);
            triangles.Add(pos.Count-i-1); 
        }

        //lefyTriangles
        for(int i=0;i<leftCount;i++){
            triangles.Add(pos.Count-i-1);
            triangles.Add(pos.Count-i-2);
            triangles.Add(i+1);
        }

        B_Spline_Mesh.vertices=pos.ToArray();
        
        if(meshType==MeshType.Lines){
            B_Spline_Mesh.SetIndices(index.ToArray(),MeshTopology.Lines,0);
        }else if(meshType==MeshType.Point){
            B_Spline_Mesh.SetIndices(index.ToArray(),MeshTopology.Points,0);
        }else if(meshType==MeshType.triangles){
            B_Spline_Mesh.triangles=triangles.ToArray();
        }
        
        meshRenderer.material=b_spline_mat;
        filter.mesh=B_Spline_Mesh;

        for(int i=0;i<pos.Count;i++){
            baseFlower_Data.vertices.Add(pos[i]);
        }

        for(int i =0;i<index.Count;i++){
            baseFlower_Data.indices.Add(index[i]);
        }

        Debug.Log("baseFlower_Data.vertices.Count:"+baseFlower_Data.vertices.Count);
        Debug.Log("baseFlower_Data.indices.Count:"+baseFlower_Data.indices.Count);
        Debug.Log("pos.Count:"+pos.Count);
        Debug.Log("index.Count:"+index.Count);
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

        List<B_Spline_Data> S=new List<B_Spline_Data>();
        for(int i=0;i<tDelta.Count;i++){
            S.Add(new B_Spline_Data(new Vector3(0,0,0),i));
        }
        
        S[0]=new B_Spline_Data(controlPoints[0],S[0].index);
       
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

    #endregion

    //MultiFlower/////////////////////////////////////////////////////////
    #region  MultiFlower
    void CalFibonacciPosition(){
        float goldenAngle=137.509f;
        for(int i=1;i<N+1;i++){
            Vector3 pos=new Vector3(0,0,0);
            pos.x=Mathf.Sqrt((float)i);
            float ang=(float)(i-1)*goldenAngle;
            pos=Quaternion.Euler(0,ang,0)*pos;
            FibonacciPosition.Add(pos);
        }
    }

    void RenderMultiFlower(){
        Mesh fibonacciMesh=new Mesh();
        List<Vector3> fibonacciVertices=new List<Vector3>();
        List<int> fibonacciIndices=new List<int>();

        fibonacciVertices=FibonacciPosition;
        for(int i=0;i<fibonacciVertices.Count;i++){
            fibonacciIndices.Add(i);
            if(i<fibonacciVertices.Count-1){
                fibonacciIndices.Add(i+1);
            }
        }
        
        fibonacciMesh.vertices=fibonacciVertices.ToArray();
        fibonacciMesh.SetIndices(fibonacciIndices.ToArray(),MeshTopology.Lines,0);

        meshRenderer.material=b_spline_mat;
        filter.mesh=fibonacciMesh;
    }

    #endregion
    
}
