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
        public Stonetype Stonetype;
        Stonetype LastStonetype;

        public delegate void StoneTypeChanged();
        public StoneTypeChanged OnStoneTypeChanged;

        Palette palette = null;

        MeshRenderer mesh;

        List<Material> _materials;

        Renderer _renderer;
        MaterialPropertyBlock _matPropertyBlock;

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
            _matPropertyBlock = new MaterialPropertyBlock();
            _renderer = GetComponentInChildren<Renderer>();
            palette = GameObject.Find("GameManager").GetComponent<Tak.TakGameManager>().palette;

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
                Setcolor(palette.GetColorByName("PlayerOne"));
            }
            else if (transform.CompareTag("1"))
            {
                Setcolor(palette.GetColorByName("PlayerTwo"));
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
                    GetComponent<SetGlowColor>().Glow = palette.GetColorByName("Glow");
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
        public void Setcolor(Color newColor)
        {
            if (_renderer == null || _matPropertyBlock == null)
            {
                Start();
            }
            _renderer.GetPropertyBlock(_matPropertyBlock);
            _matPropertyBlock.SetColor("_Color", newColor);
            _renderer.SetPropertyBlock(_matPropertyBlock);
        }
    }
}