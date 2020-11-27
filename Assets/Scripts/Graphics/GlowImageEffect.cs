using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class GlowImageEffect : MonoBehaviour
{
    public Material BlurMaterial;

    [Range(0, 10)]
    public int Iterations;

    [Range(0, 4)]
    public int DownRes;

    RenderTexture PrePass;
    RenderTexture Blurred;
    void Start()
    {
        StartCoroutine("UpdateShader");
    }
    IEnumerator UpdateShader()
    {
        while (true)
        {
            Init();
            yield return new WaitForSeconds(.25f);
        }
    }

    void Init()
    {
        string[] res = UnityStats.screenRes.Split('x');

        PrePass = new RenderTexture(int.Parse(res[0]), int.Parse(res[1]), 24);
        Blurred = new RenderTexture(int.Parse(res[0]) >> 1, int.Parse(res[1]) >> 1, 0);

        Camera camera = GetComponent<Camera>();
        Shader glowShader = Shader.Find("Keenan/GlowFlat");
        camera.targetTexture = PrePass;
        camera.SetReplacementShader(glowShader, "Glowable");

        Shader.SetGlobalTexture("_GlowPrePassTex", PrePass);
        Shader.SetGlobalTexture("_GlowBlurredTex", Blurred);
    }

    void OnEnable()
    {
        Init();
    }

    void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest);

        Graphics.SetRenderTarget(Blurred);
        GL.Clear(false, true, Color.clear);

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

        Graphics.Blit(rt, Blurred);
        Graphics.SetRenderTarget(dest);



        Shader.SetGlobalTexture("_GlowBlurredTex", Blurred);

        RenderTexture.ReleaseTemporary(rt);


    }

}
