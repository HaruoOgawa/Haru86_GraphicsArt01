using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffect_Butterfly_Render : MonoBehaviour
{
    [SerializeField] Material burrerfly_image_effect_mat;
   void OnRenderImage(RenderTexture src, RenderTexture dest){
       Graphics.Blit(src,dest,burrerfly_image_effect_mat);
   }
}
