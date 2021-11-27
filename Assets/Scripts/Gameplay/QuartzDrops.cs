using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuartzDrops : MonoBehaviour, IHealthDeath
{

    [SerializeField]
    private int quartzMin;
    [SerializeField]
    private int quartzMax;
    [SerializeField]
    private bool uniformDrop;

    private void Start()
    {
        //Double min and max if run is a quick run
        if (Floor.quickRun)
        {
            quartzMin *= 2;
            quartzMax *= 2;
        }
    }

    private void DropQuartz()
    {
        //Roll a random num between quartz min and max
        int quartzNum = RNGManager.GetEventRand(quartzMin, quartzMax + 1);

        //Decide resource Type
        ResourceType newType = (ResourceType) RNGManager.GetEventRand(0, 3);

        for (int i = 0; i < quartzNum; i++)
        {
            //Instantiate a quartz
            Quartz newQuartz = Instantiate(Quartz.QuartzPrefab, transform.position, Quaternion.identity).GetComponent<Quartz>();
            newQuartz.SetType(newType);

            //if not uniform drop, roll a new resource type
            if (!uniformDrop) newType = (ResourceType)RNGManager.GetEventRand(0, 3);
        }
    }

    public void OnDeath()
    {
        DropQuartz();
    }
}
