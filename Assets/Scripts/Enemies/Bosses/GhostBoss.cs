using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostBoss : BaseEnemy
{
    [Header("Boss attributes")]
    public GameObject summon;
    public Image screenDarken;
    public AudioClip summonSound;
    public int baseSummonAmount = 5;
    public float addSummonRatio = .15f;
    public float summonLength = 3.5f;


    private int swipes = 0;

    public enum eAction
    {
        MOVE,
        SUMMON,
        DARKEN,
        RANDOM
    }

    public List<eAction> actionList = new List<eAction>();
    private int currentAction = 0;
    private bool inAction = false;

    protected override void Start()
    {
        base.Start();
        MusicManager.Instance.PlaySong("Boss2");
        stopTime = RNGManager.GetEventRand(attackCooldown / 4f, attackCooldown / 2f);
        
    }

    private void Update()
    {
        stopTime -= Time.deltaTime;
        if (!inAction && stopTime < 0)
        {
            inAction = true;
            RunNextAction();
        }
    }

    public override void Death()
    {
        //Find all spawned enemies and kill them
        RushEnemy[] spawnedEnemies = GameObject.FindObjectsOfType<RushEnemy>();
        foreach (RushEnemy enemy in spawnedEnemies)
        {
            enemy.Death();
        }

        base.Death();
    }


    private void RunNextAction()
    {
        switch (actionList[currentAction])
        {
            case eAction.MOVE:
                StartCoroutine(MoveAction(3f));
                break;
            case eAction.SUMMON:
                StartCoroutine(SummonAction(DetermineSummonCount()));
                break;
            case eAction.DARKEN:
                StartCoroutine(DarkenAction(3));
                break;
            case eAction.RANDOM:
                break;
            default:
                break;
        }
        currentAction = (currentAction + 1) % actionList.Count;
    }

    private int DetermineSummonCount()
    {
        int summonAddition = (int) (((health.maxHealth - health.GetHealth()) / health.maxHealth) / addSummonRatio);
        return summonAddition + baseSummonAmount;
    }
    private IEnumerator SummonAction(int summonNum)
    {
        animator.SetBool("Summoning", true);
        float angleDiff = 360f / summonNum;
        for (int i = 0; i < summonNum; i++)
        {
            Vector3 offset = Utilities.GetUnitVector2(angleDiff * i) * 2f;
            Instantiate(summon, transform.position + offset, Quaternion.identity);
            yield return new WaitForSeconds(summonLength / summonNum);
        }
        stopTime = attackCooldown;
        inAction = false;
        animator.SetBool("Summoning", false);
    }

    private IEnumerator MoveAction(float chaseTime)
    {
        float currentTime = 0f;

        while (chaseTime > currentTime)
        {
            currentTime += Time.deltaTime;

            //Get distance vector
            Vector3 distance = target.transform.position - transform.position;

            //Orient sprite
            transform.localScale = new Vector3(Mathf.Sign(distance.x), transform.localScale.y, transform.localScale.z);

            //if (distance.magnitude > attackRange)
            Vector3 velocity = speed * distance.normalized;

            velocity = velocity * Time.deltaTime;
            transform.position += velocity;

            if (distance.magnitude < (attackRange * 0.7f))
            {
                ActivateAttackCombo();
                currentTime += chaseTime;
            }

            yield return null;
        }

        if (inAction) ActivateAttackCombo();
    }

    private void ActivateAttackCombo()
    {
        animator.Play("AttackCombo");
        swipes++;
        inAction = false;
        stopTime = attackCooldown;
    }

    public void FaceTarget()
    {
        //Get distance vector
        Vector3 distance = target.transform.position - transform.position;

        //Orient sprite
        transform.localScale = new Vector3(Mathf.Sign(distance.x), transform.localScale.y, transform.localScale.z);
    }
    public void Attack()
    {
        PlaySound(attackSound, false);

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

    private IEnumerator DarkenAction(int attackNum)
    {
        //Start Dimming the screen
        StartCoroutine(SetScreenDarken(2f, .9f));

        //Loop as many times as required
        for (int i = 0; i < attackNum; i++)
        {
            //Fade out anim
            Debug.Log("Playing fade anim");
            animator.Play("Fade");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 1.5f);

            //Fade in and attack anim
            Debug.Log("Playing fadeAttack anim");
            animator.Play("FadeAttack");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 1f);
        }

        //Remove the dimmed screen
        StartCoroutine(SetScreenDarken(1f, 0f));

        //Reset
        inAction = false;
        stopTime = attackCooldown;
    }

    public void TeleportToTarget()
    {
        //Teleport to location
        transform.position = ReturnTeleportLocation();
    }
    private Vector3 ReturnTeleportLocation()
    {
        //Choose left or right at random
        float direction = Mathf.Sign(RNGManager.GetEventRand(-1f, 1f));

        //Get location
        Vector3 location = target.transform.position + Vector3.right * direction * 2f;

        //Check if location is safe
        int layerMask = 1 << 0;
        RaycastHit2D hit = Physics2D.CircleCast(location, 1.5f, new Vector2(direction, 0), layerMask);

        //If it hit something
        if (hit != null)
        {
            //Just return opposite location
            return target.transform.position + Vector3.right * -direction * 2f;
        } else
        {
            return location;
        }
    }
    public IEnumerator SetScreenDarken(float darkenTime, float darkenTarget)
    {
        float timer = 0f;

        float start = screenDarken.color.a;
        float end = darkenTarget;

        Debug.Log(start + "-> " + end);

        while (timer < darkenTime)
        {
            timer += Time.deltaTime;

            Color currentColor = screenDarken.color;
            currentColor.a = Mathf.Lerp(start, end, timer / darkenTime);
            //Debug.Log(currentColor.a);
            screenDarken.color = currentColor;

            yield return null;
        }
    }


    public void FadeIn()
    {
        StartSpriteFade(.5f, false);
    }

    public void FadeOut()
    {
        StartSpriteFade(.9f, true);
    }
    public void StartSpriteFade(float time, bool fadeOut)
    {
        //Turn off collider
        //Start FadeCoroutine
        StartCoroutine(Fade(time, fadeOut));
    }

    public void TurnOffCollider()
    {
        collider.enabled = false;
    }

    public void TurnOnCollider()
    {
        collider.enabled = true;
    }

    public IEnumerator Fade(float fadeTime, bool fadeOut)
    {
        float currentTime = 0;

        float start = (fadeOut) ? 1f : 0f;
        float end = (fadeOut) ? 0f : 1f;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;

            Color currentColor = renderer.color;
            currentColor.a = Mathf.Lerp(start, end, currentTime / fadeTime);
            //Debug.Log(currentColor.a);
            renderer.color = currentColor;

            yield return null;
        }
    }

    private void PlaySound(AudioClip sound, bool loop)
    {
        //Play sound
        if (audioSource)
        {
            //Debug.Log("Play Attack");
            audioSource.clip = sound;
            audioSource.loop = loop;
            audioSource.Play();
        }
    }

    private void StopPlayback()
    {
        if (audioSource)
        {
            audioSource.Stop();
        }
    }


}
