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


    public bool AtLocation { get => (targetPos == transform.position); }
    

    public static MenuPlayer Instance { get; set; }

    private void Awake()
    {
        Instance = this;

        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        targetPos = transform.position;
    }

    public void SetTargetPos(Transform targetTransform)
    {
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
}
