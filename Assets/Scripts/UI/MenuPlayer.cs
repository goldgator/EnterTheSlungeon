using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class MenuPlayer : MonoBehaviour
{
    private Vector3 targetPos;
    [SerializeField]
    private float speed;


    private Animator animator;
    private SpriteRenderer renderer;

    [SerializeField]
    private Animator trapdoorAnimator;

    public bool listening = true;
    public bool AtLocation { get => (targetPos == transform.position); }
    

    public static MenuPlayer Instance { get; set; }

    private void Awake()
    {
        Instance = this;

        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        trapdoorAnimator.speed = 0;

        //Kill any player instance that might be alive
        if (Player.InstanceExists()) Player.DestroyInstance();
        if (PlayerCamera.InstanceExists()) PlayerCamera.DestroyInstance();

        //Set music
        MusicManager.Instance.PlaySong("MainTheme");
    }

    private void Start()
    {
        targetPos = transform.position;
    }

    public void SetTargetPos(Transform targetTransform)
    {
        //Ignore if not listening
        if (!listening) return;

        targetPos = targetTransform.position;
    }

    private void Update()
    {
        MoveToLocation();
        UpdateAnims();
    }

    private void MoveToLocation()
    {
        //If already at location, don't bother
        if (AtLocation) return;

        Vector3 distance = targetPos - transform.position;

        //Get lower magnitude
        float magnitude = Utilities.MinAbs(distance.magnitude, (speed * distance.normalized * Time.deltaTime).magnitude);

        //Add offset
        transform.position += distance.normalized * magnitude;
    }

    private void UpdateAnims()
    {
        if (AtLocation)
        {
            animator.SetFloat("Speed", 0);
        } else
        {
            animator.SetFloat("Speed", speed);

            //flip sprite according to direction of target pos
            bool isLeft = ((targetPos.x - transform.position.x) < 0);

            renderer.flipX = isLeft;
        }
    }

    public void StartFloorAnim()
    {
        //Set speed to 0 so it doesn't move
        listening = false;

        //Start coroutine so it waits till they are in place
        StartCoroutine(StartFloorCoroutine());
    }

    private IEnumerator StartFloorCoroutine()
    {
        while (!AtLocation) yield return null;

        animator.SetTrigger("Fall");
        trapdoorAnimator.speed = 1;
    }

    private void StartFloor()
    {
        SceneDirector.Instance.LoadScene("StartRun");
    }
}
