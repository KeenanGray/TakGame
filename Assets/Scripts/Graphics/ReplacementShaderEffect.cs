using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReplacementShaderEffect : MonoBehaviour
{
    public Shader ReplacementShader;
    public Color OverDrawColor;

    private void OnValidate()
    {
        Shader.SetGlobalColor("_OverDrawColor",OverDrawColor);
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
