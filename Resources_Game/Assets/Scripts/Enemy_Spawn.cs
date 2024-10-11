using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyType1;
    [SerializeField] private GameObject enemyType2;
    [SerializeField] private GameObject enemyType3;

    private float levelDuration = 0f;
    private float maxLevelTime = 120f;

    private float startSpawnTime = 5.0f;
    private float minSpawnTime = 1.5f;
    private float currentSpawnTime;

    private Coroutine spawnCoroutine;

    private int currentLevel = 1;

    void Start()
    {
        StartLevel(currentLevel);
    }

    void Update()
    {
        levelDuration += Time.deltaTime;
        AdjustSpawnTime();
    }

    public void StartLevel(int level)
    {
        currentLevel = level;
        levelDuration = 0f;

        if (currentLevel <= 3)
        {
            startSpawnTime = 5.0f;
            minSpawnTime = 1.5f;
        }
        else
        {
            startSpawnTime = 4.0f;
            minSpawnTime = 1.0f;
        }

        currentSpawnTime = startSpawnTime;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    private void AdjustSpawnTime()
    {
        float progressionFactor = Mathf.Clamp01(levelDuration / maxLevelTime);
        currentSpawnTime = Mathf.Lerp(startSpawnTime, minSpawnTime, progressionFactor);
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(currentSpawnTime);
        }
    }

    private void SpawnEnemy()
    {
        List<GameObject> enemiesToSpawn = new List<GameObject>();

        if (currentLevel >= 1) enemiesToSpawn.Add(enemyType1);
        if (currentLevel >= 2) enemiesToSpawn.Add(enemyType2);
        if (currentLevel >= 3) enemiesToSpawn.Add(enemyType3);

        foreach (var enemyPrefab in enemiesToSpawn)
        {
            float randomNum = Random.Range(-.5f, .5f); //change the y-values slightly to add some visual variation
            Vector3 randomPos = new Vector3(
                transform.position.x - 3,
                transform.position.y + randomNum,
                0
            );

            GameObject enemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        }
    }

    public void NextLevel()
    {
        StartLevel(currentLevel + 1);
    }
}
