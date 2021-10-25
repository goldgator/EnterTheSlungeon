using System;
using System.Collections;
using System.Collections.Generic;


public static class RNGManager
{
    public static string stringSeed;
    public static int seed;
    public static bool instantiated;

    //ALL OF THESE ARE System.Random classes, NOT UnityEngine.Random
    private static Random worldRand;
    private static Random itemRand;
    private static Random eventRand = new Random();


    public static void SetSeed(int newSeed, bool setItemSeed)
    {
        seed = newSeed;

        worldRand = new Random(seed);

        itemRand = (setItemSeed) ? new Random(seed) : new Random();
    }

    public static void SetSeed(string newStringSeed, bool setItemSeed)
    {
        stringSeed = newStringSeed;
        SetSeed(stringSeed.GetHashCode(), setItemSeed);
    }

    public static int GetWorldRand(int min, int max)
    {
        return worldRand.Next(min, max);
    }

    public static int GetWorldRand()
    {
        return worldRand.Next();
    }

    public static int GetItemRand(int min, int max)
    {
        return itemRand.Next(min, max);
    }

    public static float GetItemRand(float min, float max)
    {
        return (float) itemRand.NextDouble() * (max - min) + min;
    }

    public static int GetEventRand(int min, int max)
    {
        return eventRand.Next(min, max);
    }

    public static float GetEventRand(float min, float max)
    {
        return (float) eventRand.NextDouble() * (max - min) + min;
    }

}
