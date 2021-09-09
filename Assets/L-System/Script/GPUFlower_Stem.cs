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
        [HideInInspector] public ComputeBuffer stemResult_buffer;
        [HideInInspector] public ComputeBuffer stemVertex_buffer;
        [HideInInspector] public ComputeBuffer stemManage_buffer;
        [HideInInspector] public ComputeBuffer stemDataFlower_buffer;
        [HideInInspector] public ComputeBuffer stemDataLeaf_buffer;
        [SerializeField] GPUFlower_Base gPUFlower_Base;
        [SerializeField] BSplineData bSplineData;

        #endregion
        
        #region private field

        //stem本体の情報
        struct StemVertex{
            Vector3 vertice;
            Vector3 tangent;
            Vector3 normal;
            Vector3 bioNormal;
            int index;
            public StemVertex(int i){
                this.vertice=new Vector3(0,0,0);
                this.tangent=new Vector3(0,0,0);
                this.normal=new Vector3(0,0,0);
                this.bioNormal=new Vector3(0,0,0);
                this.index=i;
            }
        }

        //花の数など
        struct StemManage{
            int flowerCount;
            int flowerStartIndex;
            int leafCount;
            int leafStartIndex;
            public StemManage(int fCount){
                this.flowerCount=fCount;
                this.flowerStartIndex=1;
                this.leafCount=1;
                this.leafStartIndex=1;
            }
        }

        //花や茎を生成するための情報を載せる構造体
        struct StemData{
            int index;
            Vector3 position;
            Vector3 tangent;
            Vector3 normal;
            Vector3 bioNormal;
            public StemData(int i,Vector3 p,Vector3 t,Vector3 n,Vector3 b){
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
            stemResult_buffer.Release();
            stemVertex_buffer.Release();
            stemManage_buffer.Release();
            stemDataFlower_buffer.Release();
            stemDataLeaf_buffer.Release();
        }

        void Init(){
            int stemVertexCount=(int)((bSplineData.knotMax-bSplineData.knotMin)/bSplineData.tWidth);
            stemResult_buffer=new ComputeBuffer(stemVertexCount*gPUFlower_Base.count,Marshal.SizeOf(typeof(StemVertex)));
            stemVertex_buffer=new ComputeBuffer(stemVertexCount*gPUFlower_Base.count,Marshal.SizeOf(typeof(StemVertex)));
            stemManage_buffer=new ComputeBuffer(gPUFlower_Base.count,Marshal.SizeOf(typeof(StemManage)));
            stemDataFlower_buffer=new ComputeBuffer(gPUFlower_Base.count,Marshal.SizeOf(typeof(StemData)));
            stemDataLeaf_buffer=new ComputeBuffer(gPUFlower_Base.count*2,Marshal.SizeOf(typeof(StemData)));

            InitBufferData();
        }

        void InitBufferData(){
            List<StemVertex> initStemVertex=new List<StemVertex>();
            List<StemManage> initStemManege=new List<StemManage>();
            List<StemData> initStemData=new List<StemData>();
            List<StemData> initStemDataLeaf=new List<StemData>();
            for(int i=0;i<gPUFlower_Base.count;i++){
                Vector2 initPos=Random.insideUnitCircle;
                StemData data=new StemData(i,new Vector3(initPos.x,0,initPos.y),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0));
                
                initStemData.Add(data);
                initStemDataLeaf.Add(data);
                initStemDataLeaf.Add(data);

                initStemVertex.Add(new StemVertex(i));
                initStemManege.Add(new StemManage(1));
            }
            stemResult_buffer.SetData(initStemVertex.ToArray());
            stemVertex_buffer.SetData(initStemVertex.ToArray());
            stemManage_buffer.SetData(initStemManege.ToArray());
            stemDataFlower_buffer.SetData(initStemData.ToArray());
            stemDataLeaf_buffer.SetData(initStemDataLeaf.ToArray());

            //test
            gPUFlower_Base.stemIsDone=true;
            
        }

        void Cal_Stem(){

        }
    }

}