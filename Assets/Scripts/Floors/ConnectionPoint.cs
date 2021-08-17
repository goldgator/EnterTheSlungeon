using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    public ConnectionDir direction;
    

    public Vector2 GridPosition
    {
        get
        {
            Vector3 offset = new Vector3(10, 10, 0);
            Vector3 pos = (transform.position - offset) / Floor.CELL_SIZE;

            return pos;
        }
    }

    public static string[] RoomTypesFromDirection(ConnectionDir direction, int openings)
    {

        if (openings > 1)
        {
            switch (direction)
            {
                case ConnectionDir.UP:
                    return new string[] { "D", "DL", "DR", "UD" };
                case ConnectionDir.RIGHT:
                    return new string[] { "L", "DL", "LR", "UL" };
                case ConnectionDir.DOWN:
                    return new string[] { "U", "UL", "UR", "UD" };
                case ConnectionDir.LEFT:
                    return new string[] { "R", "DR", "LR", "UR" };
                default:
                    return null;
            }
        }else
        {
            switch (direction)
            {
                case ConnectionDir.UP:
                    return new string[] { "DL", "DR", "UD" };
                case ConnectionDir.RIGHT:
                    return new string[] { "DL", "LR", "UL" };
                case ConnectionDir.DOWN:
                    return new string[] { "UL", "UR", "UD" };
                case ConnectionDir.LEFT:
                    return new string[] { "DR", "LR", "UR" };
                default:
                    return null;
            }
        }
        
    }

    public Room GetConnectedRoom()
    {
        Room room = Floor.Instance.GetRoomAtGridPos(GridPosition);
        
        if (room)
        {
            //Find the index opposite of this point's direction
            int dirIndex = (((int)direction) +2) % 4;

            ConnectionDir testDir = (ConnectionDir) dirIndex;

            //return room if the other room can connect to this
            if (room.ConnectionPossible(testDir)) return room;

            //return null otherwise
            return null;
        } else
        {
            //return null if no room present
            return null;
        }

    }
}
