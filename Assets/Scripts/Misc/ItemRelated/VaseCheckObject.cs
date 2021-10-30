using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(QuartzDrops))]
public class VaseCheckObject : MonoBehaviour
{
    [SerializeField]
    private float startScale = 6.0f;
    [SerializeField]
    private float endScale = 1.0f;

    [SerializeField]
    private float rotationSpeed = 30f;
    [SerializeField]
    private float fallingTime = 2.0f;
    private float fallingTimer;

    [SerializeField] 
    Color startColor = Color.gray;
    [SerializeField]
    Color endColor = Color.white;

    private SpriteRenderer renderer;


    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();

        //Set random rotation
        float rotation = RNGManager.GetEventRand(0f, 360f);

        transform.rotation = Quaternion.Euler(0,0,rotation);

        fallingTimer = fallingTime;
    }

    // Update is called once per frame
    void Update()
    {
        fallingTimer -= Time.deltaTime;

        if (fallingTimer < 0)
        {
            GetComponent<QuartzDrops>().OnDeath();
            Destroy(gameObject);
        }

        UpdateScale();
        UpdateColor();
        UpdateRotation();
    }

    private void UpdateScale()
    {
        float newScale = Mathf.Lerp(endScale, startScale, fallingTimer / fallingTime);

        transform.localScale = new Vector3(newScale, newScale, 1);
    }

    private void UpdateColor()
    {
        Color newColor = Color.Lerp(endColor, startColor, fallingTimer / fallingTime);

        renderer.color = newColor;
    }

    private void UpdateRotation()
    {
        float currentAngle = transform.eulerAngles.z;

        currentAngle += rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0,0, currentAngle);
    }


}
