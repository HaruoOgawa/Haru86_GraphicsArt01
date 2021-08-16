namespace GraphicsArt.Butterfly.GPUBase_Butterfly{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.Butterfly.GPUTrail_Butterfly;
    using GraphicsArt.Butterfly.GPUBoids_Butterfly;
    using System.Runtime.InteropServices;
     

    public class GPUBase_Butterfly : MonoBehaviour
    {
        
       
        [SerializeField] GPUTrail_Butterfly gPUTrail_Butterfly;
        [SerializeField] GPUBoids_Butterfly gPUBoids_Butterfly;
        [SerializeField] Material butterflyRender_mat;
        [SerializeField] int count=1000;
        [SerializeField] Mesh plane_mesh;
        [SerializeField] float size=1.0f;
        Matrix4x4[] butterfly_TRS;
        ComputeBuffer butterfly_buffer;
        
         void OnDisable()
        {
           butterfly_buffer.Release();
        }

        void Start()
        {
                butterfly_TRS=new Matrix4x4[count];
                for(int i=0;i<count;i++){
                    butterfly_TRS[i]=Matrix4x4.TRS(
                        Random.insideUnitSphere*100f,
                        Quaternion.Euler(Random.Range(-360f,360f),Random.Range(-360f,360f),Random.Range(-360f,360f)),
                        new Vector3(size,size,size)
                    );
                }

                butterfly_buffer=new ComputeBuffer(count,Marshal.SizeOf(typeof(Matrix4x4)));
                butterfly_buffer.SetData(butterfly_TRS);
                butterflyRender_mat.SetBuffer("_ButterflyBuffer",butterfly_buffer);
        }

        void Update()
        {
            Graphics.DrawMeshInstancedProcedural(plane_mesh,0,butterflyRender_mat,new Bounds(transform.position,Vector3.one*500),count);
        }

      
    }

}