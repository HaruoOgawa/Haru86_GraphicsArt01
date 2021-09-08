namespace GraphicsArt.GPUFlower.GPUFlower_Leaf{

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphicsArt.GPUFlower.GPUFlower_Base;
using GraphicsArt.GPUFlower.GPUFlower_Stem;

    public class GPUFlower_Leaf : MonoBehaviour
    {
        #region public field
    [SerializeField] GPUFlower_Base gPUFlower_Base;
    [SerializeField] GPUFlower_Stem gPUFlower_Stem;
    [SerializeField] Material leaf_mat;
    [SerializeField] PetalData[] petalDatas;
    #endregion

    #region private region

    Mesh leaf_mesh;
   
    #endregion
   
        void Start()
        {
            Init();
        }

        void Update()
        {
            //if(gPUFlower_Base.flowersIsDone&&gPUFlower_Base.stemIsDone){
            if(gPUFlower_Base.flowersIsDone&&gPUFlower_Base.stemIsDone&&gPUFlower_Base.leafIsDone){
                leaf_mat.SetBuffer("_stemDataLeaf_buffer",gPUFlower_Stem.stemDataLeaf_buffer);
                Graphics.DrawMeshInstancedProcedural(leaf_mesh,0,leaf_mat,new Bounds(this.gameObject.transform.position,Vector3.one*500.0f),gPUFlower_Base.count);
            }
        }

        void OnDisable(){
           
        }

        void Init(){
            leaf_mesh=new Mesh();
            
            SetupFlowerdata();
        }

        void SetupFlowerdata(){
            GPUFlower_Base.BaseFlower_Data flower_data=new GPUFlower_Base.BaseFlower_Data();
            flower_data=GPUFlower_Base.Cal_BSpline_Surface(petalDatas[0].controlPoints,petalDatas[0].knotMin,petalDatas[0].knotMax,petalDatas[0].tWidth);
            
            leaf_mesh.vertices=flower_data.vertices.ToArray();
            leaf_mesh.triangles=flower_data.triangles.ToArray();
            leaf_mesh.normals=flower_data.normals.ToArray();
            leaf_mesh.RecalculateNormals();

            gPUFlower_Base.leafIsDone=true;
        }

    }

}