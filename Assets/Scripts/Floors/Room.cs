using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Components")]
    public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
    public List<Door> doors = new List<Door>();
    

    public Vector2 GridPosition
    {
        get
        {
            Vector3 offset = new Vector3(10, 10, 0);
            Vector3 pos = (transform.position - offset) / Floor.CELL_SIZE;

            return pos;
        }
    }

    public ConnectionPoint RandomConnectionPoint()
    {
        return connectionPoints[Random.Range(0, connectionPoints.Count)];
    }

    public bool ConnectionPossible(ConnectionDir direction)
    {
        foreach (ConnectionPoint point in connectionPoints)
        {
            if (point.direction == direction) return true;
        }
        return false;
    }

    public List<ConnectionPoint> ReturnOpenPoints(bool ignoreBounds)
    {
        List<ConnectionPoint> points = new List<ConnectionPoint>();

        foreach (ConnectionPoint point in connectionPoints)
        {
            Vector2 pos = point.GridPosition;
            if (!ignoreBounds)
            {
                if (pos.x < 0 || pos.x > Floor.Instance.floorSize.x) continue;
                if (pos.y < 0 || pos.y > Floor.Instance.floorSize.y) continue;
            }
            if (!Floor.Instance.IsGridPosEmpty(pos)) continue;

            //Add point if not out of bounds (if applicable) and is in a open spot
            points.Add(point);
        }

        return points;
    }

    public void UpdateState()
    {
        foreach(Door door in doors)
        {
            door.UpdateState();
        }
    }

}
