using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    public Transform spawnLocation;
    public GameObject doorCloseObject;
    public CardinalDir direction;
    public Cell parentCell;
    

    public void SetDoorState(bool state)
    {
        doorCloseObject.SetActive(state);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if player
        if (collision.CompareTag("Player"))
        {
            //Tell parent cell player is moving
            //Give direction
            //Debug.Log("Moving Player");
            parentCell.MovePlayer(direction);
        }
    }
}
