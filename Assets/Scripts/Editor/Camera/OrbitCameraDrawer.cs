using Control.Camera;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(OrbitCamera))]
public class OrbitCameraDrawer : PropertyDrawer
{

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();

        Foldout dropdown = new Foldout();
        dropdown.text = "Orbit Camera Settings";
        dropdown.value = false;

        PropertyField cameraLockProtectionFactor = new PropertyField(property.FindPropertyRelative("cameraLockProtectionFactor"));
        PropertyField minPhi = new PropertyField(property.FindPropertyRelative("minPhi"));
        PropertyField maxPhi = new PropertyField(property.FindPropertyRelative("maxPhi"));
        PropertyField defaultPhiOffset = new PropertyField(property.FindPropertyRelative("defaultPhiOffset"));

        PropertyField armLength = new PropertyField(property.FindPropertyRelative("armLength"));
        PropertyField minArmLength = new PropertyField(property.FindPropertyRelative("minArmLength"));
        PropertyField maxArmLength = new PropertyField(property.FindPropertyRelative("maxArmLength"));

        PropertyField panSpeed = new PropertyField(property.FindPropertyRelative("panSpeed"));
        PropertyField zoomSpeed = new PropertyField(property.FindPropertyRelative("zoomSensitivity"));
        PropertyField flipCameraHorizontalAxis = new PropertyField(property.FindPropertyRelative("flipCameraHorizontalAxis"));
        PropertyField flipCameraVerticalAxis = new PropertyField(property.FindPropertyRelative("flipCameraVerticalAxis"));

        dropdown.Add(armLength);
        dropdown.Add(minArmLength);
        dropdown.Add(maxArmLength);
        dropdown.Add(panSpeed);
        dropdown.Add(zoomSpeed);
        dropdown.Add(flipCameraHorizontalAxis);
        dropdown.Add(flipCameraVerticalAxis);

        dropdown.Add(cameraLockProtectionFactor);
        dropdown.Add(minPhi);
        dropdown.Add(maxPhi);
        dropdown.Add(defaultPhiOffset);

        container.Add(dropdown);

        return container;
    }

}
