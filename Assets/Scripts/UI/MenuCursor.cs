using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interacts with in-game interactables and UI elements
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class MenuCursor : MonoBehaviour
{
    [SerializeField]
    private bool uiState = false;

    //Store the current Interactable
    private Interactable currentInteractable;
    private SpriteRenderer renderer;

    private Collider2D collider;
    [SerializeField]
    private GameObject canvasObject;
    [SerializeField]
    private RectTransform cursorUIObject;

    public static MenuCursor Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        collider = GetComponent<Collider2D>();
        renderer = GetComponentInChildren<SpriteRenderer>();

        SetCursorUIState(uiState);
    }

    private void OnEnable()
    {
        //Subscribe to fire event
        InputManager.Instance.fireStartEvent += OnPlayerClick;

        //Hide cursor
        Cursor.visible = false;
    }

    public void SetCursorUIState(bool state)
    {
        uiState = state;

        collider.enabled = !state;
        canvasObject.SetActive(state);
        renderer.enabled = !state;

        if (state) currentInteractable = null;
    }

    private void OnDisable()
    {
        //Unsubscribe
        if (InputManager.Instance) InputManager.Instance.fireStartEvent -= OnPlayerClick;

        //Show cursor
        Cursor.visible = true;
    }


    private void OnPlayerClick()
    {
        currentInteractable?.TriggerInteractable();
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        
        if (uiState){
            cursorUIObject.position = InputManager.Instance.MousePosition;
        } else
        {
            Vector3 newPos = InputManager.Instance.MouseWorldPosition;
            newPos.z = 0;
            transform.position = newPos;
        }   

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentInteractable == null)
        {
            Interactable newInteractable = collision.GetComponent<Interactable>();

            if (newInteractable != null)
            {
                currentInteractable = newInteractable;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable newInteractable = collision.GetComponent<Interactable>();

        if (currentInteractable == newInteractable)
        {
            currentInteractable = null;
        }
    }
}
