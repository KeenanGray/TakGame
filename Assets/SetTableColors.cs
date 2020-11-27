using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tak;

[ExecuteInEditMode]
public class SetTableColors : MonoBehaviour
{
    public float Speed = 1, Offset;

    MaterialPropertyBlock _PropBlock;
    Renderer _TableTopRenderer;
    Renderer _TableLegsRenderer;
    Renderer _TableDrawerRenderer;
    Palette palette;

    // Update is called once per frame
    void Update()
    {
        if (_TableTopRenderer == null || _PropBlock == null || palette == null)
        {
            _PropBlock = new MaterialPropertyBlock();
            _TableTopRenderer = transform.Find("TableTop").GetComponent<Renderer>();
            _TableLegsRenderer = transform.Find("TableLegs").GetComponent<Renderer>();
            _TableDrawerRenderer = transform.Find("TableDrawer").GetComponent<Renderer>();
            palette = GameObject.Find("GameManager").GetComponent<TakGameManager>().palette;
        }

        // Get the current value of the material properties in the renderer.
        _TableTopRenderer.GetPropertyBlock(_PropBlock);
        // Assign our new value.
        _PropBlock.SetColor("_Color", palette.GetColorByName("TableTop"));
        // Apply the edited values to the renderer.
        _TableTopRenderer.SetPropertyBlock(_PropBlock);

        // Get the current value of the material properties in the renderer.
        _TableLegsRenderer.GetPropertyBlock(_PropBlock);
        // Assign our new value.
        _PropBlock.SetColor("_Color", palette.GetColorByName("TableLegs"));
        // Apply the edited values to the renderer.
        _TableLegsRenderer.SetPropertyBlock(_PropBlock);

        // Get the current value of the material properties in the renderer.
        _TableDrawerRenderer.GetPropertyBlock(_PropBlock);
        // Assign our new value.
        _PropBlock.SetColor("_Color", palette.GetColorByName("TableDrawer"));
        // Apply the edited values to the renderer.
        _TableDrawerRenderer.SetPropertyBlock(_PropBlock);
    }
}
