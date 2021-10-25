using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System;

public static class FloorGenerator
{
    public enum FloorType
    {
        Expansive,
        Tight,
        ConstrainedExpansive
    }

    [Serializable]
    public struct FloorGenData
    {
        public int floorLevel;
        public int patternSize;
        public FloorType floorType;

        public FloorGenData(int newLevel, int newSize, FloorType newType)
        {
            floorLevel = newLevel;
            patternSize = newSize;
            floorType = newType;
        }
    }

    //First one is default, values in inspector will take priority
    public static FloorGenData[] floorGenSequence = {
            new FloorGenData(1, 7, FloorType.Expansive),
            new FloorGenData(2, 7, FloorType.Tight),
            new FloorGenData(3, 7, FloorType.ConstrainedExpansive)
        };

    public static FloorData GenerateFloor(FloorType floorType, int patternSize, bool seeded)
    {
        FloorData createdFloor = null;
        do
        {
            switch (floorType)
            {
                case FloorType.Expansive:
                    createdFloor = ExpansiveFloor(patternSize);
                    break;
                case FloorType.Tight:
                    createdFloor = TightFloor(patternSize);
                    break;
                case FloorType.ConstrainedExpansive:
                    createdFloor = ConstrainedFloor(patternSize);
                    break;
            }

        } while (!TestFloorQuality(createdFloor));

        //Add created floors to file
        if (!seeded)
        {
            TrackData(createdFloor);
        }

        return createdFloor;
    }

    public static bool TestFloorQuality(FloorData floorData)
    {
        //Don't even bother if floorData was not even created
        if (floorData == null) return false;

        //Determine how many rooms remain in their original spot
        //Reject if over 1
        if (floorData.UnshuffledRooms() > 1)
        {
            Debug.LogError("Generated floor rejected for: poor shuffling");
            return false;
        }

        //Determine if they added resource data
        //Reject if it didn't
        if (floorData.resourceData.Count < 2)
        {
            Debug.LogError("Generated floor rejected for: no resources");
            return false;
        }

        //If never rejected, floor has good quality
        return true;
    }

    

    public static FloorData ExpansiveFloor(int patternSize)
    {
        //Create base pattern
        List<RoomData> roomData = CreatePattern(patternSize);

        //Create FloorData to hold RoomData and other details
        FloorData floorData = new FloorData(roomData, FloorType.Expansive);
        floorData.UpdateOriginalPositions();

        //Shuffle rooms in floor Data
        //Old return not needed anymore
        floorData.Shuffle(5, 4);

        //Assign roomTypes
        floorData.AssignRoomTypes();

        //Sprinkle rooms
        int sprinkleAmount = (int)(patternSize * .8f);
        floorData.AddSprinkleRooms(sprinkleAmount);
        floorData.AddMineRoom();

        //Assign Room Contents

        //Send to floor to instantiate
        return floorData;
    }

    public static FloorData TightFloor(int patternSize)
    {
        //Create base pattern
        List<RoomData> roomData = CreatePattern(patternSize);

        //Create FloorData to hold RoomData and other details
        FloorData floorData = new FloorData(roomData, FloorType.Tight);
        floorData.UpdateOriginalPositions();

        //Shuffle rooms in floor Data
        //Old return not needed anymore
        floorData.Shuffle(5, 4);

        //Set room assignments
        floorData.AssignRoomTypes();

        //Sprinkle rooms
        int sprinkleAmount = (int)(patternSize * .5f);
        floorData.AddSprinkleRooms(sprinkleAmount);
        floorData.AddMineRoom();

        //Assign Room Contents

        //Send to floor to instantiate
        return floorData;
    }

    public static FloorData ConstrainedFloor(int patternSize)
    {
        return null;
    }

    /// <summary>
    /// Creates the basic puzzle layout for the floor
    /// </summary>
    /// <param name="cellSize">amount of cells created MUST BE >1</param>
    /// <returns>A list of rooms unshuffled without doors assigned</returns>
    public static List<RoomData> CreatePattern(int cellSize)
    {
        //Create list of rooms to store
        List<RoomData> roomData = new List<RoomData>();

        //Create boss room at 0,0
        RoomData bossRoom = new RoomData(RoomData.RoomType.Boss, new Vector2());
        roomData.Add(bossRoom);

        //Add opening in random direction
        bossRoom.cellData[0].openings.Add((CardinalDir)RNGManager.GetWorldRand(0,4));

        //Add Generic room next to opening
        RoomData firstRoom = new RoomData(RoomData.RoomType.Generic, Utilities.CardinalDirToVector2(bossRoom.cellData[0].openings[0]));
        roomData.Add(firstRoom);

        //Choose random cell (excluding boss room) and place in a random open spot nearby
        //Until roomData.Count is cellSize + 1
        while (roomData.Count < (cellSize + 1))
        {
            TryPlaceRoom(roomData, cellSize);
        }

        //Assign all openings
        AssignDoors(roomData);

        //Roll a chance to combine rooms into a large room
        if (RNGManager.GetWorldRand(0, 100) < 40)
        {
            Debug.Log("Attempting big room");
            AddBigRoom(roomData);
        }

        //Return populated RoomData List
        return roomData;
    }

