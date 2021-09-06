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
    #endregion

    #region private region
    struct Flowers_data{

    }
    ComputeBuffer flowers_buffer;
    Mesh flowers_mesh;
    #endregion
   
        void Start()
        {
            Init();
        
        }

        void Update()
        {
            if(gPUFlower_Base.flowersIsDone&&gPUFlower_Base.stemIsDone&&gPUFlower_Base.leafIsDone){
                Graphics.DrawMeshInstancedProcedural(flowers_mesh,0,flowers_mat,new Bounds(this.gameObject.transform.position,Vector3.one*500.0f),gPUFlower_Base.count);
            }
        }

        void Init(){
            flowers_mesh=new Mesh();
            flowers_buffer=new ComputeBuffer(gPUFlower_Base.count,Marshal.SizeOf(typeof(Flowers_data)));
        }

    }
}

