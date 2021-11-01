using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour, IHealthDeath
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
    private SpriteRenderer renderer;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Health playerHealth;
    public AudioClip dodgeAudio;


    //Private Fields
    private bool playerEnabled = true;
    private Vector3 lastMoveDir;
    private Vector3 velocity;
    private Wallet playerWallet = new Wallet();
    private const string PLAYER_PATH = "Prefabs/Player/Player";
    public static GameObject secondaryWeapon;

    //Events & delegates
    public delegate void PlayerEvent();
    public delegate void PlayerMoveEvent(Vector2 velocity);
    public delegate void PlayerStatEvent(float stat);

    public static event PlayerMoveEvent playerMoveEvent;
    public static event PlayerStatEvent playerHealthEvent;
    public static event PlayerEvent playerDodgeEvent;
    



    //Singleton
    private static Player instance;
    public static Player Instance { get { 
            if (instance == null)
            {
                GameObject newPlayer = Instantiate(Resources.Load<GameObject>(PLAYER_PATH));
                return newPlayer.GetComponent<Player>();
            }

            return instance;
        }
    }
    public static bool InstanceExists()
    {
        return (instance != null);
    }
    public static void DestroyInstance()
    {
        if (instance != null) Destroy(instance.gameObject);
    }

    private void Awake()
    {
        instance = this;
        stats = GetComponent<StatBlock>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<Health>();
        weaponManager = GetComponentInChildren<WeaponManager>();

        //Ensure player info carries to future floors
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InstantiateGuns();
        InputManager.Instance.dodgeStartEvent += Dodge;
    }

    private void InstantiateGuns()
    {
        foreach(GameObject newWeapon in startingWeapons)
        {
            weaponManager.AddGun(newWeapon);
        }

        weaponManager.AddGun(secondaryWeapon);
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

    public Wallet GetWallet()
    {
        return playerWallet;
    }

    private void ProcessMove()
    {
        velocity = InputManager.Instance.PlayerMovement.normalized;
        velocity *= stats.GetStatValue("MoveSpeed");

        playerMoveEvent?.Invoke(velocity);

        lastMoveDir = velocity.normalized;
    }
    public void ForceStop() {
        velocity = new Vector2(0, 0);
    }

    public void TintPlayer(Color tint)
    {
        renderer.color = tint;
    }

    public void OnDamageTaken()
    {
        playerHealthEvent?.Invoke(playerHealth.HealthRatio);
    }

    private void Dodge()
    {
        if (playerEnabled)
        {
            animator.SetTrigger("Dodge");
            playerHealth.SetInvincibleTimer(0.25f);
            audioSource.clip = dodgeAudio;
            audioSource.Play();
            playerDodgeEvent?.Invoke();
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
        if (dodgeDir.magnitude <= 0.1f) dodgeDir = new Vector3(1, 0, 0);

        FlipSprite((dodgeDir.x < 0));
        //transform.localScale = new Vector3(Mathf.Sign(dodgeDir.x), 1, 1);

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

    public void FlipSprite(bool spriteFlip)
    {
        renderer.flipX = spriteFlip;
    }

    public void AddQuartz(ResourceType type, int amount)
    {
        playerWallet.AddQuartz(type, amount);
    }

    public void OnDeath()
    {
        OnPlayerDeath();
    }
}
