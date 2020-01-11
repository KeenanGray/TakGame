using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Board))]
public class CustomBoardInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();

        SerializedProperty boardSize = property.FindPropertyRelative("BoardSize");

        // Store old indent level and set it to 0, the PrefixLabel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.PropertyField(position,
           boardSize,
           new GUIContent("DS", "JFK"));

        EditorGUI.EndChangeCheck();
        EditorGUI.EndProperty();
    }

}