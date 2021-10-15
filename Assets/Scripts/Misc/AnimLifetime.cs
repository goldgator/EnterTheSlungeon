using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class AnimLifetime : MonoBehaviour
{
    //private Animator animator;
    private Animator animator;
    public string clipName;
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play(clipName);
        Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }

    public void ForceDeath()
    {
        Destroy(gameObject);
    }
}
