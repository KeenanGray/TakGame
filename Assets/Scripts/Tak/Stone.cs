using System.Collections;
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

        public Material Selected;
        public Material PlayerOneMat;
        public Material PlayerTwoMat;

        public Stonetype Stonetype;
        Stonetype LastStonetype;

        public delegate void StoneTypeChanged();
        public StoneTypeChanged OnStoneTypeChanged;

        Material Deselected = null;

        public Mesh[] StoneMeshes;
        // Start is called before the first frame update
        void Awake()
        {
            myCol = GetComponentInChildren<Collider>();
            Init();

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

            OnStoneTypeChanged += ChangeStoneTypeModel;
            LastStonetype = Stonetype;
        }

        void ChangeStoneTypeModel()
        {
            Debug.Log("new Stone Type " + Stonetype);
            var stoneModel = transform.GetChild(0);
            float height = stoneModel.GetComponent<MeshRenderer>().bounds.extents.x;
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
                    Debug.Log("here");
                    if(transform.parent.GetChild(transform.GetSiblingIndex()+1).GetComponent<Stone>()){
                        Debug.Log("Collapsing standing stone");
                        //a capstone is on top of a standing stone
                        Stonetype=Stonetype.FlatStone;
                    }
                }
            }
        }

        void RaycastForInput()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f, TakGameManager.inputMask))
            {
                if (hit.collider.gameObject == transform.GetChild(0).gameObject)
                {
                    //print("hitting " + hit.collider.name);
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