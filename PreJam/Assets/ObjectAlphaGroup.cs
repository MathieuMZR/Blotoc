using UnityEngine;

public class ObjectAlphaGroup : MonoBehaviour
{
    [Range(0, 1)]
    public float alpha = 1f;

    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private Material[][] fadeMaterials;

    void Awake()
    {
        // Get all renderers in this object and its children
        renderers = GetComponentsInChildren<Renderer>();
        
        // Initialize arrays
        originalMaterials = new Material[renderers.Length][];
        fadeMaterials = new Material[renderers.Length][];

        // Store original materials and create fade materials
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
            fadeMaterials[i] = new Material[originalMaterials[i].Length];

            for (int j = 0; j < originalMaterials[i].Length; j++)
            {
                fadeMaterials[i][j] = new Material(originalMaterials[i][j]);
                fadeMaterials[i][j].SetFloat("_Mode", 2); // Set to Fade mode
                fadeMaterials[i][j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                fadeMaterials[i][j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                fadeMaterials[i][j].SetInt("_ZWrite", 0);
                fadeMaterials[i][j].DisableKeyword("_ALPHATEST_ON");
                fadeMaterials[i][j].EnableKeyword("_ALPHABLEND_ON");
                fadeMaterials[i][j].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                fadeMaterials[i][j].renderQueue = 3000;
            }
        }

        SetAlpha(alpha);
    }

    public void SetAlpha(float newAlpha)
    {
        alpha = Mathf.Clamp01(newAlpha);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = fadeMaterials[i];

            for (int j = 0; j < fadeMaterials[i].Length; j++)
            {
                Color color = fadeMaterials[i][j].color;
                color.a = alpha;
                fadeMaterials[i][j].color = color;
            }
        }
    }

    void OnDisable()
    {
        // Restore original materials when disabled
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterials[i];
        }
    }

    void OnDestroy()
    {
        // Clean up fade materials
        for (int i = 0; i < fadeMaterials.Length; i++)
        {
            for (int j = 0; j < fadeMaterials[i].Length; j++)
            {
                Destroy(fadeMaterials[i][j]);
            }
        }
    }
}