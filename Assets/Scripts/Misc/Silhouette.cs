using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Silhouette : MonoBehaviour
{
    private SpriteRenderer original;
    private SpriteRenderer copy;

    private readonly string MAT_PATH = "Materials/Flash";



    private void Awake()
    {
        original = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        CreateSilhouetteObject();
    }

    private void CreateSilhouetteObject()
    {
        copy = new GameObject("Silhouette").AddComponent<SpriteRenderer>();
        copy.transform.parent = transform;
        copy.transform.localPosition = new Vector3();
        

        copy.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        copy.material = Resources.Load<Material>(MAT_PATH);
        copy.color = new Color(.65f,.65f,.65f, original.color.a * .5f);
        copy.sortingOrder = 690;
    }

    void Update()
    {
        copy.sprite = original.sprite;
        copy.flipX = original.flipX;
        copy.flipY = original.flipY;
        copy.enabled = original.enabled;
        copy.transform.rotation = original.transform.rotation;
        copy.transform.localScale = original.transform.localScale;
    }
}
