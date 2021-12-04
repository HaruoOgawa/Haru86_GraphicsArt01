using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUFlowers_CameraAnim : MonoBehaviour
{
    [SerializeField] Material material;
    [Range(0.0f,1.0f)]
    [SerializeField] float GapOffsetR=0.0f;
    [Range(0.0f,1.0f)]
    [SerializeField] float GapOffsetG=0.0f;
    [Range(0.0f,1.0f)]
    [SerializeField] float GapOffsetB=0.0f;

     [Range(0.0f,1.0f)]
    [SerializeField] float GapOffsetRY=0.0f;
    [Range(0.0f,1.0f)]
    [SerializeField] float GapOffsetGY=0.0f;
    [Range(0.0f,1.0f)]
    [SerializeField] float GapOffsetBY=0.0f;

    [Range(0.0f,1.0f)]
    [SerializeField] float gapOffsetPower=1.0f;
    void Start()
    {
        
    }

    void Update()
    {
        this.gameObject.transform.Rotate(0,1.0f*Time.deltaTime,0,Space.World);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest){
        var colorGapTexture=RenderTexture.GetTemporary(src.width,src.height,0,src.format);

        material.SetVector("_GapOffsetX",new Vector4(GapOffsetR,GapOffsetG,GapOffsetB,0.0f));
        material.SetVector("_GapOffsetY",new Vector4(GapOffsetRY,GapOffsetGY,GapOffsetBY,0.0f));
        material.SetFloat("_gapOffsetPower",gapOffsetPower);
        Graphics.Blit(src,colorGapTexture,material,0);

        material.SetTexture("_colorGapTexture",colorGapTexture);
        Graphics.Blit(src,dest,material,1);

        colorGapTexture.Release();
    }

}
