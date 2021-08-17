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
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ProcessMove();
    }

    private void ProcessMove()
    {
        float inpX = Input.GetAxisRaw("Horizontal");
        float inpY = Input.GetAxisRaw("Vertical");

        Vector3 velocity = new Vector2(inpX, inpY).normalized;
        velocity *= baseSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + velocity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
