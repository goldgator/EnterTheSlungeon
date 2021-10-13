using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Transform[] openingTransforms = new Transform[4];
    public GameObject cornerTileMapParent;
    private List<GameObject> cornerTiles;
    private Door[] doors = new Door[4];
    private const string PREFAB_PATH = "Prefabs/Rooms/Cell/";
    private string levelFolder
    {
        //return level folder from floor level (F1, F2, F3, etc)
        get { return "F" + Floor.Instance.floorLevel + "/"; }
    }

    private CellData myCellData;
    private Room roomOwner;
    private GameObject doorObject;
    private GameObject wallObject;

    private void Awake()
    {
        doorObject = Resources.Load<GameObject>(PREFAB_PATH + levelFolder + "DoorWall");
        wallObject = Resources.Load<GameObject>(PREFAB_PATH + levelFolder + "FullWall");
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
        cornerTileMapParent.GetAllChildren(out cornerTiles, false);
        //Debug.Log(cornerTiles);
        cellData.cellObject = this;

        roomOwner = newRoomOwner;

        //Delete any transforms to neighboring cell siblings
        //List<CardinalDir> neighboringSiblingCells = cellData.ReturnCellSiblingDirs();

        foreach(CardinalDir dir in cellData.siblings) openingTransforms[(int)dir] = null;

        //Iterate through each transform, and Instantiate a wall and door prefab where applicable
        for (int i = 0; i < 4; i++)
        {
            CardinalDir currentDir = (CardinalDir)i;

            //Corner toggling
            if (cellData.HasSibDir(currentDir))
            {
                //Disable two corner tiles maps, same index as dir and one previous
                int currInd = (int)currentDir;
                int prevInd = (currInd == 0) ? 3 : currInd - 1;

                Debug.Log("Turning off tiles");
                cornerTiles[currInd].SetActive(false);
                cornerTiles[prevInd].SetActive(false);
            }

            //Continue if current openingTransform is null
            if (openingTransforms[i] == null) continue;

            //openings
            if (cellData.HasConnDir(currentDir))
            {
                //Door
                Door newDoor = Instantiate(doorObject, openingTransforms[i]).GetComponent<Door>();
                newDoor.parentCell = this;
                newDoor.direction = currentDir;
                doors[i] = newDoor;
            } else
            {
                //Wall
                Instantiate(wallObject, openingTransforms[i]);
            }

            
        }
    }

    public void UpdateState()
    {
        for (int i = 0; i < 4; i++)
        {
            if (doors[i] == null) continue;
            CardinalDir currentDir = (CardinalDir)i;
            CardinalDir neededDir = Utilities.GetRelativeDir(currentDir, 2);

            //Check if there is a cell at direction
            CellData neighborCell = Floor.Instance.CellDataAtPos(myCellData.position + Utilities.CardinalDirToVector2(currentDir));
            //Check if cell exists and has direction, and is not locked
            bool state = false;
            if (neighborCell != null && neighborCell.HasConnDir(neededDir)) {
                if (neighborCell.cellObject)
                {
                    if (neighborCell.cellObject.GetRoom())
                    {
                        state = !neighborCell.cellObject.GetRoom().locked;
                    } else
                    {
                        state = false;
                    }
                } else
                {
                    state = false;
                }
            }

            //Update door to the result
            doors[i].SetDoorState(!state);
        }
    }

    public void MovePlayer(CardinalDir attemptedDir)
    {
        Cell targetCell = Floor.Instance.CellAtPos(myCellData.position + Utilities.CardinalDirToVector2(attemptedDir));
        if (targetCell)
        {
            //Check if cell has direction
            CardinalDir takenDir = Utilities.GetRelativeDir(attemptedDir, 2);
            if (targetCell.myCellData.HasConnDir(takenDir))
            {
                targetCell.TakePlayer(takenDir);
            } else
            {
                Debug.LogError("Target Cell does not have needed direction, were doors created/updated correctly?");
            }
        } else
        {
            Debug.LogError("Target Cell not found, were doors created/updated correctly?");
        }
    }

    public void TakePlayer(CardinalDir takenDir)
    {
        //Grab door spawn location (door should have already been confirmed to exist)
        Vector3 spawnPosition = doors[(int)takenDir].spawnLocation.position;
        Player.Instance.transform.position = spawnPosition;

        roomOwner.TakePlayer();
    }

    public void CloseAllDoors()
    {
        for (int i = 0; i < 4; i++)
        {
            if (doors[i] == null) continue;

            //Update door to the result
            doors[i].SetDoorState(true);
        }
    }
}
