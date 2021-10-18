using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseItem : MonoBehaviour
{
    public Sprite itemSprite;
    protected ItemManager managerParent;
    protected SpriteRenderer renderer;
    protected Collider2D collider;

    public virtual void SetupItem()
    {
        Destroy(renderer);
        Destroy(collider);
        transform.SetParent(managerParent.transform, false);
        transform.position = Vector3.zero;
    }

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
        if (renderer == null) renderer = gameObject.AddComponent<SpriteRenderer>();

        renderer.sprite = itemSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.CompareTag("Player"))
        {
            managerParent = collision.GetComponentInChildren<ItemManager>();
            managerParent?.AddItem(this);
        }
    }
}
