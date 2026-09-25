using UnityEngine;

// Sounds and effects: combo sound, fireworks, feathers, and hard-mode rain.
public class GameEffects : MonoBehaviour
{
    private const int SparkCount = 24;
    private const int FeatherCount = 12;
    private const int RainDropCount = 40;
    private const float RainSpawnInterval = 0.045f;

    private AudioSource source;
    private AudioClip comboClip;
    private AudioClip fireworkClip;
    private Spark[] sparks;
    private Feather[] feathers;
    private RainDrop[] rainDrops;
    private Sprite sparkSprite;
    private Sprite featherSprite;
    private Sprite rainSprite;
    private bool raining;
    private float rainSpawnTimer;

    private struct Spark
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public Vector2 velocity;
        public float life;
        public float maxLife;
    }

    private struct Feather
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public Vector2 velocity;
        public float spin;
        public float flutter;
        public float life;
        public float maxLife;
    }

    private struct RainDrop
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public Vector2 velocity;
        public float life;
        public float maxLife;
    }

    private void Awake()
    {
        EnsureReady();
    }

    private void OnEnable()
    {
        EnsureReady();
    }

    private void EnsureReady()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
            if (source == null)
            {
                source = gameObject.AddComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.spatialBlend = 0f;
        }

        if (comboClip == null)
        {
            comboClip = SpriteLibrary.LoadClip("wow");
        }

        if (fireworkClip == null)
        {
            fireworkClip = SpriteLibrary.LoadClip("firework");
        }

        if (sparkSprite == null)
        {
            sparkSprite = SpriteLibrary.Load("spark");
            if (sparkSprite == null)
            {
                sparkSprite = MakeFallbackSpark();
            }
        }

        if (!PoolIsValid())
        {
            CreateSparkPool();
        }

        if (featherSprite == null)
        {
            featherSprite = MakeFeatherSprite();
        }

        if (!FeatherPoolIsValid())
        {
            CreateFeatherPool();
        }

        if (rainSprite == null)
        {
            rainSprite = MakeRainDropSprite();
        }

        if (!RainPoolIsValid())
        {
            CreateRainPool();
        }
    }

    private bool PoolIsValid()
    {
        if (sparks == null || sparks.Length != SparkCount)
        {
            return false;
        }

        for (int i = 0; i < sparks.Length; i++)
        {
            if (sparks[i].transform == null || sparks[i].renderer == null)
            {
                return false;
            }
        }

        return true;
    }

    private bool FeatherPoolIsValid()
    {
        if (feathers == null || feathers.Length != FeatherCount)
        {
            return false;
        }

        for (int i = 0; i < feathers.Length; i++)
        {
            if (feathers[i].transform == null || feathers[i].renderer == null)
            {
                return false;
            }
        }

        return true;
    }

    private bool RainPoolIsValid()
    {
        if (rainDrops == null || rainDrops.Length != RainDropCount)
        {
            return false;
        }

        for (int i = 0; i < rainDrops.Length; i++)
        {
            if (rainDrops[i].transform == null || rainDrops[i].renderer == null)
            {
                return false;
            }
        }

        return true;
    }

    private void CreateSparkPool()
    {
        sparks = new Spark[SparkCount];
        for (int i = 0; i < SparkCount; i++)
        {
            var go = new GameObject("Spark");
            go.transform.SetParent(transform, false);
            SpriteRenderer renderer = SpriteLibrary.CreateRenderer(go, sparkSprite, "Bird", 50);
            go.SetActive(false);

            sparks[i] = new Spark
            {
                transform = go.transform,
                renderer = renderer,
                velocity = Vector2.zero,
                life = 0f,
                maxLife = 1f
            };
        }
    }

    private void CreateFeatherPool()
    {
        feathers = new Feather[FeatherCount];
        for (int i = 0; i < FeatherCount; i++)
        {
            var go = new GameObject("Feather");
            go.transform.SetParent(transform, false);
            SpriteRenderer renderer = SpriteLibrary.CreateRenderer(go, featherSprite, "Bird", 55);
            go.SetActive(false);

            feathers[i] = new Feather
            {
                transform = go.transform,
                renderer = renderer,
                velocity = Vector2.zero,
                spin = 0f,
                flutter = 0f,
                life = 0f,
                maxLife = 1f
            };
        }
    }

    private void CreateRainPool()
    {
        rainDrops = new RainDrop[RainDropCount];
        for (int i = 0; i < RainDropCount; i++)
        {
            var go = new GameObject("RainDrop");
            go.transform.SetParent(transform, false);
            SpriteRenderer renderer = SpriteLibrary.CreateRenderer(go, rainSprite, "Bird", 8);
            go.SetActive(false);

            rainDrops[i] = new RainDrop
            {
                transform = go.transform,
                renderer = renderer,
                velocity = Vector2.zero,
                life = 0f,
                maxLife = 1f
            };
        }
    }

    // Turn rain on or off. Looks only, does not hurt the bird.
    public void SetRaining(bool enabled)
    {
        EnsureReady();
        raining = enabled;
        if (!enabled)
        {
            rainSpawnTimer = 0f;
            HideRain();
        }
    }

    public void PlayCombo()
    {
        EnsureReady();
        if (comboClip != null && source != null)
        {
            source.PlayOneShot(comboClip);
        }
    }

    public void PlayFireworks(Vector3 center)
    {
        EnsureReady();
        if (fireworkClip != null && source != null)
        {
            source.PlayOneShot(fireworkClip, 0.7f);
        }

        Burst(center + new Vector3(-1.8f, 1.9f, 0f), new Color(1f, 0.85f, 0.3f));
        Burst(center + new Vector3(1.9f, 2.4f, 0f), new Color(1f, 0.45f, 0.55f));
        Burst(center + new Vector3(0.2f, 3.1f, 0f), new Color(0.5f, 0.9f, 1f));
    }

    public void BurstFeathers(Vector3 origin, Color tint)
    {
        EnsureReady();
        if (feathers == null)
        {
            return;
        }

        for (int i = 0; i < feathers.Length; i++)
        {
            if (feathers[i].transform == null || feathers[i].renderer == null)
            {
                continue;
            }

            float angle = Random.Range(20f, 160f) * Mathf.Deg2Rad;
            float speed = Random.Range(2.2f, 4.8f);
            float shade = Random.Range(0.82f, 1.08f);
            Color color = new Color(
                Mathf.Clamp01(tint.r * shade),
                Mathf.Clamp01(tint.g * shade),
                Mathf.Clamp01(tint.b * shade),
                1f);

            feathers[i].transform.position = origin + (Vector3)Random.insideUnitCircle * 0.12f;
            feathers[i].transform.localScale = Vector3.one * Random.Range(0.7f, 1.15f);
            feathers[i].transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-40f, 40f));
            feathers[i].velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
            feathers[i].spin = Random.Range(-380f, 380f);
            feathers[i].flutter = Random.Range(0f, Mathf.PI * 2f);
            feathers[i].maxLife = Random.Range(0.7f, 1.15f);
            feathers[i].life = feathers[i].maxLife;
            feathers[i].renderer.color = color;
            feathers[i].transform.gameObject.SetActive(true);
        }
    }

    private void Burst(Vector3 center, Color color)
    {
        if (sparks == null)
        {
            return;
        }

        int spawned = 0;
        for (int i = 0; i < sparks.Length && spawned < 8; i++)
        {
            if (sparks[i].transform == null || sparks[i].renderer == null || sparks[i].transform.gameObject.activeSelf)
            {
                continue;
            }

            float angle = Random.Range(0f, Mathf.PI * 2f);
            float speed = Random.Range(1.6f, 3.4f);

            sparks[i].transform.position = center;
            sparks[i].transform.localScale = Vector3.one * Random.Range(0.7f, 1.2f);
            sparks[i].velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
            sparks[i].maxLife = Random.Range(0.5f, 0.9f);
            sparks[i].life = sparks[i].maxLife;
            sparks[i].renderer.color = color;
            sparks[i].transform.gameObject.SetActive(true);
            spawned++;
        }
    }

    private void Update()
    {
        if (sparks == null)
        {
            EnsureReady();
            if (sparks == null)
            {
                return;
            }
        }

        float dt = Time.deltaTime;
        for (int i = 0; i < sparks.Length; i++)
        {
            Transform sparkTransform = sparks[i].transform;
            SpriteRenderer sparkRenderer = sparks[i].renderer;
            if (sparkTransform == null || sparkRenderer == null || !sparkTransform.gameObject.activeSelf)
            {
                continue;
            }

            sparks[i].life -= dt;
            if (sparks[i].life <= 0f)
            {
                sparkTransform.gameObject.SetActive(false);
                continue;
            }

            sparks[i].velocity += Vector2.down * 3.2f * dt;
            sparkTransform.Translate(sparks[i].velocity * dt, Space.World);

            Color color = sparkRenderer.color;
            color.a = Mathf.Clamp01(sparks[i].life / sparks[i].maxLife);
            sparkRenderer.color = color;
        }

        UpdateFeathers(dt);
        UpdateRain(dt);
    }

    public void StopAll()
    {
        raining = false;
        rainSpawnTimer = 0f;

        if (sparks != null)
        {
            for (int i = 0; i < sparks.Length; i++)
            {
                if (sparks[i].transform != null)
                {
                    sparks[i].transform.gameObject.SetActive(false);
                }
            }
        }

        if (feathers != null)
        {
            for (int i = 0; i < feathers.Length; i++)
            {
                if (feathers[i].transform != null)
                {
                    feathers[i].transform.gameObject.SetActive(false);
                }
            }
        }

        HideRain();
    }

    private void UpdateFeathers(float dt)
    {
        if (feathers == null)
        {
            return;
        }

        for (int i = 0; i < feathers.Length; i++)
        {
            Transform featherTransform = feathers[i].transform;
            SpriteRenderer featherRenderer = feathers[i].renderer;
            if (featherTransform == null || featherRenderer == null || !featherTransform.gameObject.activeSelf)
            {
                continue;
            }

            feathers[i].life -= dt;
            if (feathers[i].life <= 0f)
            {
                featherTransform.gameObject.SetActive(false);
                continue;
            }

            feathers[i].velocity += Vector2.down * 5.4f * dt;
            feathers[i].velocity.x += Mathf.Sin((feathers[i].maxLife - feathers[i].life) * 14f + feathers[i].flutter) * 2.6f * dt;
            featherTransform.Translate(feathers[i].velocity * dt, Space.World);
            featherTransform.Rotate(0f, 0f, feathers[i].spin * dt);

            Color color = featherRenderer.color;
            color.a = Mathf.Clamp01(feathers[i].life / feathers[i].maxLife);
            featherRenderer.color = color;
        }
    }

    private void UpdateRain(float dt)
    {
        if (rainDrops == null)
        {
            return;
        }

        if (raining)
        {
            rainSpawnTimer -= dt;
            while (rainSpawnTimer <= 0f)
            {
                rainSpawnTimer += RainSpawnInterval;
                SpawnRainDrop();
            }
        }

        for (int i = 0; i < rainDrops.Length; i++)
        {
            Transform dropTransform = rainDrops[i].transform;
            SpriteRenderer dropRenderer = rainDrops[i].renderer;
            if (dropTransform == null || dropRenderer == null || !dropTransform.gameObject.activeSelf)
            {
                continue;
            }

            rainDrops[i].life -= dt;
            if (rainDrops[i].life <= 0f || dropTransform.position.y < -5.2f)
            {
                dropTransform.gameObject.SetActive(false);
                continue;
            }

            dropTransform.Translate(rainDrops[i].velocity * dt, Space.World);

            Color color = dropRenderer.color;
            float fade = Mathf.Clamp01(rainDrops[i].life / Mathf.Max(0.15f, rainDrops[i].maxLife));
            color.a = 0.35f + fade * 0.45f;
            dropRenderer.color = color;
        }
    }

    private void SpawnRainDrop()
    {
        if (rainDrops == null)
        {
            return;
        }

        for (int i = 0; i < rainDrops.Length; i++)
        {
            if (rainDrops[i].transform == null || rainDrops[i].renderer == null || rainDrops[i].transform.gameObject.activeSelf)
            {
                continue;
            }

            float x = Random.Range(-5.4f, 5.4f);
            float y = Random.Range(4.9f, 6.2f);
            float fall = Random.Range(7.5f, 11.5f);
            float drift = Random.Range(-1.8f, -0.4f);

            rainDrops[i].transform.position = new Vector3(x, y, 0f);
            rainDrops[i].transform.localScale = new Vector3(
                Random.Range(0.55f, 0.9f),
                Random.Range(1.1f, 1.7f),
                1f);
            rainDrops[i].transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-18f, -8f));
            rainDrops[i].velocity = new Vector2(drift, -fall);
            rainDrops[i].maxLife = Random.Range(0.85f, 1.35f);
            rainDrops[i].life = rainDrops[i].maxLife;

            float shade = Random.Range(0.85f, 1.05f);
            rainDrops[i].renderer.color = new Color(0.55f * shade, 0.78f * shade, 1f, 0.7f);
            rainDrops[i].transform.gameObject.SetActive(true);
            return;
        }
    }

    private void HideRain()
    {
        if (rainDrops == null)
        {
            return;
        }

        for (int i = 0; i < rainDrops.Length; i++)
        {
            if (rainDrops[i].transform != null)
            {
                rainDrops[i].transform.gameObject.SetActive(false);
            }
        }
    }

    private static Sprite MakeRainDropSprite()
    {
        const int width = 4;
        const int height = 10;
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Point
        };

        Color32 clear = new Color32(0, 0, 0, 0);
        Color32 tip = new Color32(210, 235, 255, 255);
        Color32 body = new Color32(160, 210, 255, 230);
        Color32[] pixels = new Color32[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clear;
        }

        // Simple teardrop silhouette.
        int[] rowStarts = { 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 };
        int[] rowEnds = { 2, 2, 2, 3, 3, 3, 2, 2, 2, 2 };
        for (int y = 0; y < height; y++)
        {
            for (int x = rowStarts[y]; x <= rowEnds[y]; x++)
            {
                pixels[y * width + x] = y >= height - 2 ? tip : body;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply(false, false);

        var sprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 32f);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }

    private static Sprite MakeFeatherSprite()
    {
        const int width = 8;
        const int height = 12;
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Point
        };

        Color32 clear = new Color32(0, 0, 0, 0);
        Color32 white = new Color32(255, 255, 255, 255);
        Color32[] pixels = new Color32[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clear;
        }

        int[] marks =
        {
            3, 4,
            2, 3, 4, 5,
            1, 2, 3, 4, 5, 6,
            1, 2, 3, 4, 5, 6,
            1, 2, 3, 4, 5, 6,
            2, 3, 4, 5,
            2, 3, 4, 5,
            3, 4,
            3, 4,
            3,
            3,
            3
        };

        int[] rowCounts = { 2, 4, 6, 6, 6, 4, 4, 2, 2, 1, 1, 1 };
        int index = 0;
        for (int y = height - 1; y >= 0; y--)
        {
            int count = rowCounts[height - 1 - y];
            for (int n = 0; n < count; n++)
            {
                pixels[y * width + marks[index]] = white;
                index++;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply(false, false);

        var sprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.2f), 32f);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }

    private static Sprite MakeFallbackSpark()
    {
        const int size = 8;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Point
        };

        Color32 clear = new Color32(0, 0, 0, 0);
        Color32 core = new Color32(255, 240, 180, 255);
        Color32[] pixels = new Color32[size * size];
        int mid = size / 2;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool on = x == mid || y == mid || x == y || x + y == size - 1;
                pixels[y * size + x] = on ? core : clear;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply(false, false);

        var sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 32f);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }
}
