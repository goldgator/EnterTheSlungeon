using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Quartz : MonoBehaviour
{
    private ResourceType type;

    public const string QUARTZ_PATH = "Textures/Quartz/";


    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    private Player target;

    private float lifeTime = 0f;
    private float spinSpeed = 10.0f;
    private float flyTime = 1.7f;
    private float speed = 14.0f;
    private float speedUpTime = 3.0f;

    private static GameObject quartzPrefab;
    public static GameObject QuartzPrefab
    {
        get
        {
            if (quartzPrefab == null) quartzPrefab = Resources.Load<GameObject>("Prefabs/Misc/Quartz");

            return quartzPrefab;
        }
    }

    public static Sprite GetQuartzSprite(ResourceType type, bool chunk = false)
    {
        string path = (chunk) ? QUARTZ_PATH + "Chunks/" : QUARTZ_PATH;

        return Resources.Load<Sprite>(path + type.ToString());
    }


    public static Color GetQuartzColor(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Time:
                return new Color(.9f, .9f, .9f);
            case ResourceType.Space:
                return new Color(1f, .3f, .5f);
            case ResourceType.Void:
                return new Color(0f, 0f, 0f);
            case ResourceType.Valuable:
                return new Color(1f, 0f, 1f);
            default:
                return new Color(0f, .5f, 0f);
        }
    }

    public void SetType(ResourceType newType)
    {
        type = newType;

        renderer.sprite = GetQuartzSprite(type);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        target = Player.Instance;
        SetRandomVelocity();
    }

    private void SetRandomVelocity()
    {
        //randomize spinSpeed direction
        //Choose random angle
        Vector2 direction = Utilities.GetUnitVector2(RNGManager.GetEventRand(0f,360f));
        //Set velocity of rigid body
        rb.velocity = direction * RNGManager.GetEventRand(speed / 2, speed) * .5f;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime < flyTime)
        {
            SlowDown();
        } else
        {
            FlyToPlayer();
        }

        UpdateSpin();
    }

    private void UpdateSpin()
    {
        //Find currentAngle
        float currentAngle = transform.eulerAngles.z;

        //Shift angle by rotate speed and current velocity;
        float angleShift = (rb.velocity.magnitude / speed) * spinSpeed;
        currentAngle += angleShift;

        transform.rotation = Quaternion.Euler(new Vector3(0,0,currentAngle));
    }


    private void SlowDown()
    {
        //Get velocity normal
        Vector2 normal = rb.velocity.normalized;
        //Set velocity magnitude to appropriate based on time
        rb.velocity = Mathf.Lerp(speed * .5f, 0, lifeTime / flyTime) * normal;
    }

    private void FlyToPlayer()
    {
        //Get direction to player
        Vector2 normal = (target.transform.position - transform.position).normalized;
        //Set velocity magnitude to appropriate level based on time
        rb.velocity = Mathf.Lerp(speed, speed * 2.5f, (lifeTime - flyTime) / speedUpTime) * normal;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<Player>().AddQuartz(type, 1);
            Destroy(gameObject);
        }
    }
}