    /// <summary>
    /// Adds a big room to the pattern, assumes there is no others present
    /// </summary>
    /// <param name="roomData"></param>
    private static void AddBigRoom(List<RoomData> roomData)
    {
        //TODO add L rooms

        //Chose random room and take the cell (exluding boss room)
        RoomData baseRoom = roomData[RNGManager.GetWorldRand(1, roomData.Count)];
        CellData baseCell = baseRoom.cellData[0];

        //Randomly determine how many extra cells there will be
        //int extraCells = RNGManager.GetWorldRand(1, 2);
        int extraCells = 1;
        extraCells = Mathf.Min(baseCell.openings.Count, extraCells); //lower extra cell count if not enough openings

        //Randomly grab additional cell from adjacent cell
        List<RoomData> grabbedRooms = roomData.GrabRandAdjacentRooms(baseRoom, extraCells);

        //IF successfully grabbed a room
        List<CellData> grabbedCells = new List<CellData>();
        if (grabbedRooms.Count > 0)
        {
            //Combine all CellData into one list and remove old RoomData from list
            roomData.Remove(baseRoom);
            grabbedCells.Add(baseCell);
            foreach (RoomData room in grabbedRooms) {
                grabbedCells.Add(room.cellData[0]);
                roomData.Remove(room);
            }

            //Create new RoomData with CellData inside
            RoomData newBigRoom = new RoomData(RoomData.RoomType.Generic, grabbedCells);

            //Add new bigRoom to the list
            roomData.Add(newBigRoom);
            Debug.Log("Big room added");
        }
        //ELSE
        else
        {
            //Throw exception saying bigroom failed
            Debug.Log("Big room failed");
        }

    }

    private static List<RoomData> GrabRandAdjacentRooms(this List<RoomData> allRooms, RoomData baseRoom, int extraCells, bool excludeBoss = true)
    {
        //Create a list of roomData
        List<RoomData> grabbedRooms = new List<RoomData>();

        //Copy list of directions and shuffle using world gen rand pool
        List<CardinalDir> shuffleDir = new List<CardinalDir>(baseRoom.cellData[0].openings);
        shuffleDir = shuffleDir.OrderBy(ctx => RNGManager.GetWorldRand()).ToList();

        //Loop until out of directions or reached required rooms
        foreach (CardinalDir dir in shuffleDir)
        {
            RoomData neighborRoom = allRooms.RoomAtPos(baseRoom.cellData[0].position + Utilities.CardinalDirToVector2(dir));
            //Skip room if null or a boss room
            if (neighborRoom == null || (excludeBoss && neighborRoom.roomType == RoomData.RoomType.Boss)) continue;

            //Save room
            grabbedRooms.Add(neighborRoom);

            //Exit loop if found enough rooms
            if (grabbedRooms.Count >= extraCells) break;
        }

        return grabbedRooms;
    }

    public static void TryPlaceRoom(List<RoomData> roomData, int cellSize)
    {
        //Create a Vector2 to hold a free space once found
        bool failed = false;
        Vector2 tryPlace;
        do
        {
            failed = false;
            //Select random room (not boss room)
            int randInd = RNGManager.GetWorldRand(1, roomData.Count);
            RoomData currentRoom = roomData[randInd];

            //Select a random cell and its position from that room
            int randCellInt = RNGManager.GetWorldRand(0,currentRoom.cellData.Count);
            Vector2 randCell = currentRoom.cellData[randCellInt].position;

            //Create a random offset from that cell
            Vector2 randomDir = Utilities.GetRandomCardinalVector();

            //Combine offset and cell position
            tryPlace = randCell + randomDir;

            //Find another position if cell would be placed at outermost border
            //This is to prevent patterns from being straight lines
            if (Mathf.Abs(tryPlace.x) >= cellSize || Mathf.Abs(tryPlace.y) >= cellSize)
            {
                failed = true;
                continue;
            }

            //Also reject if it would create a 2x2  (excluding boss room)
            if (WouldCreateTwoByTwo(roomData, tryPlace))
            {
                failed = true;
                
                continue;
            }

        //Loop if there is already a room with that position, or failed checks
        } while (roomData.HasRoomAtPos(tryPlace) || failed);

        //Add room at new found empty position.
        RoomData nextRoom = new RoomData(RoomData.RoomType.Generic, tryPlace);
        roomData.Add(nextRoom);
    }

