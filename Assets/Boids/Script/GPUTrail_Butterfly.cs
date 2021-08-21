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
      

        #region debug

        [SerializeField] Mesh debug_mesh;

        #endregion
         
        void Start()
        {
           

        }

        // void Update()
        // {


        //     butterfly_trail_mat.SetBuffer("_node_data_read",butterflyTrailCalNode.buffer_node);
        //     butterfly_trail_mat.SetBuffer("_trail_data_read",butterflyTrailCalNode.buffer_trail);
        //     butterfly_trail_mat.SetFloat("_TrailWidth",trailWidth);
        //     butterfly_trail_mat.SetInt("_nodeSegment",butterflyTrailCalNode.nodeSegment);
        //     //butterfly_trail_mat.SetPass(0);
        //     Mesh pointMesh=new Mesh();
        //     Vector3[] vertex=new Vector3[1];
        //     vertex[0]=new Vector3(0,0,0);
        //     pointMesh.vertices=vertex;
        //     Graphics.DrawMeshInstancedProcedural(pointMesh,0,butterfly_trail_mat,new Bounds(transform.position,Vector3.one*400.0f),butterflyTrailCalNode.nodeSum);
        //     //Graphics.DrawProceduralNow(MeshTopology.Points,butterflyTrailCalNode.nodeSegment,butterflyTrailCalNode.nodeSum);

           
        // }

          void OnRenderObject(){
             butterfly_trail_mat.SetBuffer("_node_data_read",butterflyTrailCalNode.buffer_node);
              butterfly_trail_mat.SetBuffer("_trail_data_read",butterflyTrailCalNode.buffer_trail);
            butterfly_trail_mat.SetFloat("_TrailWidth",trailWidth);
            butterfly_trail_mat.SetInt("_nodeSegment",butterflyTrailCalNode.nodeSegment);
            butterfly_trail_mat.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points,butterflyTrailCalNode.nodeSegment,butterflyTrailCalNode.nodeSum);
        }
        
    }

}