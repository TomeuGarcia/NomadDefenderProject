using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPartBody : CardPart
{
    [Header("CARD INFO")]
    [SerializeField] protected CanvasGroup[] cgsInfoHide;

    [Header("Base card info")]
    [SerializeField] private TextMeshProUGUI bodyNameText;
    [SerializeField] private TextMeshProUGUI bodyDescriptionText;


    [Header("CANVAS COMPONENTS")]
    [SerializeField] private Image damageFillImage;
    [SerializeField] private Image cadenceFillImage;
    [SerializeField] private CanvasGroup cgCadence;
    [SerializeField] private CanvasGroup cgDamage;

    [Header("PART")]
    [SerializeField] public TurretPartBody turretPartBody;

    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private Image bodyImage;
    private Material bodyMaterial;




    protected override void AwakeInit()
    {
        base.AwakeInit();

        //bodyMaterial = bodyMeshRenderer.material;
        bodyMaterial = new Material(bodyImage.material);
        bodyImage.material = bodyMaterial;

        SetupCardInfo();
    }

    public override void Init()
    {
        bodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);
        //bodyMaterial.SetColor("_PaintColor", turretPartAttack.materialColor); // Projectile color     ???? WHAT TO DO ????

        damageFillImage.fillAmount = turretPartBody.GetDamagePer1();
        cadenceFillImage.fillAmount = turretPartBody.GetCadencePer1();

        InitInfoVisuals();
    }


    protected override void InitInfoVisuals()
    {
        bodyNameText.text = '/' + turretPartBody.partName;
        bodyDescriptionText.text = turretPartBody.abilityDescription;
    }
    protected override void SetupCardInfo()
    {
        // general
        infoInterface.SetActive(true);
        isShowInfoAnimationPlaying = false;

        // body
        bodyNameText.alpha = 0;
        bodyDescriptionText.alpha = 0;
    }

    public override void ShowInfo()
    {
        base.ShowInfo();

        showInfoCoroutine = StartCoroutine(ShowInfoAnimation());
    }

    public override void HideInfo()
    {
        if (isHideInfoAnimationPlaying) return;

        base.HideInfo();

        if (isShowInfoAnimationPlaying)
        {
            StopCoroutine(showInfoCoroutine);
        }

        StartCoroutine(HideInfoAnimation());
    }

    private IEnumerator ShowInfoAnimation()
    {
        canInfoInteract = false;
        isShowInfoAnimationPlaying = true;

        // hide generics
        float t = 0.05f;
        for (int i = 0; i < cgsInfoHide.Length; ++i)
        {
            cgsInfoHide[i].DOFade(0f, t);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t);
        }


        float t2 = 0.1f;

        // show body text
        bodyNameText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);
        bodyDescriptionText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);


        canInfoInteract = true;
        isShowInfoAnimationPlaying = false;
    }

    private IEnumerator HideInfoAnimation()
    {
        canInfoInteract = false;


        float t2 = 0.1f;

        // hide body text
        bodyDescriptionText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);
        bodyNameText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);


        // show generics
        float t = 0.05f;

        for (int i = cgsInfoHide.Length - 1; i >= 0; --i)
        {
            cgsInfoHide[i].DOFade(1f, t);
            GameAudioManager.GetInstance().PlayCardInfoHidden();
            yield return new WaitForSeconds(t);
        }


        canInfoInteract = true;
    }


    public void PlayTutorialBlinkAnimation()
    {
        StartCoroutine(TutorialBlinkAnimation());
    }
    private IEnumerator TutorialBlinkAnimation()
    {
        float t1 = 0.1f;

        for (int i = 0; i < 8; ++i)
        {
            cgDamage.DOFade(0f, t1);
            cgCadence.DOFade(0f, t1);
            yield return new WaitForSeconds(t1);

            cgDamage.DOFade(1f, t1);
            cgCadence.DOFade(1f, t1);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t1);
        }        
    }


}
