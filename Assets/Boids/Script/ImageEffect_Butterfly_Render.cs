using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffect_Butterfly_Render : MonoBehaviour
{
    [SerializeField] Material burrerfly_image_effect_mat;
   void OnRenderImage(RenderTexture src, RenderTexture dest){
       //create RenderTexture
       RenderTexture blurRenderTexture=RenderTexture.GetTemporary(src.width,src.height,0,src.format);
       RenderTexture chromaticAberrationRenderTexture=RenderTexture.GetTemporary(src.width,src.height,0,src.format);
       RenderTexture vignetteRenderTexture=RenderTexture.GetTemporary(src.width,src.height,0,src.format);
       blurRenderTexture.Create();
       chromaticAberrationRenderTexture.Create();
       vignetteRenderTexture.Create();

       //cal image effect
       Graphics.Blit(src,blurRenderTexture,burrerfly_image_effect_mat,0);
       Graphics.Blit(src,chromaticAberrationRenderTexture,burrerfly_image_effect_mat,1);
       Graphics.Blit(src,vignetteRenderTexture,burrerfly_image_effect_mat,2);
       Graphics.Blit(src,dest,burrerfly_image_effect_mat,3);

       //relese RenderTexture
       RenderTexture.ReleaseTemporary(blurRenderTexture);
       RenderTexture.ReleaseTemporary(chromaticAberrationRenderTexture);
       RenderTexture.ReleaseTemporary(vignetteRenderTexture);
   }
}
