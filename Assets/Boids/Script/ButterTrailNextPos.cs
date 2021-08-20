namespace GraphicsArt.Butterfly.ButterTrailNextPos{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.Butterfly.GPUTrail_Butterfly;

    public class ButterTrailNextPos : MonoBehaviour
    {
        int kernel_NextInputPos;
        GPUTrail_Butterfly trail_instance=null;
        void Start()
        {
            trail_instance=GPUTrail_Butterfly.instance;
            kernel_NextInputPos=trail_instance.trail_cs.FindKernel("NextInputPos");
        }

        void Update()
        {
            trail_instance.trail_cs.SetBuffer(kernel_NextInputPos,"_nextCalTrailPosition_write",trail_instance.buffer_input);
            trail_instance.trail_cs.SetBuffer(kernel_NextInputPos,"_boids_data_read",trail_instance.gPUBoids_Butterfly.comouteBuffer_boids_data);
            trail_instance.trail_cs.Dispatch(kernel_NextInputPos,trail_instance.count/256,1,1);
        }
    }

}