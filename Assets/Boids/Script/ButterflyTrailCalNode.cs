namespace GraphicsArt.Butterfly.ButterflyTrailCalNode{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.Butterfly.GPUTrail_Butterfly;
    using GraphicsArt.Butterfly.GPUBoids_Butterfly;
    using GraphicsArt.Butterfly.GPUBase_Butterfly;
    using System.Runtime.InteropServices;

    public class ButterflyTrailCalNode : MonoBehaviour
    {
         int kernel_NextInputPos;
         int kernel_NodeInfo;

        /////////////////////////////

          #region define struct
        
        struct trail{
            int nextCalNodeIndex;

            public trail(int index){
                this.nextCalNodeIndex=index;
            }
        }

        struct node{
            Vector3 node_position;
            float node_life;
            //自身が更新されてないときとnode のnode_positionが初期値のままの時にフラグを切る為の変数
            int renderFlag;

            public node(Vector3 node_position,float node_life){
                this.node_position=node_position;
                this.node_life=node_life;
                this.renderFlag=-1;
            }
        }

        struct input_data{
            Vector3 nextInputPosition;
            public input_data(Vector3 nextInputPosition){
                this.nextInputPosition=nextInputPosition;
            }
        }

        #endregion
        
        #region public field
        public ComputeShader trail_cs;
        [SerializeField] GPUBoids_Butterfly gPUBoids_Butterfly;
        public static GPUTrail_Butterfly instance=null;
        public int nodeSegment=60;
        [SerializeField] int initNodeLife=5;
        [HideInInspector] public int count=0;
        [HideInInspector] public int nodeSum=0;
        [SerializeField] float nodeDistanceMin=1.0f;
        
        [HideInInspector] public ComputeBuffer buffer_trail;
        [HideInInspector] public ComputeBuffer buffer_node;
        [HideInInspector] public ComputeBuffer buffer_input;

          [HideInInspector] public ComputeBuffer debug_buffer_node_position;
      
        #endregion

        #region private field

        node[] init_node;
        Matrix4x4[] debug_position;

        #endregion
        
        void Start()
        {
            kernel_NextInputPos=trail_cs.FindKernel("NextInputPos");
            kernel_NodeInfo=trail_cs.FindKernel("NodeInfo");

            //////
             //gPUBoids_Butterfly=this.gameObject.GetComponent<GPUBoids_Butterfly>();

            count=GPUBase_Butterfly.instance.count;
            nodeSum=count*nodeSegment;

            buffer_trail=new ComputeBuffer(count,Marshal.SizeOf(typeof(trail)));
            buffer_node=new ComputeBuffer(nodeSum,Marshal.SizeOf(typeof(node)));
            buffer_input=new ComputeBuffer(count,Marshal.SizeOf(typeof(input_data)));
            
          
            trail[] init_trail=new trail[count];
            init_node=new node[nodeSum];
            input_data[] init_input_Data=new input_data[count];


            debug_position=new Matrix4x4[nodeSum];

            for(int i=0;i<count;i++){
                init_trail[i]=new trail(0);
                init_input_Data[i]=new input_data(new Vector3(0,0,0));

                for(int q=0;q<nodeSegment;q++){
                        init_node[nodeSegment*i+q]=new node(new Vector3(0f,0f,0f),initNodeLife);
                        debug_position[nodeSegment*i+q]=Matrix4x4.identity;
                }
            }

            buffer_trail.SetData(init_trail);
            buffer_node.SetData(init_node);
            buffer_input.SetData(init_input_Data);

          //debug
            debug_buffer_node_position=new ComputeBuffer(nodeSum,Marshal.SizeOf(typeof(Matrix4x4)));
            debug_buffer_node_position.SetData(debug_position);

            
        }

        void Update()
        {
            //NextInputPos
            trail_cs.SetBuffer(kernel_NextInputPos,"_nextCalTrailPosition_write",buffer_input);
            trail_cs.SetBuffer(kernel_NextInputPos,"_boids_data_read",gPUBoids_Butterfly.comouteBuffer_boids_data);
            trail_cs.Dispatch(kernel_NextInputPos,count/256,1,1);

            //NodeInfo
            trail_cs.SetBuffer(kernel_NodeInfo,"_nextCalTrailPosition_read",buffer_input);
            trail_cs.SetBuffer(kernel_NodeInfo,"_trailIndexData_write",buffer_trail);
            trail_cs.SetBuffer(kernel_NodeInfo,"_trailIndexData_read",buffer_trail);
            trail_cs.SetBuffer(kernel_NodeInfo,"_node_data_write",buffer_node);
            trail_cs.SetBuffer(kernel_NodeInfo,"_node_data_read",buffer_node);

            trail_cs.SetBuffer(kernel_NodeInfo,"_debug_buffer_node_position",debug_buffer_node_position);

            trail_cs.SetInt("_nodeSegment",nodeSegment);
            trail_cs.SetFloat("_nodeDistanceMin",nodeDistanceMin);
            trail_cs.SetFloat("_DTime",Time.deltaTime);
            trail_cs.Dispatch(kernel_NodeInfo,nodeSum/512,1,1);

            debug_buffer_node_position.GetData(debug_position);
            Debug.Log("init_node[500-20]:"+debug_position[500-20]);
        }

        void OnDisable(){
            buffer_trail.Release();
            buffer_node.Release();
            buffer_input.Release();
        }
    }

}