using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashSprite : MonoBehaviour
{
    public Material flashMaterial;
    private Material originalMat;
    public float flashTime = 0.05f;
    public int flashAmount;
    private new SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        originalMat = renderer.material;
    }

    public void Flash()
    {
        StartCoroutine(DoFlash());
    }

    public IEnumerator DoFlash()
    {
        if (renderer.material != originalMat) yield break;

        for (int i = 0; i < flashAmount; i++)
        {
            renderer.material = flashMaterial;
            yield return new WaitForSeconds(flashTime);
            renderer.material = originalMat;
            yield return new WaitForSeconds(flashTime);
        }
    }
}
