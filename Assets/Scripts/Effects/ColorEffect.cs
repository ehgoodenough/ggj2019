using UnityEngine;

public class ColorEffect : MonoBehaviour
{
    private Camera cam;
    private Camera colorBufferCam;
    public Shader colorBufferShader;
    private RenderTexture colorBuffer;
    private RenderTexture depthBuffer;

    public Material halftoneMat;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;

        colorBufferCam = PostProcessUtils.GenerateBufferCamera(1 << LayerMask.NameToLayer("Color") | 1 << LayerMask.NameToLayer("ColorAndOutline"), "Color", cam);
    }

    void Update()
    {
        PostProcessUtils.GenerateBufferIfNecessary(ref colorBuffer, 0, RenderTextureFormat.R8, "Buffer");
        PostProcessUtils.GenerateBufferIfNecessary(ref depthBuffer, 0, RenderTextureFormat.Depth, "Depth buffer");
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        colorBufferCam.RenderWithShader(colorBufferShader, "");
        colorBufferCam.SetTargetBuffers(colorBuffer.colorBuffer, depthBuffer.depthBuffer);

        halftoneMat.SetTexture("_ColorBuffer", colorBuffer);
        halftoneMat.SetTexture("_DepthBuffer", depthBuffer);
        Graphics.Blit(source, destination, halftoneMat);
    }
}