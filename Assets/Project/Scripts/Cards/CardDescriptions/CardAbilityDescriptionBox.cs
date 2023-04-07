using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardAbilityDescriptionBox : MonoBehaviour
{
    [Header("TEXTS")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("ICON")]
    [SerializeField] private Image iconImage;

    [Header("OTHER")]
    [SerializeField] private GameObject parentHolder;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private CanvasGroup cgNameAndIcon;
    [SerializeField] private CanvasGroup cgDescription;

    private Coroutine activeAnimationCoroutine;
    private bool isHiding = false;

    private static Quaternion rotationPositionedAtRight = Quaternion.Euler(0f, 180f, 0f);
    private static Quaternion rotationPositionedAtLeft= Quaternion.identity;



    private void Awake()
    {
        backgroundImage.fillAmount = 0f;
        cgNameAndIcon.alpha = 0f;
        cgDescription.alpha = 0f;
        parentHolder.SetActive(false);        
    }

    public void SetupTextsAndIcon(ICardDescriptionProvider.SetupData cardDescriptionSetupData)
    {
        nameText.text = "/" + cardDescriptionSetupData.abilityName;
        descriptionText.text = cardDescriptionSetupData.abilityDescription;

        iconImage.sprite = cardDescriptionSetupData.icon;
        iconImage.color = cardDescriptionSetupData.iconColor;
    }





    public void Show(bool positionAtTheRight)
    {
        if (activeAnimationCoroutine != null)
        {
            StopCoroutine(activeAnimationCoroutine);
        }

        isHiding = false;
        activeAnimationCoroutine = StartCoroutine(PlayShowAnimation(positionAtTheRight));
    }

    private IEnumerator PlayShowAnimation(bool positionAtTheRight)
    {        
        parentHolder.SetActive(true);
        backgroundImage.DOComplete();
        cgNameAndIcon.DOComplete();
        cgDescription.DOComplete();


        float t1 = 0.1f;
        float t2 = 0.2f;


        backgroundImage.rectTransform.rotation = positionAtTheRight ? rotationPositionedAtRight : rotationPositionedAtLeft;

        backgroundImage.DOFillAmount(1f, t2);
        yield return new WaitForSeconds(t1);

        cgNameAndIcon.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        cgDescription.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        activeAnimationCoroutine = null;
    }


    public void Hide()
    {
        if (isHiding) return;

        if (activeAnimationCoroutine != null)
        {
            StopCoroutine(activeAnimationCoroutine);
        }

        isHiding = true;
        activeAnimationCoroutine = StartCoroutine(PlayHideAnimation());
    }

    private IEnumerator PlayHideAnimation()
    {
        backgroundImage.DOComplete();
        cgNameAndIcon.DOComplete();
        cgDescription.DOComplete();


        float t1 = 0.1f;

        cgDescription.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cgNameAndIcon.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        backgroundImage.DOFillAmount(0f, t1);
        yield return new WaitForSeconds(t1);



        activeAnimationCoroutine = null;
        parentHolder.SetActive(false);
        isHiding = false;
    }


}
