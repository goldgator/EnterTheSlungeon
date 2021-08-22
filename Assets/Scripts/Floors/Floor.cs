using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Floor : MonoBehaviour
{
    //[Header("FloorDetails")]
    public GameObject testPrefab;
    public GameObject testBorder;
    public GameObject visitedParent;
    public const float CELL_SIZE = 20;
    public Vector3 Offset
    {
        get
        {
            return new Vector3(CELL_SIZE / 2, CELL_SIZE / 2, 0);
        }
    }

    [Header("GenerationStats")]
    public FloorGenerator.FloorType floorType = FloorGenerator.FloorType.Expansive;

    FloorData generatedFloor;


    // Start is called before the first frame update
    void Start()
    {
        generatedFloor = FloorGenerator.GenerateFloor(floorType);
        InstantiateFloor();
        Debug.Log(generatedFloor.FloorSize);
    }

    private void Update()
    {
        //Just here to attach a debug point
        int blah = 0;

    }

    private void InstantiateFloor()
    {
        GameObject border = Instantiate(testBorder);
        Debug.Log(border);
        Debug.Log(border.transform);
        border.transform.localScale = (generatedFloor.roomMax + new Vector2(1, 1)) * CELL_SIZE;

        //Place visited cells
        for (int i = 0; i < generatedFloor.cells.GetLength(0); i++)
        {
            for (int j = 0; j < generatedFloor.cells.GetLength(1); j++)
            {
                if (generatedFloor.cells[i,j] == FloorData.CellType.Visited)
                {
                    Vector3 newPosition = (new Vector2(i, j) * CELL_SIZE);
                    newPosition += Offset;

                    GameObject testObject = Instantiate(testPrefab, newPosition, Quaternion.identity);
                    testObject.transform.SetParent(visitedParent.transform, false);
                    TMP_Text text = testObject.GetComponent<TMP_Text>();

                    text.text = "Visited";
                }
            }
        }

        for (int i = 0; i < generatedFloor.roomData.Count; i++)
        {
            foreach (CellData cell in generatedFloor.roomData[i].cellData)
            {
                Vector3 newPosition = (cell.position * CELL_SIZE);
                newPosition += Offset;

                GameObject testObject = Instantiate(testPrefab, newPosition, Quaternion.identity);
                TMP_Text text = testObject.GetComponent<TMP_Text>();

                text.text = cell.ToString() + "\nRoom #" + i;
            }
        }
    }
}
