namespace GraphicsArt.Butterfly.GPUTrail_Butterfly{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.Butterfly.GPUBoids_Butterfly;
    using GraphicsArt.Butterfly.GPUBase_Butterfly;
    using System.Runtime.InteropServices;

    public class GPUTrail_Butterfly : MonoBehaviour
    {
        #region define struct
        
        struct trail{
            int nextCalNodeIndex;
        }

        struct node{
            Vector3 node_position;
            float node_life;
        }

        struct input_data{
            Vector3 nextInputPosition;
        }

        #endregion
        
        #region public field
        public ComputeShader trail_cs;
        public GPUBoids_Butterfly gPUBoids_Butterfly;
        public static GPUTrail_Butterfly instance=null;
        [SerializeField] Material butterfly_trail_mat;
        [SerializeField] int nodeSegment=60;
        [SerializeField] int initNodeLife=10;
        
        [HideInInspector] public ComputeBuffer buffer_trail;
        [HideInInspector] public ComputeBuffer buffer_node;
        [HideInInspector] public ComputeBuffer buffer_input;

        #endregion

        #region private field

        
        int count=0;

        #endregion

        void Awake(){
            if(instance==null) instance=this;
            gPUBoids_Butterfly=this.gameObject.GetComponent<GPUBoids_Butterfly>();
        }   
        void Start()
        {
            count=GPUBase_Butterfly.instance.count;
        buffer_trail=new ComputeBuffer(count,Marshal.SizeOf(typeof(trail)));
        buffer_node=new ComputeBuffer(count*nodeSegment,Marshal.SizeOf(typeof(node)));
        buffer_input=new ComputeBuffer(count,Marshal.SizeOf(typeof(input_data)));
        }

        void Update()
        {
            butterfly_trail_mat.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points,nodeSegment,count);
        }

        void OnDisable(){
            buffer_trail.Release();
            buffer_node.Release();
            buffer_input.Release();
        }
    }

}