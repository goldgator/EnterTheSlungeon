using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerInteractPrompt : MonoBehaviour
{
    public static PlayerInteractPrompt Instance { get; set; }
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
