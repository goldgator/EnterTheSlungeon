using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    // INSPECTOR VALUES
    [Header("Spawn Values")]
    public int roomStopAmount;
    public Vector2 floorSize;
    public bool randomStart = false;
    public Vector2 startPosition;

    [Header("Components")]
    public GameObject camera;
    public GameObject player;

    //PUBLIC VALUES
    public static Floor Instance { get; set; }
    [HideInInspector]
    public const float CELL_SIZE = 20.0f;

    // PRIVATE VALUES
    private List<Room> rooms = new List<Room>();


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnFloor();
        SetPlayer();
    }

    private void SetPlayer()
    {
        camera.transform.position = rooms[0].transform.position;
        camera.transform.Translate(new Vector3(0, 0, -10));
        player.transform.position = rooms[0].transform.position;
    }

    private void SpawnFloor()
    {
        rooms.Add(SpawnRandomRoom("Entry", new Vector2(0,0)));

        while (rooms.Count < roomStopAmount)
        {
            GenerateRoom();
        }

        //Move everything into the first quadrant (no position is in negatives)
        AdjustRoom();

        UpdateState();

    }

    private void AdjustRoom()
    {
        int lowestPoint = 0;
        int leftMostPoint = 0;

        int highestPoint = 0;
        int rightMostPoint = 0;

        foreach(Room room in rooms)
        {
            Vector2 pos = room.GridPosition;
            if (pos.x < leftMostPoint) leftMostPoint = (int) pos.x;
            if (pos.x > rightMostPoint) rightMostPoint = (int) pos.x;

            if (pos.y < lowestPoint) lowestPoint = (int) pos.y;
            if (pos.y > highestPoint) highestPoint = (int) pos.y;
        }

        //Adjust based bottom left of generated dungeon
        Vector3 adjustVector = new Vector2(leftMostPoint - 1, lowestPoint - 1);

        transform.position -= (adjustVector * CELL_SIZE);


        floorSize = new Vector2(rightMostPoint - adjustVector.x + 1, highestPoint - adjustVector.y + 1);
        Debug.Log(floorSize);
    }

    private void GenerateRoom()
    {
        //Get a list of OPEN connection points
        List<ConnectionPoint> openPoints = new List<ConnectionPoint>();
        foreach (Room room in rooms) openPoints.AddRange(room.ReturnOpenPoints(true));

        //Select a random from the bunch
        ConnectionPoint chosenPoint = openPoints[Random.Range(0, openPoints.Count)];

        //Get a list of roomtypes that will work for that connection
        string[] roomTypes = ConnectionPoint.RoomTypesFromDirection(chosenPoint.direction, openPoints.Count);

        //Select a random roomtype
        int randInd = Random.Range(0, roomTypes.Length);
        string roomType = roomTypes[randInd];

        //Instantiate a room based on that type
        rooms.Add(SpawnRandomRoom(roomType, chosenPoint.GridPosition));
    }

    public Room SpawnRandomRoom(string roomType, Vector2 position)
    {
        GameObject[] roomOptions = Resources.LoadAll<GameObject>("Prefabs/Rooms/" + roomType);

        GameObject randomRoom = roomOptions[Random.Range(0, roomOptions.Length)];
        Vector3 spawnPosition = GetWorldPosFromGrid(position);

        GameObject spawnedRoom = Instantiate(randomRoom, spawnPosition, Quaternion.identity, transform);

        return spawnedRoom.GetComponent<Room>();
    }

    public void UpdateState()
    {
        foreach (Room room in rooms)
        {
            room.UpdateState();
        }
    }




    #region Query Functions
    public Vector3 GetWorldPosFromGrid(Vector2 gridPos)
    {
        return (gridPos * CELL_SIZE) + new Vector2(CELL_SIZE / 2, CELL_SIZE / 2);
    }

    public Room GetRoomAtGridPos(Vector2 gridPos)
    {
        foreach (Room room in rooms)
        {
            if (room.GridPosition == gridPos)
            {
                return room;
            }
        }
        return null;
    }

    public bool IsGridPosEmpty(Vector2 gridPos)
    {
        return (GetRoomAtGridPos(gridPos) == null);
    }

    #endregion


}
