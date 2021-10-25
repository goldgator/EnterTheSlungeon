using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySplit : MonoBehaviour, IHealthDeath

{
    public List<GameObject> deathDrops;
    public float splitStrength = 5.0f;
    

    public void DeathDrop()
    {
        //Grab base enemy script if object has it
        BaseEnemy deadEnemy = GetComponent<BaseEnemy>();

        foreach(GameObject newObject in deathDrops)
        {
            GameObject spawnedObject = Instantiate(newObject);
            spawnedObject.transform.position = transform.position + new Vector3(RNGManager.GetEventRand(0f, 0.05f), RNGManager.GetEventRand(0f, 0.05f), 0);

            if (deadEnemy)
            {
                BaseEnemy spawnedEnemy = spawnedObject.GetComponent<BaseEnemy>();
                //if current object and spawned object are both enemies, share spawner
                if (spawnedEnemy) deadEnemy.ShareSpawner(spawnedEnemy);
            }

            //Choose random angle and set a force in that direction if there is a rigid body
            Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();

            if (rb)
            {
                float randAngle = RNGManager.GetEventRand(0f, 360f);
                Debug.Log(randAngle);
                Vector2 direction = Utilities.GetUnitVector2(randAngle);
                rb.velocity = direction * splitStrength;
            }
        }
    }

    public void OnDeath()
    {
        DeathDrop();
    }
}
