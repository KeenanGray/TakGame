using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlights : MonoBehaviour
{
    internal Collider myCol;

    public void SetShouldRaycast(bool state)
    {
        if (state == true)
        {
            myCol.gameObject.layer = 0;//default layer
        }
        else if (state == false)
        {
            myCol.gameObject.layer = 2;//ignore raycast layer
        }
    }

}
