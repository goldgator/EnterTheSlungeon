using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class StartGamePlayer : MonoBehaviour
{
    [SerializeField]
    private StartGameUI startGameUI;

    [SerializeField]
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        Floor.currentGenData = 0;
    }

    public void StartGamePlayerLoadScene()
    {
        startGameUI.LoadFirstFloor();
    }

    public void RevealOptions()
    {
        startGameUI.RevealOptions();
    }

    public void StartFallAnim()
    {
        animator.SetTrigger("Fall");
    }
}
