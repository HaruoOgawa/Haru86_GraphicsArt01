using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPass_Butterfly_Render : MonoBehaviour
{
    [SerializeField] Material butterfly_grabpass_mat;

    [Header("BlurRenderTexture")]
    [SerializeField] int _BlurTexelSize=1;
    [SerializeField] float _blurPower=0.1f;
    [SerializeField] float _blurRange=3.0f;


    [Header("ChromaticAberration")]
    [SerializeField] float _ColorGapVal_R=0.0f;
    [SerializeField] float _ColorGapVal_G=0.0f;
    [SerializeField] float _ColorGapVal_B=0.0f;
    [SerializeField] float _chromaticAberrationPower=1.0f;
    [Range(0.0f,1.0f)]
    [SerializeField] float _colorGapAlpha=1.0f;

    [Header("Vignette")]
    [SerializeField] float _vignetteRange=3.0f;
    [SerializeField] float _vignettePower=0.1f;
    [Range(0.0f,1.0f)]
    [SerializeField] float _vignetteAlpha=1.0f;

    void Start()
    {
        //Blur
       butterfly_grabpass_mat.SetInt("_BlurTexelSize",_BlurTexelSize);
       butterfly_grabpass_mat.SetFloat("_blurRange",_blurRange);
      
       //Color
       butterfly_grabpass_mat.SetFloat("_ColorGapVal_R",_ColorGapVal_R);
       butterfly_grabpass_mat.SetFloat("_ColorGapVal_G",_ColorGapVal_G);
       butterfly_grabpass_mat.SetFloat("_ColorGapVal_B",_ColorGapVal_B);
       butterfly_grabpass_mat.SetFloat("_colorGapAlpha",_colorGapAlpha);
       
       //Vignette
       butterfly_grabpass_mat.SetFloat("_vignetteRange",_vignetteRange);
       butterfly_grabpass_mat.SetFloat("_vignetteAlpha",_vignetteAlpha);
      
    }

    void Update()
    {
            //Blur
       butterfly_grabpass_mat.SetInt("_BlurTexelSize",_BlurTexelSize);
       butterfly_grabpass_mat.SetFloat("_blurRange",_blurRange);
      
       //Color
       butterfly_grabpass_mat.SetFloat("_ColorGapVal_R",_ColorGapVal_R);
       butterfly_grabpass_mat.SetFloat("_ColorGapVal_G",_ColorGapVal_G);
       butterfly_grabpass_mat.SetFloat("_ColorGapVal_B",_ColorGapVal_B);
       butterfly_grabpass_mat.SetFloat("_colorGapAlpha",_colorGapAlpha);
       
       //Vignette
       butterfly_grabpass_mat.SetFloat("_vignetteRange",_vignetteRange);
       butterfly_grabpass_mat.SetFloat("_vignetteAlpha",_vignetteAlpha);
    }
}
