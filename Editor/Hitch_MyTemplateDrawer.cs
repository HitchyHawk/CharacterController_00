using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Hitch_AnimationMeta))]
public class Hitch_AnimationMetaDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var animationNameRect = new Rect(position.x, position.y, 160, position.height);
        var frameAmountRect = new Rect(position.x + 165, position.y, 30, position.height);
        var distancedistancePerCycleRect = new Rect(position.x + 200, position.y, 200, position.height);
        var isStationaryRect = new Rect(position.x + 405, position.y, 25, position.height);
        var loopsRect = new Rect(position.x + 430, position.y, 40, position.height);
        //200 = 160 + 5 + 30 + 5

        EditorGUI.PropertyField(animationNameRect, property.FindPropertyRelative("animationName"), GUIContent.none);
        EditorGUI.PropertyField(frameAmountRect, property.FindPropertyRelative("frameAmount"), GUIContent.none);
        EditorGUI.Slider(distancedistancePerCycleRect, property.FindPropertyRelative("distancePerCycle"),0.01f,10, GUIContent.none);
        EditorGUI.PropertyField(isStationaryRect, property.FindPropertyRelative("isStationary"), GUIContent.none);
        EditorGUI.PropertyField(loopsRect, property.FindPropertyRelative("loops"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        //EditorGUI.Slider(distancedistancePerCycleRect,pro)

        EditorGUI.EndProperty();
    }
}
