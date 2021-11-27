using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gangster : BaseEnemy
{
    [Header("Gangster")]
    [SerializeField]
    private AudioClip flashbangSound;
    [SerializeField]
    private Image flashImage;
    [SerializeField]
    private GameObject smokeParent;
    [SerializeField]
    private GameObject weaponParent;
    [SerializeField]
    private Vector2 targetRange;
    [SerializeField]
    private float strafeSpeed = 3;
    [SerializeField]
    private float moveThreshold = 3.5f;

    [SerializeField]
    private GameObject afterImageObject;

    [SerializeField]
    private float dodgeCooldown;
    private float dodgeTimer;
    [SerializeField]
    private float dodgeSpeed;
    [SerializeField]
    private AnimationCurve dodgeCurve;

    [SerializeField]
    private float healthSmokeRatio = .3f;
    
    private Vector3 targetPosition;
    private Vector2 strafeOffset;

    private bool moving = false;
    private bool active = true;

    private bool smoke = false;


    protected override void Start()
    {
        base.Start();

        collider = GetComponent<BoxCollider2D>();

        targetPosition = transform.position;
        MusicManager.Instance.PlaySong("Boss3");
    }

    private void FixedUpdate()
    {
        stopTime -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;

        //Update animator
        animator.SetFloat("Speed", rb.velocity.magnitude);

        //Orient sprite
        renderer.flipX = ((target.transform.position - transform.position).x < 0);

        if (health.HealthRatio <= healthSmokeRatio && !smoke)
        {
            smoke = true;
            rb.velocity = new Vector2();

            active = false;

            //Disable weapon parent
            weaponParent.SetActive(false);
            StopAllCoroutines();

            //Start smoke animation
            //Debug.Log("Playing smokescreen");
            animator.Play("SmokeScreen");

            float animationTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            //Add stop time equal to animation time plus 6 extra seconds
            stopTime = animationTime + 6;
            //Add invincibility equal to animation time plus 1 extra second
            health.SetInvincibleTimer(animationTime + 1);
        }

        if (!health.HasDied && stopTime <= 0)
        {
            UpdateTargetPos();

            if (moving)
            {
                OnMove();
            }
        }
    }


    public void CreateSmokeScreen()
    {
        //Start smoke screen coroutine
        StartCoroutine(AddSmokeScreen());
    }

    public IEnumerator AddSmokeScreen()
    {
        //Play flashbang sound
        audioSource.clip = flashbangSound;
        audioSource.Play();

        //Play new music
        MusicManager.Instance.PlaySong("Boss3-5");

        //Flash the screen
        float time = 0f;
        while (time < .1f)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(Color.clear, Color.white, time / .1f);
            flashImage.color = newColor;
            yield return null;
        }

        //Enable Smoke Parent
        smokeParent.SetActive(true);

        //Wait a full second
        yield return new WaitForSeconds(1);

        //slowly remove flash over 2.5 seconds
        time = 0f;
        while (time < 2.5f)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(Color.white, Color.clear, time / 2.5f);
            Debug.Log("Color: " + newColor);
            flashImage.color = newColor;
            yield return null;
        }

        //Enable weapon again
        weaponParent.SetActive(true);

        active = true;
    }

    public IEnumerator DeathRoutine()
    {
        active = false;

        //Disable weapon
        weaponParent.SetActive(false);

        //Flash the screen
        float time = 0f;
        while (time < .1f)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(Color.clear, Color.white, time / .1f);
            flashImage.color = newColor;
            yield return null;
        }

        //Disable Smoke Parent
        smokeParent.SetActive(false);

        //Wait a full second
        yield return new WaitForSeconds(1);

        //slowly remove flash over 2.5 seconds
        time = 0f;
        while (time < 2.5f)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(Color.white, Color.clear, time / 2.5f);
            Debug.Log("Color: " + newColor);
            flashImage.color = newColor;
            yield return null;
        }

        
        MusicManager.Instance.PlaySong("Victory");

        Destroy(transform.parent.gameObject, .1f);
        Death();
    }

    public override void OnDeath()
    {
        //Play take hit anim
        animator.Play("Dying");

        //reset velocity
        rb.velocity = new Vector2();

        StartCoroutine(DeathRoutine());
    }


    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, .2f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Ray(targetPosition, strafeOffset));

        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(target.transform.position, targetRange.x);
            Gizmos.DrawWireSphere(target.transform.position, targetRange.y);
        }
    }

    private void UpdateTargetPos()
    {
        //Move targetPosition so it is within targetRange of target
        Vector3 targetOffset = targetPosition - target.transform.position;
        if (targetOffset.magnitude < targetRange.x || targetOffset.magnitude > targetRange.y)
        {
            float magnitude = Mathf.Clamp(targetOffset.magnitude, targetRange.x, targetRange.y);
            targetPosition = target.transform.position + targetOffset.normalized * magnitude;
        }

        //Slowly drift targetPosition to center of targetRange
        //Drift faster if target is too close
        Vector3 targetDistance = target.transform.position - transform.position;
        targetOffset = targetOffset.normalized * ((targetRange.x + targetRange.y) / 2);

        //Drift targetPosition perpendicular of direction to target (strafe)
        strafeOffset = Vector2.Perpendicular(new Vector2(targetOffset.x, targetOffset.y).normalized) * strafeSpeed  * Time.deltaTime;
        targetOffset += new Vector3(strafeOffset.x, strafeOffset.y);

        //Lerp to position
        targetOffset += target.transform.position;
        targetPosition = Vector3.Lerp(targetPosition, targetOffset, Time.deltaTime);

        //Get distance vector towards targetPosition
        Vector3 distance = targetPosition - transform.position;

        //If not moving and targetPos is far away, and is active
        if (!moving && distance.magnitude > moveThreshold && active)
        {
            moving = true;
        }
    }

    private void OnMove()
    {
        Vector3 distance = targetPosition - transform.position;
        rb.velocity = distance.normalized * speed;

        if (distance.magnitude <= .1f)
        {
            moving = false;
            rb.velocity = new Vector3();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon") && dodgeTimer < 0 && active)
        {
            dodgeTimer = dodgeCooldown;

            //Start dodge Coroutine
            StartCoroutine(DoDodge(.5f));

            //Start afterImage Coroutine
            StartCoroutine(SpawnAfterImages(.5f, 7));
        }
    }

    private IEnumerator DoDodge(float dodgeTime)
    {
        active = false;
        moving = false;

        //Give immunity frames
        health.SetInvincibleTimer(dodgeTime);

        //Direction of targetPos
        Vector3 dodgeDir = (targetPosition - transform.position).normalized;

        while (dodgeTime > 0)
        {
            rb.velocity = dodgeDir * dodgeSpeed * dodgeCurve.Evaluate(.5f - dodgeTime);
            //Debug.Log(velocity);

            yield return null;
            dodgeTime -= Time.deltaTime;
        }

        //Remove velocity
        rb.velocity = new Vector3();

        active = true;
    }

    private IEnumerator SpawnAfterImages(float time, int count)
    {
        float spawnTime = time / (count - 1);

        float endTime = 1;

        Color startColor = new Color(1, 1, 1, .2f);
        Color endColor = new Color(1, 1, 1, 1f);

        for (int i = 0; i < count; i++)
        {
            //Spawn afterImage object
            Afterimage afterimage = Instantiate(afterImageObject, transform.position, Quaternion.identity).GetComponent<Afterimage>();

            //Determine sorting order
            int order = (count - i);
            int sortingOrder = renderer.sortingOrder - order;

            //Determine delay
            float delay = order * (time / count);

            //Determine Color
            Color newColor = Color.Lerp(startColor, endColor, i / count);

            //Instantiate object
            afterimage.InstantiateAfterimage(renderer, sortingOrder, newColor, delay, endTime);

            //Wait for next spawntime
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
