using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GlyphMap", menuName = "Else Return Home/Controllers/Glyph Map")]
public class GlyphMap : ScriptableObject
{
    [System.Serializable]
    public class GlyphMapping
    {
        public int elementId;
        public Sprite mainGlyph;
        public Sprite negativeGlyph;
        public Sprite positiveGlyph;
    }

    public string controllerGuid;

    [SerializeField]
    private List<GlyphMapping> glyphMappings = new List<GlyphMapping>();

    public void AddGlyphMapping(GlyphMapping glyphMapping)
    {
        glyphMappings.Add(glyphMapping);
    }

    public GlyphMapping GetGlyphForId(int elementId)
    {
        return glyphMappings.Find((x) => x.elementId == elementId);
    }

    public int GetIndexForId(int elementId)
    {
        return glyphMappings.FindIndex((x) => x.elementId == elementId);
    }
}
