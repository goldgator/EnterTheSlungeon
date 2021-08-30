using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;
    public Health playerHealth;


    // Update is called once per frame
    void Update()
    {
        float value = playerHealth.GetHealth() / playerHealth.maxHealth;
        healthSlider.value = value;
        fillImage.color = Color.Lerp(Color.red, Color.green, value);
    }
}
