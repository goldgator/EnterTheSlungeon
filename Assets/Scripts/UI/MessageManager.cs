using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles pop ups and off-screen pointers
/// </summary>
public class MessageManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Transform popupParent;
    [SerializeField]
    private Image popupImage;
    [SerializeField]
    private RectTransform titleBackground;
    [SerializeField]
    private TMP_Text popupTitle;
    [SerializeField]
    private TMP_Text popupDesc;

    [Header("Values")]
    [SerializeField]
    private float popupSpeed = 950.0f;
    private bool popupActive = false;
    private float popupTimer = 0f;
    private float targetHeight = 35;
    private float restHeight = -300;


    private static MessageManager instance;
    public static MessageManager Instance { get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<MessageManager>();

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        UpdatePopup();
    }

    private void UpdatePopup()
    {
        //Update timer
        popupTimer -= Time.deltaTime;
        if (popupTimer < 0) ClosePopup();

        //Update position
        //Check if active or not
        if (popupActive)
        {
            MovePopup(targetHeight);
        } else
        {
            MovePopup(restHeight);
        }

        UpdatePopupTitleBG();
    }

    private void UpdatePopupTitleBG()
    {
        //Update the title background width to match the title length
        Vector2 size = titleBackground.sizeDelta;
        size.x = ((RectTransform)popupTitle.transform).sizeDelta.x + 40f;

        //Don't bother if sizes are the same
        if (titleBackground.sizeDelta.x == size.x) return;

        titleBackground.sizeDelta = size;
    }

    private void MovePopup(float targetY)
    {
        float distance = targetY - popupParent.transform.position.y;

        //if distance is 0, don't bother
        if (distance == 0) return;
        
        //Get offset
        
        float offset = Utilities.MinAbs(distance, popupSpeed * Mathf.Sign(distance) * Time.deltaTime);

        //Add offset
        popupParent.transform.position += new Vector3(0, offset, 0);
    }


    public void SendPopup(string title, string description, float time = float.MaxValue, Sprite newSprite = null)
    {
        //Set the text of the title to the passed in text
        popupTitle.text = title;
        //Set the text of the description to the passed in text
        popupDesc.text = description;

        //Check if they gave a sprite
        //If they did, set the sprite of the image and enable the object
        if (newSprite != null)
        {
            popupImage.sprite = newSprite;
            popupImage.gameObject.SetActive(true);
        //Otherwise disable the object
        } else
        {
            popupImage.gameObject.SetActive(false);
        }

        //set popupActive to true
        popupActive = true;

        //set popupTimer to passed in value
        popupTimer = time;
    }

    public void SetPopupTime(float time)
    {
        popupTimer = time;
    }

    public void ClosePopup()
    {
        popupTimer = 0;
        popupActive = false;
    }
}

