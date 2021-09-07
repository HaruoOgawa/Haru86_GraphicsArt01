namespace GraphicsArt.GPUFlower.GPUFlower_Base{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GPUFlower_Base : MonoBehaviour
    {
        #region Public Field
        [Header("Public Field")]
        public int count=500;
        #endregion

        #region Flower Field
        [Space(5)]
        [Header("Flower Field")]
        [HideInInspector]public bool flowersIsDone=false;
        #endregion

        #region Stem Field
        [Space(5)]
        [Header("Stem Field")]
        [HideInInspector]public bool stemIsDone=false;
        #endregion

        #region Leaf Field
        [Space(5)]
        [Header("Leaf Field")]
        [HideInInspector]public bool leafIsDone=false;
        #endregion

        #region BaseFlower Region
        public struct B_Spline_Data{
            public Vector3 position;
            public int index;

            public B_Spline_Data(Vector3 p,int i){
                this.position=p;
                this.index=i;
            }
        }

        public struct BaseFlower_Data{
            public List<Vector3> vertices;
            public List<Vector3> normals;
            public List<int> triangles;
        }

        #endregion

        void Awake(){
            count=Mathf.NextPowerOfTwo(count);
            flowersIsDone=false;
            stemIsDone=false;
            leafIsDone=false;
        }


        #region BaseFlower Func

        public static BaseFlower_Data Cal_BSpline_Surface(List<Vector3> controlPoints,float knotMin,float knotMax,float tWidth=0.01f){
            BaseFlower_Data baseFlower_Data=new BaseFlower_Data();
            baseFlower_Data.vertices=new List<Vector3>();
            baseFlower_Data.normals=new List<Vector3>();
            baseFlower_Data.triangles=new List<int>();

            //Mesh B_Spline_Mesh=new Mesh();
            List<B_Spline_Data> data=new List<B_Spline_Data>();
            data.Clear();
            data=Cal_BSplineCurve(controlPoints,knotMin,knotMax,tWidth);


            List<Vector3> pos=new List<Vector3>();
            pos.Clear();
            List<int> triangles=new List<int>();
            triangles.Clear();

            for(int i=0;i<data.Count;i++){
                Vector3 p=data[i].position;
                p=Quaternion.Euler(0,0,90)*p;
                pos.Add(p);
            }

            for(int i=0;i<data.Count;i++){
                Vector3 p=data[i].position;
                p.y=-p.y;
                p=Quaternion.Euler(0,0,90)*p;
                pos.Add(p);
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

            // B_Spline_Mesh.vertices=pos.ToArray();
            // B_Spline_Mesh.triangles=triangles.ToArray();
            
            
            for(int i=0;i<pos.Count;i++){
                baseFlower_Data.vertices.Add(pos[i]);
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

            //triangles
            for(int i=0;i<triangles.Count;i++){
                baseFlower_Data.triangles.Add(triangles[i]);
            }
        
            return baseFlower_Data;

        }

        public List<Vector3> ReverseCurvePos(List<Vector3> pos){
            List<Vector3> posWithReverse=pos;
            for(int i=0;i<pos.Count;i++){
                Vector3 p=pos[i];
                p.y=-p.y;
                posWithReverse.Add(p);
            }


            return posWithReverse;
        }

        public static List<B_Spline_Data> Cal_BSplineCurve(List<Vector3> controlPoints,float knotMin,float knotMax,float tWidth=0.01f){
            int p=controlPoints.Count;
            int n=3;
            int m=p+n+1;
            
            float[] u=GetKnotVector(m,n,knotMin,knotMax);
            
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

        public static float[] GetKnotVector(int m,int n,float knotMin,float knotMax){
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

        public static float GetBasisFunction(float[] u,int j,int k,float t){
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

    }

}