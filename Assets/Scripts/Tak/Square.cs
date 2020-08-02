using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tak
{
    public class Square : Highlights
    {
        RaycastHit hit;

        [SerializeField]
        Material[] SquareMats = new Material[2];

        [SerializeField]
        Material[] DiamondMats = new Material[2];
        Material Selected, Deselected;

        GameObject quadCollider;

        // Start is called before the first frame update
        void Start()
        {
            Selected = DiamondMats[0];
            Deselected = DiamondMats[1];
            transform.localScale = new Vector3(1, 1, 1);

            quadCollider = transform.GetChild(0).gameObject;
            myCol = quadCollider.GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f, TakGameManager.inputMask))
            {
                if (hit.collider.transform.parent.gameObject == gameObject)
                {
                    //only do this if no piece is on top
                    if (!(transform.childCount > 1))
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

        void RedoHeights()
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                var yPos = 0f;
                float stoneOffset = 0.006f;
                yPos = (transform.GetChild(i).GetSiblingIndex() - 1) * stoneOffset;
                transform.GetChild(i).localPosition = new Vector3(0, yPos, 0);
                transform.GetChild(i).GetComponent<Highlights>().SetShouldRaycast(true);
                transform.GetChild(i).eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }
}