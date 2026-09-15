using UnityEngine;
using UnityEditor;
using DataStructures.Localization;
using System.Reflection;

[CustomPropertyDrawer(typeof(LocalizableString))]
public class LocalizableStringDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin the property to handle prefabs and multi-selection correctly
        EditorGUI.BeginProperty(position, label, property);

        // Target the inner "value" string field
        SerializedProperty valueProp = property.FindPropertyRelative("value");

        // Draw the text input field using the parent variable's label
        if (valueProp != null)
        {
            // apply the change to the serialized property first
            valueProp.stringValue = EditorGUI.TextField(position, label, valueProp.stringValue);

        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Error: 'value' property not found");
        }

        EditorGUI.EndProperty();
    }
}
