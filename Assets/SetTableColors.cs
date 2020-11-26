using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetTableColors : MonoBehaviour
{
    public Palette palette;

    public float Speed = 1, Offset;

    MaterialPropertyBlock _TableTopPropBlock;
    Renderer _TableTopRenderer;

    MaterialPropertyBlock _TableLegsPropBlock;
    Renderer _TableLegsRenderer;

    MaterialPropertyBlock _TableDrawerPropBlock;
    Renderer _TableDrawerRenderer;

    void Awake()
    {
        _TableTopPropBlock = new MaterialPropertyBlock();
        _TableTopRenderer = transform.Find("TableTop").GetComponent<Renderer>();

        _TableLegsPropBlock = new MaterialPropertyBlock();
        _TableLegsRenderer = transform.Find("TableLegs").GetComponent<Renderer>();

        _TableDrawerPropBlock = new MaterialPropertyBlock();
        _TableDrawerRenderer = transform.Find("TableDrawer").GetComponent<Renderer>();
    }
    // Update is called once per frame
    void Update()
    {
        // Get the current value of the material properties in the renderer.
        _TableTopRenderer.GetPropertyBlock(_TableTopPropBlock);
        // Assign our new value.
        _TableLegsPropBlock.SetColor("_Color",palette.TableTop.Value);
        // Apply the edited values to the renderer.
        _TableTopRenderer.SetPropertyBlock(_TableTopPropBlock);

        // Get the current value of the material properties in the renderer.
        _TableLegsRenderer.GetPropertyBlock(_TableLegsPropBlock);
        // Assign our new value.
        _TableLegsPropBlock.SetColor("_Color",palette.Table.Value);
        // Apply the edited values to the renderer.
        _TableLegsRenderer.SetPropertyBlock(_TableLegsPropBlock);

           // Get the current value of the material properties in the renderer.
        _TableDrawerRenderer.GetPropertyBlock(_TableDrawerPropBlock);
        // Assign our new value.
        _TableDrawerPropBlock.SetColor("_Color",palette.Drawer.Value);
        // Apply the edited values to the renderer.
        _TableDrawerRenderer.SetPropertyBlock(_TableDrawerPropBlock);
    }
}
