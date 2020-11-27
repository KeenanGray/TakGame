using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CompositeImageEffect : MonoBehaviour
{
    public Material _compositeMat;
    public float Intensity;

    private void OnEnable()
    {

    }
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        _compositeMat.SetFloat("_Intensity", Intensity);
        Graphics.Blit(src, dest, _compositeMat, 0);
    }
}
