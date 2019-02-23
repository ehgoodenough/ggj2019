using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ElementIdentifierMap))]
public class ElementIdentifierMapEditor : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        ElementIdentifierMap elementIdentifierMap = (ElementIdentifierMap)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Reload from file"))
        {
            elementIdentifierMap.LoadMap();
        }
    }
}
