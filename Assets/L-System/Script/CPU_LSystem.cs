
namespace GraphicsArt.LSystem.CPU_LSystem{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.LSystem.CPU_LSystem_Calculator;

    public class CPU_LSystem : MonoBehaviour
    {
        [SerializeField] Material cpu_lsystem_mat;
        [SerializeField] float len=10.0f;
        [SerializeField] float attenuation =0.95123f;
        [Range(1,10)]
        [SerializeField] int generations=5;
        [SerializeField] float ang=20f;
        void Start()
        {
            
        }

        void Update()
        {
         
        }

        void OnRenderObject(){
            DrawLSystem(generations,len);
        }

        void DrawFractalLsystem(){

        }
        void DrawLSystem(int gene,float l){
             cpu_lsystem_mat.SetPass(0);
             
             CalBaseLSystem(transform.localToWorldMatrix,gene,l);
        }   

        void CalBaseLSystem(Matrix4x4 current,int gene,float l){
            if(gene<=0)return;
 
            GL.MultMatrix(current);
            GL.Begin(GL.LINES);
            GL.Vertex(Vector3.zero);
            GL.Vertex(new Vector3(0,l,0));
            GL.End();
            
            //1///////////////////////////////////
            GL.PushMatrix();
            Random.InitState(gene+1*100);
            var posCurrent=current*Matrix4x4.TRS(
                new Vector3(0,l,0),
                Quaternion.AngleAxis((Random.value*2.0f-1.0f)*ang,Vector3.forward),
                Vector3.one
            );
            Random.InitState(gene+1*1000);
            CalBaseLSystem(posCurrent,gene - 1,l*Random.value*0.5f+0.5f);
            GL.PopMatrix();
            
            //2/////////////////////////////////////
            GL.PushMatrix();
            Random.InitState(gene+2*100);
            Matrix4x4 negCurrent=current*Matrix4x4.TRS(
                new Vector3(0,l,0),
                Quaternion.AngleAxis((Random.value*2.0f-1.0f)*ang,Vector3.forward),
                Vector3.one
            );
            Random.InitState(gene+2*1000);
            CalBaseLSystem(negCurrent,gene - 1,l*Random.value*0.5f+0.5f);
            GL.PopMatrix();
            
            
        }
    }
}

//  void CalNode(Matrix4x4 current,int gene,float l,int i){
//             GL.PushMatrix();
//             Random.InitState(gene+i*100);
//             var posCurrent=current*Matrix4x4.TRS(
//                 new Vector3(0,l,0),
//                 Quaternion.AngleAxis((Random.value*2.0f-1.0f)*ang,Vector3.forward),
//                 Vector3.one
//             );
//             Random.InitState(gene+i*1000);
//             CalBaseLSystem(posCurrent,gene - 1,l*Random.value*0.5f+0.5f,i);
//             GL.PopMatrix();
//         }