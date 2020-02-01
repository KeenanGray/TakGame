using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tak
{
    public class Highlights : MonoBehaviour
    {
        internal Collider myCol;

        public void SetShouldRaycast(bool state)
        {
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