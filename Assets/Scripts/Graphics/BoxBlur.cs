using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoxBlur : MonoBehaviour
{
    public Material BlurMaterial;

    [Range(0, 10)]
    public int Iterations;

    [Range(0, 4)]
    public int DownRes;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        int width = src.width >> DownRes;
        int height = src.height >> DownRes;

        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(src, rt);

        for (int i = 0; i < Iterations; i++)
        {
            RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(rt, rt2, BlurMaterial);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        Graphics.Blit(rt, dest);
        RenderTexture.ReleaseTemporary(rt);
    }
}
