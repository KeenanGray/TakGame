using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tak
{
    public class SelectionStone : MonoBehaviour
    {
        public TurnData turnData;
        public Palette palette;

        [SerializeField]
        Stonetype myType = Stonetype.FlatStone;

        MaterialPropertyBlock _matPropertyBlock;
        Renderer _renderer;

        bool mouseIsOver;

        void Awake()
        {
            turnData.PlaceType = Stonetype.FlatStone;
            _matPropertyBlock = new MaterialPropertyBlock();
            _renderer = GetComponentInChildren<Renderer>();
        }

        void Start()
        {
            turnData.PlaceType = Stonetype.FlatStone;
        }

        void Update()
        {
            Color color = palette.PlayerOne.Value;

            if (turnData.CurrentPlayer.Value == 0)
                color = palette.PlayerOne.Value;
            else
                color = palette.PlayerTwo.Value;

            _renderer.GetPropertyBlock(_matPropertyBlock);
            _matPropertyBlock.SetColor("_Color", color);
            _renderer.SetPropertyBlock(_matPropertyBlock);


            if (myType == turnData.PlaceType)
            {
                _renderer.GetPropertyBlock(_matPropertyBlock);
                _matPropertyBlock.SetColor("_GlowColor", palette.SelectedHighlight.Value);
                _renderer.SetPropertyBlock(_matPropertyBlock);
            }
            else if (!mouseIsOver)
            {
                _renderer.GetPropertyBlock(_matPropertyBlock);
                _matPropertyBlock.SetColor("_GlowColor", new Color(0, 0, 0, 1));
                _renderer.SetPropertyBlock(_matPropertyBlock);
            }

            //limit capstone placement to 1
            if (myType == Stonetype.CapStone)
                transform.GetChild(0).gameObject.SetActive(turnData.CapStones[turnData.CurrentPlayer.Value] == 1);
        }

        private void OnMouseEnter()
        {
            mouseIsOver = true;
            if (turnData.PlaceType == myType)
                return;

            _renderer.GetPropertyBlock(_matPropertyBlock);
            _matPropertyBlock.SetColor("_GlowColor", palette.Glow.Value);
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
