using System.Collections.Generic;
using UnityEngine;
using Rewired;

[CreateAssetMenu(fileName = "KeyboardGlyphMap", menuName = "Else Return Home/Controllers/Keyboard Glyph Map")]
public class KeyboardGlyphMap : ScriptableObject
{
    [System.Serializable]
    public class GlyphMapping
    {
        public Rewired.KeyboardKeyCode keyCode;
        public Sprite glyph;
    }

    [SerializeField]
    private List<GlyphMapping> glyphMappings = new List<GlyphMapping>();

    public void AddGlyphMapping(GlyphMapping glyphMapping)
    {
        glyphMappings.Add(glyphMapping);
    }

    public GlyphMapping GetGlyphForKeyCode(KeyboardKeyCode keyCode)
    {
        return glyphMappings.Find((x) => x.keyCode == keyCode);
    }

    public int GetIndexForKeyCode(KeyboardKeyCode keyCode)
    {
        return glyphMappings.FindIndex((x) => x.keyCode == keyCode);
    }
}
