using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Range(1,3)]
    public int floorLevel = 1;
    public FloorGenerator.FloorType floorType = FloorGenerator.FloorType.Expansive;
    public int patternSize = 7;
    public string stringSeed;
    public int seed;
    public bool debug = false;
    private int currentGenData = 0;
    

    [Header("Components")]
    public GameObject floorCanvas;
    public GameObject tempVictoryPanel;
    public GameObject tempLossPanel;


    private FloorData generatedFloor;
    private List<Room> rooms = new List<Room>();
    private Room bossRoom;

    private bool gameOver = false;
    

    private static Floor instance;
    public static Floor Instance { get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Floor>();
                if (instance == null) return null;
                
            }
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
    }

    public void SetFloorAttributes()
    {
        currentGenData++;

        floorLevel = FloorGenerator.floorGenSequence[currentGenData].floorLevel;
        floorType = FloorGenerator.floorGenSequence[currentGenData].floorType;
        patternSize = FloorGenerator.floorGenSequence[currentGenData].patternSize;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartFloor();
    }

    static bool seeded = false;
    private void StartFloor()
    {
        if (!RNGManager.instantiated) seeded = SetRandomSeed();
        if (generatedFloor == null) generatedFloor = FloorGenerator.GenerateFloor(floorType, patternSize, seeded);
        InstantiateFloor();
    }

    private bool SetRandomSeed()
    {
        bool seeded = false;
        if (stringSeed != "")
        {
            Debug.Log("Seed: " + stringSeed);
            RNGManager.SetSeed(stringSeed, true);
            seeded = true;
        } else
        {
            if (seed == 0) {
                seed = Random.Range(0, int.MaxValue);
            } else
            {
                seeded = true;
            }
            Debug.Log("Seed: " + seed);
            RNGManager.SetSeed(seed, true);
        }

        RNGManager.instantiated = true;
        return seeded;
    }

    private void Update()
    {
        if (InputManager.Instance.Map && !PlayerInActiveRoom())
        {
            bool currentState = floorCanvas.gameObject.activeSelf;
            Player.Instance.ForceStop();

            floorCanvas.SetActive(!currentState);
            Player.Instance.SetPlayerEnabled(currentState);
        }

        //Check if game won
        /*if (bossRoom.Completed)
        {
            OnFloorFinish();
        }*/

        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("FloorScene");
            }
        }
    }

    private void OnFloorFinish()
    {
        tempVictoryPanel.SetActive(true);
        Player.Instance.SetPlayerEnabled(false);
        Player.Instance.ForceStop();
        MusicManager.Instance.PlaySong("Victory");
        gameOver = true;
    }

    public void OnPlayerDeath()
    {
        tempLossPanel.SetActive(true);
        MusicManager.Instance.PlaySong("Victory");
        gameOver = true;
    }

    public bool PlayerInActiveRoom()
    {
        Room currentRoom = CurrentPlayerCell().GetRoom();

        return (!currentRoom.Completed);
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
            //Debug.Log(border);
            //Debug.Log(border.transform);
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
            if (roomData.roomType == RoomData.RoomType.Boss) bossRoom = newRoom;
        }
    }

    public void UpdateBossRoomState()
    {
        //Check each original spot and see if it has a cell (skip boss original spot)
        bool patternFinished = PatternState();

        //If none are empty, set the lock to false
        bossRoom.locked = !patternFinished;
    }

    public bool PatternState()
    {
        for (int i = 1; i < generatedFloor.originalSpots.Count; i++)
        {
            Cell cell = CellAtPos(generatedFloor.originalSpots[i]);

            //If spot is empty (no cell) then set lock to true and leave method
            if (cell == null)
            {
                bossRoom.locked = true;
                return false;
            }
            else
            {
                //Check if cell's room has not been completed
                if (!cell.GetRoom().Completed)
                {
                    bossRoom.locked = true;
                    return false;
                }
            }
        }

        return true;
    }

    public Cell CurrentPlayerCell()
    {
        return CellAtWorldPos(Player.Instance.transform.position);
    }

    public Cell CellAtWorldPos(Vector3 position)
    {
        return generatedFloor.CellDataAtPos(WorldToCellPos(position)).cellObject;
    }

    public Cell CellAtPos(Vector2 position)
    {
        CellData data = generatedFloor.CellDataAtPos(position);
        if (data != null)
        {
            return data.cellObject;
        } else
        {
            return null;
        }
    }

    public CellData CellDataAtPos(Vector2 position)
    {
        return generatedFloor.CellDataAtPos(position);
    }

    public bool HasCellAtPos(Vector2 position)
    {
        return (generatedFloor.CellDataAtPos(position) != null);
    }

    public Vector2 WorldToCellPos(Vector3 position)
    {
        float posX = position.x / 20f;
        float posY = position.y / 20f;

        posX = Mathf.Floor(posX);
        posY = Mathf.Floor(posY);

        return new Vector2(posX, posY);
    }

    public Cell FindCellInLine(Vector2 testPos, Vector2 stepOffset, bool ignoreSelf = true)
    {
        RoomData originRoom = CellDataAtPos(testPos).roomOwner;
        Cell foundCell = null;
        while (foundCell == null)
        {
            testPos += stepOffset;

            //return null if out of bounds
            if (!generatedFloor.IsInBounds(testPos)) return null;

            //Grab cellData, but set back to null if same as origin room and ignoreSelf is true
            CellData cellData = generatedFloor.CellDataAtPos(testPos);
            if (cellData != null)
            {
                if (cellData.roomOwner == originRoom && ignoreSelf)
                {
                    cellData = null;
                }
                else {
                    foundCell = cellData.cellObject;
                }
            }
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
