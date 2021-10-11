using System;
using System.Collections.Generic;
using UnityEngine;

public class CellData
{
    public Vector2 position;
    public Vector2 startPosition;
    public RoomData roomOwner;
    public List<CardinalDir> openings = new List<CardinalDir>();
    public List<CardinalDir> siblings = new List<CardinalDir>();
    public GameObject roomContent;
    public Cell cellObject;

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

    public void RemoveConnDir(CardinalDir direction)
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

    public bool HasConnDir(CardinalDir direction)
    {
        return openings.Contains(direction);
    }

    public bool HasSibDir(CardinalDir direction)
    {
        return siblings.Contains(direction);
    }

    public void RemoveSibDir(CardinalDir direction)
    {
        for (int i = 0; i < siblings.Count; i++)
        {
            if (siblings[i] == direction)
            {
                siblings.RemoveAt(i);
                break;
            }
        }
    }

    public List<CardinalDir> ReturnCellSiblingDirs()
    {
        List<CardinalDir> allDirs = new List<CardinalDir>();
        foreach(CellData otherCell in roomOwner.cellData)
        {
            //Skip if it is this cell
            if (otherCell == this) continue;

            //position difference
            Vector2 diff = otherCell.position - position;

            //if magnitude is >1 then it is diagonal, and not a direct neighbor, continue to next cell
            if (diff.magnitude > 1) continue;

            //Find the direction the neighboring cell is in
            allDirs.Add(Utilities.Vector2ToCardinalDir(diff));
        }

        return allDirs;
    }
}