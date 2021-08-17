using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [Header("Components")]
    public ConnectionPoint connectionPoint;
    public ConnectionDir connectionDir;
    public GameObject wallCollider;
    public Transform spawnLocation;
    private Room connectedRoom;

    public void UpdateState() {
        //Keep track of current connectedRoom
        connectedRoom = connectionPoint.GetConnectedRoom();

        //Bring wall back if there is no connected room
        wallCollider.SetActive( (connectedRoom == null) );
    }
}
