using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatBlock : MonoBehaviour
{
    public List<BaseStat> allStats = new List<BaseStat>();

    public BaseStat GetStat(string tag)
    {
        foreach (BaseStat stat in allStats)
        {
            if (stat.statName == tag) return stat;
        }
        return null;
    }

    public float GetStatValue(string tag)
    {
        BaseStat stat = GetStat(tag);
        
        try
        {
            return stat.Value;
        } catch(NullReferenceException nre)
        {
            Debug.LogWarning("Could not find stat '" + tag + "' in object '" + gameObject.name + "'\nDefaulting to value 0");
            Debug.LogWarning(nre.Message);
            return 0;
        }
    }

    public void AddStat(string tag, float baseValue)
    {
        BaseStat stat = GetStat(tag);
        if (stat != null)
        {
            Debug.LogWarning("Stat '" + tag + "' in object '" + gameObject.name + "' already exists.\nSetting its new base value to " + baseValue);
            stat.SetBaseValue(baseValue);
            return;
        }

        allStats.Add(new BaseStat(tag, baseValue));
    }

    public List<string> FindStatDiffs(StatBlock otherBlock)
    {
        List<string> statDiffs = new List<string>();

        //Compare all stats from the first gun
        foreach (BaseStat stat in allStats)
        {
            if (!stat.CompareStat(otherBlock))
            {
                if (!statDiffs.Contains(stat.statName)) statDiffs.Add(stat.statName);
            }
        }

        //Compare all stats from the second gun
        foreach (BaseStat stat in otherBlock.allStats)
        {
            if (!stat.CompareStat(this))
            {
                if (!statDiffs.Contains(stat.statName)) statDiffs.Add(stat.statName);
            }
        }

        return statDiffs;
    }



    

    public void AddModifier(StatModifier modifier)
    {
        BaseStat stat = GetStat(modifier.targetStat);
        stat?.AddModifier(modifier);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        BaseStat stat = GetStat(modifier.targetStat);
        stat?.RemoveModifier(modifier);
    }
}
