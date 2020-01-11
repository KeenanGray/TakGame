﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
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
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if we have more than one child and have not swapped layers, swap the QUAD to non-raycast layer
        if (quadCollider.layer == 0 && transform.childCount > 1)
        {
            quadCollider.layer = 2;//ignore raycast layer;
        }
        else if (quadCollider.layer == 2 && transform.childCount == 1)
        {
            quadCollider.layer = 0;
        }

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
}