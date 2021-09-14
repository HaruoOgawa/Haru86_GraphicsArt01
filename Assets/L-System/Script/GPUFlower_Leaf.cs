namespace GraphicsArt.GPUFlower.GPUFlower_Leaf{

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphicsArt.GPUFlower.GPUFlower_Base;
using GraphicsArt.GPUFlower.GPUFlower_Stem;
using System.Runtime.InteropServices;

    public class GPUFlower_Leaf : MonoBehaviour
    {
        #region public field
    [SerializeField] GPUFlower_Base gPUFlower_Base;
    [SerializeField] GPUFlower_Stem gPUFlower_Stem;
    [SerializeField] Material leaf_mat;
    [SerializeField] PetalData[] petalDatas;
    [SerializeField] ComputeShader cal_leaf_cs;
    [SerializeField] float isLeafTime=0.2f;
    #endregion

    #region private region
    [HideInInspector] public ComputeBuffer stemDataLeaf_buffer;
    int kernel_CalLeafGrowth; 

    Mesh leaf_mesh;
   
    #endregion
   
        void Start()
        {
            Init();
        }

        void Update()
        {
            if(gPUFlower_Base.flowersIsDone&&gPUFlower_Base.stemIsDone&&gPUFlower_Base.leafIsDone){
                Cal_Leaf_Growth();
                leaf_mat.SetBuffer("_read_stemDataLeaf_buffer",stemDataLeaf_buffer);
                Graphics.DrawMeshInstancedProcedural(leaf_mesh,0,leaf_mat,new Bounds(this.gameObject.transform.position,Vector3.one*500.0f),2*gPUFlower_Base.count);
            }
        }

        void OnDisable(){
           stemDataLeaf_buffer.Release();
        }

        #region Init Field
        void Init(){
            kernel_CalLeafGrowth=cal_leaf_cs.FindKernel("CalLeafGrowth");
            
            InitBuffer();

            leaf_mesh=new Mesh();
            
            SetupFlowerdata();
        }

        void InitBuffer(){
            stemDataLeaf_buffer=new ComputeBuffer(gPUFlower_Base.count*2,Marshal.SizeOf(typeof(GPUFlower_Stem.StemData)));
            List<GPUFlower_Stem.StemData> initStemDataLeaf=new List<GPUFlower_Stem.StemData>();
            
            for(int i=0;i<gPUFlower_Base.count;i++){
                Vector2 initPos=Random.insideUnitCircle;
                GPUFlower_Stem.StemData data=new GPUFlower_Stem.StemData(i,new Vector3(initPos.x,0,initPos.y),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0));
                
                initStemDataLeaf.Add(data);
                initStemDataLeaf.Add(data);
            }
           
            stemDataLeaf_buffer.SetData(initStemDataLeaf.ToArray());
        }

        void SetupFlowerdata(){
            GPUFlower_Base.BaseFlower_Data flower_data=new GPUFlower_Base.BaseFlower_Data();
            flower_data=GPUFlower_Base.Cal_BSpline_Surface(petalDatas[0].controlPoints,petalDatas[0].knotMin,petalDatas[0].knotMax,petalDatas[0].tWidth);
            
            leaf_mesh.vertices=flower_data.vertices.ToArray();
            leaf_mesh.triangles=flower_data.triangles.ToArray();
            leaf_mesh.normals=flower_data.normals.ToArray();
            leaf_mesh.RecalculateNormals();

            gPUFlower_Base.leafIsDone=true;
        }

        #endregion

        #region Cal Stem Data
        void Cal_Leaf_Growth(){
            cal_leaf_cs.SetBuffer(kernel_CalLeafGrowth,"_write_stemDataLeaf_buffer",stemDataLeaf_buffer);
            cal_leaf_cs.SetBuffer(kernel_CalLeafGrowth,"_read_stemVertex_buffer",gPUFlower_Stem.stemVertex_buffer);
            cal_leaf_cs.SetBuffer(kernel_CalLeafGrowth,"_read_stemManage_buffer",gPUFlower_Stem.stemManage_buffer);
            cal_leaf_cs.SetInt("_stemVertexCount",gPUFlower_Stem.stemVertexCount);
            cal_leaf_cs.SetFloat("_isLeafTime",isLeafTime);
            cal_leaf_cs.SetFloat("_DTime",Time.deltaTime);
            cal_leaf_cs.Dispatch(kernel_CalLeafGrowth,(2*gPUFlower_Base.count)/gPUFlower_Stem.numthreds_val,1,1);
        }
        #endregion
    }

}