
 public class ConnectionData
{
    public CellData thisCell;

    public CellData otherCell;
    public CardinalDir otherConnectionDir;

    public ConnectionData(CellData newThisCell, CellData newOtherCell, CardinalDir newConnectionDir)
    {
        thisCell = newThisCell;
        otherCell = newOtherCell;
        otherConnectionDir = newConnectionDir;
    }
}

