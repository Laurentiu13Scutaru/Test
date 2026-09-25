using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BirdController : MonoBehaviour
{
    [SerializeField] private Sprite[] flapFrames;
    [SerializeField] private GameAudio gameAudio;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Sprite[] altFrames;
    private Sprite[] activeFrames;
    private Sprite[] mainFrames;
    private Vector3 startPosition;
    private float idleTime;
    private float animTime;
    private bool dead;
    private bool visualsReady;

    private GameConfig Config => GameManager.Instance.Config;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        RebuildVisuals();
        altFrames = LoadAltFrames();
    }

    private void Start()
    {
        // Materials / Resources may finish importing after Awake on a fresh open.
        RebuildVisuals();
        ResetBird();
    }

    private void OnEnable()
    {
        RebuildVisuals();
    }

    // Make sure the bird can be seen: load sprite + material, or draw a backup.
    private void RebuildVisuals()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        activeFrames = ResolveFrames();
        if (activeFrames == null || activeFrames.Length == 0 || activeFrames[0] == null)
        {
            activeFrames = BuildProceduralFrames();
        }

        mainFrames = activeFrames;

        Material material = SpriteLibrary.SpriteMaterial;
        if (material != null)
        {
            spriteRenderer.sharedMaterial = material;
        }

        spriteRenderer.enabled = true;
        // Default layer is always lit by the global 2D light, even if custom
        // sorting-layer masks get reset by the editor.
        spriteRenderer.sortingLayerName = "Default";
        spriteRenderer.sortingOrder = 200;
        spriteRenderer.sprite = activeFrames[0];

        Color color = spriteRenderer.color;
        color.a = 1f;
        if (PlayerPrefsHasTint())
        {
            color = ReadSavedTint();
        }

        spriteRenderer.color = color;
        transform.localScale = Vector3.one;
        visualsReady = true;
    }

    private Sprite[] ResolveFrames()
    {
        Sprite frame0 = SpriteLibrary.Load("bird_0");
        Sprite frame1 = SpriteLibrary.Load("bird_1");
        Sprite frame2 = SpriteLibrary.Load("bird_2");
        if (frame0 != null && frame1 != null && frame2 != null)
        {
            return new[] { frame0, frame1, frame2 };
        }

        if (flapFrames != null && flapFrames.Length > 0 && flapFrames[0] != null)
        {
            return flapFrames;
        }

        return null;
    }

    private static Sprite[] LoadAltFrames()
    {
        Sprite frame0 = SpriteLibrary.Load("bird_alt_0");
        Sprite frame1 = SpriteLibrary.Load("bird_alt_1");
        Sprite frame2 = SpriteLibrary.Load("bird_alt_2");

        if (frame0 == null || frame1 == null || frame2 == null)
        {
            return null;
        }

        return new[] { frame0, frame1, frame2 };
    }

    private static Sprite[] BuildProceduralFrames()
    {
        return new[]
        {
            BuildBirdSprite(0),
            BuildBirdSprite(1),
            BuildBirdSprite(2)
        };
    }

    private static Sprite BuildBirdSprite(int frame)
    {
        const int width = 34;
        const int height = 24;
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };

        var clear = new Color32(0, 0, 0, 0);
        var body = new Color32(247, 226, 107, 255);
        var bodyDark = new Color32(224, 186, 48, 255);
        var white = new Color32(255, 255, 255, 255);
        var beak = new Color32(240, 90, 40, 255);
        var outline = new Color32(40, 28, 16, 255);
        var wing = frame == 0
            ? new Color32(247, 248, 230, 255)
            : frame == 1
                ? new Color32(244, 160, 64, 255)
                : new Color32(232, 96, 40, 255);

        var pixels = new Color32[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clear;
        }

        void Set(int x, int y, Color32 color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                pixels[y * width + x] = color;
            }
        }

        void FillCircle(int cx, int cy, int radius, Color32 color)
        {
            int r2 = radius * radius;
            for (int y = cy - radius; y <= cy + radius; y++)
            {
                for (int x = cx - radius; x <= cx + radius; x++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    if (dx * dx + dy * dy <= r2)
                    {
                        Set(x, y, color);
                    }
                }
            }
        }

        void Fill(int x, int y, int w, int h, Color32 color)
        {
            for (int yy = y; yy < y + h; yy++)
            {
                for (int xx = x; xx < x + w; xx++)
                {
                    Set(xx, yy, color);
                }
            }
        }

        FillCircle(16, 11, 8, body);
        Fill(10, 6, 14, 10, body);
        Fill(12, 5, 10, 3, bodyDark);
        FillCircle(23, 14, 4, white);
        FillCircle(24, 14, 2, outline);
        Fill(24, 9, 9, 4, beak);
        FillCircle(8, 10, 4, white);

        int wingY = frame == 0 ? 14 : frame == 1 ? 10 : 6;
        FillCircle(12, wingY, 5, wing);

        texture.SetPixels32(pixels);
        texture.Apply(false, false);

        var sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 0.5f),
            32f);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }

    private void Update()
    {
        if (!visualsReady || spriteRenderer == null || spriteRenderer.sprite == null)
        {
            RebuildVisuals();
        }

        if (GameManager.Instance == null)
        {
            return;
        }

        GameState state = GameManager.Instance.State;

        if (state == GameState.Ready)
        {
            idleTime += Time.deltaTime;
            float bob = Mathf.Sin(idleTime * Config.idleBobSpeed) * Config.idleBobAmplitude;
            transform.position = new Vector3(startPosition.x, startPosition.y + bob, startPosition.z);
            transform.rotation = Quaternion.identity;
            AnimateFlap();
            return;
        }

        if (state == GameState.Playing && FlapInput.WasPressedThisFrame())
        {
            Flap();
        }

        AnimateFlap();
        UpdateTilt();
        ClampInsideWorld();
    }

    public void SetTint(Color tint)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        tint.a = 1f;
        spriteRenderer.color = tint;
    }

    public Color GetTint()
    {
        return spriteRenderer != null ? spriteRenderer.color : Color.white;
    }

    public void ApplyScoreLook(int score)
    {
        bool useAlt = altFrames != null && GameManager.Instance != null && score >= Config.birdSwapScore;
        Sprite[] wanted = useAlt ? altFrames : mainFrames;

        if (wanted == null || wanted.Length == 0 || wanted == activeFrames)
        {
            return;
        }

        activeFrames = wanted;
        spriteRenderer.sprite = activeFrames[0];
    }

    public void StartPlaying()
    {
        dead = false;
        RebuildVisuals();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = Config.gravityScale;
        Flap();
    }

    public void ResetBird()
    {
        dead = false;
        idleTime = 0f;
        animTime = 0f;
        transform.SetPositionAndRotation(startPosition, Quaternion.identity);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        RebuildVisuals();
    }

    private static bool PlayerPrefsHasTint()
    {
        return PlayerPrefs.HasKey("FlappyBird.TintR");
    }

    private static Color ReadSavedTint()
    {
        return new Color(
            PlayerPrefs.GetFloat("FlappyBird.TintR", 1f),
            PlayerPrefs.GetFloat("FlappyBird.TintG", 1f),
            PlayerPrefs.GetFloat("FlappyBird.TintB", 1f),
            1f);
    }

    public void Die()
    {
        dead = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = Config.gravityScale * 1.15f;
    }

    private void Flap()
    {
        if (dead)
        {
            return;
        }

        rb.linearVelocity = new Vector2(0f, Config.flapVelocity);

        if (gameAudio != null)
        {
            gameAudio.PlayFlap();
        }
    }

    private void AnimateFlap()
    {
        if (activeFrames == null || activeFrames.Length == 0 || dead)
        {
            return;
        }

        animTime += Time.deltaTime * Config.flapAnimFps;
        int frame = Mathf.FloorToInt(animTime) % activeFrames.Length;
        if (activeFrames[frame] != null)
        {
            spriteRenderer.sprite = activeFrames[frame];
        }
    }

    private void UpdateTilt()
    {
        float t = Mathf.InverseLerp(Config.maxFallSpeed, Config.flapVelocity, rb.linearVelocity.y);
        float angle = Mathf.Lerp(Config.rotateDownAngle, Config.rotateUpAngle, t);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0f, 0f, angle),
            Config.rotateLerp * Time.deltaTime);
    }

    private void ClampInsideWorld()
    {
        if (transform.position.y > Config.ceilingY)
        {
            transform.position = new Vector3(transform.position.x, Config.ceilingY, transform.position.z);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ReportHit(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ReportHit(other);
    }

    private void ReportHit(Collider2D other)
    {
        if (other != null && other.CompareTag("Hazard") && GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerHit();
        }
    }
}
