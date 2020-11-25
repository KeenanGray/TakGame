using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tak
{
    public class Highlights : MonoBehaviour
    {
        internal Collider myCol;

        internal void Init()
        {
            SetShouldRaycast(false);
        }

        public void SetShouldRaycast(bool state)
        {
            myCol = GetComponentInChildren<Collider>();

            if (myCol == null)
            {
                Debug.LogError("no collider on this highlightable object" + name);
                return;
            }


            if (state == true)
            {
                myCol.gameObject.layer = 8;//accept raycast
            }
            else if (state == false)
            {
                myCol.gameObject.layer = 9;//ignore raycast layer
            }
        }

    }
}