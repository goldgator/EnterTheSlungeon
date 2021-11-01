using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public const string PREFAB_PATH = "Prefabs/Enemies/Prefabs/";
    public const string BOSS_PATH = "Prefabs/Enemies/Bosses/";
    public bool isBoss = false;
    public bool dropsQuartz = true;
    public string enemyName;

    private int enemyDeaths = 0;
    private int totalDeathCount = 0;
    private List<BaseEnemy> spawnedEnemies = new List<BaseEnemy>();

    public delegate void EnemyDeath();
    public event EnemyDeath onEnemyDeath;

    private static GameObject enemySpawnPrefab;
    public static GameObject EnemySpawnPrefab { 
        get
        {
            if (enemySpawnPrefab == null)
            {
                enemySpawnPrefab = Resources.Load<GameObject>("Prefabs/Misc/Spawner");
            }

            return enemySpawnPrefab;
        }
    }

    public static EnemySpawn CreateSpawner(string newEnemyName, bool willDropQuartz = true)
    {
        EnemySpawn newSpawner = Instantiate(EnemySpawnPrefab).GetComponent<EnemySpawn>();

        newSpawner.enemyName = newEnemyName;
        newSpawner.dropsQuartz = willDropQuartz;
        return newSpawner;
    }

    public void SpawnEnemy(bool hasParticles)
    {
        if (hasParticles)
        {
            SpawnEnemyWithAnim();
        } else
        {
            OnSpawnEnemy();
        }
    }

    public void SpawnEnemy(string newEnemyName, bool hasParticles)
    {
        enemyName = newEnemyName;
        SpawnEnemy(hasParticles);
    }

    private void SpawnEnemyWithAnim()
    {
        GetComponent<Animator>()?.SetTrigger("Spawn");
    }

    public void OnSpawnEnemy()
    {
        string path = (isBoss) ? BOSS_PATH : PREFAB_PATH;

        BaseEnemy spawnedEnemy;
        if (enemyName == "")
        {
            GameObject[] allEnemies = Resources.LoadAll<GameObject>(path);
            GameObject chosenEnemy = allEnemies[RNGManager.GetEventRand(0, allEnemies.Length)];
            spawnedEnemy = Instantiate(chosenEnemy, transform.position, Quaternion.identity).GetComponent<BaseEnemy>();
        }
        else
        {
            GameObject enemy = Resources.Load<GameObject>(path + enemyName);
            spawnedEnemy = Instantiate(enemy, transform.position, Quaternion.identity).GetComponent<BaseEnemy>();
        }

        spawnedEnemy.InstantiateEnemy(this);
        if (!dropsQuartz) Destroy(spawnedEnemy.GetComponent<QuartzDrops>());
    }

    public void AddTrackedEnemy(BaseEnemy newEnemy)
    {
        spawnedEnemies.Add(newEnemy);
        totalDeathCount++;
    }



    /// <summary>
    /// Called from the enemies when they die
    /// </summary>
    public void EnemyDied(BaseEnemy deadEnemy)
    {
        spawnedEnemies.Remove(deadEnemy);
        enemyDeaths++;
        onEnemyDeath?.Invoke();
    }

    public bool SpawnsBeenKilled()
    {
        return (enemyDeaths >= totalDeathCount || totalDeathCount == 0);
    }

    public void DestroySpawner(bool destroyEnemies)
    {
        if (destroyEnemies)
        {
            for (int i = 0; spawnedEnemies.Count > 0; i = 0)
            {
                spawnedEnemies[0].Death();
            }
        }

        if (gameObject != null) Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, .5f);
    }
}
