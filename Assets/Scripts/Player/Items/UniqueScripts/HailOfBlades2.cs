using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HailOfBlades2 : PassiveItem
{
    [Header("Hail of Blades")]
    [SerializeField]
    private GameObject bladeObject;
    
    [SerializeField]
    private float minRotationSpeed = 120f;
    [SerializeField]
    private float maxRotationSpeed = 360f;

    [SerializeField]
    private float minDistance = 1.3f;
    [SerializeField]
    private float maxDistance = 2.5f;

    [SerializeField]
    private float speedUpTime = 1.0f;
    private float timer;

    GameObject[] blades = new GameObject[3];

    public override void SetupItem()
    {
        base.SetupItem();

        //Create blade objects
        SetupBlades();

        //Subscribe to player movement event
        Player.playerMoveEvent += OnPlayerMove;
    }

    private void OnPlayerMove(Vector2 movement)
    {
        if (movement.magnitude <= .1f)
        {
            //Reduce timer
            timer -= Time.deltaTime;
        } else
        {
            //Increase timer
            timer += Time.deltaTime;
        }

        //Clamp timer between 0 and total speedUpTime
        timer = Mathf.Clamp(timer, 0, speedUpTime);
    }

    private void SetupBlades()
    {
        //Create 3 Sword Objects
        for (int i = 0; i < 3; i++) {
            blades[i] = Instantiate(bladeObject);
            blades[i].transform.SetParent(transform, false);

            //Get new rotation angle
            float angle = i * 120f;

            //Override blade sprite with this items sprite
            blades[i].GetComponent<SpriteRenderer>().sprite = itemSprite;

            //Get new Position
            Vector3 newPos = Utilities.GetUnitVector2(angle) * minDistance;

            //Apply rotation and position
            blades[i].transform.rotation = Quaternion.Euler(0, 0, angle);
            blades[i].transform.localPosition = newPos;
        }
        pickedUp = true;
    }

    private void Update()
    {
        if (pickedUp)
        {
            UpdateRotation();
            UpdatePosition();
        }
    }

    private void UpdateRotation()
    {
        float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, timer / speedUpTime);

        //Find currentAngle
        float currentAngle = transform.eulerAngles.z;
        //Add angle offset
        currentAngle += rotationSpeed * Time.deltaTime;
        //Apply angle change
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));
    }

    private void UpdatePosition()
    {
        for (int i = 0; i < 3; i++)
        {
            float distance = Mathf.Lerp(minDistance, maxDistance, timer / speedUpTime);

            //Get new rotation angle
            float angle = i * 120f;

            //Get new Position
            Vector3 newPos = Utilities.GetUnitVector2(angle) * distance;

            //Apply rotation and position
            blades[i].transform.localPosition = newPos;
        }
    }


}
