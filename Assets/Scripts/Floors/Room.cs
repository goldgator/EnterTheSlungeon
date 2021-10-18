using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Cell> cells = new List<Cell>();

    //Events
    public delegate void RoomEnter();
    public RoomEnter roomEnterEvent;

    private RoomContent roomContents;
    private RoomData myRoomData;

    private const string ROOM_PATH = "Prefabs/Rooms/Cell/";
    private const string CONTENT_PATH = "Prefabs/RoomContents/";
    private string levelFolder
    {
        //return level folder from floor level (F1, F2, F3, etc)
        get { return "F" + Floor.Instance.floorLevel + "/"; }
    }

    public bool CanMove
    {
        get
        {
            return roomContents.Completed && (myRoomData.roomType != RoomData.RoomType.Boss);
        }
    }
    public bool CanBePulled
    {
        get
        {
            return (myRoomData.roomType != RoomData.RoomType.Boss);
        }
    }
    public bool Completed
    {
        get { return roomContents.Completed; }
    }
    public bool locked = false;
    public Vector2 GridSize
    {
        get { return myRoomData.GetGridSize(); }
    }

    public void InstantiateRoom(RoomData roomData)
    {
        myRoomData = roomData;
        roomData.roomObject = this;
        Vector2 bottomLeftPos = roomData.BottomLeftPos();

        transform.position = bottomLeftPos * Floor.CELL_SIZE;

        //Instantiate each cell based on their localPos
        string baseCellPath = ROOM_PATH + levelFolder + "BaseCell";
        GameObject cellPrefab = Resources.Load<GameObject>(baseCellPath);
        foreach (CellData cellData in roomData.cellData) {
            GameObject newObject = Instantiate(cellPrefab, this.transform);
            Cell newCell = newObject.GetComponent<Cell>();

            Vector2 relativePos = cellData.position - bottomLeftPos;
            newCell.transform.localPosition = relativePos * Floor.CELL_SIZE;

            newCell.InstantiateCell(cellData, this);
            cells.Add(newCell);
        }

        
        string contentPath = CONTENT_PATH + levelFolder + myRoomData.RoomContentPool;
        GameObject[] allRooms = Resources.LoadAll<GameObject>(contentPath);

        roomContents = Instantiate(allRooms[RNGManager.GetWorldRand(0, allRooms.Length)], transform).GetComponent<RoomContent>();
        roomContents.parentRoom = this;
        
        

        //Teleport player to room if entry room, and update cells
        if (roomData.roomType == RoomData.RoomType.Entry && Player.Instance)
        {
            Vector3 offset = new Vector3(Floor.CELL_SIZE / 2, Floor.CELL_SIZE / 2);
            Player.Instance.transform.position = cells[0].transform.position + offset;
            ForceComplete();
            UpdateCells();
        }

        //If room is a boss room, lock it
        locked = (roomData.roomType == RoomData.RoomType.Boss);
    }

    public void MoveRoom(Vector2 move, bool movePlayer)
    {
        myRoomData.ShiftCells(move);

        Vector3 offset = (move * Floor.CELL_SIZE);
        transform.position = transform.position + offset;

        if (movePlayer) Player.Instance.transform.position += offset;

        Floor.Instance.UpdateBossRoomState();
        UpdateCells();
    }

    public void UpdateCells()
    {
        foreach(Cell cell in cells)
        {
            cell.UpdateState();
        }
    }

    public void RoomFinished()
    {
        Floor.Instance.UpdateBossRoomState();
        UpdateCells();
    }

    public void TakePlayer()
    {
        PlayerCamera.Instance.GetNewBounds();
        RoomEntered();
    }

    private void RoomEntered()
    {
        if (Completed)
        {
            UpdateCells();
        } else
        {
            CloseAllDoors();
            roomEnterEvent?.Invoke();
        }
    }

    private void CloseAllDoors()
    {
        foreach (Cell cell in cells)
        {
            cell.CloseAllDoors();
        }
    }

    public void ForceComplete()
    {
        roomContents.ForceComplete();
    }


}
