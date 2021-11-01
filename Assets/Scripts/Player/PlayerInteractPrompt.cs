using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerInteractPrompt : MonoBehaviour
{
    public static PlayerInteractPrompt instance;
    public static PlayerInteractPrompt Instance { get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<PlayerInteractPrompt>();

            return instance;
        }
    }

    public static bool InstanceExists()
    {
        return (instance != null);
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Awake()
    {
        instance = this;

        HidePrompt();
    }

    public void ShowPrompt()
    {
        if (this == null) return;
        gameObject?.SetActive(true);
    }

    public void HidePrompt()
    {
        if (this == null) return;
        gameObject?.SetActive(false);
    }


}
