using UnityEngine;

public class PostProcessUtils
{
    public static Camera GenerateBufferCamera(int cullingMask, string name, Camera parent)
    {
        Camera newCamera = new GameObject().AddComponent<Camera>();
        newCamera.gameObject.name = name;
        newCamera.CopyFrom(parent);
        newCamera.transform.parent = parent.transform;
        newCamera.clearFlags = CameraClearFlags.Color;
        newCamera.backgroundColor = Color.black;
        newCamera.cullingMask = cullingMask;
        newCamera.renderingPath = RenderingPath.Forward;
        newCamera.depthTextureMode = DepthTextureMode.None;
        newCamera.enabled = false;
        return newCamera;
    }

    public static void GenerateBufferIfNecessary(ref RenderTexture buffer, int depth, RenderTextureFormat format, string name)
    {

        if (!buffer || buffer.width != Screen.width || buffer.height != Screen.height)
        {
            buffer = new RenderTexture(Screen.width, Screen.height, depth, format);
            buffer.name = name;
            buffer.Create();
        }
    }
}
