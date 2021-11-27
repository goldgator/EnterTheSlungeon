using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartGameUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private RectTransform gunOption1;
    [SerializeField]
    private RectTransform gunOption2;
    [SerializeField]
    private RectTransform seedInputParent;
    [SerializeField]
    private TMP_InputField worldSeedInput;

    [Header("Info")]
    [SerializeField]
    private float optionOffset;
    [SerializeField]
    private float offscreenOffset;
    [SerializeField]
    private float seedInputOffset;
    [SerializeField]
    private float transitionTime = 1.5f;


    //Unserialized fields
    //private GameObject secondaryGunPrefab;
    private string seed;

    private void Start()
    {
        //Reset Floor seed
        Floor.stringSeed = "";
        Floor.seed = 0;

        //Reset floor generation index
        Floor.currentGenData = 0;

        //Change music
        MusicManager.Instance.PlaySong("StartRun");

        //Set the PlayerCursor to UI mode
        PlayerCursor.Instance.SetCursorUIState(true);
    }

    public void SelectSecondaryGun(GameObject newPrefab)
    {
        Player.secondaryWeapon = newPrefab;
        seed = worldSeedInput.text;
        HideOptions();
    }

    public void LoadFirstFloor()
    {
        SceneDirector.Instance.LoadFloorScene(seed);
    }

    public void SetQuickRun(bool value)
    {
        Floor.quickRun = value;
    }

    public void RevealOptions()
    {
        //Start Coroutine to move them into proper positions
        StartCoroutine(RevealOptionsCoroutine());
    }

    public IEnumerator RevealOptionsCoroutine()
    {
        float timer = 0f;
        float currentOptionOffset;
        float currentInputOffset;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;

            currentOptionOffset = Mathf.Lerp(-offscreenOffset, optionOffset, timer / transitionTime);
            currentInputOffset = Mathf.Lerp(-140, seedInputOffset, timer / transitionTime);

            gunOption1.anchoredPosition = new Vector2(currentOptionOffset, gunOption1.anchoredPosition.y);
            gunOption2.anchoredPosition = new Vector2(-currentOptionOffset, gunOption2.anchoredPosition.y);
            seedInputParent.anchoredPosition = new Vector2(seedInputParent.anchoredPosition.x, currentInputOffset);
            yield return null;
        }
    }

    public void HideOptions()
    {
        StartCoroutine(HideOptionsCoroutine());
    }

    public IEnumerator HideOptionsCoroutine()
    {
        float timer = 0f;
        float currentOptionOffset;
        float currentInputOffset;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;

            currentOptionOffset = Mathf.Lerp(optionOffset, -offscreenOffset, timer / transitionTime);
            currentInputOffset = Mathf.Lerp(seedInputOffset, -140, timer / transitionTime);

            gunOption1.anchoredPosition = new Vector2(currentOptionOffset, gunOption1.anchoredPosition.y);
            gunOption2.anchoredPosition = new Vector2(-currentOptionOffset, gunOption2.anchoredPosition.y);
            
            seedInputParent.anchoredPosition = new Vector2(seedInputParent.anchoredPosition.x, currentInputOffset);
            yield return null;
        }
    }
}
