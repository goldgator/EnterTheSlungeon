using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Stats")]
    //public float baseSpeed = 6.0f;
    //public float dodgeSpeed = 12.0f;
    public AnimationCurve dodgeCurve;


    [Header("Components")]
    public List<GameObject> startingWeapons;
    private Transform currentPivot { get { return weaponManager.CurrentPivot; } }
    private BaseWeapon currentWeapon { get { return weaponManager.CurrentWeapon; } }
    private StatBlock stats;
    private WeaponManager weaponManager;
    private Animator animator;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Health playerHealth;
    public AudioClip dodgeAudio;


    //Private Fields
    private bool playerEnabled = true;
    private Vector3 lastMoveDir;
    private Vector3 velocity;

    public static Player Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        stats = GetComponent<StatBlock>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<Health>();
        weaponManager = GetComponentInChildren<WeaponManager>();

        InstantiateGuns();
    }
    private void Start()
    {
        InputManager.Instance.dodgeStartEvent += Dodge;
    }

    private void InstantiateGuns()
    {
        foreach(GameObject newWeapon in startingWeapons)
        {
            weaponManager.AddGun(newWeapon);
        }
    }

    public void OnPlayerDeath()
    {
        animator.SetBool("Death", true);
        currentPivot.gameObject.SetActive(false);
        playerEnabled = false;
        ForceStop();
        Floor.Instance.OnPlayerDeath();
    }

    private void FixedUpdate()
    {
        if (playerEnabled)
        {
            ProcessMove();
        }

        ProcessVelocity();
        animator.SetFloat("Speed", velocity.magnitude);
    }

    public void SetPlayerEnabled(bool isEnabled)
    {
        playerEnabled = isEnabled;
    }

    public bool GetPlayerEnabled()
    {
        return playerEnabled;
    }

    private void ProcessVelocity()
    {
        if (velocity.magnitude > 0)
        {
            rb.MovePosition(transform.position + velocity * Time.deltaTime);
        }
    }

    private void ProcessMove()
    {
        velocity = InputManager.Instance.PlayerMovement.normalized;
        velocity *= stats.GetStatValue("MoveSpeed");

        lastMoveDir = velocity.normalized;
    }
    public void ForceStop() {
        velocity = new Vector2(0, 0);
    }

    private void Dodge()
    {
        if (playerEnabled)
        {
            animator.SetTrigger("Dodge");
            playerHealth.SetInvincibleTimer(0.25f);
            audioSource.clip = dodgeAudio;
            audioSource.Play();
            StartCoroutine(DoDodge());
        }
    }

    private IEnumerator DoDodge()
    {
        playerEnabled = false;
        float dodgeTime = 0.5f;
        //currentWeapon.gameObject.SetActive(false);
        currentWeapon.renderer.enabled = false;
        Vector3 dodgeDir = lastMoveDir;
        if (dodgeDir.magnitude <= 0.1f) dodgeDir = new Vector3(transform.localScale.x, 0, 0);
        transform.localScale = new Vector3(Mathf.Sign(dodgeDir.x), 1, 1);

        while (dodgeTime > 0)
        {
            velocity = dodgeDir * stats.GetStatValue("DodgeSpeed") * dodgeCurve.Evaluate(.5f - dodgeTime);
            //Debug.Log(velocity);

            yield return null;
            dodgeTime -= Time.deltaTime;
        }

        //currentWeapon.gameObject.SetActive(true);
        currentWeapon.renderer.enabled = true;
        playerEnabled = true;
    }
}
