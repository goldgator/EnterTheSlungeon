using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BaseStat
{
    public string statName;
    [SerializeField]
    private float baseValue;
    [NonSerialized]
    public List<StatModifier> modifiers = new List<StatModifier>();

    private float finalValue;
    private bool updated = false;
    public float Value { get
        {
            if (!updated)
            {
                finalValue = CalculateValue();
                updated = true;
            }

            return finalValue;
        } 
    }

    public BaseStat(string newName, float newValue)
    {
        statName = newName;
        baseValue = newValue;
        finalValue = baseValue;
    }

    public void SetBaseValue(float newValue)
    {
        baseValue = newValue;
    }
    
    public void AddModifier(StatModifier newModifier)
    {
        if (modifiers == null) modifiers = new List<StatModifier>();
        updated = false;
        modifiers.Add(newModifier);
    }

    public void RemoveModifier(StatModifier statModifier)
    {
        if (modifiers == null) modifiers = new List<StatModifier>();
        updated = true;
        modifiers.Remove(statModifier);
    }

    private float CalculateValue()
    {
        if (modifiers == null) modifiers = new List<StatModifier>();
        if (modifiers.Count == 0) return baseValue;

        modifiers.OrderBy(ctx => ctx.order);
        float finalValue = baseValue;
        float percentAddMods = 0;

        for (int i = 0; i < modifiers.Count; i++)
        {
            switch (modifiers[i].modType)
            {
                case StatModifier.eModType.Flat:
                    finalValue += modifiers[i].modValue;
                    break;
                case StatModifier.eModType.PercentAdd:
                    //Add mod to sum
                    percentAddMods = modifiers[i].modValue;

                    //if at end of list or next modifier isn't percent add
                    if (i+1 >= modifiers.Count || modifiers[i+1].modType != StatModifier.eModType.PercentAdd)
                    {
                        finalValue *= 1 + percentAddMods;
                        percentAddMods = 0;
                    }

                    break;
                case StatModifier.eModType.PercentMult:
                    finalValue *= modifiers[i].modValue;
                    break;
                default:
                    break;
            }
        }


        return finalValue;
    }

}
