using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Cell> cells = new List<Cell>();


    private GameObject roomContents;
    private RoomData myRoomData;

    private const string PREFAB_PATH = "Prefabs/Rooms/";


    public void InstantiateRoom(RoomData roomData)
    {
        myRoomData = roomData;
        roomData.roomObject = this;
        Vector2 bottomLeftPos = roomData.BottomLeftPos();

        transform.position = bottomLeftPos * Floor.CELL_SIZE;

        //Instantiate each cell based on their localPos
        GameObject cellPrefab = Resources.Load<GameObject>(PREFAB_PATH + "Cell/BaseCell");
        foreach (CellData cellData in roomData.cellData) {
            GameObject newObject = Instantiate(cellPrefab, this.transform);
            Cell newCell = newObject.GetComponent<Cell>();

            Vector2 relativePos = cellData.position - bottomLeftPos;
            newCell.transform.localPosition = relativePos * Floor.CELL_SIZE;

            newCell.InstantiateCell(cellData, this);
            cells.Add(newCell);
        }

        //Teleport player to room if entry room
        if (roomData.roomType == RoomData.RoomType.Entry && Player.Instance)
        {
            Vector3 offset = new Vector3(Floor.CELL_SIZE / 2, Floor.CELL_SIZE / 2);
            Player.Instance.transform.position = cells[0].transform.position + offset;
        }
    }

    public void MoveRoom(Vector2 move, bool movePlayer)
    {
        myRoomData.ShiftCells(move);

        Vector3 offset = (move * Floor.CELL_SIZE);
        transform.position = transform.position + offset;

        if (movePlayer) Player.Instance.transform.position += offset;
    }

    
}
