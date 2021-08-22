using System;
using System.Collections.Generic;
using UnityEngine;

public class CellData
{
    


    public Vector2 position;
    public Vector2 startPosition;
    public RoomData roomOwner;
    public List<CardinalDir> openings = new List<CardinalDir>();
    public GameObject roomContent;

    public CellData(Vector2 newPosition, RoomData newOwner)
    {
        position = newPosition;
        roomOwner = newOwner;
    }

    public void UpdateStartPosition()
    {
        startPosition = position;
    }

    public override string ToString()
    {
        string concat = "Openings:";
        foreach(CardinalDir dir in openings)
        {
            concat += "\n" + dir;
        }

        concat += "\nRoomType:\n" + roomOwner.roomType;
        return concat;
    }

    public void RemoveDir(CardinalDir direction)
    {
        for (int i = 0; i < openings.Count; i++)
        {
            if (openings[i] == direction)
            {
                openings.RemoveAt(i);
                break;
            }
        }
    }
}