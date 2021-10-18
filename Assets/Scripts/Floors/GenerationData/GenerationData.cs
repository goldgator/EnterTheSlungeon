using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GenerationData
{
    [SerializeField]
    public Dictionary<string, int> roomSpawnCounts = new Dictionary<string, int>();


    public void AddCount(string key)
    {
        bool success = roomSpawnCounts.TryGetValue(key, out int value);

        if (success)
        {
            roomSpawnCounts[key] = value + 1;
        } else
        {
            roomSpawnCounts.Add(key, 1);
        }
    }
}

