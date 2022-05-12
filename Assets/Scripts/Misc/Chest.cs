using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Chest : MonoBehaviour
{
    
    [SerializeField]
    private Transform spawnTransform;
    private GameObject spawnedItem;
    private Collider2D collider;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        SelectItem();
    }

    
    private void SelectItem()
    {
        spawnedItem = BaseItem.SelectRandomItem();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.Play("Open");
            collider.enabled = false;
        }
    }

    public void SpawnItem()
    {
        Instantiate(spawnedItem, spawnTransform.position, Quaternion.identity);
    }
}
