using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Transform[] openingTransforms = new Transform[4];
    private const string PREFAB_PATH = "Prefabs/Rooms/Cell/";

    private CellData myCellData;
    private Room roomOwner;
    private GameObject doorObject;
    private GameObject wallObject;

    private void Awake()
    {
        doorObject = Resources.Load<GameObject>(PREFAB_PATH + "DoorWall");
        wallObject = Resources.Load<GameObject>(PREFAB_PATH + "FullWall");
    }

    public CellData GetData()
    {
        return myCellData;
    }

    public Room GetRoom()
    {
        return roomOwner;
    }

    public void InstantiateCell(CellData cellData, Room newRoomOwner)
    {
        myCellData = cellData;
        cellData.cellObject = this;

        roomOwner = newRoomOwner;

        //Delete any transforms to neighboring cell siblings
        List<CardinalDir> neighboringSiblingCells = cellData.ReturnCellSiblingDirs();
        foreach(CardinalDir dir in neighboringSiblingCells) openingTransforms[(int)dir] = null;

        //Iterate through each transform, and Instantiate a wall and door prefab where applicable
        for (int i = 0; i < 4; i++)
        {
            if (openingTransforms[i] == null) continue;

            CardinalDir currentDir = (CardinalDir)i;

            if (cellData.openings.Contains(currentDir))
            {
                //Door
                Instantiate(doorObject, openingTransforms[i]);
            } else
            {
                //Wall
                Instantiate(wallObject, openingTransforms[i]);
            }
        }
    }
}
