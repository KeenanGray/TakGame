using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGlowColor : MonoBehaviour
{
    List<Material> _materials;

    public Color Glow;
    Color _currentColor;
    float lerpFactor = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        _materials = new List<Material>();
        GetComponentInChildren<MeshRenderer>().GetMaterials(_materials);

        Glow = new Color(0,0,0,1);
    }

    // Update is called once per frame
    void Update()
    { 
        if (_materials != null)
        {
            _currentColor = Color.Lerp(
                _currentColor,Glow,
                lerpFactor*Time.deltaTime
            );

            for (int i = 0; i < _materials.Count; i++)
            {
                _materials[i].SetColor("_GlowColor", _currentColor);
            }
        }
        else
        {
            _materials = new List<Material>();
            GetComponentInChildren<MeshRenderer>().GetMaterials(_materials);
        }
        
    }
}
