namespace GraphicsArt.LSystem.GPU_LSystem_Calculator{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Runtime.InteropServices;

    public struct L_System_Data{
        Vector3 jointPos;
    }
    public class GPU_LSystem_Calculator : MonoBehaviour
    {
        #region public field
        public ComputeBuffer lsystem_data_buffer;
        //public ComputeBuffer lsystem_points_buffer;
        public int count=1000;
        public int numthreads=256;
        #endregion

        #region SerializeField
        [SerializeField] ComputeShader lsystem_cs;
        //[SerializeField] int lsystem_vetex_count
  
        #endregion

        #region private field
        int kernel_CalLSystemPoint;
        #endregion

        void Awake(){
            count=Mathf.NextPowerOfTwo(count);
        }
        void Start()
        {
            Init();
            AnalizeCharPatern();
            CalLSystemPoint();
        }

        void Update()
        {
            
        }

        void OnDisable(){
            lsystem_data_buffer.Release();
           // lsystem_points_buffer.Release();
        }

        void Init(){
            L_System_Data[] initLSystemData=new L_System_Data[count];
            
            for(int i=0;i<count;i++){
                initLSystemData[i]=new L_System_Data();
            }

            lsystem_data_buffer=new ComputeBuffer(count,Marshal.SizeOf(typeof(L_System_Data)));
            lsystem_data_buffer.SetData(initLSystemData);

            kernel_CalLSystemPoint=lsystem_cs.FindKernel("CalLSystemPoint");
        }

        void AnalizeCharPatern(){

        }

        void CalLSystemPoint(){

            lsystem_cs.SetBuffer(kernel_CalLSystemPoint,"_lsystem_data_buffer_write",lsystem_data_buffer);
            lsystem_cs.Dispatch(kernel_CalLSystemPoint,count/numthreads,1,1);
        }
    }

}