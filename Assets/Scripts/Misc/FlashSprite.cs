using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashSprite : MonoBehaviour
{
    public Material flashMaterial;
    public float flashTime;
    public int flashAmount;
    private SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Flash()
    {
        StartCoroutine(DoFlash());
    }

    public IEnumerator DoFlash()
    {
        Material oldMat = renderer.material;

        for (int i = 0; i < flashAmount; i++)
        {
            renderer.material = flashMaterial;
            yield return new WaitForSeconds(flashTime);
            renderer.material = oldMat;
            yield return new WaitForSeconds(flashTime);
        }
    }
}
