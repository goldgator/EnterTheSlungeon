using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorData
{
    public List<RoomData> roomData;
    private List<RoomData> roomInteractOrder;
    public List<ResourceData> resourceData = new List<ResourceData>();
    public CellType[,] cells;
    public List<Vector2> originalSpots = new List<Vector2>();
    public FloorGenerator.FloorType floorType;
    private int sprinkleRoomsAdded = 0;

    public Vector2 roomMin;
    public Vector2 roomMax;
    public Vector2 FloorSize
    {
        get
        {
            return new Vector2(roomMax.x - roomMin.x, roomMax.y - roomMin.y) + new Vector2(1,1);
        }
    }

    public int FloorVolume
    {
        get
        {
            Vector2 roomSize = FloorSize;
            return (int) (roomSize.x * roomSize.y);
        }
    }

    public enum CellType
    {
        Unvisited,
        Visited,
        Invalid
    }

    public FloorData(List<RoomData> newRoomData, FloorGenerator.FloorType floorType)
    {
        roomData = newRoomData;
        Vector2[] bounds = roomData.GetBounds();
        roomMin = bounds[0];
        roomMax = bounds[1];

        switch (floorType)
        {
            case FloorGenerator.FloorType.Expansive:
                AdjustSizeExpansive();
                break;
            case FloorGenerator.FloorType.Tight:
                AdjustSizeTight();
                break;
            case FloorGenerator.FloorType.ConstrainedExpansive:
                AdjustSizeConstrainted();
                break;
            default:
                break;
        }

        int cellWidth = (int)roomMax.x;
        int cellHeight = (int)roomMax.y;

        cells = new CellType[cellWidth+1, cellHeight+1];
        UpdateRoomData();
    }
    private void UpdateRoomData()
    {
        foreach (RoomData room in roomData)
        {
            room.UpdateCellData();

            foreach (CellData cell in room.cellData)
            {
                cells[(int)cell.position.x, (int)cell.position.y] = CellType.Visited;
            }
        }
    }

    private void UpdateCellVisitState()
    {
        foreach (RoomData room in roomData)
        {
            foreach (CellData cell in room.cellData)
            {
                cells[(int)cell.position.x, (int)cell.position.y] = CellType.Visited;
            }
        }
    }

    #region Adjust Methods
    private void ExpandInDirection(CardinalDir direction, int amount)
    {
        Vector2 offset = Utilities.CardinalDirToVector2(direction);

        if (offset.x < 0 || offset.y < 0)
        {
            roomMin += offset * amount;
        } else
        {
            roomMax += offset * amount;
        }
    }
    
    private void AdjustSizeExpansive()
    {
        //Store old min and max bounds
        Vector2 roomMinBound = roomMin;
        Vector2 roomMaxBound = roomMax;

        //Get a random direction to buffer
        CardinalDir randDir1 = Utilities.GetRandomDir();

        //Get another random direction that isn't the previous
        CardinalDir randDir2;
        do
        {
            randDir2 = Utilities.GetRandomDir();
        } while (randDir2 == randDir1);

        //Expand each direction by 1
        ExpandInDirection(randDir1, 1);
        ExpandInDirection(randDir2, 1);

        //Obtain remaining directions
        CardinalDir randDir3 = Utilities.GetRelativeDir(randDir1, 1);
        while (randDir3 == randDir1 || randDir3 == randDir2) randDir3 = Utilities.GetRelativeDir(randDir3, 1);

        CardinalDir randDir4 = Utilities.GetRelativeDir(randDir2, 1);
        while (randDir4 == randDir1 || randDir4 == randDir2 || randDir4 == randDir3) randDir4 = Utilities.GetRelativeDir(randDir4, 1);

        //Expand each direction until volume is at least 6 times the starting room count
        while (FloorVolume < (roomData.Count * 5.5))
        {
            ExpandInDirection(randDir3, 1);

            if (FloorVolume >= (roomData.Count * 6)) break;

            ExpandInDirection(randDir4, 1);
        }

        //Adjust everything to the first quadrant (Add everything by the roomMin)
        ShiftAllRooms(-roomMin);
        roomMaxBound -= roomMin;
        roomMinBound -= roomMin;

        roomMax -= roomMin;
        roomMin -= roomMin;

        //Find the range the pattern can move within new bounds
        Vector2 xShuffle = new Vector2( roomMin.x - roomMinBound.x, roomMax.x - roomMaxBound.x );
        Vector2 yShuffle = new Vector2( roomMin.y - roomMinBound.y, roomMax.y - roomMaxBound.y );

        //Select random amount within each range to shift the pattern
        Vector2 shuffle = new Vector2(RNGManager.GetWorldRand((int)xShuffle.x, (int)xShuffle.y + 1), RNGManager.GetWorldRand((int)yShuffle.x, (int)yShuffle.y + 1));

        //Add that random amount to all rooms
        ShiftAllRooms(shuffle);
    }

    private void AdjustSizeTight()
    {
        //Store old min and max bounds
        Vector2 roomMinBound = roomMin;
        Vector2 roomMaxBound = roomMax;

        //Get a random direction to buffer
        CardinalDir randDir1 = Utilities.GetRandomDir();

        //Get another random direction that isn't the previous
        CardinalDir randDir2;
        do
        {
            randDir2 = Utilities.GetRandomDir();
        } while (randDir2 == randDir1);

        //Expand each direction until volume is at least 2.5 times the starting room count
        while (FloorVolume < (roomData.Count * 3))
        {
            ExpandInDirection(randDir1, 1);

            if (FloorVolume >= (roomData.Count * 2.5)) break;

            ExpandInDirection(randDir2, 1);
        }

        //Adjust everything to the first quadrant (Add everything by the roomMin)
        ShiftAllRooms(-roomMin);
        roomMaxBound -= roomMin;
        roomMinBound -= roomMin;

        roomMax -= roomMin;
        roomMin -= roomMin;

        //Find the range the pattern can move within new bounds
        Vector2 xShuffle = new Vector2(roomMin.x - roomMinBound.x, roomMax.x - roomMaxBound.x);
        Vector2 yShuffle = new Vector2(roomMin.y - roomMinBound.y, roomMax.y - roomMaxBound.y);

        //Select random amount within each range to shift the pattern
        Vector2 shuffle = new Vector2(RNGManager.GetWorldRand((int)xShuffle.x, (int)xShuffle.y + 1), RNGManager.GetWorldRand((int)yShuffle.x, (int)yShuffle.y + 1));

        //Add that random amount to all rooms
        ShiftAllRooms(shuffle);
    }

    private void AdjustSizeConstrainted()
    {
        //TODO implement
    }
    #endregion

    #region Basic Room manipulation
    private void ShiftAllRooms(Vector2 offset)
    {
        foreach (RoomData room in roomData)
        {
            foreach (CellData cell in room.cellData)
            {
                cell.position += offset;
            }
        }
    }

    private bool MoveRoom(RoomData room, Vector2 offset)
    {
        foreach (CellData cell in room.cellData)
        {
            //Check if offset is free
            Vector2 testPos = cell.position + offset;

            //Fail if future spot is out of bounds
            if (!IsInBounds(testPos)) return false;

            //Fail if there is a different room at that spot
            RoomData adjRoom = RoomAtPos(testPos);
            if (!(adjRoom == null || adjRoom == room)) return false;
        }

        //If reached this point, all cells are valid, Shift cells
        room.ShiftCells(offset);

        //Move in front of interact order
        int index = roomInteractOrder.FindIndex(ctx => ctx == room);
        roomInteractOrder.MoveIndexToFront(index);

        return true;
    }

    /// <summary>
    /// Attempts to move a room in one direction for a set distance
    /// </summary>
    /// <param name="room"></param>
    /// <param name="direction"></param>
    /// <param name="pushAmount"></param>
    /// <returns>If the room was able to move the full distance or not</returns>
    public bool TryMoveRoom(RoomData room, CardinalDir direction, int pushAmount)
    {
        //loop for amount of pushes
        for (int i = 0; i < pushAmount; i++)
        {
            //Test if next square(s) are open
            bool canMove = RoomCanMoveInDirection(room, direction);

            //IF can't move, break loop
            if (!canMove) return false;

            //adjust position to that square
            //Set new cell position to visited
            Vector2 offset = Utilities.CardinalDirToVector2(direction);
            foreach (CellData cell in room.cellData)
            {
                cell.position += offset;

                cells[(int)cell.position.x, (int)cell.position.y] = CellType.Visited;
            }

            //Move in front of interact order
            int index = roomInteractOrder.FindIndex(ctx => ctx == room);
            roomInteractOrder.MoveIndexToFront(index);
        }

        //Was able to move the full distance
        return true;
    }

    public void AssignRoomTypes()
    {
        //Assign item room (last single room thats not boss room in interact order)
        RoomData itemRoom = roomInteractOrder.FindLast(ctx => ctx.cellData.Count == 1 && ctx.roomType != RoomData.RoomType.Boss);
        itemRoom.roomType = RoomData.RoomType.Item;

        //Assign Entry room, (first room in interact order)
        if (roomInteractOrder[0].cellData.Count > 1)
        {
            //if first room is a big room, split the room, save the cell to find the old entry position
            Debug.Log("Split Big room");
            CellData startCell = roomInteractOrder[0].cellData[0];
            SplitBigRoom(roomInteractOrder[0]);
            startCell.roomOwner.roomType = RoomData.RoomType.Entry;
            RandomRetreat(startCell.roomOwner, 3);
        } else
        {
            roomInteractOrder[0].roomType = RoomData.RoomType.Entry;
        }


    }

    private void SplitBigRoom(RoomData bigRoom)
    {
        //Grab all of the old cells
        List<CellData> oldCells = new List<CellData>(bigRoom.cellData);

        //Remove bigroom from previous data
        int prevIndex1 = roomData.IndexOf(bigRoom);
        roomData.RemoveAt(prevIndex1);

        int prevIndex2 = roomInteractOrder.IndexOf(bigRoom);
        roomInteractOrder.RemoveAt(prevIndex2);

        //Create a room for each cell created
        foreach(CellData cell in oldCells)
        {
            //cell.openings.AddRange(cell.siblings);
            cell.siblings.Clear();
            RoomData newRoom = new RoomData(RoomData.RoomType.Generic, cell);
            roomData.Insert(prevIndex1, newRoom);
            roomInteractOrder.Insert(prevIndex2, newRoom);
        }
    }
    #endregion

    #region Shuffle
    /// <summary>
    /// Shuffles the given rooms using rules defined by player moveset
    /// </summary>
    /// <param name="maxVisits">Must be >0</param>
    /// <param name="maxPush"> Must be >0 </param>
    public RoomData Shuffle(int maxVisits, int maxPush)
    {
        //Keep track of room shuffle order
        roomInteractOrder = new List<RoomData>(roomData);

        //Select the first room (room connected to boss)
        RoomData currentRoom = roomData[1];

        //Keep track of previous room
        RoomData previousRoom = null;

        //Increment visited amount;
        currentRoom.visits++;

        List<RoomData> unshuffledRooms = new List<RoomData>();
        do
        {
            //Push all neighboring adjacent rooms
            PushAdjacentRooms(currentRoom, maxPush);

            //Find if a neighboring room wasn't able to be pushed
            RoomData nextRoom = FindNextAdjacentTarget(currentRoom, previousRoom, maxVisits);

            //Update unshuffled rooms
            unshuffledRooms = FindUnshuffledRooms();

            //move previous room to the back of the list
            if (previousRoom != null)
            {
                bool wasRemoved = unshuffledRooms.Remove(previousRoom);

                if (wasRemoved) unshuffledRooms.Add(previousRoom);
            }

            //if unshuffled rooms only has one and its the current room
            if (unshuffledRooms.Count == 1 && unshuffledRooms[0] == currentRoom)
            {
                //Randomly shift room
                RandomRetreat(currentRoom, 4);
                //Break loop
                break;
            }

            //IF nextRoom is null and there are still unshuffled rooms
            if (nextRoom == null && unshuffledRooms.Count > 0)
            {
                //Move room to a compatible unshuffled room
                nextRoom = FindNextTarget(currentRoom, unshuffledRooms, maxVisits);
                
                //IF no unshuffled rooms are compatible
                if (nextRoom == null)
                {
                    nextRoom = CreateNextTarget(currentRoom, unshuffledRooms, maxVisits);

                    //IF still null, try to find an adjacent target again, not caring if it was the previous room or not
                    if (nextRoom == null)
                    {
                        nextRoom = FindNextAdjacentTarget(currentRoom, null, maxVisits);
                    }
                }
            }

            //If nextRoom is null by this point, no more shuffling necessary or is impossible
            if (nextRoom == null) break;

            //Select nextRoom
            previousRoom = currentRoom;
            currentRoom = nextRoom;
            currentRoom.visits++;

            //Update unshuffled rooms
            unshuffledRooms = FindUnshuffledRooms();
            unshuffledRooms.Remove(previousRoom);
        //Loop if there is an unshuffled room
        } while (unshuffledRooms.Count > 0);

        //Return the last shuffled room
        return currentRoom;
    }

    /// <summary>
    /// Makes the room move to a random spot away from its current position
    /// </summary>
    /// <param name="room"></param>
    /// <param name="maxMoves"></param>
    private void RandomRetreat(RoomData room, int maxMoves)
    {
        //Get list of open directions
        List<CardinalDir> openDirs = new List<CardinalDir>();
        for (int i = 0; i < 4; i++)
        {
            CardinalDir testDir = (CardinalDir)i;

            //Check if each cell can move there
            bool canMove = RoomCanMoveInDirection(room, testDir);

            if (canMove) openDirs.Add(testDir);
        }

        //Return function if no directions are open
        if (openDirs.Count == 0) return;

        //Choose a random open direction, and set it to the original and currentDir
        CardinalDir originalDir = openDirs[RNGManager.GetWorldRand(0, openDirs.Count)];
        CardinalDir unwantedDir = Utilities.GetRelativeDir(originalDir, 2);
        CardinalDir currentDir = originalDir;
        

        //Successful moves
        for (int i = 0; i < maxMoves; i++)
        {
            //Move room in direction
            TryMoveRoom(room, currentDir, 1);

            //Get new dir
            openDirs.Clear();
            for (int j = 0; j < 4; j++)
            {
                CardinalDir testDir = (CardinalDir)j;
                if (testDir == unwantedDir) continue;

                //Check if each cell can move there
                bool canMove = RoomCanMoveInDirection(room, testDir);

                if (canMove) openDirs.Add(testDir);
            }

            //Return function if no directions are open
            if (openDirs.Count == 0) return;

            //Assign new currentDir
            currentDir = openDirs[RNGManager.GetWorldRand(0, openDirs.Count)];
        }
    }

    private RoomData CreateNextTarget(RoomData currentRoom, List<RoomData> unshuffledRooms, int maxVisits)
    {
        foreach (RoomData targetRoom in unshuffledRooms)
        {
            if (targetRoom == currentRoom || targetRoom.visits >= maxVisits) continue;
            foreach(CellData targetCell in targetRoom.cellData)
            {
                foreach(CellData currentCell in currentRoom.cellData)
                {
                    //Get all wanted directions
                    List<CardinalDir> wantedDirs = new List<CardinalDir>();
                    foreach (CardinalDir dir in currentCell.openings) wantedDirs.Add(Utilities.GetRelativeDir(dir, 2));

                    //Check for compatibility for each direction
                    foreach (CardinalDir wantedDir in wantedDirs)
                    {
                        //skip this dir if cell already has it
                        //If it failed for FindNextTarget, it would fail here too
                        if (targetCell.HasConnDir(wantedDir)) continue;
                            
                        //Add direction to this cell
                        targetCell.openings.Add(wantedDir);

                        //Test if rooms can interact (remember doors facing out of bounds)
                        ConnectionData connectionData = ConnectionPoint(currentRoom, targetCell.roomOwner);
                        //IF not, move to new dir
                        if (connectionData == null)
                        {
                            targetCell.RemoveConnDir(wantedDir);
                            continue;
                        }

                        //Move to that room (based on connections)
                        bool success = AttemptConnectRooms(connectionData);

                        //Add that room as the next room on success
                        if (success) return targetRoom;

                        //remove failed dir
                        targetCell.RemoveConnDir(wantedDir);
                    }
                }
            }
        }

        //If all loops fail
        return null;
    }

    private RoomData FindNextTarget(RoomData currentRoom, List<RoomData> unshuffledRooms, int maxVisits)
    {
        //Find an unshuffled room
        RoomData nextRoom = null;

        foreach (RoomData testRoom in unshuffledRooms)
        {
            //Might not be needed???
            if (testRoom == currentRoom)
            {
                if (unshuffledRooms.Count > 1) continue;
                //Choose random room to set to next room
                do
                {
                    nextRoom = roomData[RNGManager.GetWorldRand(0, roomData.Count)];
                } while (nextRoom == currentRoom);
                break;
            }

            if (testRoom.visits >= maxVisits) continue;

            //Test if rooms can interact (remember doors facing out of bounds)
            ConnectionData connectionData = ConnectionPoint(currentRoom, testRoom);
            //IF not, find a different unshuffled room
            if (connectionData == null) continue;

            //Move to that room (based on connections)
            bool success = AttemptConnectRooms(connectionData);
            //Add that room as the next room on success
            if (success) return nextRoom;
        }
        return null;
    }

    private RoomData FindNextAdjacentTarget(RoomData currentRoom, RoomData previousRoom, int maxVisits)
    {
        //Attempt to find connected room that wasn't pushed
        RoomData nextRoom = null;
        List<RoomData> allNeighbors = new List<RoomData>();
        foreach (CellData cell in currentRoom.cellData)
        {
            foreach (CardinalDir currentDir in cell.openings)
            {
                nextRoom = FindAdjacentRoom(cell, currentDir);
                if (nextRoom == null) continue;

                //if room doesn't have opposite opening, skip room
                Vector2 adjacentCellPos = cell.position + Utilities.CardinalDirToVector2(currentDir);
                CardinalDir oppositeDir = Utilities.GetRelativeDir(currentDir, 2);
                if (!nextRoom.HasOpening(adjacentCellPos, oppositeDir)) continue;

                //Skip room if boss room
                if (nextRoom.roomType == RoomData.RoomType.Boss) continue;

                //Skip room if previous room
                if (nextRoom == previousRoom) continue;

                //Skip room if already visited too much
                if (nextRoom.visits >= maxVisits) continue;

                allNeighbors.Add(nextRoom);
            }
        }

        if (allNeighbors.Count == 0) return null;

        allNeighbors.OrderBy(ctx => ctx.visits);
        return allNeighbors[0];
    }

    private void PushAdjacentRooms(RoomData currentRoom, int maxPush)
    {
        //Iterate through each cell, and push connected neighbors
        foreach (CellData cell in currentRoom.cellData)
        {
            //iterate through each door direction and attempt to push each adjacent connected room
            foreach (CardinalDir currentDir in cell.openings)
            {
                //find adjacent room
                RoomData adjacentRoom = FindAdjacentRoom(cell, currentDir);

                //if the room doesn't exist or is the boss room, move to next direction
                if (adjacentRoom == null) continue;
                if (adjacentRoom.roomType == RoomData.RoomType.Boss) continue;

                //if room doesn't have opposite opening, skip room
                Vector2 adjacentCellPos = cell.position + Utilities.CardinalDirToVector2(currentDir);
                CardinalDir oppositeDir = Utilities.GetRelativeDir(currentDir, 2);
                if (!adjacentRoom.HasOpening(adjacentCellPos, oppositeDir)) continue;

                //TryMove found room
                int randPush = RNGManager.GetWorldRand(1, maxPush);
                TryMoveRoom(adjacentRoom, currentDir, randPush);
            }
        }
    }

    /// <summary>
    /// Will try to organically connect two rooms together via these two cells taken from ConnectionPoint()
    /// </summary>
    /// <param name="connections">The two cells that need to be connected</param>
    /// <returns>If the room was successfully moved towards or not</returns>
    public bool AttemptConnectRooms(ConnectionData connectionData)
    {
        CellData thisCell = connectionData.thisCell;

        //Determine finishing location
        Vector2 targetPos = connectionData.otherCell.position + Utilities.CardinalDirToVector2(connectionData.otherConnectionDir);

        //Early escape just in case of rare situations
        if (thisCell.position == targetPos) return true;

        //Create a list of original positions for each cell of the first room.
        List<Vector2> originalCellPos = new List<Vector2>();
        foreach (CellData cell in thisCell.roomOwner.cellData)
        {
            originalCellPos.Add(cell.position);
        }

        //Create a list of coordinates that will be changed to visited if movement succeeds
        List<Vector2> newVisitedCells = new List<Vector2>();

        //Create a Queue of movements
        Queue<Vector2> movements = new Queue<Vector2>();

        //Add first movement
        movements.Enqueue(QueryMovement(thisCell, targetPos));

        //Loop until first cell is at desired location or failed to reach
        int queryAttempts = 3;
        while (thisCell.position != targetPos)
        {
            Vector2 movement = movements.Dequeue();

            //Attempt to move room and update visited cells
            bool success = MoveRoom(thisCell.roomOwner, movement);

            if (success)
            {
                //Update visited cells
                AddVisitedCells(thisCell.roomOwner, newVisitedCells);
                //Add basic movement if none more available
                if (movements.Count == 0) movements.Enqueue(QueryMovement(thisCell, targetPos));
            }
            else
            {
                //Query a detour if something blocks basic movement
                List<Vector2> newMovements = QueryDetour(thisCell, Utilities.Vector2ToCardinalDir(movement));
                queryAttempts--;

                //If query failed and/or queries ran out (movement failed)
                if (newMovements == null || queryAttempts < 0)
                {
                    //Failed to reach reset cell position and return false;
                    for (int i = 0; i < thisCell.roomOwner.cellData.Count; i++)
                    {
                        thisCell.roomOwner.cellData[i].position = originalCellPos[i];
                    }

                    return false;
                }

                //query was successful
                foreach (Vector2 newMovement in newMovements)
                {
                    movements.Enqueue(newMovement);
                }
            }
        }

        //If reached here, it is in the correct position, update visited cells
        foreach (Vector2 visitedCell in newVisitedCells)
        {
            cells[(int)visitedCell.x, (int)visitedCell.y] = CellType.Visited;
        }

        return true;
    }

    private Vector2 QueryMovement(CellData cell, Vector2 targetPos)
    {
        //Determine x Direction
        int xDirection = (int)(targetPos.x - cell.position.x);
        if (xDirection != 0)
        {
            xDirection = (int)Mathf.Sign(xDirection);
        }
        //Determine y Direction
        int yDirection = (int)(targetPos.y - cell.position.y);
        if (yDirection != 0)
        {
            yDirection = (int)Mathf.Sign(yDirection);
        }

        //if xDirection != 0
        bool xValid = false;
        if (xDirection != 0)
        {
            //Check next cellState
            int xPos = (int)cell.position.x + xDirection;
            //Check for out of bounds
            if (!(xPos < 0 || xPos >= cells.GetLength(0)))
            {
                CellType type = cells[xPos, (int)cell.position.y];
                if (type == CellType.Visited) return new Vector2(xDirection, 0);

                xValid = (type == CellType.Unvisited);
            }
        }

        if (yDirection != 0)
        {
            //Check next cellState
            int yPos = (int)cell.position.y + yDirection;
            //Check for out of bounds
            if (!(yPos < 0 || yPos >= cells.GetLength(1)))
            {
                CellType type = cells[(int)cell.position.x, yPos];
                if (type == CellType.Visited) return new Vector2(0, yDirection);
            }
        }

        if (xValid)
        {
            return new Vector2(xDirection, 0);
        }
        else
        {
            return new Vector2(0, yDirection);
        }
    }
    private List<Vector2> QueryDetour(CellData cell, CardinalDir attemptedDir)
    {
        //Create list to hold movements
        List<Vector2> movements = new List<Vector2>();

        //Test clockwise direction first
        CardinalDir testDir = Utilities.GetRelativeDir(attemptedDir, 1);

        //Loop until not conflicting
        bool isSafe = true;
        bool swapped = false;
        int testSteps = 0;
        do
        {
            testSteps++;
            Vector2 testVector = Utilities.CardinalDirToVector2(testDir) * testSteps;

            //Test if out of bounds
            if (!IsInBounds(cell.position + testVector))
            {
                //If already tested both directions, query failed
                if (swapped) return null;

                //swap directions
                swapped = true;
                testDir = Utilities.GetRelativeDir(testDir, 2);
                testSteps = 0;
                continue;
            }

            //Test if new place will conflict
            bool test1 = TestConfliction(cell, cell.position + testVector);
            bool test2 = TestConfliction(cell, cell.position + testVector + Utilities.CardinalDirToVector2(attemptedDir));

            isSafe = (test1 && test2);
        } while (!isSafe);

        //Add movements 
        for (int i = 0; i < testSteps; i++)
        {
            movements.Add(Utilities.CardinalDirToVector2(testDir));
        }
        movements.Add(Utilities.CardinalDirToVector2(attemptedDir));

        //return movements
        return movements;
    }

    private void AddVisitedCells(RoomData room, List<Vector2> newVisitedCells)
    {
        foreach (CellData cell in room.cellData)
        {
            if (!newVisitedCells.Contains(cell.position))
            {
                newVisitedCells.Add(cell.position);
            }
        }
    }
    #endregion

    #region Query Methods

    public bool RoomCanMoveInDirection(RoomData room, CardinalDir direction)
    {
        foreach(CellData cell in room.cellData)
        {
            //See if location is out of bounds
            if (!IsInBounds(cell.position + Utilities.CardinalDirToVector2(direction))) return false;
            //Attempt to grab room in direction
            RoomData adjacentRoom = FindAdjacentRoom(cell, direction);
            //IF room isn't null AND isn't same as current room
            if (adjacentRoom != null && adjacentRoom != room) return false;
        }

        return true;
    }

    public bool IsInBounds(Vector2 position)
    {
        if (position.x < roomMin.x || position.x > roomMax.x) return false;
        if (position.y < roomMin.y || position.y > roomMax.y) return false;
        return true;
    }

    /// <summary>
    /// Determine if two rooms can connect, and give the cells that can.
    /// </summary>
    /// <param name="otherRoom"></param>
    /// <returns>2 element array, 1st element is the cell from this room that'll connect, 2nd element is the cell from the other room that can connect</returns>
    public ConnectionData ConnectionPoint(RoomData thisRoom, RoomData otherRoom)
    {
        //For each cell
        foreach (CellData cell in thisRoom.cellData)
        {
            //For each connection in this cell
            foreach(CardinalDir direction in cell.openings)
            {
                //Check if other room has the opposite connection
                foreach (CellData otherCell in otherRoom.cellData)
                {
                    //Find opposite direction and check for connection
                    CardinalDir oppositeDir = Utilities.GetRelativeDir(direction, 2);

                    //check for opening, if it doesn't exist, move to next cell
                    bool hasOpening = otherRoom.HasOpening(otherCell.position, oppositeDir);
                    if (!hasOpening) continue;

                    //Check if this room can be placed near that opening
                    Vector2 newCellPos = otherCell.position + Utilities.CardinalDirToVector2(oppositeDir);

                    //If any of the potential cell positions conflict with something existing move onto the next cell
                    bool safeSpot = TestConfliction(cell, newCellPos);

                    if (!safeSpot) continue;

                    //If reached this point, these two rooms can connect with these cells, return them both
                    return new ConnectionData(cell, otherCell, oppositeDir);
                }
            }
        }

        //If reached this point, rooms cannot connect, return null
        return null;
    }

    /// <summary>
    /// Returns true if a room could be placed
    /// </summary>
    /// <param name="cellData"></param>
    /// <param name="newCellPos"></param>
    /// <returns></returns>
    public bool TestConfliction(CellData cellData, Vector2 newCellPos)
    {
        Vector2 movement = newCellPos - cellData.position;
        foreach (CellData testCell in cellData.roomOwner.cellData)
        {
             if (!IsValidPosition(testCell.position + movement))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Finds all rooms (excluding boss room) that are still in their original position
    /// </summary>
    /// <returns>All rooms that are unshuffled, sorted by visits</returns>
    private List<RoomData> FindUnshuffledRooms()
    {
        List<RoomData> unshuffledRooms = new List<RoomData>();
        foreach (RoomData room in roomData)
        {
            if (room.roomType == RoomData.RoomType.Boss) continue;
            foreach (CellData cell in room.cellData)
            {
                if (cell.position == cell.startPosition) {
                    
                    unshuffledRooms.Add(room);
                    break;
                }
            }
        }

        unshuffledRooms.OrderBy(ctx => ctx.visits);
        return unshuffledRooms;
    }
    public RoomData RoomAtPos(Vector2 position)
    {
        return roomData.RoomAtPos(position);
    }

    public CellData CellDataAtPos(Vector2 position)
    {
        foreach (RoomData data in roomData)
        {
            foreach (CellData cell in data.cellData)
            {
                if (cell.position == position) return cell;
            }
        }
        return null;
    }

    public bool HasRoomAtPos(Vector2 position)
    {
        return (roomData.RoomAtPos(position) != null);
    }

    public int UnshuffledRooms()
    {
        int count = 0;
        foreach(RoomData room in roomData)
        {
            if (!room.Shuffled) count++;
        }

        return count;
    }

    public bool IsValidPosition(Vector2 position)
    {
        if (HasRoomAtPos(position)) return false;
        if (!IsInBounds(position)) return false;

        return true;
    }

    public RoomData FindAdjacentRoom(CellData cell, CardinalDir direction)
    {
        Vector2 offset = Utilities.CardinalDirToVector2(direction);

        RoomData adjacentRoom = RoomAtPos(offset + cell.position);

        //return null if the room is the same as the cell owner
        if (adjacentRoom == cell.roomOwner) return null;

        //if not, return whatever it grabbed
        return adjacentRoom;
    }

    public void UpdateOriginalPositions()
    {
        foreach (RoomData room in roomData)
        {
            foreach (CellData cell in room.cellData)
            {
                originalSpots.Add(cell.position);
            }
        }
    }

    public bool CheckCellState(CellType cellType, Vector2 checkPosition)
    {
        //Check if out of range, if so return false
        if (checkPosition.x < 0 || checkPosition.x >= FloorSize.x) return false;
        if (checkPosition.y < 0 || checkPosition.y >= FloorSize.y) return false;

        //Check if cell is same as passed cellType
        return (cells[(int)checkPosition.x, (int)checkPosition.y] == cellType); 
    }

    #endregion

    #region Sprinkle rooms
    
    public void AddSprinkleRooms(int amount)
    {
        //Grab all rooms besides boss room
        List<RoomData> roomLoop = new List<RoomData>();
        roomLoop = roomData.Skip(1).ToList();

        //shuffle rooms
        roomLoop.OrderBy(ctx => RNGManager.GetWorldRand());

        //Try to add a shuffle room to each room one at a time
        int roomsAdded = 0;
        List<CardinalDir> dirs;
        foreach (RoomData room in roomLoop)
        {
            if (roomsAdded >= amount) break;
            //Do for each cell
            foreach (CellData cell in room.cellData)
            {
                //Copy openings
                dirs = new List<CardinalDir>(cell.openings);

                //shuffle openings
                dirs.OrderBy(ctx => RNGManager.GetWorldRand());


                //Try to add shuffle room in a direction
                List<Vector2> goodPositions = new List<Vector2>();
                CardinalDir goodDir = CardinalDir.North;
                foreach(CardinalDir dir in dirs)
                {
                    goodDir = dir;
                    Vector2 checkPos = cell.position;
                    Vector2 checkVec = Utilities.CardinalDirToVector2(dir);

                    do
                    {
                        checkPos += checkVec;

                        if (checkPos.x < 0 || checkPos.x >= FloorSize.x) break;
                        if (checkPos.y < 0 || checkPos.y >= FloorSize.y) break;

                        //Check if unvisited
                        if (CheckCellState(CellType.Unvisited, checkPos))
                        {
                            //Add spot to sprinkle room list
                            goodPositions.Add(checkPos);
                        }

                    } while (!HasRoomAtPos(checkPos));

                    if (goodPositions.Count > 0) break;
                }

                //if found good positions, choose a random one out of the list
                if (goodPositions.Count == 0) continue;
                Vector2 position = goodPositions[RNGManager.GetWorldRand(0, goodPositions.Count)];

                //Place shuffle room with corresponding dir, and potentially other random ones
                RoomData newRoom = new RoomData(RoomData.RoomType.Generic, position);

                CellData newCell = newRoom.cellData[0];
                newCell.openings.Add(Utilities.GetRelativeDir(goodDir, 2));

                //Try to add every opening with a 50% chance
                for (int i = 0; i < 4; i++)
                {
                    CardinalDir newDir = (CardinalDir)i;
                    bool doesAdd = RNGManager.GetWorldRand(0, 2) == 1;
                    if (doesAdd && !newCell.HasConnDir(newDir)) newCell.openings.Add(newDir);
                }

                roomData.Add(newRoom);
                roomsAdded++;
                break;
            }
        }

        sprinkleRoomsAdded = roomsAdded;
        UpdateCellVisitState();
    }
    #endregion

    #region Misc
    public void AddMineRoom()
    {
        //Get a list of all successfully added sprinkle rooms (back of the roomData list)
        List<RoomData> sprinkleRooms = roomData.GetRange(roomData.Count - sprinkleRoomsAdded, sprinkleRoomsAdded);
        if (sprinkleRooms.Count == 0) {
            Debug.LogError("Failed to add Mine room");
            return;
        };

        RoomData mineRoom = sprinkleRooms[0];
        List<Vector2> connectedCells = CountConnectedCells(CellType.Unvisited, mineRoom.cellData[0].position, 3);
        for (int i = 1; i < sprinkleRooms.Count; i++)
        {
            RoomData otherRoom = sprinkleRooms[i];
            List<Vector2> otherCells = CountConnectedCells(CellType.Unvisited, otherRoom.cellData[0].position, 3);

            //Update mineRoom and connected cells if new room has more space
            if (otherCells.Count > connectedCells.Count)
            {
                mineRoom = otherRoom;
                connectedCells = otherCells;
            }
        }

        mineRoom.roomType = RoomData.RoomType.Mine;

        AddResources(connectedCells);
    }

    private void AddResources(List<Vector2> places)
    {
        if (places.Count < 2)
        {
            //Debug.LogError("No space for resources");
            return;
        }

        ResourceType firstType = (ResourceType)RNGManager.GetWorldRand(0, 3);
        ResourceType secondType = (ResourceType)RNGManager.GetWorldRand(0, 3);
        while (secondType == firstType) secondType = (ResourceType)RNGManager.GetWorldRand(0, 3);

        //Assign random spot to each
        Vector2 firstSpot = places[RNGManager.GetWorldRand(0, places.Count)];
        places.Remove(firstSpot);

        Vector2 secondSpot = places[RNGManager.GetWorldRand(0, places.Count)];

        resourceData.Add(new ResourceData(firstType, firstSpot));
        resourceData.Add(new ResourceData(secondType, secondSpot));
    }

    private List<Vector2> CountConnectedCells(CellType cellType, Vector2 startPos, int stepCount)
    {
        List<Vector2> countedCells = new List<Vector2>();
        int count = 0;
        
        for (int i = 0; i < 4; i++)
        {
            CardinalDir dir = (CardinalDir)i;
            Vector2 newPos = startPos + Utilities.CardinalDirToVector2(dir);
            CountConnectedCells(cellType, newPos, stepCount, 0, countedCells);
        }

        return countedCells;
    }

    private void CountConnectedCells(CellType cellType, Vector2 currentPos, int stepCount, int currentStep, List<Vector2> countedCells)
    {
        //increment step, and return 0 if over stepCount
        currentStep++;
        if (currentStep > stepCount) return;

        //Check cellState
        bool state = CheckCellState(cellType, currentPos);

        //if true, call method in every dir and add up totals
        if (state)
        {
            countedCells.Add(currentPos);
            for (int i = 0; i < 4; i++)
            {
                CardinalDir dir = (CardinalDir)i;
                Vector2 newPos = currentPos + Utilities.CardinalDirToVector2(dir);
                //continue of position is in counted cells
                if (countedCells.Contains(newPos)) continue;

                CountConnectedCells(cellType, newPos, stepCount, currentStep, countedCells);
            }

            return;
        } else
        {
            return;
        }

        
    }
    #endregion

}