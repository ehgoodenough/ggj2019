using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    private Camera cam;
    private Camera outlineBufferCam;
    private RenderTexture outlineBuffer;
    private RenderTexture depthBuffer;

    public Shader outlineBufferShader;
    public Material outlineMat;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;

        outlineBufferCam = PostProcessUtils.GenerateBufferCamera(1 << LayerMask.NameToLayer("Outline") | 1 << LayerMask.NameToLayer("ColorAndOutline"), "Outline", cam);
    }

    void Update()
    {
        PostProcessUtils.GenerateBufferIfNecessary(ref outlineBuffer, 0, RenderTextureFormat.R8, "Buffer");
        PostProcessUtils.GenerateBufferIfNecessary(ref depthBuffer, 0, RenderTextureFormat.Depth, "Depth buffer"); // redundant... refactor later
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        outlineBufferCam.RenderWithShader(outlineBufferShader, "");
        outlineBufferCam.SetTargetBuffers(outlineBuffer.colorBuffer, depthBuffer.depthBuffer);

        outlineMat.SetTexture("_SceneTex", source);
        outlineMat.SetTexture("_OutlineDepthBuffer", depthBuffer);
        outlineMat.SetTexture("_OutlineBuffer", outlineBuffer);
        Graphics.Blit(outlineBuffer, destination, outlineMat);
    }
}