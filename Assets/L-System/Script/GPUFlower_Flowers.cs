namespace GraphicsArt.GPUFlower.GPUFlower_Flowers{

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphicsArt.GPUFlower.GPUFlower_Base;
using System.Runtime.InteropServices;

    public class GPUFlower_Flowers : MonoBehaviour
    {
    #region public field
    [SerializeField] GPUFlower_Base gPUFlower_Base;
    [SerializeField] ComputeShader flowers_cs;
    [SerializeField] Material flowers_mat;
    [SerializeField] PetalData[] petalDatas;
    #endregion

    #region private region

    ComputeBuffer flowers_buffer;
    Mesh flowers_mesh;
    #endregion
   
        void Start()
        {
            Init();
            SetupFlowerdata();
        }

        void Update()
        {
            if(gPUFlower_Base.flowersIsDone&&gPUFlower_Base.stemIsDone&&gPUFlower_Base.leafIsDone){
                Graphics.DrawMeshInstancedProcedural(flowers_mesh,0,flowers_mat,new Bounds(this.gameObject.transform.position,Vector3.one*500.0f),gPUFlower_Base.count);
            }
        }

        void Init(){
            flowers_mesh=new Mesh();
            flowers_buffer=new ComputeBuffer(gPUFlower_Base.count,Marshal.SizeOf(typeof(GPUFlower_Base.BaseFlower_Data)));
        }

        void SetupFlowerdata(){
            List<GPUFlower_Base.BaseFlower_Data> flower_data=new List<GPUFlower_Base.BaseFlower_Data>();
            flower_data.Add(GPUFlower_Base.Cal_BSpline_Surface(petalDatas[0].controlPoints,petalDatas[0].knotMin,petalDatas[0].knotMax,petalDatas[0].tWidth));
            flowers_buffer.SetData(flower_data.ToArray());
        }

    }
}

