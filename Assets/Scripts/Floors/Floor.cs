using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Floor : MonoBehaviour
{
    //[Header("FloorDetails")]
    public GameObject testPrefab;
    public GameObject testBorder;
    public GameObject visitedParent;
    public static float CELL_SIZE = 20;
    public Vector3 Offset
    {
        get
        {
            return new Vector3(CELL_SIZE / 2, CELL_SIZE / 2, 0);
        }
    }

    [Header("GenerationStats")]
    public FloorGenerator.FloorType floorType = FloorGenerator.FloorType.Expansive;
    public int patternSize;
    public bool debug = false;

    [Header("Components")]
    public GameObject floorCanvas;


    private FloorData generatedFloor;
    private List<Room> rooms = new List<Room>();



    public static Floor Instance { get; set; }
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        generatedFloor = FloorGenerator.GenerateFloor(floorType, patternSize);
        InstantiateFloor();
        Debug.Log(generatedFloor.FloorSize);
    }

    private void Update()
    {
        if (InputManager.Instance.Map)
        {
            bool currentState = floorCanvas.gameObject.activeSelf;

            floorCanvas.SetActive(!currentState);
            Player.Instance.SetPlayerEnabled(currentState);
        }
    }

    public FloorData GetFloorData()
    {
        return generatedFloor;
    }

    private void InstantiateFloor()
    {
        if (debug)
        {
            GameObject border = Instantiate(testBorder);
            Debug.Log(border);
            Debug.Log(border.transform);
            border.transform.localScale = (generatedFloor.roomMax + new Vector2(1, 1)) * CELL_SIZE;

            //Place visited cells
            for (int i = 0; i < generatedFloor.cells.GetLength(0); i++)
            {
                for (int j = 0; j < generatedFloor.cells.GetLength(1); j++)
                {
                    if (generatedFloor.cells[i, j] == FloorData.CellType.Visited)
                    {
                        Vector3 newPosition = (new Vector2(i, j) * CELL_SIZE);
                        newPosition += Offset;

                        GameObject testObject = Instantiate(testPrefab, newPosition, Quaternion.identity);
                        testObject.transform.SetParent(visitedParent.transform, false);
                        TMP_Text text = testObject.GetComponent<TMP_Text>();

                        text.text = "Visited";
                    }
                }
            }
        }

        GameObject roomPrefab = Resources.Load<GameObject>("Prefabs/Rooms/BaseRoom");
        Room roomComponent = roomPrefab.GetComponent<Room>();
        foreach (RoomData roomData in generatedFloor.roomData)
        {
            Room newRoom = Instantiate(roomComponent);
            newRoom.InstantiateRoom(roomData);
        }
    }

    public Cell CurrentPlayerCell()
    {
        return CellAtWorldPos(Player.Instance.transform.position);
    }

    public Cell CellAtWorldPos(Vector3 position)
    {
        return generatedFloor.CellAtPos(WorldToCellPos(position)).cellObject;
    }

    public Vector2 WorldToCellPos(Vector3 position)
    {
        float posX = position.x / 20f;
        float posY = position.y / 20f;

        posX = Mathf.Floor(posX);
        posY = Mathf.Floor(posY);

        return new Vector2(posX, posY);
    }

    public Cell FindCellInLine(Vector2 testPos, Vector2 stepOffset)
    {
        Cell foundCell = null;
        while (foundCell == null)
        {
            testPos += stepOffset;

            //return null if out of bounds
            if (!generatedFloor.IsInBounds(testPos)) return null;

            CellData cellData = generatedFloor.CellAtPos(testPos);
            if (cellData != null) foundCell = cellData.cellObject;

        }
        return foundCell;
    }

    public bool IsValidPosition(Vector2 cellPosition)
    {
        return generatedFloor.IsValidPosition(cellPosition);
    }

    public bool RoomCanMoveInDirection(RoomData roomData, CardinalDir dir)
    {
        return generatedFloor.RoomCanMoveInDirection(roomData, dir);
    }

    public bool IsOriginalPosition(Vector2 position)
    {
        return (generatedFloor.originalSpots.Contains(position));
    }
}
