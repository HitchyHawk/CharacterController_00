using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ToggleType))]
public class Hitch_ToggleTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var toggleRect = new Rect(position.x, position.y, 25, position.height);
        var valueRect = new Rect(position.x + 25, position.y, position.xMax - (position.x + 25), position.height);

        
        SerializedProperty activeProperty = property.FindPropertyRelative("isActive");
        bool isActive = activeProperty.boolValue;
        SerializedProperty valueProperty = property.FindPropertyRelative(isActive? "value" : "defaultValue");

        EditorGUI.PropertyField(toggleRect, activeProperty, GUIContent.none);
        EditorGUI.LabelField(toggleRect, new GUIContent("", "Activate for custom, or continue using default"));

        GUI.enabled = isActive;
        EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
        GUI.enabled = true;


        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
