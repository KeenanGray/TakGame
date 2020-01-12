using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : Highlights
{
    RaycastHit hit;

    [SerializeField]
    IntReference iref_BoardSize;

    [SerializeField]
    Material[] SquareMats = new Material[2];

    [SerializeField]
    Material[] DiamondMats = new Material[2];
    Material Selected, Deselected;

    GameObject quadCollider;

    // Start is called before the first frame update
    void Start()
    {

        transform.localScale = new Vector3(1, 1, 1);
        if (iref_BoardSize.Value % 2 != 0)
        {
            Selected = DiamondMats[0];
            Deselected = DiamondMats[1];
            transform.GetChild(0).localScale = new Vector3(0.01875f, .025f, .25f);
        }
        else
        {
            Selected = SquareMats[0];
            Deselected = SquareMats[1];
            transform.GetChild(0).localScale = new Vector3(0.0375f, .0375f, 0.15f);
        }
        quadCollider = transform.GetChild(0).gameObject;
        myCol = quadCollider.GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100.0f))
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
        }
    }
}
