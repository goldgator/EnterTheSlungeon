using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HailOfBlades : PassiveItem
{
    [Header("Hail of Blades")]
    [SerializeField]
    private GameObject bladeObject;
    [SerializeField]
    private float rotationSpeed = 270f;
    [SerializeField]
    private float distance = 1.8f;

    public override void SetupItem()
    {
        base.SetupItem();

        //Create blade objects
        SetupBlades();


    }

    private void SetupBlades()
    {
        //Create 3 Sword Objects
        for (int i = 0; i < 3; i++) {
            GameObject newSword = Instantiate(bladeObject);
            newSword.transform.SetParent(transform, false);

            //Get new rotation angle
            float angle = i * 120f;

            //Get new Position
            Vector3 newPos = Utilities.GetUnitVector2(angle) * distance;

            //Apply rotation and position
            newSword.transform.rotation = Quaternion.Euler(0, 0, angle);
            newSword.transform.localPosition = newPos;
        }
        pickedUp = true;
    }

    private void Update()
    {        
        if (pickedUp)
        {
            //Find currentAngle
            float currentAngle = transform.eulerAngles.z;
            //Add angle offset
            currentAngle += rotationSpeed * Time.deltaTime;
            //Apply angle change
            transform.rotation = Quaternion.Euler(new Vector3(0,0,currentAngle));
        }
    }


}
