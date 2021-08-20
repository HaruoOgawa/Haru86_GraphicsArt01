namespace GraphicsArt.Butterfly.GPUTrail_Butterfly{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.Butterfly.ButterflyTrailCalNode;
    

    public class GPUTrail_Butterfly : MonoBehaviour
    {
        [SerializeField] Material butterfly_trail_mat;
        [SerializeField] float trailWidth=1.0f;
        [SerializeField] ButterflyTrailCalNode butterflyTrailCalNode;
      

         
        void Start()
        {
           

        }

        void Update()
        {
            butterfly_trail_mat.SetBuffer("_node_data_read",butterflyTrailCalNode.buffer_node);
            butterfly_trail_mat.SetFloat("_TrailWidth",trailWidth);
            butterfly_trail_mat.SetInt("_nodeSegment",butterflyTrailCalNode.nodeSegment);
            butterfly_trail_mat.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points,butterflyTrailCalNode.nodeSegment,butterflyTrailCalNode.nodeSum);

           
        }

        
    }

}