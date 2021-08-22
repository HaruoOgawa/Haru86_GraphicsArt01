using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Chrysanthemum_Flower_Instancing : MonoBehaviour
{
    [SerializeField] Mesh flower_mesh;
    [SerializeField] Material flower_mat;
    [SerializeField] int count=1000;
    private ComputeBuffer flower_buffer;
    private Matrix4x4[] flower_TRS;
    void Start()
    {
        float randomSize=Random.Range(0.5f,1.5f)*500.0f;
        flower_TRS=new Matrix4x4[count];
        for(int i=0;i<count;i++){
            Vector2 rpos=Random.insideUnitCircle*100.0f;
            flower_TRS[i]=Matrix4x4.TRS(
                new Vector3(rpos.x,0.0f,rpos.y),
                Quaternion.Euler(-90.0f,Random.Range(-360.0f,360.0f),0.0f),
                new Vector3(randomSize,randomSize,randomSize)
            );
        }
        flower_buffer=new ComputeBuffer(count,Marshal.SizeOf(typeof(Matrix4x4)));
        flower_buffer.SetData(flower_TRS);
    }

    void Update()
    {
        flower_mat.SetBuffer("_flower_buffer",flower_buffer);
        Graphics.DrawMeshInstancedProcedural(flower_mesh,0,flower_mat,new Bounds(transform.position,Vector3.one*500.0f),count);
    }

    void OnDisable(){
        flower_buffer.Release();
    }
}
