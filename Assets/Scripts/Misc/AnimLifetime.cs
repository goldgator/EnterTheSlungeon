using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animation))]
public class AnimLifetime : MonoBehaviour
{
    //private Animator animator;
    private Animation animation;
    

    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
        animation.Play();
        Destroy(gameObject, animation.clip.length);
    }
}
