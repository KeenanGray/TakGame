using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGlowColor : MonoBehaviour
{
    List<Material> _materials;

    [HideInInspector]
    public Color Glow;
    Color _currentColor;
    float lerpFactor = 1.5f;

    MaterialPropertyBlock _matPropertyBlock;
    Renderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        Glow = new Color(0, 0, 0, 1);
        _matPropertyBlock = new MaterialPropertyBlock();
        _renderer = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _currentColor = Color.Lerp(
            _currentColor, Glow,
            lerpFactor * Time.deltaTime
        );

        _renderer.GetPropertyBlock(_matPropertyBlock);
        _matPropertyBlock.SetColor("_GlowColor", _currentColor);
        _renderer.SetPropertyBlock(_matPropertyBlock);

    }
}
