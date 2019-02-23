using System.Collections.Generic;
using UnityEngine;
using Rewired;

[CreateAssetMenu(fileName = "GlyphDatabase", menuName = "Else Return Home/Controllers/Glyph Database")]
public class GlyphDatabase : ScriptableObject
{
    // This is just here so it gets included in the build
    [SerializeField]
    private ElementIdentifierMap elementIdentifierMap;

    [SerializeField]
    private List<GlyphMap> glyphMaps;

    [SerializeField]
    private KeyboardGlyphMap keyboardGlyphMap;

    [SerializeField]
    private MouseGlyphMap mouseGlyphMap;

    public Sprite defaultGlyph;

    public Sprite GetGlyph(ActionElementMap actionElementMap)
    {
        Controller controller = actionElementMap.controllerMap.controller;

        if (controller.type == ControllerType.Joystick)
        {
            Joystick joystick = controller as Joystick;
            string guid = joystick.hardwareTypeGuid.ToString();
            GlyphMap glyphMap = glyphMaps.Find((x) => x.controllerGuid == guid);

            if (glyphMap == null)
            {
                return defaultGlyph;
            }

            GlyphMap.GlyphMapping glyphMapping = glyphMap.GetGlyphForId(actionElementMap.elementIdentifierId);
            if (actionElementMap.axisRange == AxisRange.Full || actionElementMap.elementType == ControllerElementType.Button)
            {
                return glyphMapping.mainGlyph ?? defaultGlyph;
            }
            else if (actionElementMap.axisRange == AxisRange.Negative)
            {
                return glyphMapping.negativeGlyph ?? defaultGlyph;
            }
            else if (actionElementMap.axisRange == AxisRange.Positive)
            {
                return glyphMapping.positiveGlyph ?? defaultGlyph;
            }

            return defaultGlyph;
        }
        else if (controller.type == ControllerType.Keyboard)
        {
            var glyphMapping = keyboardGlyphMap.GetGlyphForKeyCode(actionElementMap.keyboardKeyCode);
            if (glyphMapping != null)
            {
                return glyphMapping.glyph;
            }

            return defaultGlyph;
        }
        else if (controller.type == ControllerType.Mouse)
        {
            MouseGlyphMap.GlyphMapping glyphMapping = mouseGlyphMap.GetGlyphForId(actionElementMap.elementIdentifierId);
            if (actionElementMap.axisRange == AxisRange.Full || actionElementMap.elementType == ControllerElementType.Button)
            {
                return glyphMapping.mainGlyph ?? defaultGlyph;
            }
            else if (actionElementMap.axisRange == AxisRange.Negative)
            {
                return glyphMapping.negativeGlyph ?? defaultGlyph;
            }
            else if (actionElementMap.axisRange == AxisRange.Positive)
            {
                return glyphMapping.positiveGlyph ?? defaultGlyph;
            }

            return defaultGlyph;
        }
        else
        {
            Debug.LogError("Unsupported controller type: " + controller.type, this);
            return defaultGlyph;
        }
    }
}
