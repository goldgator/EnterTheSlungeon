using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryObject : MonoBehaviour, IRoomObject
{
    public bool ContentPassed()
    {
        return true;
    }

    public void OnPlayerVictory()
    {
        Floor.Instance.OnFloorFinish();
    }

    public void SubscribeToRoomContent(RoomContent roomContent)
    {
        roomContent.roomFinishEvent += OnPlayerVictory;
    }
}
