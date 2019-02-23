using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(KeyboardGlyphMap))]
public class KeyboardGlyphMapEditor : Editor
{
    SerializedProperty glyphMappingsProperty;

    private void OnEnable()
    {
        glyphMappingsProperty = serializedObject.FindProperty("glyphMappings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        KeyboardGlyphMap glyphMap = (KeyboardGlyphMap)target;

        foreach (Rewired.KeyboardKeyCode keyCode in Enum.GetValues(typeof(Rewired.KeyboardKeyCode)))
        {
            KeyboardGlyphMap.GlyphMapping currentMapping = glyphMap.GetGlyphForKeyCode(keyCode);
            KeyboardGlyphMap.GlyphMapping newMapping = new KeyboardGlyphMap.GlyphMapping();

            Sprite glyph = null;
            if (currentMapping != null)
            {
                glyph = currentMapping.glyph;
            }

            newMapping.glyph = (Sprite)EditorGUILayout.ObjectField(new GUIContent(keyCode.ToString()), glyph, typeof(Sprite), false);

            if (newMapping.glyph != glyph)
            {
                int index = glyphMap.GetIndexForKeyCode(keyCode);
                if (index < 0)
                {
                    index = glyphMappingsProperty.arraySize;
                    glyphMappingsProperty.InsertArrayElementAtIndex(glyphMappingsProperty.arraySize);
                }

                SerializedProperty glyphMappingArrayProperty = glyphMappingsProperty.GetArrayElementAtIndex(index);
                glyphMappingArrayProperty.FindPropertyRelative("keyCode").intValue = (int)keyCode;
                glyphMappingArrayProperty.FindPropertyRelative("glyph").objectReferenceValue = newMapping.glyph;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
