using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class CPU_Flower_Simulation : MonoBehaviour
{
    struct MultiFlower_Data{
        public Vector3 position;
        public MultiFlower_Data(Vector3 pos){
            this.position=pos;
        }
    }

    #region flower field
    ComputeBuffer multiFlower_buffer;
    [HideInInspector]public Mesh multiFlowerMesh;
    [HideInInspector] public bool isDone=false;
    [SerializeField] Material multiFlowerMat;
    #endregion

    #region Leaf field
    ComputeBuffer firstLeaf_buffer;
    ComputeBuffer secondLeaf_buffer;
    [HideInInspector] public Mesh firstLeafMesh;
    [HideInInspector] public Mesh secondLeafMesh;
    [HideInInspector] public bool leafIsDone=false;
    [SerializeField] Material leafMat;
    #endregion

    #region Stem field
    ComputeBuffer Stem_buffer;
    [HideInInspector]public Mesh StemMesh;
    [HideInInspector] public bool StemIsDone=false;
    [SerializeField] Material StemMat;
    #endregion

    [SerializeField] int count=1000;
    
    
    void Awake(){
        isDone=false;
        leafIsDone=false;
        StemIsDone=false;
    }
    void Start()
    {
       Matrix4x4[] init_data=new Matrix4x4[count];
       for(int i=0;i<count;i++){
           Vector2 randomPos=Random.insideUnitCircle*200.0f;
           float randSize=Random.Range(0.5f,1.0f);
           init_data[i]=Matrix4x4.TRS(
               new Vector3(randomPos.x,0,randomPos.y),
               Quaternion.Euler(0,Random.Range(0.0f,2.0f*Mathf.PI),0),
               new Vector3(randSize,randSize,randSize)
           );
       }

       multiFlower_buffer=new ComputeBuffer(count,Marshal.SizeOf(typeof(Matrix4x4)));
       multiFlower_buffer.SetData(init_data);
    }

    void Update()
    {
        if(isDone){
            multiFlowerMat.SetBuffer("_multiFlower_Data",multiFlower_buffer);
            Graphics.DrawMeshInstancedProcedural(multiFlowerMesh,0,multiFlowerMat,new Bounds(this.gameObject.transform.position,Vector3.one*500f),count);
        }

        if(leafIsDone){
            Graphics.DrawMeshInstancedProcedural(firstLeafMesh,0,multiFlowerMat,new Bounds(this.gameObject.transform.position,Vector3.one*500f),count);
            Graphics.DrawMeshInstancedProcedural(secondLeafMesh,0,multiFlowerMat,new Bounds(this.gameObject.transform.position,Vector3.one*500f),count);
        }

        if(StemIsDone){
            Graphics.DrawMeshInstancedProcedural(StemMesh,0,StemMat,new Bounds(this.gameObject.transform.position,Vector3.one*500f),count);
        }
    }

   
    
}
