using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ElementIdentifierMap", menuName = "Else Return Home/Controllers/Element Identifier Map")]
public class ElementIdentifierMap: ScriptableObject
{
    [SerializeField]
    private TextAsset elementIdentifierCsv;

    public enum ElementIdentifierType
    {
        Axis = 0,
        Button = 1,
        Stick = 100
    }

    public class ElementIdentifier
    {
        public string name;
        public ElementIdentifierType type;
    }

    public class ControllerMapping
    {
        public string controllerName;
        public Dictionary<int, ElementIdentifier> elementIdToNameMap = new Dictionary<int, ElementIdentifier>();
    }

    private Dictionary<string, ControllerMapping> controllerMappings = new Dictionary<string, ControllerMapping>();

    private void OnEnable()
    {
        LoadMap();
    }

    public void LoadMap()
    {
        if (elementIdentifierCsv != null)
        {
            Parse(elementIdentifierCsv.ToString());
        }
    }

    private void Parse(string elementIdentifierCsv)
    {
        controllerMappings.Clear();

        List<string> lines = new List<string>(elementIdentifierCsv.Split('\n'));
        IEnumerator<string> lineEnumerator = lines.GetEnumerator();

        // Skip the version line
        lineEnumerator.MoveNext();
        // Skip the column name header line
        lineEnumerator.MoveNext();

        while (lineEnumerator.MoveNext())
        {
            string[] row = lineEnumerator.Current.Split(',');
            string controllerName = row[1].Replace("\"", "");
            string controllerGuid = row[2].Replace("\"", "");
            string elementId = row[3].Replace("\"", "");
            string elementName = row[4].Replace("\"", "");
            string type = row[5].Replace("\"", "");

            ControllerMapping controllerMapping;
            if (!controllerMappings.TryGetValue(controllerGuid, out controllerMapping))
            {
                controllerMapping = new ControllerMapping();
                controllerMapping.controllerName = controllerName;
                controllerMappings.Add(controllerGuid, controllerMapping);
            }

            ElementIdentifier elementIdentifier = new ElementIdentifier();
            elementIdentifier.name = elementName;
            elementIdentifier.type = (ElementIdentifierType)int.Parse(type);

            controllerMapping.elementIdToNameMap.Add(int.Parse(elementId), elementIdentifier);
        }
    }

    public ControllerMapping GetMappingForGuid(string guid)
    {
        if (guid == null)
        {
            return null;
        }

        ControllerMapping mapping = null;
        controllerMappings.TryGetValue(guid, out mapping);
        return mapping;
    }
}
