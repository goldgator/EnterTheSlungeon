using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Spikes : MonoBehaviour, IRoomObject
{
    public float damage = 4.0f;
    private bool activated = false;
    public bool isOn = false;
    public float offTime = 1.0f;
    public float onTime = 1.0f;
    public float startTime = 0f;
    private float flipTimer = 0f;

    private SpriteRenderer renderer;
    private Animator animator;
    private Collider2D collider;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        flipTimer = startTime;
    }

    
    void Update()
    {
        if (activated)
        {
            flipTimer += Time.deltaTime;
            float targetTime = (isOn) ? onTime : offTime;

            if (flipTimer > targetTime)
            {
                flipTimer = 0;
                if (isOn) Retract(); else Extend();
            }
        }
    }

    public bool ContentPassed()
    {
        return true;
    }

    public void SubscribeToRoomContent(RoomContent roomContent)
    {
        roomContent.roomActivateEvent += Enable;
        roomContent.roomFinishEvent += Disable;
    }

    private void Enable()
    {
        activated = true;
        if (isOn) Extend(); else Retract();
    }

    private void Disable()
    {
        activated = false;
        Retract();
    }

    private void Retract()
    {
        Debug.Log("Retract");
        //Do retract anim
        animator.Play("Retract");
        //Disable collider
        collider.enabled = false;
        isOn = false;
    }

    private void Extend()
    {
        Debug.Log("Extend");
        //Do extend anim
        animator.Play("Extend");
        //Enable collider
        collider.enabled = true;
        isOn = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();

            playerHealth.SubtractHealth(damage);
        }
    }

}
