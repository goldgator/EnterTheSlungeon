using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //State variables
    public static bool isGamepad = false;
    public Camera playerCamera;
    public BaseControls baseControls;


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
            return baseControls.Controls.Fire.ReadValue<bool>();
        }
    }

    public bool Dodge
    {
        get
        {
            return baseControls.Controls.Dodge.ReadValue<bool>();
        }
    }


    public static InputManager Instance { get; set; }
    private void Start()
    {
        Instance = this;
        baseControls = new BaseControls();
        baseControls.Enable();
        DontDestroyOnLoad(gameObject);
    }
}
