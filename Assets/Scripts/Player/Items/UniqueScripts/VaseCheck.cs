using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaseCheck : BaseItem
{
    [SerializeField]
    GameObject vaseObject;

    public override void SetupItem()
    {
        base.SetupItem();

        //Add event to entryRoomEvent (new floor)
        Room.entryRoomEvent += OnVaseCheck;

        //Run it once for the first time
        OnVaseCheck(Floor.Instance.CurrentPlayerCell().GetRoom());
    }

    private void OnVaseCheck(Room room)
    {
        //Create vase Object at a specific location in room
        Vector3 spawnPosition = room.transform.position + new Vector3(10, 12.5f, 0);

        Instantiate(vaseObject, spawnPosition, Quaternion.identity);
    }

    
}
