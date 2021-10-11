using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
    public Image[] wallImages = new Image[4];
    public Image background;
    public Sprite wallSprite;
    public Sprite doorSprite;
    private CellData cellData;


    public void InstantiateCellUI(CellData cell)
    {
        cellData = cell;

        UpdateColor();

        //Change sprite to door if cell has opening (is a wall sprite by default)
        //Remove sprite if sibling is in that direction
        for (int i = 0; i < 4; i++)
        {
            CardinalDir currentDir = (CardinalDir)i;

            if (cellData.HasConnDir(currentDir)) wallImages[i].sprite = doorSprite;
            if (cellData.HasSibDir(currentDir)) wallImages[i].gameObject.SetActive(false);
        }
    }

    public void UpdateColor()
    {
        if (cellData.roomOwner.roomType == RoomData.RoomType.Boss) background.color = Color.red;
        if (cellData.cellObject.GetRoom().Completed) background.color = Color.green;
    }

    public Vector2 GetCellPos()
    {
        return cellData.position;
    }
}
