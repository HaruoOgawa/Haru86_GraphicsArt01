
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
        [HideInInspector] public ComputeBuffer comouteBuffer_boids_data;
        [HideInInspector] public ComputeBuffer comouteBuffer_boids_force;
        ComputeBuffer debugBuffer;
        public ComputeShader boids_cs;
        [SerializeField] float NoiseValue=10.0f;
      
        
        [SerializeField] float maxBoidsDist=1.0f;
        [SerializeField] float centerPosPower=1000.0f;
        
        
        #endregion

        #region private_val
        Butterfly[] butterflies;
        int count=0;
        
        int CalVector_Kernel;
        int ResultVector_Kernel;
        int NUMTHREADS_X_NUM=256;

         float maxBoidsField=500.0f;

        #endregion
    
        void Start()
        {
            count=GPUBase_Butterfly.instance.count;
            maxBoidsField=GPUBase_Butterfly.instance.maxBoidsField;
            //count=Mathf.NextPowerOfTwo(count);
            
            butterflies=new Butterfly[count];
            comouteBuffer_boids_data=new ComputeBuffer(count,Marshal.SizeOf(typeof(Butterfly)));
            comouteBuffer_boids_force=new ComputeBuffer(count,Marshal.SizeOf(typeof(Vector3)));
            debugBuffer=new ComputeBuffer(count,Marshal.SizeOf(typeof(Matrix4x4)));
           
            CalVector_Kernel=boids_cs.FindKernel("CalVector");
            ResultVector_Kernel=boids_cs.FindKernel("ResultVector");
           
            Debug.Log("count:"+count);
            Vector3[] initForce=new Vector3[count];
            Matrix4x4[] initMatrix=new Matrix4x4[count];

            //prepare buffer
            for(int i=0;i<count;i++){
                Vector3 initPos=Random.insideUnitSphere*GPUBase_Butterfly.instance.initRadius;
                Vector3 initVec=Vector3.Normalize(new Vector3(Random.Range(-10.0f,10.0f),Random.Range(-10.0f,10.0f),Random.Range(-10.0f,10.0f)))-Vector3.Normalize(Random.insideUnitSphere);
                initVec=Vector3.Normalize(initVec);
                //initVec=new Vector3(0,0,0)-initVec;
                butterflies[i]=new Butterfly(initPos,initVec);
                initForce[i]=Vector3.zero;
                initMatrix[i]=Matrix4x4.identity;
            }
            comouteBuffer_boids_data.SetData(butterflies);
            comouteBuffer_boids_force.SetData(initForce);
            debugBuffer.SetData(initMatrix);
           
        }

        void Update()
        {
            boids_cs.SetInt("_butterfly_count",count);
            boids_cs.SetFloat("_maxBoidsDist",maxBoidsDist);
            boids_cs.SetFloat("_maxBoidsField",maxBoidsField);
            boids_cs.SetFloat("_DTime",Time.deltaTime);
            boids_cs.SetFloat("_Time",Time.time);
            boids_cs.SetFloat("_centerPosPower",centerPosPower);
            boids_cs.SetFloat("_NoiseValue",NoiseValue);
            boids_cs.SetBuffer(CalVector_Kernel,"_boids_force_write",comouteBuffer_boids_force);
            boids_cs.SetBuffer(CalVector_Kernel,"_boids_data_read",comouteBuffer_boids_data);
            boids_cs.SetBuffer(CalVector_Kernel,"_debugBuffer",debugBuffer);
            boids_cs.Dispatch(CalVector_Kernel,count/NUMTHREADS_X_NUM,1,1);
            //comouteBuffer_boids_read=comouteBuffer_boids_write;

            boids_cs.SetInt("_butterfly_count",count);
            boids_cs.SetFloat("_DTime",Time.deltaTime);
            boids_cs.SetFloat("_Time",Time.time);
            boids_cs.SetFloat("_centerPosPower",centerPosPower);
            boids_cs.SetBuffer(ResultVector_Kernel,"_boids_force_read",comouteBuffer_boids_force);
            boids_cs.SetBuffer(ResultVector_Kernel,"_boids_data_write",comouteBuffer_boids_data);
            boids_cs.Dispatch(ResultVector_Kernel,count/NUMTHREADS_X_NUM,1,1);

            Vector3[] result=new Vector3[count];
            comouteBuffer_boids_force.GetData(result);
            //Debug.Log("result[10]:"+result[10]);

            Matrix4x4[] debugResult=new Matrix4x4[count];
            debugBuffer.GetData(debugResult);
            Debug.Log("debugResult[10]:"+debugResult[10]);
            
        }

       /* void LateUpdate(){
        
        }*/

        void OnDisable(){
            comouteBuffer_boids_force.Release();
            comouteBuffer_boids_data.Release();
            debugBuffer.Release();
        }
    }


}