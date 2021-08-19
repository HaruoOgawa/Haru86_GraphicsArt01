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
        public int count=1000;
        public static GPUBase_Butterfly instance=null;
        public  float maxBoidsField=500.0f;
        public float initRadius=500.0f;
        [SerializeField] Mesh plane_mesh;
         [SerializeField] float boidsScale=1.0f;
        Matrix4x4[] butterfly_TRS;
        

        void Awake(){
            count=Mathf.NextPowerOfTwo(count);
            if(instance==null)instance=this;
        }

        void Start()
        {
               
        }

        void Update()
        {
            butterflyRender_mat.SetFloat("_boidsScale",boidsScale);
             butterflyRender_mat.SetBuffer("_boidsBuffer",gPUBoids_Butterfly.comouteBuffer_boids_data);
             butterflyRender_mat.SetBuffer("_boidsForce",gPUBoids_Butterfly.comouteBuffer_boids_force);
            Graphics.DrawMeshInstancedProcedural(plane_mesh,0,butterflyRender_mat,new Bounds(transform.position,Vector3.one*500),count);
        }

      
    }

}