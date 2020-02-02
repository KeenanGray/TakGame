using System;
using UnityEditor;
using UnityEngine;

namespace Tak
{
    [CustomEditor(typeof(TakBoard))]
    public class Takboard_CustomInspector : Editor
    {
        public int selGridInt = 0;
        public string[] selStrings;

        public override void OnInspectorGUI()
        {
            //base oninpsectorgui will draw the properties from takboard,
            //we can recreate this behavior somehow, what is the code?
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("BoardSize"));

            var playerProp = EditorGUIHelpers.GetTargetObjectOfProperty(serializedObject.FindProperty("CurrentPlayer"));
            IntReference curPlayer = (IntReference)playerProp;

            EditorGUI.BeginDisabledGroup(true);

            if (curPlayer.Value == 1)
                EditorGUILayout.ColorField(new Color(255, 255, 255, 255));
            else if (curPlayer.Value == 0)
                EditorGUILayout.ColorField(new Color(0, 0, 0, 255));

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("ChangePlayer"))
            {
                SwitchPlayer(curPlayer);
            }

            TakBoard board = ((TakBoard)target);

            int size = board.GetBoardSizeProperty().getSize();

            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, size * 40);
            rect.y = 100 + size * 20 / 2;
            rect.x = EditorGUIUtility.currentViewWidth / 2;
            int k = -1;
            for (int i = size / 2 + size % 2; i > -size / 2; i--)
            {
                for (int j = -size / 2 - size % 2; j < size / 2; j++)
                {
                    k++;

                    GUILayout.BeginArea(new Rect(rect.x + (20 * j), rect.y + (20 * i), 20, 20));
                    if (GUILayout.Button(k.ToString(), GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        MoveStoneHere(k, curPlayer);
                        SwitchPlayer(curPlayer);
                    }
                    GUILayout.EndArea();
                }
            }



            if (GUILayout.Button("Reset", GUILayout.Width(100), GUILayout.Height(33)))
            {
                ResetBoard();
            }

            if (EditorGUI.EndChangeCheck())
            {
            }

        }

        private void SwitchPlayer(IntReference current)
        {
            if (current.Value == 0)
            {
                current.Value = 1;
            }
            else
            {
                current.Value = 0;
            }
            serializedObject.ApplyModifiedProperties();
        }

        void MoveStoneHere(int index, IntReference current)
        {
            TakBoard board = ((TakBoard)target);
            var stone = board.GetPiece();
            stone.transform.tag = current.Value.ToString();
            var square = board.transform.GetChild(0).GetChild(index);
            stone.transform.SetParent(square);
            stone.SendMessageUpwards("RedoHeights");
        }

        void ResetBoard()
        {
            TakBoard board = ((TakBoard)target);
            board.ReturnPiecesToPool();
        }


    }
}

