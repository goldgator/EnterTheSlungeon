using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneDirector : MonoBehaviour
{
    [SerializeField]
    private Image transitionImage1;
    [SerializeField]
    private Image transitionImage2;

    [SerializeField]
    private float transitionTime = 1.0f;
    private float timer;

    private static SceneDirector instance;
    public static SceneDirector Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<SceneDirector>();

            return instance;
        }
    }


    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            //Add event listener
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //Start Untranstiation Coroutine
        Debug.Log("Scene loaded");
        StartCoroutine(OnSceneLoadCoroutine());
    }

    public void LoadScene(string name)
    {
        //Start scene coroutine
        StartCoroutine(LoadSceneCoroutine(name));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        timer = 0;
        float targetWidth = ((RectTransform)transform).sizeDelta.x / 2;
        float height = ((RectTransform)transform).sizeDelta.y;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float currentWidth = Mathf.Lerp(0, targetWidth, timer / transitionTime);
            transitionImage1.rectTransform.sizeDelta = new Vector2(currentWidth, height);
            transitionImage2.rectTransform.sizeDelta = new Vector2(currentWidth, height);
            yield return null;
        }

        //Load the scene
        Debug.Log("Started scene load");
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator OnSceneLoadCoroutine()
    {
        timer = transitionTime;
        float targetWidth = ((RectTransform)transform).sizeDelta.x / 2;
        float height = ((RectTransform)transform).sizeDelta.y;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            float currentWidth = Mathf.Lerp(0, targetWidth, timer / transitionTime);
            transitionImage1.rectTransform.sizeDelta = new Vector2(currentWidth, height);
            transitionImage2.rectTransform.sizeDelta = new Vector2(currentWidth, height);
            yield return null;
        }
    }


    public void CloseGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
   
    
}
