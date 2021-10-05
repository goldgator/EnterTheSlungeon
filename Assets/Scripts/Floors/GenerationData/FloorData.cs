using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorData
{
    public List<RoomData> roomData;
    public List<ResourceData> resourceData;
    public CellType[,] cells;
    public List<Vector2> originalSpots = new List<Vector2>();
    public FloorGenerator.FloorType floorType;

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
        while (FloorVolume < (roomData.Count * 6))
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
        while (FloorVolume < (roomData.Count * 2.5))
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
        }

        //Was able to move the full distance
        return true;
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

            //If nextRoom is null by this point, no more shuffling necessary
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
                        if (targetCell.HasDir(wantedDir)) continue;
                            
                        //Add direction to this cell
                        targetCell.openings.Add(wantedDir);

                        //Test if rooms can interact (remember doors facing out of bounds)
                        ConnectionData connectionData = ConnectionPoint(currentRoom, targetCell.roomOwner);
                        //IF not, move to new dir
                        if (connectionData == null)
                        {
                            targetCell.RemoveDir(wantedDir);
                            continue;
                        }

                        //Move to that room (based on connections)
                        bool success = AttemptConnectRooms(connectionData);

                        //Add that room as the next room on success
                        if (success) return targetRoom;

                        //remove failed dir
                        targetCell.RemoveDir(wantedDir);
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

        //If reached here, it is in the correct position
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
            if (adjacentRoom != null) return false;
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

    #endregion

    #region Sprinkle rooms
    public void AddSprinkleRooms(int amount)
    {
        //Find all spaces a sprinkle room can be placed
        List<Vector2> possibleSpots = new List<Vector2>();
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j] == CellType.Unvisited) possibleSpots.Add(new Vector2(i, j));
            }
        }

        //loop while there are spots in the list and haven't made enough rooms
        System.Random rng = new System.Random();
        int roomCount = 0;
        while (possibleSpots.Count > 0 && roomCount < amount)
        {
            //Choose random spot
            Vector2 spot = possibleSpots[RNGManager.GetWorldRand(0, possibleSpots.Count)];

            //Remove from list
            possibleSpots.Remove(spot);

            //Create a list of all possible openings
            List<CardinalDir> possibleOpenings = new List<CardinalDir>();
            for (int i = 0; i < 4; i++)
            {
                CardinalDir currentDir = (CardinalDir)i;

                if (IsValidPosition(spot + Utilities.CardinalDirToVector2(currentDir)))
                {
                    possibleOpenings.Add(currentDir);
                }
            }

            //If no openings, remove from list and continue loop
            if (possibleOpenings.Count <= 0) continue;

            //Shuffle list
            possibleOpenings.OrderBy(a => rng.Next());

            //Remove a random amount from [0->(count-1)]
            possibleOpenings.RemoveRange(0, RNGManager.GetWorldRand(0, possibleOpenings.Count));

            //Create a room
            RoomData room = new RoomData(RoomData.RoomType.Generic, spot);
            roomData.Add(room);

            //Add openings to its cell
            room.cellData[0].openings = possibleOpenings;
            roomCount++;
        }
    }
    #endregion

}