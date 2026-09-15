using Control.Camera;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(PlayerTransparency))]
public class PlayerTransparencyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();

        Foldout dropdown = new Foldout();
        dropdown.text = "player transparency settings";
        dropdown.value = false;

        PropertyField fadeDistance = new PropertyField(property.FindPropertyRelative("fadeDistance"));
        PropertyField maxFade = new PropertyField(property.FindPropertyRelative("maxFade"));

        dropdown.Add(fadeDistance);
        dropdown.Add(maxFade);

        container.Add(dropdown);

        return container;
    }
}
