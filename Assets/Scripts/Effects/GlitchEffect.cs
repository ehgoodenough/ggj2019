/**
This work is licensed under a Creative Commons Attribution 3.0 Unported License.
http://creativecommons.org/licenses/by/3.0/deed.en_GB

You are free:

to copy, distribute, display, and perform the work
to make derivative works
to make commercial use of the work
*/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Taken from Unity ImageEffects source
[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
public class ImageEffectBase : MonoBehaviour
{
    /// Provides a shader property that is set in the inspector
    /// and a material instantiated from the shader
    public Shader shader;
    private Material m_Material;

    protected void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        // Disable the image effect if the shader can't
        // run on the users graphics card
        if (!shader || !shader.isSupported)
            enabled = false;
    }

    protected Material material
    {
        get
        {
            if (m_Material == null)
            {
                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }

    protected void OnDisable()
    {
        if (m_Material)
        {
            DestroyImmediate(m_Material);
        }
    }
}


#if UNITY_EDITOR
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/GlitchEffect")]
#endif
public class GlitchEffect : ImageEffectBase
{
    public float overallIntensity;

    public Texture2D displacementMap;
    float glitchup, glitchdown, flicker,
            glitchupTime = 0.05f, glitchdownTime = 0.05f, flickerTime = 0.5f;

    [Header("Glitch Intensity")]

    [Range(0, 1)]
    public float intensity;

    [Range(0, 1)]
    public float flipIntensity;

    [Range(0, 1)]
    public float colorIntensity;

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        material.SetFloat("_Intensity", intensity);
        material.SetFloat("_ColorIntensity", colorIntensity);
        material.SetTexture("_DispTex", displacementMap);

        flicker += Time.deltaTime * colorIntensity * overallIntensity;
        if (flicker > flickerTime)
        {
            material.SetFloat("filterRadius", Random.Range(-3f, 3f) * colorIntensity);
            material.SetVector("direction", Quaternion.AngleAxis(Random.Range(0, 360) * colorIntensity, Vector3.forward) * Vector4.one);
            flicker = 0;
            flickerTime = Random.value;
        }

        if (colorIntensity == 0)
            material.SetFloat("filterRadius", 0);

        glitchup += Time.deltaTime * flipIntensity * overallIntensity;
        if (glitchup > glitchupTime)
        {
            if (Random.value < 0.1f * flipIntensity)
                material.SetFloat("flip_up", Random.Range(0, 1f) * flipIntensity);
            else
                material.SetFloat("flip_up", 0);

            glitchup = 0;
            glitchupTime = Random.value / 10f;
        }

        if (flipIntensity == 0)
            material.SetFloat("flip_up", 0);


        glitchdown += Time.deltaTime * flipIntensity * overallIntensity;
        if (glitchdown > glitchdownTime)
        {
            if (Random.value < 0.1f * flipIntensity)
                material.SetFloat("flip_down", 1 - Random.Range(0, 1f) * flipIntensity);
            else
                material.SetFloat("flip_down", 1);

            glitchdown = 0;
            glitchdownTime = Random.value / 10f;
        }

        if (flipIntensity == 0)
            material.SetFloat("flip_down", 1);

        if (Random.value < 0.05 * intensity)
        {
            material.SetFloat("displace", Random.value * intensity);
            material.SetFloat("scale", 1 - Random.value * intensity);
        }
        else
            material.SetFloat("displace", 0);

        Graphics.Blit(source, destination, material);
    }
}