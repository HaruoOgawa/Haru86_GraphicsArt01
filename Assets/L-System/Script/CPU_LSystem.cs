
namespace GraphicsArt.LSystem.CPU_LSystem{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsArt.LSystem.CPU_LSystem_Calculator;

    public class CPU_LSystem : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] CPU_LSystem_Calculator cpu_LSystem_Calculator;
        #endregion

        #region 
        Mesh pointMesh;
        #endregion
        
        void Start()
        {
            pointMesh=new Mesh();
            Vector3[] vertex={new Vector3(0,0,0),new Vector3(1,0,0),new Vector3(1,1,0)};
            pointMesh.vertices=vertex;
        }

        void Update()
        {
         
        }
    }
}