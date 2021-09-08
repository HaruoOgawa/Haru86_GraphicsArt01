namespace GraphicsArt.GPUFlower.GPUFlower_Flowers{

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphicsArt.GPUFlower.GPUFlower_Base;
using GraphicsArt.GPUFlower.GPUFlower_Stem;
using System.Runtime.InteropServices;

    public class GPUFlower_Flowers : MonoBehaviour
    {
    #region public field
    [SerializeField] GPUFlower_Base gPUFlower_Base;
    [SerializeField] GPUFlower_Stem gPUFlower_Stem;
    [SerializeField] Material flowers_mat;
    [SerializeField] PetalData[] petalDatas;
    #endregion

    #region private region

    Mesh flowers_mesh;
    public struct Multi_Flower_Data{
        public List<Vector3> vertices;
        public List<Vector3> normals;
        public List<int> triangles;
    }
    #endregion
   
        void Start()
        {
            Init();
        }

        void Update()
        {
            if(gPUFlower_Base.flowersIsDone&&gPUFlower_Base.stemIsDone){
            // if(gPUFlower_Base.flowersIsDone&&gPUFlower_Base.stemIsDone&&gPUFlower_Base.leafIsDone){
                flowers_mat.SetBuffer("_stemDataFlower_buffer",gPUFlower_Stem.stemDataFlower_buffer);
                Graphics.DrawMeshInstancedProcedural(flowers_mesh,0,flowers_mat,new Bounds(this.gameObject.transform.position,Vector3.one*500.0f),gPUFlower_Base.count);
            }
        }

        void OnDisable(){
           
        }

        void Init(){
            flowers_mesh=new Mesh();
            
            SetupFlowerdata();
        }

        void SetupFlowerdata(){
            GPUFlower_Base.BaseFlower_Data flower_data=new GPUFlower_Base.BaseFlower_Data();
            flower_data=GPUFlower_Base.Cal_BSpline_Surface(petalDatas[0].controlPoints,petalDatas[0].knotMin,petalDatas[0].knotMax,petalDatas[0].tWidth);
            Multi_Flower_Data multi_Flower_Data=RenderMultiFlower(flower_data,new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0));
            
            flowers_mesh.vertices=multi_Flower_Data.vertices.ToArray();
            flowers_mesh.triangles=multi_Flower_Data.triangles.ToArray();
            flowers_mesh.normals=multi_Flower_Data.normals.ToArray();
            flowers_mesh.RecalculateNormals();

            gPUFlower_Base.flowersIsDone=true;
        }

        #region  MultiFlower
        public static void CalFibonacciPosition(ref List<Vector3> FibonacciPosition,ref List<Quaternion> FibonacciRotation,ref List<Vector4> FibonacciGrowthData,int N=50){
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
                
                FibonacciPosition.Add(pos);
                FibonacciRotation.Add(fibRot);
                FibonacciGrowthData.Add(new Vector4(crossVec.x,crossVec.y,crossVec.z,angVal));
            }
        }

        public static Multi_Flower_Data RenderMultiFlower(GPUFlower_Base.BaseFlower_Data flower_data,Vector3 flowerPosition,Vector3 flowerTangent,Vector3 flowerBioNormal,float flowerTime=1.0f,int N=50){
            Multi_Flower_Data data=new Multi_Flower_Data();
            data.vertices=new List<Vector3>();
            data.normals=new List<Vector3>();
            data.triangles=new List<int>();
            
            List<Vector3> FibonacciPosition=new List<Vector3>();
            List<Quaternion> FibonacciRotation=new List<Quaternion>();
            List<Vector4> FibonacciGrowthData=new List<Vector4>();
            CalFibonacciPosition(ref FibonacciPosition,ref FibonacciRotation,ref FibonacciGrowthData);

            List<Vector3> fibonacciVertices=new List<Vector3>();
            List<int> fibonacciIndices=new List<int>();
            List<Vector3> fibonacciNormals=new List<Vector3>();

            for(int i=0;i<FibonacciPosition.Count;i++){
                Vector3 fibPos=FibonacciPosition[i];
                
                Vector4 fibGroth=FibonacciGrowthData[i];
                Quaternion fibRot=Quaternion.AngleAxis(flowerTime*fibGroth.w,new Vector3(fibGroth.x,fibGroth.y,fibGroth.z))*FibonacciRotation[i];
                fibRot=Quaternion.AngleAxis(Vector3.Angle(flowerTangent,new Vector3(0,0,1)),flowerBioNormal)*fibRot;

                for(int q=0;q<flower_data.vertices.Count;q++){
                    float size=(float)(i+1)*0.01f;
                    fibonacciVertices.Add((flowerTime*size*(fibRot*flower_data.vertices[q]+fibPos*(1.0f/(float)(i+1.0f)))+flowerPosition));
                }

                for(int p=0;p<flower_data.normals.Count;p++){
                    fibonacciNormals.Add((fibRot*flower_data.normals[p]));
                }
            }

            for(int i=0;i<FibonacciPosition.Count;i++){
                for(int p=0;p<flower_data.triangles.Count;p++){
                    fibonacciIndices.Add(i*(flower_data.vertices.Count)+flower_data.triangles[p]);
                }
            }
        
            for(int i=0;i<fibonacciVertices.Count;i++){
                data.vertices.Add(fibonacciVertices[i]);
            }

            for(int i=0;i<fibonacciNormals.Count;i++){
                data.normals.Add(fibonacciNormals[i]);
            }

            for(int i=0;i<fibonacciIndices.Count;i++){
                data.triangles.Add(fibonacciIndices[i]);
            }

            //Debug.Log("data.triangles.Count: "+data.triangles.Count);

            return data;
        }

        #endregion
    }
}

