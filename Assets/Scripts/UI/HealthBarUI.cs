using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    Slider slider;
    public Health monitoredHealth;


    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (monitoredHealth)
        {
            slider.value = monitoredHealth.GetHealth() / monitoredHealth.maxHealth;
        }
    }
}
