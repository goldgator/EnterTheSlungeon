using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PressurePlate : MonoBehaviour, IRoomObject
{
    public Sprite pressedSprite;
    private bool pressed = false;
    private SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public bool ContentPassed()
    {
        return pressed;
    }

    public void SubscribeToRoomContent(RoomContent roomContent)
    {
        //Not needed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressed = true;
            renderer.sprite = pressedSprite;
        }
    }
}
