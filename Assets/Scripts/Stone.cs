using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tak
{
    [ExecuteInEditMode]
    public class Stone : Highlights
    {
        RaycastHit hit;

        [SerializeField]
        IntReference CurrentPlayer;

        public Material Selected;
        public Material PlayerOneMat;
        public Material PlayerTwoMat;
        Material Deselected = null;

        // Start is called before the first frame update
        void Start()
        {
            myCol = GetComponent<Collider>();
            Deselected = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (Deselected == null)
            {
                if (transform.parent.name.Contains("One"))
                {
                    Deselected = PlayerOneMat;
                }
                else if (transform.parent.name.Contains("Two"))
                {
                    Deselected = PlayerTwoMat;
                }

            }
            else
            {
                //don't do raycast stuff in editor
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    return;
                else
                    RaycastForInput();
            }
        }

        void RaycastForInput()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f, TakGameManager.inputMask))
            {
                //print("hitting " + hit.collider.name);
                if (hit.collider.gameObject == gameObject)
                {
                    GetComponentInChildren<MeshRenderer>().material = Selected;
                }
                else
                {
                    GetComponentInChildren<MeshRenderer>().material = Deselected;
                }
            }
            else
            {
                GetComponentInChildren<MeshRenderer>().material = Deselected;
            }
        }
    }
}