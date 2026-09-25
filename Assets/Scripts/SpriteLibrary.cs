using UnityEngine;

// Loads art and audio from Resources so the game works after a clone too.
public static class SpriteLibrary
{
    private const string ArtPath = "FlappyArt/";
    private const string AudioPath = "FlappyAudio/";

    private static readonly string[] ShaderCandidates =
    {
        "Sprites/Default",
        "Universal Render Pipeline/2D/Sprite-Unlit-Default",
        "Universal Render Pipeline/2D/Sprite-Lit-Default"
    };

    private static Material spriteMaterial;
    private static bool materialResolved;

    public static Sprite Load(string spriteName)
    {
        return Resources.Load<Sprite>(ArtPath + spriteName);
    }

    public static AudioClip LoadClip(string clipName)
    {
        return Resources.Load<AudioClip>(AudioPath + clipName);
    }

    // Use a material from Resources so sprites show up right away.
    public static Material SpriteMaterial
    {
        get
        {
            if (materialResolved && spriteMaterial != null)
            {
                return spriteMaterial;
            }

            materialResolved = true;
            spriteMaterial = Resources.Load<Material>("FlappySpriteUnlit");
            if (spriteMaterial == null)
            {
                spriteMaterial = BuildSpriteMaterial();
            }

            return spriteMaterial;
        }
    }

    private static Material BuildSpriteMaterial()
    {
        for (int i = 0; i < ShaderCandidates.Length; i++)
        {
            Shader shader = Shader.Find(ShaderCandidates[i]);
            if (IsUsable(shader))
            {
                return new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            }
        }

        return null;
    }

    private static bool IsUsable(Shader shader)
    {
        return shader != null && shader.isSupported && shader.name != "Hidden/InternalErrorShader";
    }

    // Fix materials on all sprites in the scene so nothing stays invisible.
    public static void FixAllSceneMaterials()
    {
        Material material = SpriteMaterial;
        if (material == null)
        {
            return;
        }

        SpriteRenderer[] renderers = Object.FindObjectsByType<SpriteRenderer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].sharedMaterial = material;
            }
        }
    }

    public static SpriteRenderer CreateRenderer(GameObject owner, Sprite sprite, string sortingLayer, int order)
    {
        Material material = SpriteMaterial;

        SpriteRenderer renderer = owner.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = order;

        if (material != null)
        {
            renderer.sharedMaterial = material;
        }

        if (!string.IsNullOrEmpty(sortingLayer) && SortingLayerExists(sortingLayer))
        {
            renderer.sortingLayerName = sortingLayer;
        }

        return renderer;
    }

    private static bool SortingLayerExists(string layerName)
    {
        SortingLayer[] layers = SortingLayer.layers;
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name == layerName)
            {
                return true;
            }
        }

        return false;
    }
}
