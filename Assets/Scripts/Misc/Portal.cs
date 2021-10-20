using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))]
public class Portal : MonoBehaviour
{
    private float enableTimer = 0.1f;
    private bool enabled = false;

    private SpriteRenderer renderer;
    private CircleCollider2D collider;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        renderer.color = Color.gray;
    }


    // Update is called once per frame
    void Update()
    {
        if (enableTimer < 0)
        {
            enabled = true;
            renderer.color = Color.white;
            collider.radius = 0.8f;
        }

        enableTimer -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (enableTimer > 0 && !enabled) enableTimer = 0.1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled && collision.CompareTag("Player"))
        {
            LoadNextFloor();
        }
    }

    private void LoadNextFloor()
    {
        SceneManager.sceneLoaded += SceneLoaded;

        SceneManager.LoadScene("FloorScene");
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Floor.Instance.SetFloorAttributes();
    }
}
