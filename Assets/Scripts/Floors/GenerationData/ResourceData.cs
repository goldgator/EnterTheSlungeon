using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceData
{
    public ResourceType resourceType;
    public Vector2 position;
    public bool harvested;

    public ResourceData(ResourceType newType, Vector2 newPosition)
    {
        resourceType = newType;
        position = newPosition;
    }
}

public enum ResourceType { 
    Time,
    Space,
    Void,
    Valuable
}