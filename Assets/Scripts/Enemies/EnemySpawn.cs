using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public const string PREFAB_PATH = "Prefabs/Enemies/Prefabs";
    public string enemyName;


    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (enemyName == "")
        {
            
            GameObject[] allEnemies = Resources.LoadAll<GameObject>(PREFAB_PATH);
            GameObject chosenEnemy = allEnemies[Random.Range(0, allEnemies.Length)];
            Debug.Log(chosenEnemy.name);
            Instantiate(chosenEnemy, transform.position, Quaternion.identity);
        }
        else
        {
            GameObject enemy = Resources.Load<GameObject>(PREFAB_PATH + enemyName);
            Instantiate(enemy, transform.position, Quaternion.identity);
        }
    }

    
}
