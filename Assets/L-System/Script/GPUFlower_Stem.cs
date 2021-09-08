namespace GraphicsArt.GPUFlower.GPUFlower_Stem{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Runtime.InteropServices;
    using GraphicsArt.GPUFlower.GPUFlower_Base;
 
    public class GPUFlower_Stem : MonoBehaviour
    {
        #region public field
        // stemVertexData Struct 
        // [HideInInspector] public ComputeBuffer stemVertex_buffer;
        // [HideInInspector] public ComputeBuffer stemManage_buffer;
        [HideInInspector] public ComputeBuffer stemDataFlower_buffer;
        [SerializeField] GPUFlower_Base gPUFlower_Base;
        #endregion
        
        #region private field
        struct StemVertex{
            Vector3 vertice;
            Vector3 normal;
            Vector3 index;
        }

        struct StemManage{
            int flowerCount;
            int flowerStartIndex;
            int leafCount;
            int leafStartIndex;
        }

        struct StemDataFlower{
            int index;
            Vector3 position;
            Vector3 tangent;
            Vector3 normal;
            Vector3 bioNormal;
            public StemDataFlower(int i,Vector3 p,Vector3 t,Vector3 n,Vector3 b){
                this.index=i;
                this.position=p;
                this.tangent=t;
                this.normal=n;
                this.bioNormal=b;
            }
        }
        #endregion
        void Start()
        {
            Init();
        }

        void Update()
        {
            
        }

        void OnDisable(){
            // stemVertex_buffer.Release();
            // stemManage_buffer.Release();
            stemDataFlower_buffer.Release();
        }

        void Init(){
            //stemVertex_buffer=new ComputeBuffer(,Marshal.SizeOf(typeof(StemVertex)));
            //stemManage_buffer=new ComputeBuffer(,Marshal.SizeOf(typeof(StemManage)));
            stemDataFlower_buffer=new ComputeBuffer(gPUFlower_Base.count,Marshal.SizeOf(typeof(StemDataFlower)));

            InitBufferData();
        }

        void InitBufferData(){
            List<StemDataFlower> initStemData=new List<StemDataFlower>();
            for(int i=0;i<gPUFlower_Base.count;i++){
                Vector2 initPos=Random.insideUnitCircle;
                StemDataFlower data=new StemDataFlower(i,new Vector3(initPos.x,0,initPos.y),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0));
                initStemData.Add(data);
            }
            stemDataFlower_buffer.SetData(initStemData.ToArray());

            //test
            gPUFlower_Base.stemIsDone=true;
        }
    }

}