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

        if (cellData.roomOwner.roomType == RoomData.RoomType.Boss) background.color = Color.red;

        //Change sprite to door if cell has opening (is a wall sprite by default)
        for (int i = 0; i < 4; i++)
        {
            CardinalDir currentDir = (CardinalDir)i;

            if (cellData.HasDir(currentDir))
            {
                wallImages[i].sprite = doorSprite;
            } 
        }
    }

    public Vector2 GetCellPos()
    {
        return cellData.position;
    }
}
