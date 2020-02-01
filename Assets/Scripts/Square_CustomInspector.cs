using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tak
{
    [CustomEditor(typeof(Square))]
    public class Square_CustomInspector : Editor
    {
        TakBoard gameBoard;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Square Here", GUILayout.Width(100), GUILayout.Height(100)))
            {
                Debug.Log("Placing Square");
                MoveSquareHere();
            }

            if (GUILayout.Button("Reset", GUILayout.Width(100), GUILayout.Height(33)))
            {
                Debug.Log("Resetting Board");
                ResetBoard();
            }
        }

        void MoveSquareHere()
        {
            if (gameBoard == null)
                gameBoard = FindObjectOfType<TakBoard>();
            gameBoard.GetPiece().transform.SetParent(Selection.gameObjects[0].transform);
        }

        void ResetBoard()
        {
            if (gameBoard == null)
                gameBoard = FindObjectOfType<TakBoard>();
            gameBoard.ReturnPiecesToPool();
        }
    }

}
