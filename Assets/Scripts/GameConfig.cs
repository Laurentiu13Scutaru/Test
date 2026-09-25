using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Flappy Bird/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Bird")]
    public float flapVelocity = 5.6f;
    public float maxFallSpeed = -10f;
    public float gravityScale = 2.7f;
    public float idleBobAmplitude = 0.18f;
    public float idleBobSpeed = 3.4f;
    public float rotateUpAngle = 28f;
    public float rotateDownAngle = -90f;
    public float rotateLerp = 8f;
    public float flapAnimFps = 10f;

    [Header("World")]
    public float scrollSpeed = 2.55f;
    public float groundY = -4.7f;
    public float ceilingY = 4.85f;

    [Header("Hard mode")]
    [Tooltip("From this score the world scrolls faster.")]
    public int hardModeFromScore = 15;
    [Tooltip("Scroll speed multiplier after hard mode starts (1.5 = 50% faster).")]
    public float hardModeScrollMultiplier = 1.5f;

    [Header("Pipes")]
    public float pipeSpawnInterval = 1.5f;
    public float pipeSpawnX = 7.5f;
    public float pipeDespawnX = -8.5f;
    public float pipeGap = 2.45f;
    public float pipeMinCenterY = -1.15f;
    public float pipeMaxCenterY = 2.05f;

    [Header("Flow")]
    public float gameOverUiDelay = 0.85f;

    [Header("Rewards")]
    [Tooltip("Play the celebration sound every N points.")]
    public int comboSoundEvery = 5;
    [Tooltip("Fireworks start showing from this score.")]
    public int fireworksFromScore = 10;
    [Tooltip("The bird changes appearance from this score.")]
    public int birdSwapScore = 20;

    [Header("Dragon")]
    [Tooltip("The dragon only joins the game from this score.")]
    public int dragonFromScore = 15;
    public float dragonEnterSeconds = 1.6f;
    public float dragonHoverX = 6.1f;
    public float dragonFollowSpeed = 1.5f;
    public float dragonFirstShotDelay = 2.5f;
    public float dragonShotInterval = 2.6f;
    public float dragonMinShotInterval = 1.5f;
    public float fireballSpeed = 4.4f;
}
