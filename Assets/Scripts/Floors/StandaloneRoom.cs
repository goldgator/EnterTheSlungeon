using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandaloneRoom : MonoBehaviour
{
    [SerializeField]
    private Vector2 cameraRangeX;
    [SerializeField]
    private Vector2 cameraRangeY;

    [SerializeField]
    private Transform playerStart;

    private void OnDrawGizmosSelected()
    {
        float lengthX = (cameraRangeX.y - cameraRangeX.x);
        float lengthY = (cameraRangeY.y - cameraRangeY.x);

        float midX = (lengthX / 2) + cameraRangeX.x;
        float midY = (lengthY / 2) + cameraRangeY.x;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(midX, midY, 0), new Vector3(lengthX, lengthY, 1));
    }

    private void Start()
    {
        Player.Instance.transform.position = playerStart.position;
        PlayerCamera.Instance.TakeBounds(cameraRangeX, cameraRangeY);
    }
}
