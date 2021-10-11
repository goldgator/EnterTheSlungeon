using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour, IRoomObject
{
    private int currentWave = 0;
    private float currentTime = 0;
    public List<EnemySpawnWave> waves = new List<EnemySpawnWave>();
    public bool canSpawnEarly = true;

    bool wavesCompleted = false;

    [System.Serializable]
    public struct EnemySpawnWave
    {
        public bool hasParticles;
        public float timeToSpawn;
        public List<EnemySpawn> spawners;
    }

    public bool ContentPassed()
    {
        return wavesCompleted;
    }

    public void SubscribeToRoomContent(RoomContent content)
    {
        content.roomActivateEvent += BeginWaves;
    }

    private void BeginWaves()
    {
        currentTime = waves[currentWave].timeToSpawn;
        StartCoroutine(ManageWave());
    }

    private void StartNextWave()
    {

        foreach (EnemySpawn spawn in waves[currentWave].spawners)
        {
            spawn.SpawnEnemy(waves[currentWave].hasParticles);
        }

        //Increment currentWave
        currentWave++;
        //Set new currentTime if available
        if (currentWave < waves.Count) currentTime = waves[currentWave].timeToSpawn;
    }

    private IEnumerator ManageWave()
    {
        while(currentWave < waves.Count)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0 || WaveCompleted(currentWave - 1))
            {
                StartNextWave();
            }

            if (currentWave < waves.Count) yield return null;
        }

        while (!AllWavesCompleted()) yield return null;

        //If outside of both while loops, all waves have been completed
        Debug.Log("Waves Completed");
        wavesCompleted = true;
    }

    private bool WaveCompleted(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= waves.Count) return false;

        //Check if each enemy from spawners was killed
        foreach (EnemySpawn spawn in waves[waveIndex].spawners)
        {
            if (!spawn.SpawnHasBeenKilled()) return false;
        }

        //If passes through foreach loop, all spawns have had their enemy die
        Debug.Log("Wave " + waveIndex + " completed");
        return true;
    }

    private bool AllWavesCompleted()
    {
        foreach (EnemySpawnWave spawnWave in waves)
        {
            foreach (EnemySpawn spawn in spawnWave.spawners)
            {
                if (!spawn.SpawnHasBeenKilled()) return false;
            }
        }
        return true;
    }
}
