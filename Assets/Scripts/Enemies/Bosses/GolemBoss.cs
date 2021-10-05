using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBoss : BaseEnemy
{
    public AudioClip laserSound;
    public Transform laserOrigin;
    public HitBox laserHitBox;
    public Animator laserAnimator;
    public float laserSpeed = 30.0f;

    private int punches = 0;

    enum eState
    {
        IDLE,
        MOVE,
        LASER
    }

    eState currentState;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        MusicManager.Instance.PlaySong("Boss2");
        stopTime = RNGManager.GetEventRand(attackCooldown / 4f, attackCooldown / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        stopTime -= Time.deltaTime;
        if (!health.HasDied)
        {
            HandleStates();
        }
    }

    private void HandleStates()
    {
        switch (currentState)
        {
            case eState.IDLE:
                OnIdle();
                break;
            case eState.MOVE:
                OnMove();
                break;
            case eState.LASER:
                //OnLaser();
                break;
            default:
                break;
        }
    }

    private void OnIdle()
    {

        if (stopTime < 0)
        {
            stopTime = attackCooldown;

            //Roll 0-2
            //Do laser if 2
            System.Random rng = new System.Random();
            if (rng.Next(3) == 2 || punches >= 3)
            {
                //Do laser stuff 
                ActivateLaser();
                punches = 0;
            } else
            {
                //Do move stuff
                currentState = eState.MOVE;
            }
        }
    }


    private void OnMove()
    {
        if (stopTime < 0)
        {
            ActivateAttack();
            return;
        }

        //Get distance vector
        Vector3 distance = target.transform.position - transform.position;

        //Orient sprite
        transform.localScale = new Vector3(Mathf.Sign(distance.x), transform.localScale.y, transform.localScale.z);

        //if (distance.magnitude > attackRange)
        Vector3 velocity = speed * distance.normalized;

        velocity = velocity * Time.deltaTime;
        transform.position += velocity;

        OnAttack();
    }

    private void ActivateLaser()
    {
        transform.localScale = new Vector3(1, 1, 1);
        currentState = eState.LASER;
        animator.SetBool("Laser", true);
        StartCoroutine(OnLaser());
    }

    private IEnumerator OnLaser()
    {

        //Wait 1.0 seconds (show laser)
        laserOrigin.gameObject.SetActive(true);
        float timer = 1.0f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        //Grab laser angle then wait another .5 seconds
        float angle = GetLaserAngle();
        timer = .5f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        //change laser animation
        laserAnimator.SetBool("Blast", true);
        laserHitBox.boxEnabled = true;
        PlayLaserSound();
        //Rotate laser slowly towards target rotation for 3 seconds
        timer = 3.0f;
        PlayerCamera.Instance.SetScreenShake(0.17f, 3.5f);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float targetAngle = GetLaserAngle();
            angle += GetLaserAngleChange(angle, targetAngle);
            //Assign angle to transform
            laserOrigin.rotation = Quaternion.Euler(new Vector3(0,0,angle));
            yield return null;
        }

        //Set stopTime to cooldown before going back to idle, and clean up
        stopTime = attackCooldown;
        StartCoroutine(SlowStopSound(0.2f));
        laserHitBox.boxEnabled = false;
        laserAnimator.SetBool("Blast", false);
        laserOrigin.transform.rotation = Quaternion.identity;
        laserOrigin.gameObject.SetActive(false);
        currentState = eState.IDLE;
        animator.SetBool("Laser", false);
    }

    private void PlayAttackSound()
    {
        //Play sound
        if (audioSource) {
            //Debug.Log("Play Attack");
            audioSource.clip = attackSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    private void PlayLaserSound()
    {
        //Play sound
        if (audioSource)
        {
            //Debug.Log("Play laser");
            audioSource.clip = laserSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private IEnumerator SlowStopSound(float time)
    {
        
        if (audioSource)
        {
            float currentTime = time;
            float startVolume = audioSource.volume;
            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0, startVolume, currentTime / time);

                yield return null;
            }

            audioSource.volume = startVolume;
            audioSource.Stop();
        }
    }

    private float GetLaserAngleChange(float currentAngle, float targetAngle)
    {
        if (currentAngle < 0) currentAngle += 360.0f;
        if (targetAngle < 0) targetAngle += 360.0f;

        float angleDiff = targetAngle - currentAngle;
        int mod = (Mathf.Abs(angleDiff) < 180) ? 1 : -1;
        mod =(int) (mod * Mathf.Sign(angleDiff));

        return (laserSpeed * Time.deltaTime) * mod;
    }

    private float GetLaserAngle()
    {
        Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;
        return (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }


    private void OnAttack()
    {
        Vector3 distance = target.transform.position - transform.position;

        if (distance.magnitude < (attackRange * 0.7f))
        {
            ActivateAttack();
            punches++;
        }
    }

    private void ActivateAttack()
    {
        animator.Play("Attack");
        stopTime = attackCooldown;
        currentState = eState.IDLE;
    }


    public void Attack()
    {
        PlayAttackSound();

        //Attempt to attack
        Vector2 position = new Vector2(transform.position.x, transform.position.y) + collider.offset;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(position, attackRange, new Vector2());
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Health health = Player.Instance.GetComponent<Health>();
                if (health)
                {
                    health.SubtractHealth(damage);
                }
                break;
            }
        }
    }
}
