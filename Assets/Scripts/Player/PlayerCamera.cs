using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public float maxDrift;
    public float driftSpeed = 2.0f;
    public bool lockedToBounds = true;
    private float screenShakeMagnitude = 0;
    private float shakeTime;

    private new Camera camera;
    private Transform cameraFocus;
    private Vector3 currentDrift = new Vector3();
    
    private float screenWidth;
    private float screenHeight;
    private Rect screenSize;

    private Vector2 xRange;
    private Vector2 yRange;




    public static PlayerCamera Instance { get; set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            return;
        }
    }



    private void InstantiateFocus()
    {
        if (cameraFocus == null) cameraFocus = Instantiate(new GameObject(), target.position, Quaternion.identity).transform;
        cameraFocus.localPosition = new Vector3(0, 0, -20);
        transform.position = cameraFocus.position;

        DontDestroyOnLoad(cameraFocus.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        GetScreenWorldSize();
        if (target == null) target = Player.Instance.transform;
        InstantiateFocus();
        GetNewBounds();
    }

    public void SetScreenShake(float screenShake, float newShakeTime = 1.0f)
    {
        shakeTime = newShakeTime;
        screenShakeMagnitude = screenShake;
    }

    private void GetScreenWorldSize()
    {
        Vector3 bottomLeft = camera.ViewportToWorldPoint(Vector2.zero);
        Vector3 topRight = camera.ViewportToWorldPoint(new Vector2(1,1));

        screenWidth = topRight.x - bottomLeft.x;
        screenHeight = topRight.y - bottomLeft.y;
    }

    // Update is called once per frame
    void Update()
    {
        AdjustFocus();
        FollowFocus();
        ApplyScreenShake();
    }

    private void ApplyScreenShake()
    {
        //Decrement shakeTime
        shakeTime -= Time.deltaTime;

        //Get random Vector2
        float randAngle = RNGManager.GetEventRand(0, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));

        //Get currentStrength
        float currentStrength = Mathf.Lerp(0, screenShakeMagnitude, shakeTime);

        Vector3 offset = new Vector3(direction.x * currentStrength, direction.y * currentStrength);
        transform.position += offset;
    }

    private void AdjustFocus()
    {

        //Get direction and target distance
        Vector3 worldMousePos = InputManager.Instance.MouseWorldPosition;
        Vector3 targetLocation = (worldMousePos - target.position);

        float targetDist = Mathf.Min(maxDrift, targetLocation.magnitude);
        targetLocation = targetDist * targetLocation.normalized;

        currentDrift = Vector3.Lerp(currentDrift, targetLocation, Time.deltaTime * driftSpeed);

        cameraFocus.position = currentDrift + target.position;
    }


    private void FollowFocus()
    {
        //Move to focus
        transform.position = new Vector3(cameraFocus.position.x, cameraFocus.position.y, -20);

        //Shift
        Vector2 position2D = new Vector2(transform.position.x, transform.position.y);

        if (lockedToBounds)
        {
            //adjust accordingly
            position2D.x = Mathf.Clamp(position2D.x, xRange.x, xRange.y);
            position2D.y = Mathf.Clamp(position2D.y, yRange.x, yRange.y);

            transform.position = new Vector3(position2D.x, position2D.y, -20);
        }
    }

    public void GetNewBounds()
    {
        if (lockedToBounds)
        {
            //Find current room
            Room currentRoom = Floor.Instance.CurrentPlayerCell().GetRoom();
            //Create room bounds (CURRENTLY ONLY WORKS WITH SINGLE CELL ROOMS)
            Vector2 bottomLeft = new Vector2(currentRoom.transform.position.x, currentRoom.transform.position.y);
            Vector2 size = currentRoom.GridSize * Floor.CELL_SIZE;

            xRange.x = bottomLeft.x + screenWidth / 2;
            xRange.y = bottomLeft.x + size.x - (screenWidth / 2);

            yRange.x = bottomLeft.y + screenHeight / 2;
            yRange.y = bottomLeft.y + size.y - (screenHeight / 2);
        }
    }
}
