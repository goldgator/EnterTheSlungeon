using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //State variables
    public static bool isGamepad = false;
    public Camera playerCamera;
    public BaseControls baseControls;

    //Delegates and events
    public delegate void MoveDelegate(Vector2 position);
    public delegate void FireDelegate(bool pressed);

    public event MoveDelegate moveStartEvent;
    public event MoveDelegate moveUpdateEvent;
    public event MoveDelegate moveStopEvent;

    public event FireDelegate fireStartEvent;
    public event FireDelegate fireUpdateEvent;
    public event FireDelegate fireStopEvent;

    #region Input Properties
    //Input values
    public Vector2 PlayerMovement
    {
        get
        {
            return baseControls.Controls.Move.ReadValue<Vector2>();
        }
    }

    public Vector2 MousePosition
    {
        get
        {
            return baseControls.Controls.MousePos.ReadValue<Vector2>();
        }
    }

    public Vector2 StickAim
    {
        get
        {
            return baseControls.Controls.StickAim.ReadValue<Vector2>();
        }
    }

    public Vector3 MouseWorldPosition
    {
        get
        {
            return playerCamera.ScreenToWorldPoint(MousePosition);
        }
    }

    public bool Fire
    {
        get
        {
            return (0 < baseControls.Controls.Fire.ReadValue<Single>());
        }
    }

    public bool Dodge
    {
        get
        {
            return baseControls.Controls.Dodge.triggered;
        }
    }

    public bool DodgeUpdate
    {
        get
        {
            return (baseControls.Controls.Dodge.ReadValue<Single>() > 0);
        }
    }

    public bool Map
    {
        get
        {
            return baseControls.Controls.Map.triggered;
        }
    }
    #endregion

    #region Event Subscription

    #endregion

    private static InputManager instance;
    public static InputManager Instance { get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<InputManager>();
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        baseControls = new BaseControls();
        baseControls.Enable();
        DontDestroyOnLoad(gameObject);

        baseControls.Controls.Move.started += ctx => MoveStart(ctx);
        baseControls.Controls.Move.canceled += ctx => MoveStart(ctx);

        baseControls.Controls.Fire.started += ctx => FireStart(ctx, ctx.ReadValueAsButton());
        baseControls.Controls.Fire.canceled += ctx => FireStop(ctx, ctx.ReadValueAsButton());
    }

    void MoveStart(InputAction.CallbackContext context)
    {
        moveStartEvent?.Invoke(PlayerMovement);
    }

    void MoveStop(InputAction.CallbackContext context)
    {
        moveStopEvent?.Invoke(PlayerMovement);
    }

    void FireStart(InputAction.CallbackContext context, bool pressed)
    {
        fireStartEvent?.Invoke(pressed);
    }

    void FireStop(InputAction.CallbackContext context, bool pressed)
    {
        fireStopEvent?.Invoke(pressed);
    }

    private void Update()
    {
        moveUpdateEvent?.Invoke(PlayerMovement);

        fireUpdateEvent?.Invoke(Fire);
    }
}
