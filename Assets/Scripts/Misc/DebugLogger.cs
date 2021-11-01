using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugLogger
{
    public static void WriteToFile(string filename, string message, bool concat = false)
    {
        //Load file
        try
        {
            if (concat)
            {
                message = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + filename) + message;
            }
        } catch (Exception e)
        {
            //Do nothing
        }


        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + filename, message);
    }
}
