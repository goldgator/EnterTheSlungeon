using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interacts with in-game interactables and UI elements
/// </summary>
public class PlayerCursor : MonoBehaviour
{
    [SerializeField]
    private bool uiState = false;

    //Store the current Interactable
    private Interactable currentInteractable;
    private Image cursorImage;

    private Collider2D collider;
    [Header("Components")]
    [SerializeField]
    private GameObject canvasObject;
    [SerializeField]
    private RectTransform cursorUIObject;
    [SerializeField]
    private GameObject colliderObject;

    [Header("Images")]
    [SerializeField]
    private Sprite uiSprite;
    [SerializeField]
    private float uiRotation = 30f;
    [SerializeField]
    private Vector2 uiOffset = new Vector2(15f, -20f);
    [SerializeField]
    private Sprite gameSprite;
    [SerializeField]
    private float gameRotation = 0f;
    [SerializeField]
    private Vector2 gameOffset = new Vector2(0,0);


    private static readonly string FILEPATH = "Prefabs/UI/PlayerCursor";
    private static PlayerCursor instance;
    public static PlayerCursor Instance { get
        {
            //Try to find cursor in scene first
            if (instance == null) instance = GameObject.FindObjectOfType<PlayerCursor>();

            //Create new cursor if still null
            if (instance == null) instance = Instantiate(Resources.Load<GameObject>(FILEPATH)).GetComponent<PlayerCursor>();

            //return instance
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            collider = colliderObject.GetComponent<Collider2D>();
            cursorImage = cursorUIObject.GetComponentInChildren<Image>();

            SetCursorUIState(uiState);
        } else
        {
            Destroy(gameObject);
        }
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
        //canvasObject.SetActive(state);

        cursorImage.sprite = (state) ? uiSprite : gameSprite;
        float rotation = (state) ? uiRotation : gameRotation;
        cursorImage.transform.rotation = Quaternion.Euler(0,0,rotation);
        Vector3 offset = (state) ? uiOffset : gameOffset;
        cursorImage.transform.localPosition = offset;

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

        Cursor.visible = false;
    }

    private void FollowMouse()
    {
        cursorUIObject.position = InputManager.Instance.MousePosition;
        
        if (colliderObject.activeSelf)
        {
            Vector3 newPos = InputManager.Instance.MouseWorldPosition;
            newPos.z = 0;
            colliderObject.transform.position = newPos;
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
