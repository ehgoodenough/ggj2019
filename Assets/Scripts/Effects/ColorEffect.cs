using UnityEngine;

public class ColorEffect : MonoBehaviour
{
    private Camera cam;
    private Camera colorBufferCam;
    public Shader colorBufferShader;
    private RenderTexture colorBuffer;
    private RenderTexture depthBuffer;

    public Material halftoneMat;

    public Transform saturationWaveOrigin;

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

        if (saturationWaveOrigin)
        {
            halftoneMat.SetVector("_EffectOrigin", saturationWaveOrigin.position);
        }

        halftoneMat.SetTexture("_ColorBuffer", colorBuffer);
        halftoneMat.SetTexture("_DepthBuffer", depthBuffer);
        //Graphics.Blit(source, destination, halftoneMat);
        RaycastCornerBlit(source, destination, halftoneMat);
    }
    void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
    {
        // Compute Frustum Corners
        float camFar = cam.farClipPlane;
        float camFov = cam.fieldOfView;
        float camAspect = cam.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = cam.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = cam.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (cam.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (cam.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (cam.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (cam.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
        RenderTexture.active = dest;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
}
