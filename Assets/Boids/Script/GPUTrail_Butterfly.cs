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
        [HideInInspector] public GPUBoids_Butterfly gPUBoids_Butterfly;
        public static GPUTrail_Butterfly instance=null;
        [SerializeField] Material butterfly_trail_mat;
        public int nodeSegment=60;
        [SerializeField] int initNodeLife=5;
        [HideInInspector] public int count=0;
        [HideInInspector] public int nodeSum=0;
        [HideInInspector] public float nodeDistanceMin=1.0f;
        
        [HideInInspector] public ComputeBuffer buffer_trail;
        [HideInInspector] public ComputeBuffer buffer_node;
        [HideInInspector] public ComputeBuffer buffer_input;
      
        #endregion

        #region private field

        
        

        #endregion

        void Awake(){
            if(instance==null) instance=this;
            gPUBoids_Butterfly=this.gameObject.GetComponent<GPUBoids_Butterfly>();
        }   
        void Start()
        {
            count=GPUBase_Butterfly.instance.count;
            nodeSum=count*nodeSegment;

            buffer_trail=new ComputeBuffer(count,Marshal.SizeOf(typeof(trail)));
            buffer_node=new ComputeBuffer(nodeSum,Marshal.SizeOf(typeof(node)));
            buffer_input=new ComputeBuffer(count,Marshal.SizeOf(typeof(input_data)));

            trail[] init_trail=new trail[count];
            node[] init_node=new node[count];
            input_data[] init_input_Data=new input_data[count];



        for(int i=0;i<count;i++){
            init_trail[i]=new trail(0);
           init_input_Data[i]=new input_data(new Vector3(0,0,0));

           for(int q=0;q<nodeSegment;q++){
                init_node[nodeSegment*i+q]=new node(new Vector3(0f,0f,0f),initNodeLife);
           }
        }

        buffer_trail.SetData(init_trail);
        buffer_node.SetData(init_node);
        buffer_input.SetData(init_input_Data);


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