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
        void Awake()
        {
            myCol = GetComponent<Collider>();
            if (transform.CompareTag("0"))
            {
                Deselected = PlayerOneMat;
            }
            else if (transform.CompareTag("1"))
            {
                Deselected = PlayerTwoMat;
            }
            try
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
            catch
            {

            }
        }

        // Update is called once per frame
        void Update()
        {
            RaycastForInput();
        }

        void RaycastForInput()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f, TakGameManager.inputMask))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    //print("hitting " + hit.collider.name);
                    GetComponent<MeshRenderer>().material = Selected;
                }
                else
                {
                    GetComponent<MeshRenderer>().material = Deselected;
                }
            }
            else
            {
                GetComponent<MeshRenderer>().material = Deselected;
            }
        }
    }
}