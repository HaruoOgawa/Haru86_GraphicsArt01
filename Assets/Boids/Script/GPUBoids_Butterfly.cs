
namespace GraphicsArt.Butterfly.GPUBoids_Butterfly{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.Butterfly.GPUTrail_Butterfly;
    using GraphicsArt.Butterfly.GPUBase_Butterfly;
    using System.Runtime.InteropServices;

    public class GPUBoids_Butterfly : MonoBehaviour
    {
        
        struct Butterfly{
            Vector3 position;
            Vector3 velocity;
            
            public Butterfly(Vector3 pos,Vector3 vec){
                this.position=pos;
                this.velocity=vec;
            }
        }

        #region public_val
        [HideInInspector] public ComputeBuffer comouteBuffer_boids_write;
        public ComputeShader boids_cs;
        #endregion

        #region private_val
        Butterfly[] butterflies;
        int count=0;
        ComputeBuffer comouteBuffer_boids_read;
        int CalVector_Kernel;
        int ResultVector_Kernel;
        int NUMTHREADS_X_NUM=256;

        #endregion
    
        void Start()
        {
            count=GPUBase_Butterfly.instance.count;
            
            butterflies=new Butterfly[count];
            comouteBuffer_boids_write=new ComputeBuffer(count,Marshal.SizeOf(typeof(Butterfly)));
            comouteBuffer_boids_read=new ComputeBuffer(count,Marshal.SizeOf(typeof(Butterfly)));
            
            CalVector_Kernel=boids_cs.FindKernel("CalVector");
            ResultVector_Kernel=boids_cs.FindKernel("ResultVector");
            count=Mathf.NextPowerOfTwo(count);
            Debug.Log("count:"+count);

            //prepare buffer
            for(int i=0;i<count;i++){
                Vector3 initPos=Random.insideUnitSphere;
                Vector3 initVec=Vector3.Normalize(Random.insideUnitSphere);
                butterflies[i]=new Butterfly(initPos,initVec);
            }
            comouteBuffer_boids_write.SetData(butterflies);
            comouteBuffer_boids_read.SetData(butterflies);
        }

        void Update()
        {
            boids_cs.SetInt("_butterfly_count",count);
            boids_cs.SetBuffer(CalVector_Kernel,"_comouteBuffer_boids_write",comouteBuffer_boids_write);
            boids_cs.SetBuffer(CalVector_Kernel,"_comouteBuffer_boids_read",comouteBuffer_boids_read);
            boids_cs.Dispatch(CalVector_Kernel,count/NUMTHREADS_X_NUM,1,1);
        }

        void LateUpdate(){
            boids_cs.SetInt("_butterfly_count",count);
            boids_cs.SetBuffer(ResultVector_Kernel,"_comouteBuffer_boids_write",comouteBuffer_boids_write);
            boids_cs.SetBuffer(ResultVector_Kernel,"_comouteBuffer_boids_read",comouteBuffer_boids_read);
            boids_cs.Dispatch(ResultVector_Kernel,count/NUMTHREADS_X_NUM,1,1);
        }

        void OnDisable(){
            comouteBuffer_boids_write.Release();
            comouteBuffer_boids_read.Release();
        }
    }


}