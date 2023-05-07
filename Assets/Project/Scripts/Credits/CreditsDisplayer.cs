using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsDisplayer : MonoBehaviour
{

    [System.Serializable]
    private struct CreditsBlock
    {
        [SerializeField] public string title;
        [SerializeField, Min(0)] public float preSpawnHeightGap;
        [SerializeField] public string[] items;
    };

    [System.Serializable]
    private struct CreditsSettings
    {
        [Header("BIG TITLE")]
        [SerializeField] public Color bigTitleColor;
        [SerializeField] public TMPro.TMP_FontAsset bigTitleFont;
        [SerializeField, Min(1)] public int bigTitleFontSize;
        [SerializeField, Min(1)] public float bigTitleHeightGap;
        [Header("TITLE")]
        [SerializeField] public Color titleColor;
        [SerializeField] public TMPro.TMP_FontAsset titleFont;
        [SerializeField, Min(1)] public int titleFontSize;
        [SerializeField, Min(1)] public float titleHeightGap;
        [Header("ITEMS")]
        [SerializeField] public Color itemsColor;
        [SerializeField] public TMPro.TMP_FontAsset itemsFont;
        [SerializeField, Min(1)] public int itemsFontSize;
        [SerializeField, Min(1)] public float itemHeightGap;
        [Header("OTHER")]
        [SerializeField, Min(1)] public float heightGapBetweenBlocks; 
        [SerializeField, Min(0.0001f)] public float scrollSpeed;
        [SerializeField] public GameObject textObjectReference;
        [SerializeField] public Transform parentTransform;
    };

    [System.Serializable]
    private struct CreditsContent
    {
        [SerializeField] public string bigTitleText;
        [SerializeField] public CreditsBlock[] blocks;
    }


    [SerializeField] private KeyCode creditsSpeedUpButton = KeyCode.Mouse0;
    [SerializeField] private CreditsSettings settings;
    [SerializeField] private CreditsContent content;

    private RectTransform lastSpawnedTransform;
    private bool hasReachedEndPosition;
    private bool hasFinished;
    private bool textsAlreadySpawned;
    private Vector3 textsHolderStartPosition;
    private float defaultCreditsScrollSpeed;

    private Coroutine checkCreditsFinishedCoroutine = null;
    private Coroutine moveCreditsCoroutine = null;

    private Transform lastTitleTransform;

    public delegate void CreditsDisplayerAction();
    public event CreditsDisplayerAction OnCreditsFinished;



    private void Awake()
    {
        settings.textObjectReference.SetActive(false);        
        textsAlreadySpawned = false;
        textsHolderStartPosition = settings.parentTransform.position;
        defaultCreditsScrollSpeed = settings.scrollSpeed;

        //StartCredits(); /////////
    }
    private void Update()
    {
        if (Input.GetKeyDown(creditsSpeedUpButton)) settings.scrollSpeed = defaultCreditsScrollSpeed * 4f;
        else if (Input.GetKeyUp(creditsSpeedUpButton)) settings.scrollSpeed = defaultCreditsScrollSpeed;
    }

    public void ResetCredits() // Call this to reset credits if already playing
    {
        if (!hasFinished)
        {
            ForceFinish(true);
        }
        StartCredits();
    }

    public void ForceFinish(bool invokeFinishEvents) // Call this if you want to quit credits
    {
        if (checkCreditsFinishedCoroutine != null) StopCoroutine(checkCreditsFinishedCoroutine);
        if (moveCreditsCoroutine != null) StopCoroutine(moveCreditsCoroutine);
        
        if (invokeFinishEvents)
        {
            Finish();
        }
        else
        {
            hasFinished = true;
        }
    }

    public void StartCredits()
    {
        hasFinished = false;
        hasReachedEndPosition = false;
        settings.parentTransform.position = textsHolderStartPosition;

        if (!textsAlreadySpawned)
        {
            SpawnCredits(content.blocks, content.bigTitleText);
        }

        checkCreditsFinishedCoroutine = StartCoroutine(CheckCreditsHaveFinished());
        moveCreditsCoroutine = StartCoroutine(MoveTexts());
    }

    private IEnumerator CheckCreditsHaveFinished()
    {
        yield return new WaitUntil(() => CreditsReachedEnd());
        checkCreditsFinishedCoroutine = null;
        hasReachedEndPosition = true;
        //Finish();
        CreditsEndAnimation();
    }
    private void Finish()
    {
        hasFinished = true;
        Debug.Log("CREDITS FINISHED");
        if (OnCreditsFinished != null) OnCreditsFinished();
    }

    private bool CreditsReachedEnd()
    {
        return lastSpawnedTransform.position.y > Screen.height / 2f;
    }


    private void SpawnCredits(CreditsBlock[] blocks, string bigTitleText)
    {
        float accomulatedDistance = 500f;

        SpawnBigTitleText(bigTitleText, ref accomulatedDistance);

        for (int blockI = 0; blockI < blocks.Length; ++blockI)
        {
            accomulatedDistance += blocks[blockI].preSpawnHeightGap;

            SpawnTitleText(blocks, blockI, ref accomulatedDistance);

            for (int itemI = 0; itemI < blocks[blockI].items.Length; ++itemI)
            {
                SpawnItemText(blocks, blockI, itemI, ref accomulatedDistance);
            }

            accomulatedDistance += settings.heightGapBetweenBlocks;
        }

        textsAlreadySpawned = true;
    }


    private void SpawnBigTitleText(string bigTitleText, ref float accomulatedDistance)
    {
        TextMeshProUGUI titleText = Instantiate(settings.textObjectReference, settings.parentTransform).GetComponent<TextMeshProUGUI>();
        titleText.gameObject.SetActive(true);

        float distance = settings.titleHeightGap * settings.titleFontSize;
        accomulatedDistance += distance;

        Vector3 titlePosition = Vector3.up * -accomulatedDistance;
        titleText.rectTransform.localPosition = titlePosition;

        titleText.font = settings.bigTitleFont;
        titleText.fontSize = settings.bigTitleFontSize;
        titleText.text = bigTitleText;
        titleText.color = settings.bigTitleColor;

        lastSpawnedTransform = titleText.rectTransform;
    }

    private void SpawnTitleText(CreditsBlock[] blocks, int blockI, ref float accomulatedDistance)
    {
        TextMeshProUGUI titleText = Instantiate(settings.textObjectReference, settings.parentTransform).GetComponent<TextMeshProUGUI>();
        titleText.gameObject.SetActive(true);

        float distance = settings.titleHeightGap * settings.titleFontSize;
        accomulatedDistance += distance;

        Vector3 titlePosition = Vector3.up * -accomulatedDistance;
        titleText.rectTransform.localPosition = titlePosition;

        titleText.font = settings.titleFont;
        titleText.fontSize = settings.titleFontSize;
        titleText.text = blocks[blockI].title;
        titleText.color = settings.titleColor;

        lastSpawnedTransform = titleText.rectTransform;

        lastTitleTransform = titleText.rectTransform;
    }

    private void SpawnItemText(CreditsBlock[] blocks, int blockI, int itemI, ref float accomulatedDistance)
    {
        TextMeshProUGUI itemText = Instantiate(settings.textObjectReference, settings.parentTransform).GetComponent<TextMeshProUGUI>();
        itemText.gameObject.SetActive(true);

        float distance = settings.itemHeightGap * settings.itemsFontSize;
        accomulatedDistance += distance;

        Vector3 itemPosition = Vector3.up * -accomulatedDistance;
        itemText.rectTransform.localPosition = itemPosition;

        itemText.font = settings.itemsFont;
        itemText.fontSize = settings.itemsFontSize;
        itemText.text = blocks[blockI].items[itemI];
        itemText.color = settings.itemsColor;

        lastSpawnedTransform = itemText.rectTransform;
    }


    private IEnumerator MoveTexts()
    {
        while (!hasReachedEndPosition)
        {
            Vector3 textsHolderPosition = settings.parentTransform.position;
            textsHolderPosition.y += Time.deltaTime * settings.scrollSpeed;
            settings.parentTransform.position = textsHolderPosition;
            yield return null;
        }

        moveCreditsCoroutine = null;
    }


    private void CreditsEndAnimation()
    {
        GameAudioManager.GetInstance().PlayCardSelected();
        lastTitleTransform.DOPunchScale(Vector3.one * 0.1f, 1.5f, 4).OnComplete(() => Finish() );
    }

}
