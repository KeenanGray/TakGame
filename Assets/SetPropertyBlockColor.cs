using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tak;

[ExecuteInEditMode]
public class SetPropertyBlockColor : MonoBehaviour
{
    public string nameOfColor;

    Palette palette;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Update()
    {
        if (_propBlock == null || _renderer == null || palette == null)
        {
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
            palette = GameObject.Find("GameManager").GetComponent<TakGameManager>().palette;
        }

        _renderer = GetComponent<Renderer>();
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", palette.GetColorByName(nameOfColor));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }
}
