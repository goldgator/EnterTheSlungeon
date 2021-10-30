using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSelect : MonoBehaviour
{
    public FloorUI floorUI;
    AudioSource audioSource;

    AudioClip roomMove;
    AudioClip roomBump;

    CellData selectedCell;
    CellUI selectedUI;

    private void OnEnable()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (roomMove == null) roomMove = Resources.Load<AudioClip>("Audio/RoomMove");
        if (roomBump == null) roomBump = Resources.Load<AudioClip>("Audio/RoomBump");

        //Get FloorUI from parent if doesn't exist (first creation)
        if (floorUI == null) floorUI = transform.parent.GetComponent<FloorUI>();

        //Attach to cell player is in
        selectedCell = Floor.Instance.CurrentPlayerCell().GetData();
        Vector2 playerPos = selectedCell.position;
        GameObject newParent = floorUI.GetCellUIAtPos(playerPos);
        transform.SetParent(newParent.transform, false);

        selectedUI = newParent.GetComponent<CellUI>();
        selectedUI.ColorWalls(Color.blue);

        //Subscribe to movement start
        if (InputManager.Instance) InputManager.Instance.moveStartEvent += OnMoveInp;
    }

    private void OnDisable()
    {
        //Unsubscribe from movement start
        selectedUI.ColorWalls(Color.white);
        if (InputManager.Instance) InputManager.Instance.moveStartEvent -= OnMoveInp;
    }
    

    public void SetFloorUI(FloorUI newFloorUI)
    {
        floorUI = newFloorUI;
    }

    private void OnMoveInp(Vector2 moveInp)
    {
        //Turn Vector into cardinal vector by finding strongest direction
        float xInp = moveInp.x;
        float yInp = moveInp.y;

        Vector2 moveDir;
        if (Mathf.Abs(xInp) > Mathf.Abs(yInp))
        {
            moveDir = new Vector2(xInp, 0).normalized;
        }
        else
        {
            moveDir = new Vector2(0, yInp).normalized;
        }

        //Don't continue if moveDir is 0,0 for some reason
        if (moveDir.magnitude < 1) return;

        if (InputManager.Instance.DodgeUpdate)
        {
            //Debug.Log("Attempting pull");
            OnPull(moveDir);
        } else
        {
            //Debug.Log("Attempting move");
            OnMove(moveDir);
        }
        selectedCell.cellObject.GetRoom().UpdateCells();
        PlayerCamera.Instance.GetNewBounds();
    }

    private void OnPull(Vector2 moveDir)
    {
        //Do for every Cell in selected room
        foreach (CellData cell in selectedCell.roomOwner.cellData)
        {
            //Check if room doesn't have correct opening
            if (!cell.HasConnDir(Utilities.Vector2ToCardinalDir(moveDir)))
            {
                //Play fail noise and leave method
                audioSource.clip = roomBump;
                audioSource.Play();
                continue;
            }

            //Find in direction
            Cell foundCell = Floor.Instance.FindCellInLine(cell.position, moveDir);
            if (foundCell == null)
            {
                audioSource.clip = roomBump;
                audioSource.Play();
                continue;
            }

            //Check if room can't be pulled
            if (!foundCell.GetRoom().CanBePulled)
            {
                //Play fail noise and leave method
                audioSource.clip = roomBump;
                audioSource.Play();
                continue;
            }

            //IF if it has the right opening
            CardinalDir wantedDir = Utilities.GetRelativeDir(Utilities.Vector2ToCardinalDir(moveDir), 2);
            if (foundCell.GetData().openings.Contains(wantedDir))
            {
                //Move it one by one until it stops towards this cell
                RoomData foundRoom = foundCell.GetData().roomOwner;
                while (Floor.Instance.RoomCanMoveInDirection(foundRoom, wantedDir))
                {
                    foundCell.GetRoom().MoveRoom(moveDir * -1, false);
                }

                audioSource.clip = roomMove;
                audioSource.Play();

                floorUI.UpdateUI();
            }
            else
            {
                audioSource.clip = roomBump;
                audioSource.Play();
            }
        }
    }


    private void OnMove(Vector2 moveDir)
    {
        //Check if you can even move current room
        if (!selectedCell.cellObject.GetRoom().CanMove)
        {
            //Play fail noise and leave method
            audioSource.clip = roomBump;
            audioSource.Play();
            return;
        }

        //Check if current room can move in that direction
        if (Floor.Instance.RoomCanMoveInDirection(selectedCell.roomOwner, Utilities.Vector2ToCardinalDir(moveDir)))
        {
            //Move room in direction
            selectedCell.cellObject.GetRoom().MoveRoom(moveDir, true);

            //Play success noise
            audioSource.clip = roomMove;
            audioSource.Play();

            //UpdateUI
            floorUI.UpdateUI();
            //StartCoroutine(floorUI.UpdateUITest());
        }
        else
        {
            //Play fail noise
            audioSource.clip = roomBump;
            audioSource.Play();
        }
    }
}
