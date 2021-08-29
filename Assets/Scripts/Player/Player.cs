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
    private Animator animator;
    private Rigidbody2D rb;


    //Private Fields
    private bool playerEnabled = true;
    private Vector3 velocity;

    public static Player Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if (playerEnabled)
        {
            ProcessMove();
        }
        animator.SetFloat("Speed", velocity.magnitude);
    }

    public void SetPlayerEnabled(bool isEnabled)
    {
        playerEnabled = isEnabled;
    }

    public bool GetPlayerEnabled()
    {
        return playerEnabled;
    }

    private void ProcessMove()
    {
        velocity = InputManager.Instance.PlayerMovement.normalized;
        velocity *= baseSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + velocity);
    }
}
