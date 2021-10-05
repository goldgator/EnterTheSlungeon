using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public enum RoomType
    {
        Generic,
        Boss,
        Entry,
        Drill,
        Item
    }
    

    public RoomType roomType = RoomType.Generic;
    public List<CellData> cellData = new List<CellData>();
    public int visits = 0;
    public Room roomObject;
    public string RoomOpeningType {
        get
        {
            return cellData[0].openings.CardinalListToString();
            //return "NESW";
        }
    }

    public bool Shuffled
    {
        get
        {
            return (cellData[0].position != cellData[0].startPosition);
        }
    }
    

    public RoomData(RoomType newType, Vector2 newPosition)
    {
        roomType = newType;
        cellData.Add(new CellData(newPosition, this));
    }


    public RoomData(RoomType newType, List<Vector2> newPositions)
    {
        roomType = newType;

        foreach (Vector2 position in newPositions)
        {
            cellData.Add(new CellData(position, this));
        }
    }

    public void UpdateCellData()
    {
        foreach(CellData cell in cellData)
        {
            cell.UpdateStartPosition();
        }
    }

    public void ShiftCells(Vector2 offset)
    {
        foreach (CellData cell in cellData)
        {
            cell.position += offset;
        }
    }

    public Vector2 BottomLeftPos()
    {
        int xMin = int.MaxValue;
        int yMin = int.MaxValue;

        foreach(CellData cell in cellData)
        {
            if (cell.position.x < xMin) xMin = (int)cell.position.x;
            if (cell.position.y < yMin) yMin = (int)cell.position.y;
        }
        return new Vector2(xMin, yMin);
    }

    public bool HasOpening(Vector2 cellPos, CardinalDir opening)
    {
        
        foreach(CellData cell in cellData)
        {
            if (cell.position == cellPos)
            {
                foreach(CardinalDir direction in cell.openings)
                {
                    if (direction == opening) return true;
                }
            }
        }

        return false;
    }

    
}