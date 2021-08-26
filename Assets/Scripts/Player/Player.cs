using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Stats")]
    public float baseSpeed = 6.0f;


    [Header("Components")]
    public GameObject weaponPivot;
    public GameObject weapon;
    private Rigidbody2D rb;


    //Private Fields
    private bool playerEnabled = true;

    public static Player Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (playerEnabled)
        {
            ProcessMove();
            ProcessAim();
        }
    }

    public void SetPlayerEnabled(bool isEnabled)
    {
        playerEnabled = isEnabled;
    }

    private void ProcessMove()
    {
        Vector3 velocity = InputManager.Instance.PlayerMovement.normalized;
        velocity *= baseSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + velocity);
    }

    private void ProcessAim()
    {
        if (InputManager.isGamepad)
        {
            ProcessAimGamePad();
        } else
        {
            ProcessAimKeyboard();
        }
    }

    private void ProcessAimGamePad()
    {

    }

    private void ProcessAimKeyboard()
    {
        Vector3 worldMousePos = InputManager.Instance.MouseWorldPosition;

        //Weapon pivot
        Vector3 direction = (worldMousePos - weaponPivot.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        weaponPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //Weapon
        direction = (worldMousePos - weapon.transform.position).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
