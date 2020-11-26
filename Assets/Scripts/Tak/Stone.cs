﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tak
{
    public enum Stonetype
    {
        FlatStone,
        StandingStone,
        CapStone
    }

    public class Stone : Highlights
    {
        RaycastHit hit;

        [SerializeField]
        IntReference CurrentPlayer;
        public Stonetype Stonetype;
        Stonetype LastStonetype;

        public delegate void StoneTypeChanged();
        public StoneTypeChanged OnStoneTypeChanged;

        [SerializeField]
        Palette palette = null;

        MeshRenderer mesh;

        List<Material> _materials;

        public Mesh[] StoneMeshes;
        void Awake()
        {
            myCol = GetComponentInChildren<Collider>();
            Init();

            mesh = GetComponentInChildren<MeshRenderer>();

            OnStoneTypeChanged += ChangeStoneTypeModel;
            LastStonetype = Stonetype;
        }

        void Start()
        {
            _materials = new List<Material>();
            GetComponentInChildren<MeshRenderer>().GetMaterials(_materials);
        }

        void ChangeStoneTypeModel()
        {
            var stoneModel = transform.GetChild(0);
            float height = mesh.bounds.extents.x;
            switch (Stonetype)
            {
                case Stonetype.FlatStone:
                    stoneModel.GetComponent<MeshFilter>().mesh = Instantiate(StoneMeshes[0]);
                    stoneModel.transform.localPosition = Vector3.up * 0;
                    stoneModel.transform.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case Stonetype.StandingStone:
                    stoneModel.GetComponent<MeshFilter>().mesh = Instantiate(StoneMeshes[0]);
                    stoneModel.transform.localPosition = Vector3.up * height;
                    stoneModel.transform.localEulerAngles = new Vector3(90, 0, 0);
                    break;
                case Stonetype.CapStone:
                    stoneModel.GetComponent<MeshFilter>().mesh = Instantiate(StoneMeshes[1]);
                    stoneModel.transform.localPosition = Vector3.up * 0;
                    stoneModel.transform.localEulerAngles = new Vector3(0, 0, 0);
                    break;
            }
        }
        // Update is called once per frame
        void Update()
        {
            RaycastForInput();

            if (transform.CompareTag("0"))
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    _materials[i].SetColor("_Color", palette.PlayerOne.Value);
                }
            }
            else if (transform.CompareTag("1"))
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    _materials[i].SetColor("_Color", palette.PlayerTwo.Value);
                }
            }

            if (Stonetype != LastStonetype)
            {
                OnStoneTypeChanged.Invoke();
                LastStonetype = Stonetype;
            }

            if (Stonetype == Stonetype.StandingStone)
            {
                //if it's a standing stone, check if it's neighbor is a capstone, if so it falls down
                if (transform.parent.childCount > transform.GetSiblingIndex() + 1)
                {
                    if (transform.parent.GetChild(transform.GetSiblingIndex() + 1).GetComponent<Stone>())
                    {
                        Debug.Log("Collapsing standing stone");
                        //a capstone is on top of a standing stone
                        Stonetype = Stonetype.FlatStone;
                    }
                }
            }
        }

        void RaycastForInput()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f, TakGameManager.inputMask))
            {
                if (hit.collider == null || hit.collider.transform.parent == null)
                    return;

                if (hit.collider.transform.parent.gameObject == gameObject)
                {
                    GetComponent<SetGlowColor>().Glow = palette.Glow.Value;
                }
                else
                {
                    GetComponent<SetGlowColor>().Glow = new Color(0, 0, 0, 1);
                }
            }
            else
            {
                GetComponent<SetGlowColor>().Glow = new Color(0, 0, 0, 1);
            }
        }
    }
}