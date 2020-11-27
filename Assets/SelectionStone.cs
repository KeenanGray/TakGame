using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tak;
namespace Tak
{
    [ExecuteInEditMode]
    public class SelectionStone : MonoBehaviour
    {
        public TurnData turnData;

        [SerializeField]
        Stonetype myType = Stonetype.FlatStone;

        MaterialPropertyBlock _matPropertyBlock;
        Renderer _renderer;

        bool mouseIsOver;

        Palette palette;
        void Awake()
        {
            turnData.PlaceType = Stonetype.FlatStone;
            _matPropertyBlock = new MaterialPropertyBlock();
            _renderer = GetComponentInChildren<Renderer>();
            palette = GameObject.Find("GameManager").GetComponent<TakGameManager>().palette;
        }

        void Start()
        {
            turnData.PlaceType = Stonetype.FlatStone;
        }

        void Update()
        {
            if (_renderer == null || _matPropertyBlock == null)
            {
                _matPropertyBlock = new MaterialPropertyBlock();
                _renderer = GetComponentInChildren<Renderer>();
            }

            Color color = palette.GetColorByName("PlayerOne");

            if (turnData.CurrentPlayer.Value == 0)
                color = palette.GetColorByName("PlayerOne");
            else
                color = palette.GetColorByName("PlayerTwo");

            _renderer.GetPropertyBlock(_matPropertyBlock);
            _matPropertyBlock.SetColor("_Color", color);
            _renderer.SetPropertyBlock(_matPropertyBlock);


            if (myType == turnData.PlaceType)
            {
                _renderer.GetPropertyBlock(_matPropertyBlock);
                _matPropertyBlock.SetColor("_GlowColor", palette.GetColorByName("Glow"));
                _renderer.SetPropertyBlock(_matPropertyBlock);
            }
            else if (!mouseIsOver)
            {
                _renderer.GetPropertyBlock(_matPropertyBlock);
                _matPropertyBlock.SetColor("_GlowColor", new Color(0, 0, 0, 1));
                _renderer.SetPropertyBlock(_matPropertyBlock);
            }

            //limit capstone placement to 1
            if (Application.isPlaying)
            {
                if (myType == Stonetype.CapStone)
                    transform.GetChild(0).gameObject.SetActive(turnData.CapStones[turnData.CurrentPlayer.Value] == 1);
            }
        }

        private void OnMouseEnter()
        {
            mouseIsOver = true;
            if (turnData.PlaceType == myType)
                return;

            _renderer.GetPropertyBlock(_matPropertyBlock);
            _matPropertyBlock.SetColor("_GlowColor", palette.GetColorByName("Glow"));
            _renderer.SetPropertyBlock(_matPropertyBlock);
        }

        private void OnMouseExit()
        {
            mouseIsOver = false;
            if (turnData.PlaceType == myType)
                return;

            _renderer.GetPropertyBlock(_matPropertyBlock);
            _matPropertyBlock.SetColor("_GlowColor", new Color(0, 0, 0, 1));
            _renderer.SetPropertyBlock(_matPropertyBlock);
        }

        private void OnMouseDown()
        {
            turnData.PlaceType = myType;
        }

    }
}
