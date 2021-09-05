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
        public List<Vector3> normals;
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
    List<Quaternion> FibonacciRotation=new List<Quaternion>();
    List<Vector4> FibonacciGrothData=new List<Vector4>();
    float flowerTime=0.0f;
    

    [Space(1)]
    [Header("MultiFlower")]
    [SerializeField] int N=1;
    [SerializeField] CPU_Flower_Simulation cPU_Flower_Simulation;

    Vector3 flowerPosition=new Vector3(0,0,0);
    Vector3 flowerTangent=new Vector3(0,0,0);
    Vector3 flowerNormal=new Vector3(0,0,0);
    Vector3 flowerBioNormal=new Vector3(0,0,0);
    int flowerPoint;

    #endregion

    #region  Leaf Field
    [SerializeField] MeshRenderer firstLealMeshRenderer;
    [SerializeField] MeshFilter firstLeafFilter;
     [SerializeField] MeshRenderer secondLealMeshRenderer;
    [SerializeField] MeshFilter secondLeafFilter;
    [SerializeField] Material leafMat;
    [SerializeField] float leafTangentVal=10.0f;
    [SerializeField] float leafNormalVal=180.0f;
    [SerializeField] float leafBioNormalVal=10.0f;
    [SerializeField] float secondLeafTangentVal=10.0f;
    [SerializeField] float secondLeafNormalVal=180.0f;
    [SerializeField] float secondLeafBioNormalVal=10.0f;

    Vector3 firstLeafPosition=new Vector3(0,0,0);
    Vector3 firstLeafTangent=new Vector3(0,0,0);
    Vector3 firstLeafNormal=new Vector3(0,0,0);
    Vector3 firstLeafBioNormal=new Vector3(0,0,0);
    bool fisrtLeafCal_Flag=false;
    [SerializeField] int firstLeafPoint=5;
    Vector3 secondLeafPosition=new Vector3(0,0,0);
    Vector3 secondLeafTangent=new Vector3(0,0,0);
    Vector3 secondLeafNormal=new Vector3(0,0,0);
    Vector3 secondLeafBioNormal=new Vector3(0,0,0);
    bool secondLeafCal_Flag=false;
    [SerializeField] int secondLeafPoint=6;
    
    #endregion

    #region Stem Field
     [SerializeField] MeshRenderer stemMeshRenderer;
    [SerializeField] MeshFilter stemFilter;
    [SerializeField] Material stemMat;
    [SerializeField] float radius=1.0f;
    [SerializeField] int segments=6;

    #endregion

    void Start()
    {
        for(int i=0;i<controlPoints.Count;i++){
            controlPoints[i]+=new Vector3(0,0,2.0f*(
                    Mathf.PerlinNoise((float)i,controlPoints[i].x+controlPoints[i].y)*2.0f-1.0f
                )
            );
        }

        fisrtLeafCal_Flag=false;
        secondLeafCal_Flag=false;

        CalStem();
        Init();
        CalFibonacciPosition();
        flowerTime=1.0f;
        RenderMultiFlower();
        CalLeaf();
       
    }

    void Update()
    {
        flowerTime=(Mathf.Sin(Time.time)+1.0f)*0.5f;
        //RenderMultiFlower();
        //Init();
          //CalLeaf();
    }

   
    //BaseFlower//////////////////////////////////////////////////////////////////////////
    #region BaseFlower
    void Init(){
        baseFlower_Data=new BaseFlower_Data();
        baseFlower_Data.vertices=new List<Vector3>();
        baseFlower_Data.indices=new List<int>();
        baseFlower_Data.normals=new List<Vector3>();

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
        
        // meshRenderer.material=b_spline_mat;
        // filter.mesh=B_Spline_Mesh;

        for(int i=0;i<pos.Count;i++){
            baseFlower_Data.vertices.Add(pos[i]);
        }

        for(int i =0;i<triangles.Count;i++){
            baseFlower_Data.indices.Add(triangles[i]);
        }        

        //rightNormals 
        for(int i=0;i<rightCount;i++){
            Vector3 p0=baseFlower_Data.vertices[i];
            Vector3 p1=baseFlower_Data.vertices[i+1];
            Vector3 p2=baseFlower_Data.vertices[pos.Count-i-1]; 

            Vector3 v0=Vector3.Normalize(p1-p0);
            Vector3 v1=Vector3.Normalize(p2-p0);

            Vector3 normal=Vector3.Normalize(Vector3.Cross(v0,v1));

            baseFlower_Data.normals.Add(normal);
        }
        baseFlower_Data.normals.Add(baseFlower_Data.normals[baseFlower_Data.normals.Count-1]);

        //lefyNormals
        for(int i=0;i<leftCount;i++){
            Vector3 p0=baseFlower_Data.vertices[pos.Count-i-1];
            Vector3 p1=baseFlower_Data.vertices[pos.Count-i-2];
            Vector3 p2=baseFlower_Data.vertices[i+1];

            Vector3 v0=Vector3.Normalize(p1-p0);
            Vector3 v1=Vector3.Normalize(p2-p0);

            Vector3 normal=Vector3.Normalize(Vector3.Cross(v0,v1));

            baseFlower_Data.normals.Add(normal);

        }
        baseFlower_Data.normals.Add(baseFlower_Data.normals[baseFlower_Data.normals.Count-1]);
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
            float r=Mathf.Sqrt((float)i);
            float ang=(float)(i-1)*goldenAngle*Mathf.Deg2Rad;
            pos.x=r*Mathf.Sin(ang);
            pos.z=r*Mathf.Cos(ang);

            Vector3 crossVec=Vector3.Cross(new Vector3(0,1,0),Vector3.Normalize(pos));
            Quaternion fibRot=Quaternion.Euler(0,(ang*(180.0f/Mathf.PI)),0);
            float angVal=Mathf.Pow((float)(i-1)*0.175f,2.0f);
            //fibRot=Quaternion.AngleAxis(angVal,crossVec)*fibRot;
            
            FibonacciPosition.Add(pos);
            FibonacciRotation.Add(fibRot);
            FibonacciGrothData.Add(new Vector4(crossVec.x,crossVec.y,crossVec.z,angVal));
        }
    }

    void RenderMultiFlower(){
        Mesh fibonacciMesh=new Mesh();
        List<Vector3> fibonacciVertices=new List<Vector3>();
        List<int> fibonacciIndices=new List<int>();
        List<Vector3> fibonacciNormals=new List<Vector3>();

        for(int i=0;i<FibonacciPosition.Count;i++){
            Vector3 fibPos=FibonacciPosition[i];
            
            //Quaternion fibRot=FibonacciRotation[i];
            Vector4 fibGroth=FibonacciGrothData[i];
            Quaternion fibRot=Quaternion.AngleAxis(flowerTime*fibGroth.w,new Vector3(fibGroth.x,fibGroth.y,fibGroth.z))*FibonacciRotation[i];
            fibRot=Quaternion.AngleAxis(Vector3.Angle(flowerTangent,new Vector3(0,0,1)),flowerBioNormal)*fibRot;

            for(int q=0;q<baseFlower_Data.vertices.Count;q++){
                float size=(float)(i+1)*0.005f;
                fibonacciVertices.Add((flowerTime*size*(fibRot*baseFlower_Data.vertices[q]+fibPos*(1.0f/(float)(i+1.0f)))+flowerPosition));
            }

            for(int p=0;p<baseFlower_Data.normals.Count;p++){
                fibonacciNormals.Add((fibRot*baseFlower_Data.normals[p]));
            }
        }

        for(int i=0;i<FibonacciPosition.Count;i++){
            for(int p=0;p<baseFlower_Data.indices.Count;p++){
                fibonacciIndices.Add(i*(baseFlower_Data.vertices.Count)+baseFlower_Data.indices[p]);
            }
        }
       
        fibonacciMesh.vertices=fibonacciVertices.ToArray();
        fibonacciMesh.triangles=fibonacciIndices.ToArray();
        fibonacciMesh.normals=fibonacciNormals.ToArray();
        fibonacciMesh.RecalculateNormals();

        meshRenderer.material=b_spline_mat;
        filter.mesh=fibonacciMesh;
        
        cPU_Flower_Simulation.multiFlowerMesh=fibonacciMesh;
        cPU_Flower_Simulation.isDone=true;

    }

    #endregion
    
    #region leaf

    void CalLeaf(){
         Mesh firstLeafMesh=new Mesh();
         Mesh secondLeafMesh=new Mesh();
         List<Vector3> firstVirtices=new List<Vector3>();
         List<Vector3> secondVirtices=new List<Vector3>();
         float size=0.5f;
        

         for(int i=0;i<baseFlower_Data.vertices.Count;i++){
             firstVirtices.Add(size*((
                 
                 Quaternion.AngleAxis(leafTangentVal,firstLeafTangent)*
                 Quaternion.AngleAxis(leafNormalVal,firstLeafNormal)*
                 Quaternion.AngleAxis(leafBioNormalVal,firstLeafBioNormal)*
             Quaternion.AngleAxis(90.0f,new Vector3(0,1,0))*baseFlower_Data.vertices[i]*Mathf.Min(i,1.0f)+firstLeafPosition)));

             secondVirtices.Add(size*(((
                 Quaternion.AngleAxis(secondLeafTangentVal,secondLeafTangent)*
                 Quaternion.AngleAxis(secondLeafNormalVal,secondLeafNormal)*
                 Quaternion.AngleAxis(secondLeafBioNormalVal,secondLeafBioNormal)*
                 Quaternion.AngleAxis(-90.0f,new Vector3(0,1,0))*baseFlower_Data.vertices[i]*Mathf.Min(i,1.0f)+secondLeafPosition))));             
         }

         firstLeafMesh.vertices=firstVirtices.ToArray();
         firstLeafMesh.triangles=baseFlower_Data.indices.ToArray();
         firstLeafFilter.mesh=firstLeafMesh;
         firstLealMeshRenderer.material=leafMat;

         secondLeafMesh.vertices=secondVirtices.ToArray();
         secondLeafMesh.triangles=baseFlower_Data.indices.ToArray();
         secondLeafFilter.mesh=secondLeafMesh;
         secondLealMeshRenderer.material=leafMat;
        
    }

    #endregion

    #region Stem

    void CalStem(){
        Mesh stemMesh=new Mesh();
        List<B_Spline_Data> data=Cal_BSplineCurve();
        List<Vector3> vertices=new List<Vector3>();
        List<int> triangles=new List<int>();

        for(int i=+1;i<data.Count-1;i++){
            Vector3 tangent=Vector3.Normalize(data[i+1].position-data[i-1].position);
            Vector3 normal=Vector3.Cross(tangent,new Vector3(0,1,0));
            Vector3 bionormal=Vector3.Cross(tangent,normal);

            Vector3 pointPosition=data[i].position;

            for(int p=0;p<segments;p++){
                float angRate=(2.0f*Mathf.PI)*(p/(float)(segments-1));
                float xVal=Mathf.Cos(angRate);
                float zVal=Mathf.Sin(angRate);
                Vector3 pos=pointPosition;
                pos+=radius*Vector3.Normalize(normal*xVal+bionormal*zVal);
                pos=Quaternion.AngleAxis(90.0f,new Vector3(0,0,1))*pos;
                vertices.Add(pos);

                if(i<data.Count-1-1){
                    triangles.Add((i-1)*segments+p);
                    triangles.Add((i-1)*segments+(p+1)%segments);
                    triangles.Add((i-1)*segments+(p+1)%segments+segments);

                    triangles.Add((i-1)*segments+p);
                    triangles.Add((i-1)*segments+(p+1)%segments+segments);
                    triangles.Add((i-1)*segments+p+segments);

                 if(((i-1)*segments+p)==firstLeafPoint){
                     firstLeafPosition=pos;
                     firstLeafTangent=tangent;
                     firstLeafNormal=normal;
                     firstLeafBioNormal=bionormal;
                 }

                 if(((i-1)*segments+p)==secondLeafPoint){
                      secondLeafPosition=pos;
                      secondLeafTangent=tangent;
                      secondLeafNormal=normal;
                      secondLeafBioNormal=bionormal;
                 }   
               }
            }
            
            if(i==(data.Count-1)-1){
                flowerPosition=pointPosition;
                flowerTangent=tangent;
                flowerNormal=normal;
                flowerBioNormal=bionormal;
            }
        }



        flowerPosition=vertices[vertices.Count-1];
        flowerTangent=Vector3.Normalize(data[data.Count-1].position-data[data.Count-2].position);
  
        stemMesh.vertices=vertices.ToArray();
        stemMesh.triangles=triangles.ToArray();

        stemFilter.mesh=stemMesh;
        stemMeshRenderer.material=stemMat;

    }

    #endregion
}
