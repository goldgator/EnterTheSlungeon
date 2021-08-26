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

    public event MoveDelegate moveStartEvent;
    public event MoveDelegate moveUpdateEvent;
    public event MoveDelegate moveStopEvent;

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
            return baseControls.Controls.Fire.triggered;
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

    public static InputManager Instance { get; set; }
    private void Start()
    {
        Instance = this;
        baseControls = new BaseControls();
        baseControls.Enable();
        DontDestroyOnLoad(gameObject);

        baseControls.Controls.Move.started += ctx => MoveStart(ctx);
        baseControls.Controls.Move.canceled += ctx => MoveStart(ctx);
    }

    void MoveStart(InputAction.CallbackContext context)
    {
        moveStartEvent?.Invoke(PlayerMovement);
    }

    void MoveStop(InputAction.CallbackContext context)
    {
        moveStopEvent?.Invoke(PlayerMovement);
    }

    private void Update()
    {
        moveUpdateEvent?.Invoke(PlayerMovement);
    }
}
