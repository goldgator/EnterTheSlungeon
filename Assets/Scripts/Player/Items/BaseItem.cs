using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseItem : MonoBehaviour, IInteractEvent
{

    [Header("Base Info")]
    [SerializeField]
    private Sprite itemSprite;
    [SerializeField]
    private string name;
    [SerializeField]
    private string desc;
    protected ItemManager managerParent;
    protected Interactable interactable;
    protected SpriteRenderer renderer;
    protected Collider2D collider;

    public virtual void SetupItem()
    {
        //Destroy interactable component *before* as it has dependencies with the renderer and collider
        Destroy(interactable);
        Destroy(renderer);
        Destroy(collider);
        transform.SetParent(managerParent.transform, false);
        transform.position = Vector3.zero;
    }

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
        if (renderer == null) renderer = gameObject.AddComponent<SpriteRenderer>();

        renderer.sprite = itemSprite;
    }

    public void PickUp()
    {
        managerParent = Player.Instance.GetComponentInChildren<ItemManager>();
        managerParent?.AddItem(this);
    }

    public void OnInteract()
    {
        PickUp();
    }

    public void OnInteractEnter()
    {
        MessageManager.Instance.SendPopup(name, desc, float.MaxValue, itemSprite);
    }

    public void OnInteractLeave()
    {
        MessageManager.Instance.ClosePopup();
    }
}
