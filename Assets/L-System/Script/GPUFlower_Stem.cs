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
        [HideInInspector] public ComputeBuffer stemDataLeaf_buffer;
        [HideInInspector] public ComputeBuffer stemBasePosition_buffer;
        // ComputeBuffer stem_debug_bufer;
        [SerializeField] ComputeShader cal_stem_cs;
        
        [SerializeField] ComputeShader cal_leaf_cs;
        [SerializeField] GPUFlower_Base gPUFlower_Base;
        [SerializeField] BSplineData bSplineData;
        [SerializeField] Material stem_mat;
        [SerializeField] float stemRadius=1.0f;
        [SerializeField] int stemSegments=12;
        [SerializeField] float stemLength=1.0f;

        #endregion

        #region Test Field
        [Range(0.0f,1.0f)]
        [SerializeField] float testLife=0.1f;
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
            public float stemLifeVal;
            public float stemWaitTime;
            public float signNum;
            public int manageLifeCountFlag;
            int flowerCount;
            int flowerStartIndex;
            int leafCount;
            int leafStartIndex;
            public StemManage(int fCount){
                this.stemLifeVal=Random.Range(0.0f,1.0f);
                this.stemWaitTime=0.0f;
                this.signNum=Mathf.Sign(Random.Range(-1.0f,1.0f));
                this.manageLifeCountFlag=1;
                this.flowerCount=fCount;
                this.flowerStartIndex=1;
                this.leafCount=1;
                this.leafStartIndex=1;
            }
        }

        //花や茎を生成するための情報を載せる構造体
        public struct StemData{
            int resampleIndex;
            int resampleGroupIndex;
            Vector3 position;
            Vector3 tangent;
            Vector3 normal;
            Vector3 bioNormal;
            int renderFlag;
            float lifeTime;
            public StemData(int i,Vector3 p,Vector3 t,Vector3 n,Vector3 b){
                this.resampleIndex=i;
                this.resampleGroupIndex=-1;
                this.position=p;
                this.tangent=t;
                this.normal=n;
                this.bioNormal=b;
                this.renderFlag=0;
                this.lifeTime=0;
            }
        }

        #region Stem_Cs Field
          int stemVertexCount=-1;

        int stemResult_kernel=-1;
        int InitStemGrowth_kernel=-1;
        int CalStemManage_kernel;
        int stemGrowth_kernel=-1;

        
        [HideInInspector] public int numthreds_val=256;
        #endregion

        #endregion
        void Start()
        {
            Init();
            Cal_Stem_Result();
            Init_Stem_Growth();

            gPUFlower_Base.stemIsDone=true;
        }

        void Update()
        {
            Cal_Stem_Manage();
            Cal_Stem_Growth();
            
            Render_Stem();
        }

        void OnDisable(){
            stemResult_buffer.Release();
            stemVertex_buffer.Release();
            stemManage_buffer.Release();
            stemDataLeaf_buffer.Release();
            stemBasePosition_buffer.Release();

            // stem_debug_bufer.Release();
        }

        #region Init Field
        void Init(){
            
            stemResult_kernel=cal_stem_cs.FindKernel("CalStemBSplineCurveResult");
            InitStemGrowth_kernel=cal_stem_cs.FindKernel("InitStemGrowth");
            CalStemManage_kernel=cal_stem_cs.FindKernel("CalStemManage");
            stemGrowth_kernel=cal_stem_cs.FindKernel("CalStemGrowth");

            stemVertexCount=(int)((bSplineData.knotMax-bSplineData.knotMin)/bSplineData.tWidth);
            stemResult_buffer=new ComputeBuffer(stemVertexCount*gPUFlower_Base.count,Marshal.SizeOf(typeof(StemVertex)));
            stemVertex_buffer=new ComputeBuffer(stemVertexCount*gPUFlower_Base.count,Marshal.SizeOf(typeof(StemVertex)));
            stemManage_buffer=new ComputeBuffer(gPUFlower_Base.count,Marshal.SizeOf(typeof(StemManage)));
            stemDataLeaf_buffer=new ComputeBuffer(gPUFlower_Base.count*2,Marshal.SizeOf(typeof(StemData)));
            stemBasePosition_buffer=new ComputeBuffer(gPUFlower_Base.count,Marshal.SizeOf(typeof(Vector3)));

            // stem_debug_bufer=new ComputeBuffer(gPUFlower_Base.count,Marshal.SizeOf(typeof(Matrix4x4)));

            InitBufferData();
        }

        void InitBufferData(){
            List<StemVertex> initStemVertex=new List<StemVertex>();
            List<StemManage> initStemManege=new List<StemManage>();
            List<StemData> initStemDataLeaf=new List<StemData>();
            List<Vector3> initStemBasePosition=new List<Vector3>();

            List<Matrix4x4> initStemDebugMatrix=new List<Matrix4x4>();

            for(int i=0;i<gPUFlower_Base.count;i++){
                Vector2 initPos=Random.insideUnitCircle;
                StemData data=new StemData(i,new Vector3(initPos.x,0,initPos.y),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0));
                
                initStemDataLeaf.Add(data);
                initStemDataLeaf.Add(data);

                initStemVertex.Add(new StemVertex(i));
                
                StemManage stemManage=new StemManage(1);
                if(stemManage.stemLifeVal==0.0f||stemManage.stemLifeVal==1.0f){
                    stemManage.stemWaitTime=Random.Range(1.0f,3.0f);
                    stemManage.manageLifeCountFlag=0;
                }
                initStemManege.Add(stemManage);

                Vector2 initBasePos=Random.insideUnitCircle*500.0f;
                initStemBasePosition.Add(new Vector3(initBasePos.x,0,initBasePos.y));

                initStemDebugMatrix.Add(Matrix4x4.identity);
            }
            stemResult_buffer.SetData(initStemVertex.ToArray());
            stemVertex_buffer.SetData(initStemVertex.ToArray());
            stemManage_buffer.SetData(initStemManege.ToArray());
            stemDataLeaf_buffer.SetData(initStemDataLeaf.ToArray());
            stemBasePosition_buffer.SetData(initStemBasePosition.ToArray());

            // stem_debug_bufer.SetData(initStemDebugMatrix.ToArray());

            
        }

        #endregion

        #region Cal Stem Field
        void Cal_Stem_Result(){
            cal_stem_cs.SetBuffer(stemResult_kernel,"_write_stemResult_buffer",stemResult_buffer);
            cal_stem_cs.SetBuffer(stemResult_kernel,"_read_stemBasePosition_buffer",stemBasePosition_buffer);
            List<Vector4> contPos=new List<Vector4>(); 
            cal_stem_cs.SetInt("_contPosArrayLength",bSplineData.controlPoints.Count);
            for(int i=0;i<bSplineData.controlPoints.Count;i++){
                Vector3 controlPoint=bSplineData.controlPoints[i];
                contPos.Add(new Vector4(controlPoint.x,controlPoint.y*stemLength,controlPoint.z,0));
            }
            cal_stem_cs.SetVectorArray("_controlPoints",contPos.ToArray());
            cal_stem_cs.SetInt("_stemVertexCount",stemVertexCount);
            cal_stem_cs.SetFloat("_knotMin",bSplineData.knotMin);
            cal_stem_cs.SetFloat("_knotMax",bSplineData.knotMax);
            cal_stem_cs.SetFloat("_tWidth",bSplineData.tWidth);
            cal_stem_cs.Dispatch(stemResult_kernel,(stemVertexCount*gPUFlower_Base.count)/numthreds_val,1,1); 
        }

        void Init_Stem_Growth(){
            cal_stem_cs.SetBuffer(InitStemGrowth_kernel,"_read_stemResult_buffer",stemResult_buffer);
            cal_stem_cs.SetBuffer(InitStemGrowth_kernel,"_write_stemVertex_buffer",stemVertex_buffer);
            cal_stem_cs.SetBuffer(InitStemGrowth_kernel,"_read_stemManage_buffer",stemManage_buffer);
            cal_stem_cs.SetInt("_stemVertexCount",stemVertexCount);
            // cal_stem_cs.SetFloat("_testLife",testLife);
            cal_stem_cs.Dispatch(InitStemGrowth_kernel,(stemVertexCount*gPUFlower_Base.count)/numthreds_val,1,1);
        }

        void Cal_Stem_Manage(){
            cal_stem_cs.SetBuffer(CalStemManage_kernel,"_write_stemManage_buffer",stemManage_buffer);
            cal_stem_cs.SetFloat("_DTime",Time.deltaTime);
            //cal_stem_cs.SetBuffer(CalStemManage_kernel,"_write_stem_debug_bufer",stem_debug_bufer);
            cal_stem_cs.Dispatch(CalStemManage_kernel,gPUFlower_Base.count/numthreds_val,1,1);
        }
        void Cal_Stem_Growth(){
            cal_stem_cs.SetInt("_stemVertexCount",stemVertexCount);
            cal_stem_cs.SetBuffer(stemGrowth_kernel,"_read_stemResult_buffer",stemResult_buffer);
            cal_stem_cs.SetBuffer(stemGrowth_kernel,"_write_stemVertex_buffer",stemVertex_buffer);
            cal_stem_cs.SetBuffer(stemGrowth_kernel,"_read_stemManage_buffer",stemManage_buffer);
            //cal_stem_cs.SetBuffer(stemGrowth_kernel,"_write_stem_debug_bufer",stem_debug_bufer);
            cal_stem_cs.Dispatch(stemGrowth_kernel,(stemVertexCount*gPUFlower_Base.count)/numthreds_val,1,1);
        }

        void Render_Stem(){
            Mesh stem_point_mesh=new Mesh();
            
            Vector3[] vertices=new Vector3[1]{new Vector3(0,0,0)};
            int[] indices=new int[1]{0};

            stem_point_mesh.vertices=vertices;
            stem_point_mesh.SetIndices(indices,MeshTopology.Points,0);

            stem_mat.SetBuffer("_stemVertex_buffer",stemVertex_buffer);
            stem_mat.SetInt("_stemVertexCount",stemVertexCount);
            stem_mat.SetInt("_stemSegments",stemSegments);
            stem_mat.SetFloat("_stemRadius",stemRadius);
            stem_mat.SetFloat("_stemLength",stemLength);
            
            Graphics.DrawMeshInstancedProcedural(stem_point_mesh,0,stem_mat,new Bounds(this.gameObject.transform.position,Vector3.one*500.0f),stemVertexCount*gPUFlower_Base.count);
        }

        #endregion


    }

}


// Matrix4x4[] resultStemVertex=new Matrix4x4[stemVertexCount*gPUFlower_Base.count];
            // stem_debug_bufer.GetData(resultStemVertex);
            // int debug_offset=10;
            // for(int i=0+debug_offset*stemVertexCount;i<stemVertexCount+debug_offset*stemVertexCount;i++){
            //     Debug.Log("resultStemVertex["+i+"] "+resultStemVertex[i]);
            // }