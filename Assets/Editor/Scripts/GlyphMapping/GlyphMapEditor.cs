using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GlyphMap))]
public class GlyphMapEditor : Editor
{
    SerializedProperty guidProperty;
    SerializedProperty glyphMappingsProperty;
    GUIStyle titleStyle = new GUIStyle();
    GUIStyle smallTitleStyle = new GUIStyle();

    private void OnEnable()
    {
        guidProperty = serializedObject.FindProperty("controllerGuid");
        glyphMappingsProperty = serializedObject.FindProperty("glyphMappings");

        titleStyle.fontSize = 20;
        smallTitleStyle.fontStyle = FontStyle.Bold;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        string[] elementIdentifierMaps = AssetDatabase.FindAssets("t:ElementIdentifierMap");
        if (elementIdentifierMaps.Length <= 0)
        {
            GUILayout.Label("Couldn't find an element identifier map, here's the default inspector");
            DrawDefaultInspector();
            return;
        }

        if (elementIdentifierMaps.Length > 1)
        {
            GUILayout.Label("More than one element identifier map, using the first one");
        }

        GlyphMap glyphMap = (GlyphMap)target;
        ElementIdentifierMap elementIdentifierMap = AssetDatabase.LoadAssetAtPath<ElementIdentifierMap>(AssetDatabase.GUIDToAssetPath(elementIdentifierMaps[0]));
        ElementIdentifierMap.ControllerMapping controllerMapping = elementIdentifierMap.GetMappingForGuid(glyphMap.controllerGuid);

        if (controllerMapping == null)
        {
            GUILayout.Label("GUID doesn't match any known controllers, here's the default inspector");
            DrawDefaultInspector();
            return;
        }

        DrawControllerMapping(controllerMapping);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawControllerMapping(ElementIdentifierMap.ControllerMapping controllerMapping)
    {
        GlyphMap glyphMap = (GlyphMap)target;

        GUILayout.Label(new GUIContent(controllerMapping.controllerName), titleStyle);

        GUILayout.BeginHorizontal();
        GUILayout.Label("GUID");
        guidProperty.stringValue = GUILayout.TextField(glyphMap.controllerGuid);
        GUILayout.EndHorizontal();

        foreach (KeyValuePair<int, ElementIdentifierMap.ElementIdentifier> pair in controllerMapping.elementIdToNameMap)
        {
            int elementId = pair.Key;
            var elementIdentifier = pair.Value;

            GlyphMap.GlyphMapping currentMapping = glyphMap.GetGlyphForId(elementId);
            GlyphMap.GlyphMapping newGlyphMapping = new GlyphMap.GlyphMapping();

            Sprite mainGlyph = null;
            Sprite negativeGlyph = null;
            Sprite positiveGlyph = null;
            if (currentMapping != null)
            {
                mainGlyph = currentMapping.mainGlyph;
                negativeGlyph = currentMapping.negativeGlyph;
                positiveGlyph = currentMapping.positiveGlyph;
            }

            if (elementIdentifier.type == ElementIdentifierMap.ElementIdentifierType.Button ||
                elementIdentifier.type == ElementIdentifierMap.ElementIdentifierType.Stick)
            {
                newGlyphMapping.mainGlyph = (Sprite)EditorGUILayout.ObjectField(new GUIContent(elementIdentifier.name), mainGlyph, typeof(Sprite), false);
            }
            else if (elementIdentifier.type == ElementIdentifierMap.ElementIdentifierType.Axis)
            {
                GUILayout.Label(new GUIContent(elementIdentifier.name), smallTitleStyle);

                newGlyphMapping.mainGlyph = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Main"), mainGlyph, typeof(Sprite), false);
                newGlyphMapping.negativeGlyph = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Negative"), negativeGlyph, typeof(Sprite), false);
                newGlyphMapping.positiveGlyph = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Positive"), positiveGlyph, typeof(Sprite), false);

                EditorGUILayout.Space();
            }

            if (newGlyphMapping.mainGlyph != mainGlyph ||
                newGlyphMapping.negativeGlyph != negativeGlyph ||
                newGlyphMapping.positiveGlyph != positiveGlyph)
            {
                int index = glyphMap.GetIndexForId(elementId);
                if (index < 0)
                {
                    index = glyphMappingsProperty.arraySize;
                    glyphMappingsProperty.InsertArrayElementAtIndex(glyphMappingsProperty.arraySize);
                    glyphMappingsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("elementId").intValue = elementId;
                }

                glyphMappingsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("mainGlyph").objectReferenceValue = newGlyphMapping.mainGlyph;
                glyphMappingsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("negativeGlyph").objectReferenceValue = newGlyphMapping.negativeGlyph;
                glyphMappingsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("positiveGlyph").objectReferenceValue = newGlyphMapping.positiveGlyph;
            }
        }
    }
}
