using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuartzMiner : MonoBehaviour, IRoomObject, IInteractEvent
{
    private bool active = false;
    private bool canMine = true;

    private Vector3 startPos;
    private Vector3 targetPos;

    [Header("Stats")]
    [SerializeField]
    private float eventBaseTime = 90f;
    [SerializeField]
    private float spawnTime = 3.0f;
    [SerializeField]
    private int maxSpawns = 4;
    [SerializeField]
    private float minSpawnRadius;
    [SerializeField]
    private float maxspawnRadius;
    [SerializeField]
    private List<string> enemyPool = new List<string>();

    private float timer = 0f;
    private float enemyDefeatedTimer = 0f;

    [Header("Components")]
    [SerializeField]
    private Transform revealParent;
    [SerializeField]
    private ParticleSystem particles;
    [SerializeField]
    private SpriteRenderer quartzRenderer;
    [SerializeField]
    private Collider2D quartzCollider;
    private Interactable interactable;
    private ResourceData currentResource;
    private List<EnemySpawn> spawners = new List<EnemySpawn>();

    private RoomContent currentContent;


    private void Start()
    {
        interactable = GetComponent<Interactable>();
        particles.Stop();
    }

    private void Update()
    {
        currentResource = Floor.Instance.GetCurrentResource();

        interactable.SetInteractable(currentResource != null && (!active && canMine));
    }

    public bool ContentPassed()
    {
        return !active;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minSpawnRadius);
        Gizmos.DrawWireSphere(transform.position, maxspawnRadius);
    }

    public void SubscribeToRoomContent(RoomContent roomContent)
    {
        currentContent = roomContent;
    }

    public void StartMineEvent()
    {
        active = true;
        canMine = false;
        startPos = revealParent.localPosition;
        targetPos = startPos + new Vector3(0, 1.0f, 0);

        quartzRenderer.sprite = Quartz.GetQuartzSprite(currentResource.resourceType, true);
        particles.startColor = Quartz.GetQuartzColor(currentResource.resourceType);
        particles.Play();

        //Assign type to QuartzChunk
        quartzRenderer.GetComponent<QuartzChunk>().SetType(currentResource.resourceType);

        //Change music
        MusicManager.Instance.PlaySong("Boss2");

        timer = 0f;
        currentContent.ActivateRoom();

        //StartCoroutines
        StartCoroutine(MineSpawnEvent());
        //StartCoroutine(MineSpawnEvent());
    }

    public void StopMineEvent()
    {
        active = false;
        //revealParent.gameObject.SetActive(false);
        particles.emissionRate = 0;

        //Remove all spawners
        foreach (EnemySpawn spawn in spawners)
        {
            spawn.DestroySpawner(true);
        }

        //Enable quartz collider
        quartzCollider.enabled = true;

        //Change music
        MusicManager.Instance.PlayFloorSong();

        //Clear floor resource data
        Floor.Instance.GetFloorData().resourceData.Clear();

        //Change room type back to generic (updates map)
        Floor.Instance.CurrentPlayerCell().GetData().roomOwner.roomType = RoomData.RoomType.Generic;
    }


    public IEnumerator MineSpawnEvent()
    {
        float spawnTimer = spawnTime;
        while (timer < eventBaseTime)
        {
            //Update timer, based on if a enemy was defeated recently or not (updates twice as fast if so)
            if (enemyDefeatedTimer > 0)
            {
                timer += Time.deltaTime * 3;
                particles.emissionRate = 120;
            } else
            {
                timer += Time.deltaTime;
                particles.emissionRate = 20;
            }

            //Count enemyDefeatedTimer down
            enemyDefeatedTimer -= Time.deltaTime;

            //update revealParent pos
            revealParent.transform.localPosition = Vector3.Lerp(startPos, targetPos, timer / eventBaseTime);

            //update spawnTimer
            spawnTimer += Time.deltaTime;

            //See if an enemy needs to be spawned
            if (spawnTimer >= spawnTime && ShouldSpawnEnemy())
            {
                SpawnEnemy();
                spawnTimer = 0;
            }

            yield return null;
        }

        StopMineEvent();
    }

    private void EnemyDied()
    {
        enemyDefeatedTimer = 4.0f;
    }

    private void SpawnEnemy()
    {
        //Check if there are enough spawners
        if (spawners.Count < maxSpawns)
        {
            //Add Spawner
            EnemySpawn newSpawner = AddSpawner();
            //Have spawner create an enemy
            newSpawner.SpawnEnemy(true);
            //return
            return;
        }

        //Grab first spawner with no enemies
        foreach(EnemySpawn spawn in spawners)
        {
            if (spawn.SpawnsBeenKilled())
            {
                //Set position
                spawn.transform.localPosition = GetSpawnLocation();
                //Get new enemy
                string randomEnemy = enemyPool[RNGManager.GetEventRand(0, enemyPool.Count)];
                //Spawn new enemy
                spawn.SpawnEnemy(randomEnemy, true);
                //return
                return;
            }
        } 
    }

    private EnemySpawn AddSpawner()
    {
        //Grab a random enemy
        string randomEnemy = enemyPool[RNGManager.GetEventRand(0, enemyPool.Count)];
        //Create spawner from random selected enemy
        EnemySpawn newSpawner = EnemySpawn.CreateSpawner(randomEnemy,false);
        //Make child of this object
        newSpawner.transform.SetParent(transform, false);
        //Set position
        newSpawner.transform.localPosition = GetSpawnLocation();
        //Add event to spawner
        newSpawner.onEnemyDeath += EnemyDied;
        //Rename spawner to find in hierarchy
        newSpawner.gameObject.name = "EventSpawner";
        //Turn off quartz drops
        newSpawner.dropsQuartz = false;

        spawners.Add(newSpawner);

        return newSpawner;
    }

    /// <summary>
    /// returns LOCAL position from miner
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSpawnLocation()
    {
        //Get random direction
        Vector3 randDir = Utilities.GetUnitVector2(RNGManager.GetEventRand(0f, 360f));

        //Get random distance
        float distance = RNGManager.GetEventRand(minSpawnRadius, maxspawnRadius);

        randDir *= distance;

        //Debug.Log("Spawn Location: " + randDir);

        return randDir;
    }

    private bool ShouldSpawnEnemy()
    {
        //Check if there are enough spawners
        if (spawners.Count < maxSpawns) return true;

        //Check if any spawners are free
        foreach(EnemySpawn spawn in spawners)
        {
            if (spawn.SpawnsBeenKilled()) return true;
        }

        return false;
    }

    public void OnInteract()
    {
        StartMineEvent();
    }

    public void OnInteractEnter()
    {
        //Not needed
    }

    public void OnInteractLeave()
    {
        //Not needed
    }
}
