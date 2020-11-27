using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReplacementShaderEffect : MonoBehaviour
{
    public Shader ReplacementShader;
    public Color ModColor;
    [Range(0, 10)]
    public float Factor;

    private void OnValidate()
    {
        Shader.SetGlobalColor("_ModColor", ModColor);
        Shader.SetGlobalFloat("_Factor", Factor);

    }

    void Update()
    {
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (ReplacementShader != null)
        {
            GetComponent<Camera>().SetReplacementShader(ReplacementShader, "");
        }
    }

    void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }
}
