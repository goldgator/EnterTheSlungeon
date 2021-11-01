using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuartzChunk : MonoBehaviour, IHealthDeath
{
    [SerializeField]
    private int quartzMin;
    [SerializeField]
    private int quartzMax;

    private int quartzYield;

    private ResourceType quartzType;


    public void SetType(ResourceType newType)
    {
        quartzType = newType;
    }

    // Start is called before the first frame update
    void Start()
    {
        quartzYield = RNGManager.GetEventRand(quartzMin, quartzMax);
    }


    public void OnDeath()
    {
        for (int i = 0; i < quartzYield; i++)
        {
            SpawnQuartz();
        }

        Destroy(gameObject);
    }

    public void Fragment()
    {
        if (quartzYield > 0)
        {
            quartzYield--;
            SpawnQuartz();


            if (quartzYield == 0) OnDeath();
        }
    }

    private void SpawnQuartz()
    {
        Quartz newQuartz = Instantiate(Quartz.QuartzPrefab, transform.position, Quaternion.identity).GetComponent<Quartz>();
        newQuartz.SetType(quartzType);
    }

    
}
