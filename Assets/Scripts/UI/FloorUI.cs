using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class FloorUI : MonoBehaviour
{
    
    private FloorData floorData;
    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;

    private List<CellUI> cellUIs = new List<CellUI>();
    private List<GameObject> blankCells = new List<GameObject>();
    private List<Image> patternImages = new List<Image>();

    // Start is called before the first frame update
    private void OnEnable()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (gridLayout == null) gridLayout = GetComponent<GridLayoutGroup>();
        //Adjust parent size
        RectTransform parent = ((RectTransform)transform.parent);
        parent.sizeDelta = new Vector2(Screen.width * .9f, Screen.height *.9f);
        if (floorData == null) {
            floorData = Floor.Instance.GetFloorData();
            InstantiateUI();
        };

        UpdateColor();
    }

    
    private void InstantiateUI()
    {
        FixScale();
        CreateCells();
        AddStaticImages();
    }

    private void FixScale()
    {
        //Change width and height of rectTransform based on floor size
        Vector2 floorSize = floorData.FloorSize * 100;
        floorSize.x += gridLayout.padding.left + gridLayout.padding.right;
        floorSize.y += gridLayout.padding.top + gridLayout.padding.bottom;
        rectTransform.sizeDelta = floorSize;

        //Change scale to maximize screenspace
        //Choose smallest scale between each axis, so it doesn't expand too much.
        RectTransform parentTransform = rectTransform.parent.GetComponent<RectTransform>();
        float xScale = parentTransform.sizeDelta.x / rectTransform.sizeDelta.x;
        float yScale = parentTransform.sizeDelta.y / rectTransform.sizeDelta.y;

        float newScale;
        if (xScale < yScale)
        {
            float targetSize = parentTransform.sizeDelta.x * .9f;
            newScale = targetSize / rectTransform.sizeDelta.x;
        }
        else
        {
            float targetSize = parentTransform.sizeDelta.y * .9f;
            newScale = targetSize / rectTransform.sizeDelta.y;
        }

        //Debug.Log(newScale);
        rectTransform.localScale = new Vector3(newScale, newScale, newScale);
    }

    private void CreateCells()
    {
        //Iterate through each cell from bottom left, starting on rows
        Vector2 floorSize = floorData.FloorSize;
        CellUI cellPrefab = Resources.Load<GameObject>("Prefabs/UI/CellUI").GetComponent<CellUI>();
        GameObject blankCellPrefab = Resources.Load<GameObject>("Prefabs/UI/BlankCellUI");

        for (int y = 0; y < floorSize.y; y++)
        {
            for (int x = 0; x < floorSize.x; x++)
            {
                //If position has cell
                CellData cell = floorData.CellDataAtPos(new Vector2(x, y));
                if (cell != null)
                {
                    //Place RoomUI
                    CellUI newCellUI = Instantiate(cellPrefab, rectTransform);
                    //Modify wall and door sprites depending on door openings
                    newCellUI.InstantiateCellUI(cell);
                    cellUIs.Add(newCellUI);
                }
                //ELSE
                else
                {
                    //Place blank roomUI
                    GameObject newBlankCell = Instantiate(blankCellPrefab, rectTransform);
                    blankCells.Add(newBlankCell);
                    newBlankCell.GetComponent<Image>().color = Color.gray;
                    
                }
            }
        }

        Instantiate(Resources.Load<GameObject>("Prefabs/UI/CellSelect"), transform);
    }

    private void AddStaticImages()
    {
        for (int i = 1; i < floorData.originalSpots.Count; i++)
        {
            //Create an image for each pattern pos in floorData
            Image newImage = new GameObject().AddComponent<Image>();
            Color newColor = new Color(1, 0, 0, .6f);
            newImage.color = newColor;
            patternImages.Add(newImage);

            newImage.transform.SetParent(GetCellUIAtPos(floorData.originalSpots[i]).transform, false);
        }
    }

    private void UpdateStaticImages()
    {
        for (int i = 1; i < floorData.originalSpots.Count; i++)
        {
            patternImages[i-1].transform.SetParent(GetCellUIAtPos(floorData.originalSpots[i]).transform, false);
        }
    }

    public GameObject GetCellUIAtPos(Vector2 position)
    {
        int childIndex = CellPosToChildIndex(position);
        return transform.GetChild(childIndex).gameObject;
    }

    public int CellPosToChildIndex(Vector2 position)
    {
        return (int)(position.x + (position.y * floorData.FloorSize.x));
    }

    public void UpdateColor()
    {
        foreach (CellUI cell in cellUIs)
        {
            cell.UpdateColor();
        }
    }

    public void UpdateUI()
    {
        //Order the cellUIs based on child index
        UpdateCellPos();
        UpdateStaticImages();
    }

    private void UpdateCellPos()
    {
        cellUIs = cellUIs.OrderBy(ctx => CellPosToChildIndex(ctx.GetCellPos())).ToList();

        int cellIndex = 0;
        int blankIndex = 0;

        for (int y = 0; y < floorData.FloorSize.y; y++)
        {
            for (int x = 0; x < floorData.FloorSize.x; x++)
            {
                Vector2 position = new Vector2(x, y);
                bool hasCell = floorData.HasRoomAtPos(position);

                if (hasCell)
                {
                    int childInd = CellPosToChildIndex(position);
                    cellUIs[cellIndex].transform.SetSiblingIndex(childInd);
                    cellIndex++;
                }
                else
                {
                    int childInd = CellPosToChildIndex(position);
                    blankCells[blankIndex].transform.SetSiblingIndex(childInd);

                    /*if (Floor.Instance.IsOriginalPosition(position))
                    {
                        blankCells[blankIndex].GetComponent<Image>().color = Color.red;
                    }
                    else
                    {
                        blankCells[blankIndex].GetComponent<Image>().color = Color.gray;
                    }*/

                    blankIndex++;
                }
            }
        }
    }



}
