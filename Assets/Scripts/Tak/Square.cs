using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tak
{
    public class Square : Highlights
    {
        RaycastHit hit;

        Palette palette;

        GameObject quadCollider;

        Renderer _renderer;
        MaterialPropertyBlock _matPropertyBlock;

        [SerializeField]
        Texture2D[] textures;

        // Start is called before the first frame update
        void Start()
        {
            SetTextureAndSize(GameObject.FindObjectOfType<TakBoard>().BoardSize.getSize());

            _matPropertyBlock = new MaterialPropertyBlock();
            _renderer = GetComponentInChildren<Renderer>();
            quadCollider = transform.GetChild(0).gameObject;
            myCol = quadCollider.GetComponent<BoxCollider>();

            palette = GameObject.Find("GameManager").GetComponent<Tak.TakGameManager>().palette;
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f, TakGameManager.inputMask))
            {
                if (hit.collider == null || hit.collider.transform.parent == null)
                    return;

                if (hit.collider.transform.parent.gameObject == gameObject && hit.collider.CompareTag("Space"))
                {
                    //only do this if no piece is on top
                    //set color to selected
                    Setcolor(palette.GetColorByName("Highlight"));

                }
                else
                {
                    //set color to not selected
                    Setcolor(new Color(0, 0, 0, 0));
                }
            }
            else
            {
                //set color to not selected
                Setcolor(new Color(0, 0, 0, 0));
            }
        }


        //stone on top is childCount-1
        void RedoHeights()
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                var yPos = 0f;
                float stoneOffset = 0.006f;
                yPos = (transform.GetChild(i).GetSiblingIndex() - 1) * stoneOffset;
                transform.GetChild(i).localPosition = new Vector3(0, yPos, 0);
                transform.GetChild(i).GetComponentInChildren<Highlights>().SetShouldRaycast(true);
                transform.GetChild(i).eulerAngles = new Vector3(0, 0, 0);
            }
        }

        public void SetTextureAndSize(int size)
        {
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();
            if (_matPropertyBlock == null)
                _matPropertyBlock = new MaterialPropertyBlock();

            if (size % 2 == 0)
            {
                //set mat texture to square image
                _renderer.GetPropertyBlock(_matPropertyBlock);
                _matPropertyBlock.SetTexture("_MainTex", textures[0]);
                _renderer.SetPropertyBlock(_matPropertyBlock);
            }
            else
            {
                //set mat texture to diamond image
                _renderer.GetPropertyBlock(_matPropertyBlock);
                _matPropertyBlock.SetTexture("_MainTex", textures[1]);
                _renderer.SetPropertyBlock(_matPropertyBlock);
            }
            transform.localScale = new Vector3(1, 1, 1);
        }

        private void Setcolor(Color newColor)
        {
            _renderer.GetPropertyBlock(_matPropertyBlock);
            _matPropertyBlock.SetColor("_Color", newColor);
            _renderer.SetPropertyBlock(_matPropertyBlock);
        }
    }
}