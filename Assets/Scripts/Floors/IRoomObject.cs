using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IRoomObject
{
    /// <summary>
    /// Lets the object subscribe to their room contents join room event
    /// </summary>
    public void SubscribeToRoomContent(RoomContent roomContent);

    /// <summary>
    /// Will return false if the object believes the room to be unfinished, true if it has has been completed
    /// </summary>
    /// <returns>A boolean defining if the object has been dealt with or not</returns>
    public bool ContentPassed();
}

