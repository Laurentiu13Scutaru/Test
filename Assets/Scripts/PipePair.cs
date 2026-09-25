using UnityEngine;

public class PipePair : MonoBehaviour
{
    [SerializeField] private Transform topPipe;
    [SerializeField] private Transform bottomPipe;

    private static Transform bird;
    private static Sprite pipeSprite;

    private bool scored;

    private void Awake()
    {
        EnsurePipes();

        if (bird == null)
        {
            BirdController found = FindFirstObjectByType<BirdController>();
            if (found != null)
            {
                bird = found.transform;
            }
        }
    }

    public void Place(float x, float gapCenterY)
    {
        GameConfig config = GameManager.Instance != null ? GameManager.Instance.Config : null;
        if (config == null || topPipe == null || bottomPipe == null)
        {
            return;
        }

        scored = false;
        float halfGap = config.pipeGap * 0.5f;

        transform.position = new Vector3(x, 0f, 0f);
        topPipe.localPosition = new Vector3(0f, gapCenterY + halfGap, 0f);
        bottomPipe.localPosition = new Vector3(0f, gapCenterY - halfGap, 0f);
        gameObject.SetActive(true);
    }

    private void Update()
    {
        GameManager manager = GameManager.Instance;
        if (manager == null || manager.State != GameState.Playing)
        {
            return;
        }

        GameConfig config = manager.Config;
        transform.Translate(Vector3.left * manager.CurrentScrollSpeed * Time.deltaTime);

        float birdX = bird != null ? bird.position.x : -1.4f;
        if (!scored && transform.position.x <= birdX)
        {
            scored = true;
            manager.AddPoint();
        }

        if (transform.position.x < config.pipeDespawnX)
        {
            gameObject.SetActive(false);
        }
    }

    // If the pipe pieces are missing, build them here so the game does not crash.
    private void EnsurePipes()
    {
        if (bottomPipe == null)
        {
            bottomPipe = transform.Find("PipeBottom");
        }

        if (topPipe == null)
        {
            topPipe = transform.Find("PipeTop");
        }

        if (bottomPipe == null)
        {
            bottomPipe = CreatePipe("PipeBottom", false);
        }

        if (topPipe == null)
        {
            topPipe = CreatePipe("PipeTop", true);
        }
    }

    private Transform CreatePipe(string childName, bool flipped)
    {
        var go = new GameObject(childName);
        go.tag = "Hazard";
        go.transform.SetParent(transform, false);
        go.transform.localScale = flipped ? new Vector3(1f, -1f, 1f) : Vector3.one;

        SpriteLibrary.CreateRenderer(go, GetPipeSprite(), "Pipes", 10);
        go.AddComponent<BoxCollider2D>();
        return go.transform;
    }

    private static Sprite GetPipeSprite()
    {
        if (pipeSprite != null)
        {
            return pipeSprite;
        }

        pipeSprite = SpriteLibrary.Load("pipe");
        if (pipeSprite != null)
        {
            return pipeSprite;
        }

        SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < renderers.Length; i++)
        {
            Sprite sprite = renderers[i].sprite;
            if (sprite != null && sprite.name.StartsWith("pipe", System.StringComparison.OrdinalIgnoreCase))
            {
                pipeSprite = sprite;
                return pipeSprite;
            }
        }

        pipeSprite = BuildFallbackPipe();
        return pipeSprite;
    }

    // Backup pipe sprite if the art files are missing.
    private static Sprite BuildFallbackPipe()
    {
        const int width = 52;
        const int height = 320;
        const int lipTop = 292;

        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };

        var body = new Color32(115, 191, 46, 255);
        var dark = new Color32(73, 128, 28, 255);
        var light = new Color32(168, 224, 86, 255);
        var outline = new Color32(47, 84, 16, 255);
        var lip = new Color32(98, 168, 36, 255);
        var clear = new Color32(0, 0, 0, 0);

        var pixels = new Color32[width * height];
        for (int y = 0; y < height; y++)
        {
            bool inLip = y >= lipTop;
            int left = inLip ? 0 : 6;
            int right = inLip ? width - 1 : width - 7;

            for (int x = 0; x < width; x++)
            {
                if (x < left || x > right)
                {
                    pixels[y * width + x] = clear;
                    continue;
                }

                bool edge = x == left || x == right || y == lipTop || y == height - 1 || (!inLip && y == 0);
                Color32 color;
                if (edge)
                {
                    color = outline;
                }
                else if (x < left + (inLip ? 10 : 8))
                {
                    color = light;
                }
                else if (x > right - (inLip ? 10 : 8))
                {
                    color = dark;
                }
                else
                {
                    color = inLip ? lip : body;
                }

                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply(false, false);

        var sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 1f),
            32f);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }
}
