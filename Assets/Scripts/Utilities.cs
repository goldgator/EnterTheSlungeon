﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static GameObject AudioFXObject
    {
        get
        {
            return Resources.Load<GameObject>("Prefabs/Misc/SoundFX");
        }
    }



    public static Vector2 CardinalDirToVector2(CardinalDir direction)
    {
        switch (direction)
        {
            case CardinalDir.North:
                return new Vector2(0, 1);
            case CardinalDir.East:
                return new Vector2(1, 0);
            case CardinalDir.South:
                return new Vector2(0, -1);
            case CardinalDir.West:
                return new Vector2(-1, 0);
        }

        return new Vector2();
    }

    public static CardinalDir Vector2ToCardinalDir(Vector2 vector)
    {
        if (vector == Vector2.up)
        {
            return CardinalDir.North;
        } else if (vector == Vector2.down)
        {
            return CardinalDir.South;
        } else if (vector == Vector2.right)
        {
            return CardinalDir.East;
        } else if (vector == Vector2.left)
        {
            return CardinalDir.West;
        } else
        {
            Debug.Log(vector);
            throw new Exception("Invalid vector for Cardinal Direction");
        }
    }

    public static CardinalDir GetRandomDir()
    {
        return (CardinalDir) (UnityEngine.Random.Range(0, 4));
    }

    public static Vector2 GetRandomCardinalVector()
    {
        return CardinalDirToVector2(GetRandomDir());
    }

    public static CardinalDir GetRandomAdjacentDir(CardinalDir direction)
    {
        int steps = (UnityEngine.Random.value < .5f) ? -1 : 1;

        return GetRelativeDir(direction, steps);
    }

    public static CardinalDir GetRelativeDir(CardinalDir direction, int steps)
    {
        while (steps < 0)
        {
            steps += 4;
        }

        //move clockwise this many steps from current dir to get that dir
        return (CardinalDir)((((int)direction) + steps) % 4);
    }

    public static T GetComponentFromParent<T>(this GameObject baseObject) where T : Component
    {
        T wantedComponent = baseObject.GetComponent<T>();
        if (wantedComponent == null)
        {
            GameObject parent = baseObject.transform.parent.gameObject;
            if (parent)
            {
                return parent.GetComponentFromParent<T>();
            } else
            {
                return null;
            }
        } else
        {
            return wantedComponent;
        }
    }

}
