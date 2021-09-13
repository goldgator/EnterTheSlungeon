using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider healthAnimSlider;

    public Image fillImage;
    public Image fillAnimImage;

    public Health playerHealth;


    // Update is called once per frame
    void Update()
    {
        float value = playerHealth.GetHealth() / playerHealth.maxHealth;

        healthSlider.value = value;
        healthAnimSlider.value = Mathf.Lerp(healthAnimSlider.value, healthSlider.value, Time.deltaTime);

        Color baseColor = Color.Lerp(Color.red, Color.green, value);
        fillImage.color = baseColor;

        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        fillAnimImage.color = Color.HSVToRGB(h, s * .5f, v);
    }
}
