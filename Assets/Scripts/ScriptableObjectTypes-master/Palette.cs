using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class Palette : ScriptableObject
{
    public PaletteColor[] colors;
    public Color fallbackColor;

    public Color GetColorByName(string name)
    {
        foreach (PaletteColor pc in colors)
        {
            if (pc.name == name)
            {
                return pc.color;
            }
        }
        return fallbackColor;
    }
}

[System.Serializable]
public class PaletteColor
{
    public string name;
    public Color color;
}

#if UNITY_EDITOR
// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(PaletteColor))]
public class PaletteColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var nameRect = new Rect(position.x, position.y, 100, position.height);
        var colorRect = new Rect(position.x + 100, position.y, 50, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("color"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif

