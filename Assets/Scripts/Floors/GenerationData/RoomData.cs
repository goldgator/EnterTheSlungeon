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
    public string RoomContentPool {
        get
        {
            switch (roomType)
            {
                case RoomType.Boss:
                    return "BOSS";
                case RoomType.Entry:
                    return "ENTRY";
                case RoomType.Drill:
                    return "DRILL";
                case RoomType.Item:
                    return "ITEM";
                default:
                    //do nothing
                    break;
            }


            if (cellData.Count == 1)
            {
                return cellData[0].openings.CardinalListToString();
                //return "NESW";
            } else
            {
                //Only works with V and H currently
                switch (GetGridSize().x)
                {
                    case 1:
                        return "V";
                    case 2:
                        return "H";
                    default:
                        return "NULL";
                }
            }
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

    public RoomData(RoomType newType, List<CellData> newCells)
    {
        roomType = newType;
        cellData = newCells;

        //Change cell parent
        foreach (CellData cell in cellData)
        {
            cell.roomOwner = this;
        }
    }

    public RoomData(RoomType newType, CellData newCell)
    {
        roomType = newType;
        cellData = new List<CellData>();
        cellData.Add(newCell);
        newCell.roomOwner = this;
    }



    public void UpdateCellData()
    {
        //Update start position and sibling connections
        for (int i = 0; i < cellData.Count; i++)
        {
            cellData[i].UpdateStartPosition();

            for (int j = i + 1; j < cellData.Count; j++)
            {
                Vector2 diff = cellData[j].position - cellData[i].position;
                CardinalDir dir = Utilities.Vector2ToCardinalDir(diff);

                cellData[i].siblings.Add(dir);
                cellData[j].siblings.Add(Utilities.GetRelativeDir(dir, 2));
            }
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

    public Vector2 TopRightPos()
    {
        int xMax = int.MinValue;
        int yMax = int.MinValue;

        foreach (CellData cell in cellData)
        {
            if (cell.position.x > xMax) xMax = (int)cell.position.x;
            if (cell.position.y > yMax) yMax = (int)cell.position.y;
        }
        return new Vector2(xMax, yMax);
    }

    public Vector2 GetGridSize()
    {
        //Add (1, 1) to the difference between TopRightPos and BottomLeftPos to make it 1 based
        return (TopRightPos() - BottomLeftPos()) + new Vector2(1, 1);
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