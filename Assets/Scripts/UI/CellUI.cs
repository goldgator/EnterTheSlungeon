using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
    public Image[] wallImages = new Image[4];
    public Transform baseUI;
    public Image background;
    public Sprite wallSprite;
    public Sprite doorSprite;
    private CellData cellData;

    private string ICON_PATH = "Textures/Icons/";


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

        AddIcon();
    }

    public void ColorWalls(Color newColor)
    {
        foreach(Image image in wallImages)
        {
            image.color = newColor;
        }
    }

    private void AddIcon()
    {
        RoomData.RoomType type = cellData.roomOwner.roomType;

        Sprite icon = Resources.Load<Sprite>(ICON_PATH + type.ToString());

        if (icon == null) return;

        GameObject newObject = new GameObject();
        Image newImage = newObject.AddComponent<Image>();
        newObject.transform.SetParent(baseUI, false);

        newImage.sprite = icon;
        newImage.rectTransform.sizeDelta = new Vector2(60, 60);

        //Setting icon color
        switch (type)
        {
            case RoomData.RoomType.Boss:
                newImage.color = Color.blue;
                newImage.gameObject.name = "BossIcon";
                break;
            case RoomData.RoomType.Mine:
                newImage.color = Color.blue;
                newImage.gameObject.name = "MineIcon";
                break;
            case RoomData.RoomType.Item:
                newImage.color = Color.yellow;
                newImage.gameObject.name = "ItemIcon";
                break;
        }
    }


    public void UpdateIcon()
    {
        //Check if current cell is a mining room
        if (cellData.roomOwner.roomType == RoomData.RoomType.Mine)
        {
            UpdateMineIcon();
        }
    }

    private void UpdateMineIcon()
    {
        //Update Icon color
        Image icon = transform.Find("BaseUI/MineIcon").GetComponent<Image>();

        ResourceData currentResource = Floor.Instance.GetResource(cellData.position);
        //Magenta if no resource
        if (currentResource == null)
        {
            icon.color = Color.blue;
            //Quartz color if over a resource
        }
        else
        {
            icon.color = Quartz.GetQuartzColor(currentResource.resourceType);
        }
    }

    public void UpdateColor()
    {
        if (cellData.roomOwner.roomType == RoomData.RoomType.Boss) background.color = Color.red;
        //if (cellData.roomOwner.roomType == RoomData.RoomType.Item) background.color = Color.cyan;
        if (cellData.cellObject.GetRoom().Completed) background.color = Color.green;
    }

    public Vector2 GetCellPos()
    {
        return cellData.position;
    }
}
