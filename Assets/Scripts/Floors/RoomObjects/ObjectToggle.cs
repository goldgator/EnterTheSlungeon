using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectToggle : MonoBehaviour, IRoomObject
{
    public List<GameObject> enterToggle;
    public List<GameObject> finishToggle;

    public bool ContentPassed()
    {
        return true;
    }

    public void SubscribeToRoomContent(RoomContent roomContent)
    {
        roomContent.roomActivateEvent += ActivateToggle;
        roomContent.roomFinishEvent += FinishToggle;
    }

    private void ActivateToggle()
    {
        Toggle(enterToggle);
    }

    private void FinishToggle()
    {
        Toggle(finishToggle);
    }

    private void Toggle(List<GameObject> objectsToggle)
    {
        foreach (GameObject gameObject in objectsToggle)
        {
            bool toggle = !gameObject.activeSelf;
            gameObject.SetActive(toggle);
        }
    }

}
