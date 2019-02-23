using UnityEngine;

[CreateAssetMenu(fileName = "MouseGlyphMap", menuName = "Else Return Home/Controllers/Mouse Glyph Map")]
public class MouseGlyphMap : ScriptableObject
{
    [System.Serializable]
    public class GlyphMapping
    {
        public int elementId;
        public Sprite mainGlyph;
        public Sprite negativeGlyph;
        public Sprite positiveGlyph;
    }

    public GlyphMapping mouseXAxis;
    public GlyphMapping mouseYAxis;
    public GlyphMapping mouseScrollAxis;
    public Sprite leftMouseButton;
    public Sprite rightMouseButton;
    public Sprite middleMouseButton;
    public Sprite mouseButton4;
    public Sprite mouseButton5;

    public GlyphMapping GetGlyphForId(int elementId)
    {
        // This was gleaned by experimentation
        switch (elementId)
        {
            case 0:
                return mouseXAxis;
            case 1:
                return mouseYAxis;
            case 2:
                return mouseScrollAxis;
            case 3:
                return new GlyphMapping { elementId = elementId, mainGlyph = leftMouseButton };
            case 4:
                return new GlyphMapping { elementId = elementId, mainGlyph = rightMouseButton };
            case 5:
                return new GlyphMapping { elementId = elementId, mainGlyph = middleMouseButton };
            case 6:
                return new GlyphMapping { elementId = elementId, mainGlyph = mouseButton4 };
            case 7:
                return new GlyphMapping { elementId = elementId, mainGlyph = mouseButton5 };
            default:
                return null;
        }
    }
}
