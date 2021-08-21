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
            int leftSideFirst;
            int rightSideFirst;
            int rightSideSecond;

            public trail(int index,int nodeSegment){
                this.nextCalNodeIndex=index;
                this.leftSideFirst=0;
                this.rightSideFirst=nodeSegment-1;
                this.rightSideSecond=nodeSegment-2;
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
                this.renderFlag=0;
                //this.renderFlag=-1;
            }
        }

        struct Butterfly{
            public Vector3 position;
            public Vector3 velocity;
            
            public Butterfly(Vector3 pos,Vector3 vec){
                this.position=pos;
                this.velocity=vec;
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
        public int initNodeLife=5;
        [HideInInspector] public int count=0;
        [HideInInspector] public int nodeSum=0;
        [SerializeField] float nodeDistanceMin=1.0f;
        
        [HideInInspector] public ComputeBuffer buffer_trail;
        [HideInInspector] public ComputeBuffer buffer_node;
        [HideInInspector] public ComputeBuffer buffer_input;

         // [HideInInspector] public ComputeBuffer debug_buffer_node_position;
      
        #endregion

        #region private field

       
       // Matrix4x4[] debug_position;
        bool RenderTrailFlag=false;

        #endregion
        
        void Start()
        {
            RenderTrailFlag=false;
            kernel_NextInputPos=trail_cs.FindKernel("NextInputPos");
            kernel_NodeInfo=trail_cs.FindKernel("NodeInfo");

            count=GPUBase_Butterfly.instance.count;
            nodeSum=count*nodeSegment;

            buffer_trail=new ComputeBuffer(count,Marshal.SizeOf(typeof(trail)));
            buffer_node=new ComputeBuffer(nodeSum,Marshal.SizeOf(typeof(node)));
            buffer_input=new ComputeBuffer(count,Marshal.SizeOf(typeof(input_data)));
            
          
            trail[] init_trail=new trail[count];
            input_data[] init_input_Data=new input_data[count];


           

            for(int i=0;i<count;i++){
                init_trail[i]=new trail(0,nodeSegment);
                init_input_Data[i]=new input_data(new Vector3(0,0,0));

            }

            buffer_trail.SetData(init_trail);
            buffer_input.SetData(init_input_Data);

      

            StartCoroutine(TrailRenderFlag());
            
        }

        IEnumerator TrailRenderFlag(){

            yield return new WaitUntil(()=>gPUBoids_Butterfly.trailRenderStartFlag);

            //get node buffer init data
            Butterfly[] butterfly_init_data=new Butterfly[count];
            gPUBoids_Butterfly.comouteBuffer_boids_data.GetData(butterfly_init_data);

            // //debug
            // debug_position=new Matrix4x4[nodeSum];

            node[] init_node=new node[nodeSum];
            for(int i=0;i<count;i++){
                for(int q=0;q<nodeSegment;q++){
                        
                        init_node[nodeSegment*i+q]=new node(butterfly_init_data[i].position,initNodeLife);
                       
                        // //debug
                        // debug_position[nodeSegment*i+q]=Matrix4x4.identity;
                }
            }

            buffer_node.SetData(init_node);

            // //debug
            // debug_buffer_node_position=new ComputeBuffer(nodeSum,Marshal.SizeOf(typeof(Matrix4x4)));
            // debug_buffer_node_position.SetData(debug_position);
           
            RenderTrailFlag=true;
        }

        void Update()
        {
         
        }

        void LateUpdate(){
               StartCoroutine(RenderTrail());
        }

        IEnumerator RenderTrail(){

            yield return new WaitUntil(()=>RenderTrailFlag);

             //NextInputPos
            trail_cs.SetBuffer(kernel_NextInputPos,"_input_data_write",buffer_input);
            trail_cs.SetBuffer(kernel_NextInputPos,"_boids_data_read",gPUBoids_Butterfly.comouteBuffer_boids_data);
            trail_cs.Dispatch(kernel_NextInputPos,count/256,1,1);

            //NodeInfo
            trail_cs.SetBuffer(kernel_NodeInfo,"_input_data_read",buffer_input);
            trail_cs.SetBuffer(kernel_NodeInfo,"_trailIndexData_read",buffer_trail);
            trail_cs.SetBuffer(kernel_NodeInfo,"_node_data_read",buffer_node);

            // //debug
            // trail_cs.SetBuffer(kernel_NodeInfo,"_debug_buffer_node_position",debug_buffer_node_position);

            trail_cs.SetInt("_nodeSegment",nodeSegment);
            trail_cs.SetFloat("_nodeDistanceMin",nodeDistanceMin);
            trail_cs.SetFloat("_DTime",Time.deltaTime);
            trail_cs.SetFloat("_initNodeLife",initNodeLife);
            trail_cs.Dispatch(kernel_NodeInfo,count/256,1,1);

            // //debug
            // debug_buffer_node_position.GetData(debug_position);
            // Debug.Log("debug trail data init_node[5]:"+debug_position[5]);
        }

        void OnDisable(){
            buffer_trail.Release();
            buffer_node.Release();
            buffer_input.Release();
          //  debug_buffer_node_position.Release();
        }
    }

}