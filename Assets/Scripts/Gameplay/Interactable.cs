using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public bool isOn = true;
    [SerializeField]
    private bool usesKey = true;
    [SerializeField]
    private string interactText;
    private GameObject textObject;

    private static GameObject interactTextPrefab;
    public static GameObject InteractTextPrefab
    {
        get
        {
            if (interactTextPrefab == null) interactTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InteractTextDesc");

            return interactTextPrefab;
        }
    }


    private static float borderThickness = .15f;
    [Header("Events")]
    [SerializeField]
    private UnityEvent interactEvent;
    [SerializeField]
    private UnityEvent inRangeEvent;
    [SerializeField]
    private UnityEvent leaveRangeEvent;
    
    private Material outlineMaterial;
    private SpriteRenderer renderer;
    private Collider2D collider;

    private GameObject outline;

    // Start is called before the first frame update
    void Start()
    {
        FindInterfaces();
    }

    private void Awake()
    {
        outlineMaterial = Resources.Load<Material>("Materials/Flash");
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();

        //Create outline and hide
        CreateOutline();
        if (interactText != null && interactText != "") CreateText();
    }

    private void FindInterfaces()
    {
        IInteractEvent[] interactEvents = GetComponents<IInteractEvent>();

        foreach (IInteractEvent interactable in interactEvents)
        {
            interactEvent.AddListener(new UnityAction(interactable.OnInteract));
            inRangeEvent.AddListener(new UnityAction(interactable.OnInteractEnter));
            leaveRangeEvent.AddListener(new UnityAction(interactable.OnInteractLeave));
        }
    }

    private void CreateOutline()
    {
        outline = new GameObject("Outline");
        SpriteRenderer outlineRenderer = outline.AddComponent<SpriteRenderer>();
        outline.transform.SetParent(transform, false);
        outlineRenderer.sprite = renderer.sprite;
        outlineRenderer.sortingOrder = renderer.sortingOrder - 2;
        outlineRenderer.material = outlineMaterial;


        outline.transform.localScale = DetermineScale();

        outline.SetActive(false);
    }

    private void CreateText()
    {
        textObject = Instantiate(InteractTextPrefab);

        //Add text
        TMP_Text text = textObject.GetComponentInChildren<TMP_Text>();
        text.text = interactText;

        //Add text to self
        textObject.transform.SetParent(transform, false);

        //Find correct position using current sprite
        textObject.transform.localPosition =  new Vector3(0, renderer.bounds.extents.y + .5f, 0);

        //Set active to false
        textObject.SetActive(false);
        
    }

    private Vector3 DetermineScale()
    {
        //Find the base sprite bounds (copies the bounds)
        Bounds targetBounds = renderer.bounds;

        //Create a thickness off the bounds, based on each axis
        float xLength = targetBounds.size.x + borderThickness;
        float yLength = targetBounds.size.y + borderThickness;

        //Return a vector that represents the relative scaling
        return new Vector3(xLength / targetBounds.size.x, yLength / targetBounds.size.y, 1);
    }

    public void TriggerInteractable()
    {
        OnInteract();
    }

    private void OnInteract() {
        interactEvent?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger entered: " + collision.gameObject.name);
        if (collision.CompareTag("Player") && isOn)
        {
            InteractEnter();
        }
    }

    

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isOn)
        {
            InteractLeave();
        }
    }

    private void InteractEnter()
    {
        //Subscribe to Interact listener
        if (usesKey) InputManager.Instance.interactStartEvent += OnInteract;

        //Show outline
        outline.SetActive(true);

        //Show text if available
        textObject?.SetActive(true);

        //Update outline size
        outline.transform.localScale = DetermineScale();

        //Create Prompt
        if (PlayerInteractPrompt.InstanceExists()) PlayerInteractPrompt.Instance?.ShowPrompt();

        //Trigger relevant events
        inRangeEvent?.Invoke();
    }

    private void InteractLeave()
    {
        //Unsubscribe from Interact listener
        if (usesKey) InputManager.Instance.interactStartEvent -= OnInteract;

        //Hide outline 
        outline.SetActive(false);

        //Hide text if available
        textObject?.SetActive(false);

        //Remove Prompt
        if (PlayerInteractPrompt.InstanceExists()) PlayerInteractPrompt.Instance?.HidePrompt();

        //Trigger relevant events
        leaveRangeEvent?.Invoke();
    }

    public void SetInteractable(bool state)
    {
        isOn = state;

        if (!isOn)
        {
            InteractLeave();
        }
    }
}
