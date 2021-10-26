using System;
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


    public static string CardinalListToString(this List<CardinalDir> dirs)
    {
        dirs.Sort();
        string concat = "";
        foreach(CardinalDir dir in dirs)
        {
            switch (dir)
            {
                case CardinalDir.North:
                    concat += "N";
                    break;
                case CardinalDir.East:
                    concat += "E";
                    break;
                case CardinalDir.South:
                    concat += "S";
                    break;
                case CardinalDir.West:
                    concat += "W";
                    break;
                default:
                    break;
            }
        }

        return concat;
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
        return (CardinalDir) (RNGManager.GetWorldRand(0, 4));
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

    public static void GetAllChildren(this GameObject parent, out List<GameObject> childrenList, bool recursive = false)
    {
        childrenList = new List<GameObject>();
        GetChildrenRecursive(parent, childrenList, recursive);
    }

    private static void GetChildrenRecursive(GameObject parent, List<GameObject> childrenList, bool recursive)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            childrenList.Add(child);

            if (recursive) GetChildrenRecursive(child, childrenList, recursive);
        }
    }

    public static Vector2 GetUnitVector2(float angle)
    {
        angle *= Mathf.Deg2Rad;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        return new Vector2(x,y);
    }

    public static void MoveIndexToFront<T>(this List<T> list, int index)
    {
        T item = list[index];
        list.RemoveAt(index);
        list.Insert(0, item);
    }

}

