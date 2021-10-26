using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class InteractPrompt : MonoBehaviour
{
    public static InteractPrompt Instance { get; set; }
    private void Awake()
    {
        Instance = this;

        HidePrompt();
    }

    public void ShowPrompt()
    {
        gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        gameObject.SetActive(false);
    }


}
