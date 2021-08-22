using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffect_Butterfly_Render : MonoBehaviour
{
    [SerializeField] Material burrerfly_image_effect_mat;

    [Header("BlurRenderTexture")]
    [SerializeField] int _BlurTexelSize=1;
    [SerializeField] float _blurPower=0.1f;
    [SerializeField] float _blurRange=3.0f;


    [Header("ChromaticAberration")]
    [SerializeField] float _ColorGapVal_R=0.0f;
    [SerializeField] float _ColorGapVal_G=0.0f;
    [SerializeField] float _ColorGapVal_B=0.0f;
    [SerializeField] float _chromaticAberrationPower=1.0f;

    [Header("Vignette")]
    [SerializeField] float _vignetteRange=3.0f;
    [SerializeField] float _vignettePower=0.1f;

   void OnRenderImage(RenderTexture src, RenderTexture dest){
       //create RenderTexture
       float descVal=0.5f;
       RenderTexture blurRenderTexture=RenderTexture.GetTemporary(Mathf.NextPowerOfTwo(Mathf.CeilToInt((float)src.width*descVal)),Mathf.NextPowerOfTwo(Mathf.CeilToInt((float)src.height*descVal)),0,src.format);
       RenderTexture chromaticAberrationRenderTexture=RenderTexture.GetTemporary(Mathf.NextPowerOfTwo(Mathf.CeilToInt((float)src.width*descVal)),Mathf.NextPowerOfTwo(Mathf.CeilToInt((float)src.height*descVal)),0,src.format);
       RenderTexture vignetteRenderTexture=RenderTexture.GetTemporary(Mathf.NextPowerOfTwo(Mathf.CeilToInt((float)src.width*descVal)),Mathf.NextPowerOfTwo(Mathf.CeilToInt((float)src.height*descVal)),0,src.format);
       blurRenderTexture.Create();
       chromaticAberrationRenderTexture.Create();
       vignetteRenderTexture.Create();

       //cal image effect

       //Blur
       burrerfly_image_effect_mat.SetInt("_BlurTexelSize",_BlurTexelSize);
       burrerfly_image_effect_mat.SetFloat("_blurRange",_blurRange);
       Graphics.Blit(src,blurRenderTexture,burrerfly_image_effect_mat,0);

       //Color
       burrerfly_image_effect_mat.SetFloat("_ColorGapVal_R",_ColorGapVal_R);
       burrerfly_image_effect_mat.SetFloat("_ColorGapVal_G",_ColorGapVal_G);
       burrerfly_image_effect_mat.SetFloat("_ColorGapVal_B",_ColorGapVal_B);
       Graphics.Blit(src,chromaticAberrationRenderTexture,burrerfly_image_effect_mat,1);
       
       //Vignette
       burrerfly_image_effect_mat.SetFloat("_vignetteRange",_vignetteRange);
       Graphics.Blit(src,vignetteRenderTexture,burrerfly_image_effect_mat,2);
      
        //final render
        burrerfly_image_effect_mat.SetTexture("_blurRenderTexture",blurRenderTexture);
        burrerfly_image_effect_mat.SetFloat("_blurPower",_blurPower);
        burrerfly_image_effect_mat.SetTexture("_chromaticAberrationRenderTexture",chromaticAberrationRenderTexture);
        burrerfly_image_effect_mat.SetFloat("_chromaticAberrationPower",_chromaticAberrationPower);
        burrerfly_image_effect_mat.SetTexture("_vignetteRenderTexture",vignetteRenderTexture);
        burrerfly_image_effect_mat.SetFloat("_vignettePower",_vignettePower);
        Graphics.Blit(src,dest,burrerfly_image_effect_mat,3);

       //relese RenderTexture
       RenderTexture.ReleaseTemporary(blurRenderTexture);
       RenderTexture.ReleaseTemporary(chromaticAberrationRenderTexture);
       RenderTexture.ReleaseTemporary(vignetteRenderTexture);
   }
}
