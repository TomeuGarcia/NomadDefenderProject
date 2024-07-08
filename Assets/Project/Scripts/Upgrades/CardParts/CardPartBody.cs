using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardDescriptionProvider;

public class CardPartBody : CardPart, ICardDescriptionProvider
{
    [Header("CARD INFO")]
    [SerializeField] protected CanvasGroup[] cgsInfoHide;

    [Header("Base card info")]


    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI _damageStatValueText;
    [SerializeField] private TextMeshProUGUI _fireRateStatValueText;
    [SerializeField] private CanvasGroup _cgCadence;
    [SerializeField] private CanvasGroup _cgDamage;

    [Header("PART")]
    [SerializeField] public TurretPartBody turretPartBody;

    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private Image bodyImage;
    private Material bodyMaterial;
    [SerializeField] private TurretPartAttack defaultColorTurretPartAttack;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;


    protected override void AwakeInit()
    {
        base.AwakeInit();

        //bodyMaterial = bodyMeshRenderer.material;
        bodyMaterial = new Material(bodyImage.material);
        bodyImage.material = bodyMaterial;
    }

    public override void Init()
    {
        bodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);
        //bodyMaterial.SetColor("_PaintColor", turretPartAttack.materialColor); // Projectile color     ???? WHAT TO DO ????
        bodyMaterial.SetColor("_PaintColor", defaultColorTurretPartAttack.materialColor);

        turretPartBody.SetStatTexts(_damageStatValueText, _fireRateStatValueText);
    }


    protected override void DoShowInfo()
    {
        CardDescriptionDisplayer.GetInstance().ShowCardDescription(this);
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardDescriptionDisplayer.GetInstance().HideCardDescription();
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
            _cgDamage.DOFade(0f, t1);
            _cgCadence.DOFade(0f, t1);
            yield return new WaitForSeconds(t1);

            _cgDamage.DOFade(1f, t1);
            _cgCadence.DOFade(1f, t1);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t1);
        }        
    }



    // ICardDescriptionProvider OVERLOADS
    public ICardDescriptionProvider.SetupData[] GetAbilityDescriptionSetupData()
    {
        ICardDescriptionProvider.SetupData[] setupData = new ICardDescriptionProvider.SetupData[2];

        setupData[0] = new ICardDescriptionProvider.SetupData();
        setupData[1] = null;


        return setupData;
    }

    public Vector3 GetCenterPosition()
    {
        return CardTransform.position + CardTransform.TransformDirection(Vector3.down * 0.2f);
    }


    public DescriptionCornerPositions GetCornerPositions()
    {
        return new DescriptionCornerPositions(leftDescriptionPosition.position, rightDescriptionPosition.position);
    }

}
