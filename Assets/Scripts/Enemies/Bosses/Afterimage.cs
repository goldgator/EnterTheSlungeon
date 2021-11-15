using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Afterimage : MonoBehaviour
{
    private float timeDelay;
    private float morphTime;

    private SpriteRenderer renderer;

    private Transform parent;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void InstantiateAfterimage(SpriteRenderer parentRenderer, int sortingOrder, Color spriteColor, float newDelay, float newTime)
    {
        timeDelay = newDelay;
        morphTime = newTime;

        renderer.sprite = parentRenderer.sprite;
        renderer.flipX = parentRenderer.flipX;
        renderer.sortingOrder = sortingOrder;
        renderer.color = spriteColor;

        parent = parentRenderer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        timeDelay -= Time.deltaTime;

        if (timeDelay < 0)
        {
            transform.position = Vector3.Lerp(transform.position, parent.transform.position, (-timeDelay) / morphTime);
        }

        if (timeDelay < -morphTime)
        {
            Destroy(gameObject);
        }
    }
}
