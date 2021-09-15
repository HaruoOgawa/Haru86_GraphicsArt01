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
        [SerializeField] Material peralMaterial;
        [SerializeField] GPUFlower_Base gPUFlower_Base;
        [SerializeField] int petalGroupCount=4;
        #endregion

        #region private region
        struct PetalAnimation{
            float petalLifeTime;
            Matrix4x4 petalTransform;
            Vector4 petalColor;
            float petalSpeed;
            public PetalAnimation(float life,Matrix4x4 trs,Vector4 col,float speed){
                this.petalLifeTime=life;
                this.petalTransform=trs;
                this.petalColor=col;
                this.petalSpeed=speed;
            }

        }
        ComputeBuffer petalAnim_buffer;
        ComputeBuffer petalBasePosition_buffer;
        #endregion
        void Start()
        {
            Init();
        }

        void Update()
        {
            
        }

        void OnDisable(){
           petalAnim_buffer.Release();
           petalBasePosition_buffer.Release();
        }

        void Init(){
            InitBuffer();
        }

        void InitBuffer(){
            List<PetalAnimation> initPetalAnimation=new List<PetalAnimation>();
            List<Matrix4x4> initPetalBasePosition=new List<Matrix4x4>();
            
            petalAnim_buffer=new ComputeBuffer(gPUFlower_Base.count*petalGroupCount,Marshal.SizeOf(typeof(PetalAnimation)));
            petalBasePosition_buffer=new ComputeBuffer(gPUFlower_Base.count*petalGroupCount,Marshal.SizeOf(typeof(Matrix4x4)));
            
            for(int i=0;i<gPUFlower_Base.count*petalGroupCount;i++){
                Vector2 initPos=Random.insideUnitSphere;
                float randomScale=Random.Range(0.5f,2.0f);
                PetalAnimation petalAnimation=new PetalAnimation(
                    Random.Range(0.0f,1.0f),
                    Matrix4x4.TRS(
                        new Vector3(initPos.x,Random.Range(0.0f,250.0f),initPos.y),
                        Quaternion.Euler(Random.Range(0.0f,360.0f),Random.Range(0.0f,360.0f),Random.Range(0.0f,360.0f)),
                        new Vector3(randomScale,randomScale,randomScale)
                    ),
                    new Vector4(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),1.0f),
                    Random.Range(1.0f,10.0f)
                );
                initPetalAnimation.Add(petalAnimation);
                initPetalBasePosition.Add(Matrix4x4.identity);
            }

            petalAnim_buffer.SetData(initPetalBasePosition);
            petalBasePosition_buffer.SetData(initPetalBasePosition);

        }
    }

}