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

    private const string ROOM_PATH = "Prefabs/Rooms/";
    private const string CONTENT_PATH = "Prefabs/RoomContents/";

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
        GameObject cellPrefab = Resources.Load<GameObject>(ROOM_PATH + "Cell/BaseCell");
        foreach (CellData cellData in roomData.cellData) {
            GameObject newObject = Instantiate(cellPrefab, this.transform);
            Cell newCell = newObject.GetComponent<Cell>();

            Vector2 relativePos = cellData.position - bottomLeftPos;
            newCell.transform.localPosition = relativePos * Floor.CELL_SIZE;

            newCell.InstantiateCell(cellData, this);
            cells.Add(newCell);
        }

        //Add content if not boss room
        if (roomData.roomType != RoomData.RoomType.Boss)
        {
            GameObject[] allRooms = Resources.LoadAll<GameObject>(CONTENT_PATH + myRoomData.RoomContentPool);
            //Debug.Log(CONTENT_PATH + myRoomData.RoomOpeningType);
            //Debug.Log(allRooms[0]);
            roomContents = Instantiate(allRooms[RNGManager.GetWorldRand(0, allRooms.Length)], transform).GetComponent<RoomContent>();
            roomContents.parentRoom = this;
        } else
        {
            string bossPath = "Prefabs/BossRooms/Golem";
            GameObject[] allRooms = Resources.LoadAll<GameObject>(bossPath);
            roomContents = Instantiate(allRooms[RNGManager.GetWorldRand(0, allRooms.Length)], transform).GetComponent<RoomContent>();
            roomContents.parentRoom = this;
        }
        

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
