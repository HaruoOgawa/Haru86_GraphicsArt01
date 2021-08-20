namespace GraphicsArt.Butterfly.ButterflyTrailCalNode{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.Butterfly.GPUTrail_Butterfly;

    public class ButterflyTrailCalNode : MonoBehaviour
    {
         GPUTrail_Butterfly trail_instance=null;
         int kernel_NodeInfo;
        void Start()
        {
            trail_instance=GPUTrail_Butterfly.instance;
            kernel_NodeInfo=trail_instance.trail_cs.FindKernel("NodeInfo");

        }

        void Update()
        {
            trail_instance.trail_cs.SetBuffer(kernel_NodeInfo,"_nextCalTrailPosition_read",trail_instance.buffer_input);
            trail_instance.trail_cs.SetBuffer(kernel_NodeInfo,"_trailIndexData_write",trail_instance.buffer_trail);
            trail_instance.trail_cs.SetBuffer(kernel_NodeInfo,"_trailIndexData_read",trail_instance.buffer_trail);
            trail_instance.trail_cs.SetBuffer(kernel_NodeInfo,"_node_data_write",trail_instance.buffer_node);
            trail_instance.trail_cs.SetBuffer(kernel_NodeInfo,"_node_data_read",trail_instance.buffer_node);
            trail_instance.trail_cs.Dispatch(kernel_NodeInfo,trail_instance.count/256,1,1);
        }
    }

}