    /// <summary>
    /// Method that tests if a new placement would create a 2x2 in the pattern.
    /// Kinda gross but it works and should be faster than a brute force for loop
    /// </summary>
    /// <returns></returns>
    private static bool WouldCreateTwoByTwo(List<RoomData> roomData, Vector2 testPosition)
    {
        //Test for a room above or below, if both false, method is false
        if (roomData.HasRoomAtPos(testPosition + new Vector2(0,1)))
        {
            //Check left and right, if one is true, check corresponding diagnol, if both true, method is true
            //left room
            if (roomData.HasRoomAtPos(testPosition + new Vector2(-1, 0))) {
                //top left corner
                if (roomData.HasRoomAtPos(testPosition + new Vector2(-1, 1)))
                {
                    return true;
                }
            }

            //right room
            if (roomData.HasRoomAtPos(testPosition + new Vector2(1, 0))) {
                //top right corner
                if (roomData.HasRoomAtPos(testPosition + new Vector2(1, 1))) return true; 
            }
        } else if (roomData.HasRoomAtPos(testPosition + new Vector2(0,-1))) {
            //Check left and right, if one is true, check corresponding diagnol, if both true, method is true
            //left room
            if (roomData.HasRoomAtPos(testPosition + new Vector2(-1, 0)))
            {
                //bottom left corner
                if (roomData.HasRoomAtPos(testPosition + new Vector2(-1, -1))) return true;
            }

            //right room
            if (roomData.HasRoomAtPos(testPosition + new Vector2(1, 0)))
            {
                //bottom right corner
                if (roomData.HasRoomAtPos(testPosition + new Vector2(1, -1))) return true;
            }
        }
        return false;
    }

    public static void AssignDoors(List<RoomData> roomData)
    {
        //Give connection from first generic room to boss room
        CardinalDir oppositeBossDir = Utilities.GetRelativeDir(roomData[0].cellData[0].openings[0], 2);
        foreach(CellData cell in roomData[1].cellData)
        {
            Vector2 offset = Utilities.CardinalDirToVector2(oppositeBossDir);

            //Grab the room in that direction
            RoomData room = roomData.RoomAtPos(cell.position + offset);

            //If the room exists
            if (room != null)
            {
                //add connection to boss room, and end loop
                if (room.roomType == RoomData.RoomType.Boss)
                {
                    cell.openings.Add(oppositeBossDir);
                    break;
                }
            }
        }

        //Skip boss room and iterate starting at the 2nd room
        for (int i = 1; i < roomData.Count; i++)
        {
            //For each cell in room
            foreach (CellData cell in roomData[i].cellData)
            {
                //Check each direction for a room
                for (int j = 0; j < 4; j++)
                {
                    //Find the offset for checked direction
                    CardinalDir currentDir = (CardinalDir) j;
                    Vector2 offset = Utilities.CardinalDirToVector2(currentDir);

                    //Grab the room in that direction
                    RoomData room = roomData.RoomAtPos(cell.position + offset);

                    //If the room exists
                    if (room != null)
                    {
                        //continue if it is a boss room
                        if (room.roomType == RoomData.RoomType.Boss) continue;

                        //Otherwise, Add a connection to this cell
                        cell.openings.Add(currentDir);
                    }
                }
            }
        }
    }

    

    public static RoomData RoomAtPos(this List<RoomData> roomData, Vector2 position)
    {
        foreach (RoomData data in roomData)
        {
            foreach(CellData cell in data.cellData)
            {
                if (cell.position == position) return data;
            }
        }

        return null;
    }

    public static bool HasRoomAtPos(this List<RoomData> roomData, Vector2 position)
    {
        return (roomData.RoomAtPos(position) != null);
    }

    public static Vector2[] GetBounds(this List<RoomData> roomData)
    {
        Vector2 minBound = new Vector2();
        Vector2 maxBound = new Vector2();

        //Skip bossroom
        for (int i = 1; i < roomData.Count; i++)
        {
            //For each cell
            foreach (CellData cell in roomData[i].cellData)
            {
                if (cell.position.x < minBound.x) minBound.x = cell.position.x;
                if (cell.position.x > maxBound.x) maxBound.x = cell.position.x;

                if (cell.position.y < minBound.y) minBound.y = cell.position.y;
                if (cell.position.y > maxBound.y) maxBound.y = cell.position.y;
            }
        }

        return new Vector2[] { minBound, maxBound };
    }

    private static void TrackData(FloorData data)
    {
        //Load file
        string genDataJSON = System.IO.File.ReadAllText(Application.persistentDataPath + "/FloorData.json");
        GenerationData genData = JsonConvert.DeserializeObject<GenerationData>(genDataJSON);

        //Add to object
        foreach(RoomData room in data.roomData)
        {
            genData.AddCount(room.RoomContentPool);
        }

        //Save file
        string genString = JsonConvert.SerializeObject(genData, Formatting.Indented);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/FloorData.json", genString);
    }
}
