using UnityEngine;

// Late enemy: comes in at a high score, follows the bird, shoots fireballs.
public class Dragon : MonoBehaviour
{
    private const int FireballPoolSize = 4;

    private GameConfig config;
    private Transform bird;
    private Transform body;
    private Fireball[] fireballs;

    private bool active;
    private bool hasArt;
    private float enterTimer;
    private float shotTimer;
    private float startX;

    public bool IsActive => active;

    // Only play the dragon if its art exists, so you do not die from nothing.
    public bool CanActivate => hasArt;

    private void Awake()
    {
        BuildBody();
        BuildFireballs();
        gameObject.SetActive(false);
    }

    private void BuildBody()
    {
        Sprite sprite = SpriteLibrary.Load("dragon");
        hasArt = sprite != null;

        var visual = new GameObject("DragonBody");
        visual.transform.SetParent(transform, false);
        SpriteLibrary.CreateRenderer(visual, sprite, "Pipes", 20);
        body = visual.transform;
    }

    private void BuildFireballs()
    {
        Sprite sprite = SpriteLibrary.Load("fireball");
        if (sprite == null)
        {
            hasArt = false;
        }

        fireballs = new Fireball[FireballPoolSize];

        for (int i = 0; i < FireballPoolSize; i++)
        {
            var go = new GameObject("Fireball");
            go.tag = "Hazard";
            SpriteLibrary.CreateRenderer(go, sprite, "Pipes", 21);

            var collider = go.AddComponent<CircleCollider2D>();
            collider.radius = 0.22f;
            collider.isTrigger = true;

            fireballs[i] = go.AddComponent<Fireball>();
            go.SetActive(false);
        }
    }

    public void Activate(GameConfig gameConfig, Transform birdTransform)
    {
        config = gameConfig;
        bird = birdTransform;
        active = true;
        enterTimer = 0f;
        shotTimer = -config.dragonFirstShotDelay;
        startX = config.dragonHoverX + 4f;

        float y = bird != null ? bird.position.y : 0f;
        transform.position = new Vector3(startX, y, 0f);
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        active = false;
        HideFireballs();
        gameObject.SetActive(false);
    }

    private void HideFireballs()
    {
        if (fireballs == null)
        {
            return;
        }

        for (int i = 0; i < fireballs.Length; i++)
        {
            if (fireballs[i] != null)
            {
                fireballs[i].gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!active || config == null || GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        float dt = Time.deltaTime;
        MoveIn(dt);
        FollowBird(dt);
        Bob();

        shotTimer += dt;
        if (enterTimer >= config.dragonEnterSeconds && shotTimer >= CurrentShotInterval())
        {
            shotTimer = 0f;
            Shoot();
        }
    }

    private void MoveIn(float dt)
    {
        if (enterTimer >= config.dragonEnterSeconds)
        {
            return;
        }

        enterTimer += dt;
        float t = Mathf.Clamp01(enterTimer / config.dragonEnterSeconds);
        float x = Mathf.Lerp(startX, config.dragonHoverX, t);
        transform.position = new Vector3(x, transform.position.y, 0f);
    }

    private void FollowBird(float dt)
    {
        if (bird == null)
        {
            return;
        }

        float targetY = Mathf.Clamp(bird.position.y, -2.8f, 3.6f);
        float y = Mathf.MoveTowards(transform.position.y, targetY, config.dragonFollowSpeed * dt);
        transform.position = new Vector3(transform.position.x, y, 0f);
    }

    private void Bob()
    {
        if (body == null)
        {
            return;
        }

        float bob = Mathf.Sin(Time.time * 4f) * 0.09f;
        body.localPosition = new Vector3(0f, bob, 0f);
    }

    private float CurrentShotInterval()
    {
        int score = GameManager.Instance.Score;
        float speedUp = Mathf.Max(0, score - config.dragonFromScore) * 0.04f;
        return Mathf.Max(config.dragonMinShotInterval, config.dragonShotInterval - speedUp);
    }

    private void Shoot()
    {
        Fireball ball = GetInactiveFireball();
        if (ball == null)
        {
            return;
        }

        Vector3 muzzle = transform.position + new Vector3(-0.75f, 0f, 0f);
        ball.Launch(muzzle, config.fireballSpeed, config.pipeDespawnX);
    }

    private Fireball GetInactiveFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].gameObject.activeSelf)
            {
                return fireballs[i];
            }
        }

        return null;
    }
}
