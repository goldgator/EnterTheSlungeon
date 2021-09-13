using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public const string PREFAB_PATH = "Prefabs/Enemies/Prefabs/";
    public const string BOSS_PATH = "Prefabs/Enemies/Bosses/";
    public bool isBoss = false;
    public string enemyName;

    private int enemyDeathCount = 0;

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

    private void SpawnEnemyWithAnim()
    {
        GetComponent<Animator>()?.SetTrigger("Spawn");
    }

    public void OnSpawnEnemy()
    {
        string path = (isBoss) ? BOSS_PATH : PREFAB_PATH;

        if (enemyName == "")
        {
            GameObject[] allEnemies = Resources.LoadAll<GameObject>(path);
            GameObject chosenEnemy = allEnemies[Random.Range(0, allEnemies.Length)];
            Instantiate(chosenEnemy, transform.position, Quaternion.identity).GetComponent<BaseEnemy>().InstantiateEnemy(this);
        }
        else
        {
            GameObject enemy = Resources.Load<GameObject>(path + enemyName);
            Instantiate(enemy, transform.position, Quaternion.identity).GetComponent<BaseEnemy>().InstantiateEnemy(this);
        }
    }

    /// <summary>
    /// Called from the enemies when they die
    /// </summary>
    public void EnemyDied()
    {
        enemyDeathCount++;
    }

    public bool SpawnHasBeenKilled()
    {
        return (enemyDeathCount > 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, .5f);
    }
}
