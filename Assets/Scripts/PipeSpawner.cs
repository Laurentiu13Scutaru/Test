using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private PipePair pipePrefab;
    [SerializeField] private int poolSize = 6;

    private PipePair[] pool;
    private float spawnTimer;
    private bool spawning;

    private void Awake()
    {
        if (pipePrefab == null)
        {
            pipePrefab = FindFirstObjectByType<PipePair>();
        }

        if (pipePrefab == null)
        {
            pipePrefab = new GameObject("PipePair").AddComponent<PipePair>();
            pipePrefab.gameObject.SetActive(false);
        }

        pool = new PipePair[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(pipePrefab, transform);
            pool[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        GameManager manager = GameManager.Instance;
        if (!spawning || manager == null || manager.State != GameState.Playing)
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= manager.Config.pipeSpawnInterval)
        {
            spawnTimer = 0f;
            Spawn();
        }
    }

    public void StartSpawning()
    {
        spawning = true;
        spawnTimer = GameManager.Instance != null
            ? GameManager.Instance.Config.pipeSpawnInterval
            : 0f;
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    public void ResetPipes()
    {
        spawning = false;
        spawnTimer = 0f;

        if (pool == null)
        {
            return;
        }

        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i] != null)
            {
                pool[i].gameObject.SetActive(false);
            }
        }
    }

    private void Spawn()
    {
        PipePair pipe = GetInactive();
        if (pipe == null)
        {
            return;
        }

        GameConfig config = GameManager.Instance.Config;
        float centerY = Random.Range(config.pipeMinCenterY, config.pipeMaxCenterY);
        pipe.Place(config.pipeSpawnX, centerY);
    }

    private PipePair GetInactive()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i] != null && !pool[i].gameObject.activeSelf)
            {
                return pool[i];
            }
        }

        return null;
    }
}
