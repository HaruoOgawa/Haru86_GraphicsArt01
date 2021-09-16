namespace GraphicsArt.GPUFlower.PetalGPUParticle{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.GPUFlower.GPUFlower_Base;

    using System.Runtime.InteropServices;

    public class PetalGPUParticle : MonoBehaviour
    {
        #region public feild
        [SerializeField] PetalData petalData;
        [SerializeField] Material petalMaterial;
        [SerializeField] GPUFlower_Base gPUFlower_Base;
        [SerializeField] int petalGroupCount=4;
        [SerializeField] ComputeShader cal_petalParticle_cs;
        [SerializeField] float maxPetalParticleHeight=250.0f;
        #endregion

        #region private region
        struct PetalAnimation{
            float petalLifeTime;
            Vector3 position;
            Vector3 rotation;
            Vector4 petalColor;
            Vector3 petalAngular;
            Vector3 petalVelocity;
            public PetalAnimation(float life,Vector3 p,Vector3 r,Vector4 col,Vector3 a,Vector3 v){
                this.petalLifeTime=life;
                this.position=p;
                this.rotation=r;
                this.petalColor=col;
                this.petalAngular=a;
                this.petalVelocity=v;
            }

        }
        ComputeBuffer petalAnim_buffer;
        ComputeBuffer petalBasePosition_buffer;
        int kernel_CalPetalGPUParticle;
        int numthread=256;
        Mesh petalGPUParticle_mesh;
        #endregion
        void Start()
        {
            Init();
        }

        void Update()
        {
            Cal_PetalParticle();
            Render_PetalGPUParticle();
        }

        void OnDisable(){
           petalAnim_buffer.Release();
           petalBasePosition_buffer.Release();
        }

        void Init(){
            kernel_CalPetalGPUParticle=cal_petalParticle_cs.FindKernel("CalPetalGPUParticle");
            CreatePetalMesh();
            InitBuffer();
        }

        void CreatePetalMesh(){
            petalGPUParticle_mesh=new Mesh();

            GPUFlower_Base.BaseFlower_Data basePeralParticle=GPUFlower_Base.Cal_BSpline_Surface(petalData.controlPoints,petalData.knotMin,petalData.knotMax,petalData.tWidth);
            petalGPUParticle_mesh.vertices=basePeralParticle.vertices.ToArray();
            petalGPUParticle_mesh.triangles=basePeralParticle.triangles.ToArray();
            petalGPUParticle_mesh.normals=basePeralParticle.normals.ToArray();
            petalGPUParticle_mesh.RecalculateNormals();
        }

        void InitBuffer(){
            List<PetalAnimation> initPetalAnimation=new List<PetalAnimation>();
            List<Vector3> initPetalBasePosition=new List<Vector3>();
            
            petalAnim_buffer=new ComputeBuffer(gPUFlower_Base.count*petalGroupCount,Marshal.SizeOf(typeof(PetalAnimation)));
            petalBasePosition_buffer=new ComputeBuffer(gPUFlower_Base.count*petalGroupCount,Marshal.SizeOf(typeof(Vector3)));
            
            for(int i=0;i<gPUFlower_Base.count*petalGroupCount;i++){
                Vector2 initPos=Random.insideUnitSphere*250.0f;
                Vector3 initAngular=Random.insideUnitSphere;
                Vector3 initVelocity=Random.insideUnitSphere;
                
                float randomScale=Random.Range(0.5f,2.0f);
                PetalAnimation petalAnimation=new PetalAnimation(
                    Random.Range(0.0f,1.0f),
                    new Vector3(initPos.x,Random.Range(0.0f,250.0f),initPos.y),
                    new Vector3(Random.Range(0.0f,360.0f),Random.Range(0.0f,360.0f),Random.Range(0.0f,360.0f)),
                    new Vector4(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),1.0f),
                    new Vector3(0,Random.Range(0.5f,2.0f),0),
                    new Vector3(0.0f,Random.Range(0.5f,2.0f),0.0f)
                );

                initPetalAnimation.Add(petalAnimation);
                initPetalBasePosition.Add(new Vector3(initPos.x,Random.Range(0.0f,250.0f),initPos.y));
            }

            petalAnim_buffer.SetData(initPetalAnimation);
            petalBasePosition_buffer.SetData(initPetalBasePosition);

        }

        void Cal_PetalParticle(){
            cal_petalParticle_cs.SetBuffer(kernel_CalPetalGPUParticle,"_write_petalAnim_buffer",petalAnim_buffer);
            cal_petalParticle_cs.SetFloat("_DTime",Time.deltaTime*5.0f);
            cal_petalParticle_cs.SetFloat("_maxPetalParticleHeight",maxPetalParticleHeight);
            cal_petalParticle_cs.Dispatch(kernel_CalPetalGPUParticle,(gPUFlower_Base.count*petalGroupCount)/numthread,1,1);
        }

        void Render_PetalGPUParticle(){
            petalMaterial.SetBuffer("_read_petalAnim_buffer",petalAnim_buffer);
            Graphics.DrawMeshInstancedProcedural(petalGPUParticle_mesh,0,petalMaterial,new Bounds(this.gameObject.transform.position,Vector3.one*500.0f),gPUFlower_Base.count*petalGroupCount);
        }
    }

}