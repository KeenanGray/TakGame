using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Tak
{
    public enum BoardSizeNames
    {
        three,
        four,
        five,
        six,
        seven,
        eight
    };

    public class TakBoard : MonoBehaviour
    {
        [SerializeField]
        float[] sizes = new float[4] { 0.018f, 0.02f, 0.02f, 0.02f };

        float offset = 0.042f;

        //  [SerializeField]
        [Range(0, 1.0f)]
        float trayOffset = 0.01f;

        [SerializeField]
        BoardSizeProperty BoardSize;

        [SerializeField]
        IntReference CurrentPlayer;

        Transform boardTilesPool;
        string boardTilesPoolName = "SquaresPool";
        Transform board;
        string boardName = "BoardSpaces";

        GameObject PlayerOnePool;
        GameObject PlayerTwoPool;

        public delegate void OnBoardSizeChanged(int BoardSize);
        public static OnBoardSizeChanged OnBoardSizeChangedDelegate;

        // Start is called before the first frame update
        void Start()
        {
            if (Application.isPlaying)
            {
                Debug.Log("Play Mode");
            }
            else
            {
                Debug.Log("Edit Mode");
            }

            boardTilesPool = transform.Find(boardTilesPoolName);
            board = transform.Find(boardName);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateBoard(int size)
        {
            ReturnSquaresToPool();
            //ReturnPiecesToPool();

            boardTilesPool = GameObject.Find(boardTilesPoolName).transform;
            board = GameObject.Find(boardName).transform;

            int i = 0;

            //Add the children we need
            while (i < Mathf.Pow(size, 2))
            {
                i++;
                var t = boardTilesPool.GetChild(0).transform;
                t.SetParent(board);
                t.transform.localPosition = new Vector3(0, 0, 0.0f);
            }

            //arrange the children baybee
            float x = 0;
            float y = 0;
            int k = 0;
            var numSqares = Mathf.Pow(size, 2);

            var width = -(size - 1) * offset;
            var height = -(size - 1) * offset;

            Vector3 StartPos = new Vector3(width / 2, 0, height / 2);

            for (int j = 0; j < numSqares; j++)
            {
                if (BoardSize.getSize() % 2 != 0)
                {
                    board.transform.GetChild(j).GetChild(0).localScale = new Vector3(sizes[0], sizes[1], 1);
                }
                else
                {
                    board.transform.GetChild(j).GetChild(0).localScale = new Vector3(sizes[2], sizes[3], 1);
                }
                board.transform.GetChild(j).transform.localPosition = new Vector3(x, 0, y);


                if (j % (size) == 0 && j > 0)
                {
                    y += offset;
                    k = 0;
                }
                x = offset * k;
                k++;
                board.transform.GetChild(j).transform.localPosition = StartPos + new Vector3(x, 0, y);
                board.transform.GetChild(j).name = "Square " + (j);
            }
        }

        private void ReturnSquaresToPool()
        {
            ReturnPiecesToPool();
            var parent = GameObject.Find("SquaresPool");
            var squares = GameObject.FindObjectsOfType<Square>();
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i].gameObject.transform.SetParent(parent.transform);
                squares[i].transform.localPosition = new Vector3(0, 0, 0);
                squares[i].transform.eulerAngles = new Vector3(0, 0, 0);
                squares[i].transform.eulerAngles = new Vector3(0.025f, .025f, 0);
            }
        }

        public void ReturnPiecesToPool()
        {
            var parent = GameObject.Find("Stones");
            var stones = GameObject.FindObjectsOfType<Stone>();
            for (int j = 0; j < stones.Length; j++)
            {
                stones[j].gameObject.transform.SetParent(parent.transform);
                stones[j].transform.localPosition = new Vector3(0, 0, 0);
                stones[j].transform.eulerAngles = new Vector3(0, 0, 0);
                stones[j].transform.eulerAngles = new Vector3(0.025f, .025f, 0);
            }

            var number = 0;
            switch (BoardSize.i_Size)
            {
                case 3:
                    number = 10;
                    break;
                case 4:
                    number = 15;
                    break;
                case 5:
                    number = 21;
                    break;
                case 6:
                    number = 30;
                    break;
                case 7:
                    number = 40;
                    break;
                case 8:
                    number = 50;
                    break;
                default:
                    Debug.LogError("this number does not make sense");
                    break;
            }

            var PlayerOnePool = GameObject.Find("PlayerOnePool").transform;
            var PlayerTwoPool = GameObject.Find("PlayerTwoPool").transform;

            int i = 0;
            int Max = 30;

            var init = 0f;

            //put pieces in player 1 pool
            Material PlayerOneMat = Resources.Load("Pieces/SideOneMat") as Material;
            Material PlayerTwoMat = Resources.Load("Pieces/SideTwoMat") as Material;
            while (i < number)
            {
                Transform t1 = null;
                try
                {
                    t1 = parent.transform.GetChild(0).transform;
                }
                catch
                {
                    break;
                }
                t1.SetParent(PlayerOnePool);

                //only move it if we have room in the tray
                if (i < Max / 2)
                {
                    t1.localPosition = new Vector3(0, -init, 0f);
                    init += trayOffset;
                }
                else
                {
                    t1.localPosition = new Vector3(0, 0, 0f);
                }

                t1.localEulerAngles = new Vector3(0, 0, 0);
                t1.GetComponent<MeshRenderer>().material = PlayerOneMat;
                i++;
            }
            i = 0;
            //put pieces in player 2 pool
            while (i < number)
            {
                var t2 = parent.transform.GetChild(1).transform;
                t2.SetParent(PlayerTwoPool);

                //only move it if we have room in the tray
                if (i < Max / 2)
                {
                    t2.localPosition = new Vector3(0, -init, 0f);
                    init += trayOffset;
                }
                else
                {
                    t2.localPosition = new Vector3(0, 0, 0f);
                }

                t2.localEulerAngles = new Vector3(0, 0, 0);
                t2.GetComponent<MeshRenderer>().material = PlayerTwoMat;

                i++;
            }

        }

        public GameObject GetPiece()
        {
            if (PlayerOnePool == null || PlayerTwoPool == null)
            {
                PlayerOnePool = GameObject.Find("PlayerOnePool");

                PlayerTwoPool = GameObject.Find("PlayerTwoPool");
            }
            switch (CurrentPlayer.Value)
            {
                case 0:
                    return PlayerOnePool.transform.GetChild(0).gameObject;
                case 1:
                    return PlayerTwoPool.transform.GetChild(0).gameObject;
            }

            return null;

        }

        public BoardSizeProperty GetBoardSizeProperty()
        {
            return BoardSize;
        }
    }

    [CustomPropertyDrawer(typeof(BoardSizeProperty))]
    public class BoardSizePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Begin change check for properties
            EditorGUI.BeginChangeCheck();

            // Using BeginProperty / EndProperty on the parent property means that
            EditorGUI.BeginProperty(position, label, property);
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // define rects
            var BoardSizeRect = new Rect(position.x, position.y, +position.width * .4f - 5, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            var SizeDrawer = new GUIContent();
            SizeDrawer.text = "";
            SizeDrawer.tooltip = "The size of the board";

            // EditorGUI.PropertyField(BoardSizeRect, property.FindPropertyRelative("size"), SizeDrawer);
            SerializedProperty boardSizeEnum = property.FindPropertyRelative("size");
            EditorGUI.PropertyField(position, boardSizeEnum, GUIContent.none, false);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                property.FindPropertyRelative("i_Size").intValue = (fieldInfo.GetValue(property.serializedObject.targetObject) as BoardSizeProperty).getSize();
                property.serializedObject.ApplyModifiedProperties();

                //callback to onproperty changed delegate
                TakBoard bar = property.serializedObject.targetObject as TakBoard;
                bar.UpdateBoard((fieldInfo.GetValue(property.serializedObject.targetObject) as BoardSizeProperty).getSize());
            }
        }
    }

    [Serializable]
    public class BoardSizeProperty
    {
        [EnumToString(new String[] { "3x3", "4x4", "5x5", "6x6", "7x7", "8x8" })]
        public BoardSizeNames size;
        public string s_Size;
        public int i_Size;
        public int getSize()
        {
            return (int)size + 3;
        }
    }

    public class EnumToString : PropertyAttribute
    {
        public string[] Sizes;

        public EnumToString(string[] strings)
        {
            this.Sizes = strings;
        }
    }

    [CustomPropertyDrawer(typeof(EnumToString))]
    public class EnumToStringDrawer : PropertyDrawer
    {
        int mySize;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumToString myAttribute = attribute as EnumToString;

            EditorGUI.BeginProperty(position, GUIContent.none, property);
            EditorGUI.BeginChangeCheck();

            mySize = property.intValue;

            GUIContent[] options = new GUIContent[myAttribute.Sizes.Length];
            for (int j = 0; j < myAttribute.Sizes.Length; j++)
            {
                options[j] = new GUIContent(myAttribute.Sizes[j]);
            }

            property.enumValueIndex = EditorGUI.Popup(new Rect(position.x, position.y, 100, 20), mySize, options);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var rect = new Rect(position.position, new Vector2(100, 20));

            EditorGUI.EndProperty();
        }
    }

}

