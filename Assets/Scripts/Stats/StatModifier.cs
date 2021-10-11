using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatModifier
{
    public enum eModType
    {
        Flat,
        PercentAdd,
        PercentMult
    }

    public string targetStat;
    [SerializeField]
    public float modValue;
    [SerializeField]
    public eModType modType;
    [SerializeField]
    public int order = 0;


}
