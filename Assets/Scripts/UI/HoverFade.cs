using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverFade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<ImageFade> imageFades = new List<ImageFade>();
    private float fadeDir = -1;

    private float currentTime = 0;
    private float maxTime = .4f;
    private float alphaRatio = .5f;

    void Awake()
    {
        GrabImages();
    }

    private void GrabImages()
    {
        Image newImage = GetComponent<Image>();

        if (newImage != null) imageFades.Add(new ImageFade(newImage));

        Image[] childImages = GetComponentsInChildren<Image>();

        foreach(Image image in childImages)
        {
            imageFades.Add(new ImageFade(image));
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        fadeDir = 1;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        fadeDir = -1;
    }

    private void Update()
    {
        currentTime = Mathf.Clamp(currentTime + Time.deltaTime * fadeDir, 0, maxTime);

        if (currentTime > 0 && currentTime < maxTime)
        {
            UpdateFadeImages();
        }
    }

    private void UpdateFadeImages()
    {
        foreach(ImageFade fadeImage in imageFades)
        {
            Color color = fadeImage.fadeImage.color;

            float targetAlpha = fadeImage.baseAlpha * alphaRatio;
            color.a = Mathf.Lerp(fadeImage.baseAlpha, targetAlpha, currentTime / maxTime);

            fadeImage.fadeImage.color = color;
        }
    }
}

public struct ImageFade
{
    public Image fadeImage;
    public float baseAlpha;

    public ImageFade(Image newImage)
    {
        fadeImage = newImage;
        baseAlpha = newImage.color.a;
    }
}
