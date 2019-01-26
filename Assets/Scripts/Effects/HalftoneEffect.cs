using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalftoneEffect : MonoBehaviour
{
    public Material halftoneMat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Graphics.Blit(source, destination, halftoneMat);
    }
}
