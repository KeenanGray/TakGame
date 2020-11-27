﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetPropertyBlockColor : MonoBehaviour
{
    public ColorReference color;
 
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
 
    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }
 
    void Update()
    {
        // Get the current value of the material properties in the renderer.
      //  _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
      //  _propBlock.SetColor("_Color", color.Value);
        // Apply the edited values to the renderer.
      //  _renderer.SetPropertyBlock(_propBlock);
    }
}
