using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomContent : MonoBehaviour
{
    
    public Room parentRoom = null;

    public delegate void RoomEvent();
    public RoomEvent roomActivateEvent;
    public RoomEvent roomFinishEvent;

    private bool completed = false;
    public bool Completed { get { return completed; } }

    IRoomObject[] allRoomObjects;

    void Start()
    {
        FindAllSubscribers();
        parentRoom.roomEnterEvent += EnterRoom;
    }

    public void ForceComplete()
    {
        completed = true;
    }

    private IEnumerator CheckForCompletion()
    {
        bool completed = false;
        while (!completed)
        {
            //Check if all RoomObjects have been completed, leave the method if not
            completed = true;
            foreach (IRoomObject roomObject in allRoomObjects)
            {
                if (!roomObject.ContentPassed())
                {
                    completed = false;
                    break;
                }
            }

            if (!completed) yield return null;
        }

        //If not returned by now, room is completed, call RoomCompleted()
        //Debug.Log("Room Completed");
        RoomCompleted();
    }

    private void RoomCompleted()
    {
        completed = true;
        roomFinishEvent?.Invoke();
        parentRoom.UpdateRoomCompletion();
    }

    private void EnterRoom(Room room)
    {
        ActivateRoom();
    }

    public void ActivateRoom()
    {
        completed = false;
        roomActivateEvent?.Invoke();
        parentRoom.CloseAllDoors();
        StartCoroutine(CheckForCompletion());
    }

    void FindAllSubscribers()
    {
        allRoomObjects = GetComponentsInChildren<IRoomObject>();
        foreach(IRoomObject roomObject in allRoomObjects)
        {
            roomObject.SubscribeToRoomContent(this);
        }
    }
}
