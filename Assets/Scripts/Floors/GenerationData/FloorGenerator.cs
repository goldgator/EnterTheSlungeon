using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloorGenerator
{
    public enum FloorType
    {
        Expansive,
        Tight,
        ConstrainedExpansive
    }

    public static FloorData GenerateFloor(FloorType floorType)
    {
        switch (floorType)
        {
            case FloorType.Expansive:
                return ExpansiveFloor();
            case FloorType.Tight:
                return TightFloor();
            case FloorType.ConstrainedExpansive:
                return ConstrainedFloor();
        }

        return null;
    }

    public static FloorData ExpansiveFloor()
    {
        //Create base pattern
        List<RoomData> roomData = CreatePattern(5);

        //Assign doors to new pattern
        AssignDoors(roomData);

        //Create FloorData to hold RoomData and other details
        FloorData floorData = new FloorData(roomData, FloorType.Expansive);

        //Shuffle rooms in floor Data
        RoomData lastShuffledRoom = floorData.Shuffle(2, 4);
        //Set last shuffled room as the entry rooms
        lastShuffledRoom.roomType = RoomData.RoomType.Entry;

        //Sprinkle rooms


        //Assign Room Contents

        //Send to floor to instantiate
        return floorData;
    }

    public static FloorData TightFloor()
    {
        //Create base pattern
        List<RoomData> roomData = CreatePattern(5);

        //Assign doors to new pattern
        AssignDoors(roomData);

        //Create FloorData to hold RoomData and other details
        FloorData floorData = new FloorData(roomData, FloorType.Tight);

        //Shuffle rooms in floor Data
        RoomData lastShuffledRoom = floorData.Shuffle(2, 4);
        //Set last shuffled room as the entry rooms
        lastShuffledRoom.roomType = RoomData.RoomType.Entry;

        //Sprinkle rooms


        //Assign Room Contents

        //Send to floor to instantiate
        return floorData;
    }

    public static FloorData ConstrainedFloor()
    {
        return null;
    }

    /// <summary>
    /// Creates the basic puzzle layout for the floor
    /// </summary>
    /// <param name="cellSize">amount of cells created</param>
    /// <returns>A list of rooms unshuffled without doors assigned</returns>
    public static List<RoomData> CreatePattern(int cellSize)
    {
        //Create list of rooms to store
        List<RoomData> roomData = new List<RoomData>();

        //Create boss room at 0,0
        RoomData bossRoom = new RoomData(RoomData.RoomType.Boss, new Vector2());
        roomData.Add(bossRoom);

        //Add opening in random direction
        bossRoom.cellData[0].openings.Add((CardinalDir) Random.Range(0,4));

        //Add Generic room next to opening
        RoomData firstRoom = new RoomData(RoomData.RoomType.Generic, Utilities.CardinalDirToVector2(bossRoom.cellData[0].openings[0]));
        roomData.Add(firstRoom);

        //Choose random cell (excluding boss room) and place in a random open spot nearby
        //Until roomData.Count is cellSize + 1
        while (roomData.Count < (cellSize + 1))
        {
            TryPlaceRoom(roomData);
            //TODO implement big rooms
        }

        //Return populated RoomData List
        return roomData;
    }

    public static void TryPlaceRoom(List<RoomData> roomData)
    {
        //Create a Vector2 to hold a free space once found
        Vector2 tryPlace;
        do
        {
            //Select random room (not boss room)
            int randInd = Random.Range(1, roomData.Count);
            RoomData currentRoom = roomData[randInd];

            //Select a random cell and its position from that room
            int randCellInt = Random.Range(0,currentRoom.cellData.Count);
            Vector2 randCell = currentRoom.cellData[randCellInt].position;

            //Create a random offset from that cell
            Vector2 randomDir = Utilities.GetRandomCardinalVector();

            //Combine offset and cell position
            tryPlace = randCell + randomDir;

        //Loop if there is already a room with that position
        } while (roomData.HasRoomAtPos(tryPlace));

        //Add room at new found empty position.
        RoomData nextRoom = new RoomData(RoomData.RoomType.Generic, tryPlace);
        roomData.Add(nextRoom);
    }

    //TODO implement big rooms
    public static void TryPlaceBigRoom()
    {

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
}
