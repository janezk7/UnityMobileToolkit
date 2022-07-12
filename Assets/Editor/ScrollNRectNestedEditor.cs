using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;

[CustomEditor(typeof(ScrollRectNested))]
public class ScrollRectNestedEditor : ScrollRectEditor
{
    SerializedProperty parentScrollRectProp;
    GUIContent parentScrollRectGUIContent = new GUIContent("Parent ScrollRect");

    protected override void OnEnable()
    {
        base.OnEnable();
        parentScrollRectProp = serializedObject.FindProperty("parentScrollRect");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(parentScrollRectProp, parentScrollRectGUIContent);
        serializedObject.ApplyModifiedProperties();
    }
}