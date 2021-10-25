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
    public delegate void UpdateDelegate(bool pressed);
    public delegate void EventDelegate();
    

    public event MoveDelegate moveStartEvent;
    public event MoveDelegate moveUpdateEvent;
    public event MoveDelegate moveStopEvent;

    public event EventDelegate fireStartEvent;
    public event UpdateDelegate fireUpdateEvent;
    public event EventDelegate fireStopEvent;

    public event EventDelegate dodgeStartEvent;
    public event UpdateDelegate dodgeUpdateEvent;
    public event EventDelegate dodgeStopEvent;
    
    public event EventDelegate scrollStartEvent;
    public event UpdateDelegate scrollUpdateEvent;
    public event EventDelegate scrollStopEvent;

    public event EventDelegate reloadStartEvent;
    public event UpdateDelegate reloadUpdateEvent;
    public event EventDelegate reloadStopEvent;

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
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        baseControls = new BaseControls();
        baseControls.Enable();
        DontDestroyOnLoad(gameObject);

        baseControls.Controls.Move.started += ctx => MoveStart(ctx);
        baseControls.Controls.Move.canceled += ctx => MoveStop(ctx);

        baseControls.Controls.Fire.started += ctx => FireStart(ctx);
        baseControls.Controls.Fire.canceled += ctx => FireStop(ctx);

        baseControls.Controls.Dodge.started += ctx => DodgeStart(ctx);
        baseControls.Controls.Dodge.canceled += ctx => DodgeStop(ctx);

        baseControls.Controls.NextWeapon.started += ctx => ScrollStart(ctx);
        baseControls.Controls.NextWeapon.canceled += ctx => ScrollStop(ctx);

        baseControls.Controls.Reload.started += ctx => ReloadStart(ctx);
        baseControls.Controls.Reload.canceled += ctx => ReloadStop(ctx);
    }

    void MoveStart(InputAction.CallbackContext context)
    {
        //Debug.Log(PlayerMovement);
        moveStartEvent?.Invoke(PlayerMovement);
    }

    void MoveStop(InputAction.CallbackContext context)
    {
        moveStopEvent?.Invoke(PlayerMovement);
    }

    void FireStart(InputAction.CallbackContext context)
    {
        fireStartEvent?.Invoke();
    }

    void FireStop(InputAction.CallbackContext context)
    {
        fireStopEvent?.Invoke();
    }

    void DodgeStart(InputAction.CallbackContext context)
    {
        dodgeStartEvent?.Invoke();
    }

    void DodgeStop(InputAction.CallbackContext context)
    {
        dodgeStopEvent?.Invoke();
    }

    void ScrollStart(InputAction.CallbackContext context)
    {
        scrollStartEvent?.Invoke();
    }

    void ScrollStop(InputAction.CallbackContext context)
    {
        scrollStopEvent?.Invoke();
    }

    void ReloadStart(InputAction.CallbackContext context)
    {
        reloadStartEvent?.Invoke();
    }

    void ReloadStop(InputAction.CallbackContext context)
    {
        reloadStopEvent?.Invoke();
    }

    private void Update()
    {
        moveUpdateEvent?.Invoke(PlayerMovement);

        fireUpdateEvent?.Invoke(Fire);

        dodgeUpdateEvent?.Invoke(DodgeUpdate);
    }
}
