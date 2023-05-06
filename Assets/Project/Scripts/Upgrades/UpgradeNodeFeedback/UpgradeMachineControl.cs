using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;

public class UpgradeMachineControl : MonoBehaviour
{
    [Header("REFERENCES")]
    [Header("Left")]
    [SerializeField] private MeshRenderer leftArm;
    [SerializeField] private UpgradeCardSlot leftCardSlot;
    [SerializeField] private List<MeshRenderer> leftCables = new List<MeshRenderer>();
    [SerializeField] private Light leftLight;

    [Header("Right")]
    [SerializeField] private MeshRenderer rightArm;
    [SerializeField] private UpgradeCardSlot rightCardSlot;
    [SerializeField] private List<MeshRenderer> rightCables = new List<MeshRenderer>();
    [SerializeField] private Light rightLight;

    [Header("Screen")]
    [SerializeField] private UpgradeCardSlot screenCardSlot;
    [SerializeField] private MeshRenderer screen;
    [SerializeField] private MeshRenderer screenButton;
    [SerializeField] private MeshRenderer screenButtonOutline;
    [SerializeField] private SpriteRenderer screenButtonText;
    [SerializeField] private Color32 screenButtonActiveTextColor;
    private Color32 screenButtonUnactiveTextColor;



    [Header("MATERIAL LERP DATA")]
    [SerializeField] private MaterialLerp.FloatData screenTransitionFD;
    [SerializeField] private MaterialLerp.FloatData armOnFD;
    [SerializeField] private MaterialLerp.FloatData armOffFD;
    [SerializeField] private MaterialLerp.FloatData screenFD;
    [SerializeField] private MaterialLerp.FloatData buttonFD;
    [SerializeField] private MaterialLerp.FloatData cableIndAlphaFD;
    [SerializeField] private MaterialLerp.FloatData cableTransitionCoefFD;
    [SerializeField] private MaterialLerp.FloatData cableEnergyCoefFD;



    private List<Material> tempMaterials = new List<Material>();


    public delegate void UpgradeMachineControlAction();
    public event UpgradeMachineControlAction OnReplaceStart;
    public event UpgradeMachineControlAction OnReplaceCardPrinted;


    private void Awake()
    {
        RenderSettings.reflectionIntensity = 0.0f;

        StartCoroutine(Activate());
        StartCoroutine(LightBlink(rightLight));
        //SCREEN TURN ON
    }
    private void OnDestroy()
    {
        RenderSettings.reflectionIntensity = 1.0f;
    }

    private IEnumerator Activate()
    {
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(leftCardSlot.Activate());
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(rightCardSlot.Activate());
    }

    private IEnumerator LightBlink(Light lightSource)
    {
        int blinksBeforeRest = Random.Range(1, 6);
        for (int i = 0; i < blinksBeforeRest; i++)
        {
            lightSource.enabled = false;
            yield return new WaitForSeconds(Random.Range(0.01f, 0.175f));
            lightSource.enabled = true;
            yield return new WaitForSeconds(Random.Range(0.01f, 0.175f));
        }

        yield return new WaitForSeconds(2.0f + Random.Range(0.0f, 4.0f));
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(LightBlink(lightSource));
        }
    }

    void Update()
    {
        //left
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectLeftCard();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            RetrieveLeftCard();
        }

        //right
        if (Input.GetKeyDown(KeyCode.A))
        {
            SelectRightCard();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            RetrieveRightCard();
        }

        //button
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ActivateButton();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            DeactivateButton();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Replace();
        }
    }

    public void SelectLeftCard()
    {
        leftCardSlot.PulsePanel(1);
        StartCoroutine(MaterialLerp.FloatLerp(armOffFD, new Material[1] { leftArm.materials[1] }));

        screenFD.variableReference = "_FirstFillCoef";
        screenFD.invert = false;
        StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));

        //Cable IndAlpha a 0.5
        cableIndAlphaFD.invert = false;
        tempMaterials.Clear();
        foreach (MeshRenderer mesh in leftCables)
        {
            tempMaterials.Add(mesh.material);
        }
        StartCoroutine(MaterialLerp.FloatLerp(cableIndAlphaFD, tempMaterials.ToArray()));
    }

    public void RetrieveLeftCard()
    {
        leftCardSlot.PulsePanel(0);
        StartCoroutine(MaterialLerp.FloatLerp(armOnFD, new Material[1] { leftArm.materials[1] }));

        screenFD.variableReference = "_FirstFillCoef";
        screenFD.invert = true;
        StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));

        //Cable IndAlpha a 0
        cableIndAlphaFD.invert = true;
        tempMaterials.Clear();
        foreach (MeshRenderer mesh in leftCables)
        {
            tempMaterials.Add(mesh.material);
        }
        StartCoroutine(MaterialLerp.FloatLerp(cableIndAlphaFD, tempMaterials.ToArray()));
    }

    public void SelectRightCard()
    {
        rightCardSlot.PulsePanel(1);
        StartCoroutine(MaterialLerp.FloatLerp(armOffFD, new Material[1] { rightArm.materials[1] }));

        screenFD.variableReference = "_SecondFillCoef";
        screenFD.invert = false;
        StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));

        //Cable IndAlpha a 0.5
        cableIndAlphaFD.invert = false;
        tempMaterials.Clear();
        foreach (MeshRenderer mesh in rightCables)
        {
            tempMaterials.Add(mesh.material);
        }
        StartCoroutine(MaterialLerp.FloatLerp(cableIndAlphaFD, tempMaterials.ToArray()));
    }

    public void RetrieveRightCard()
    {
        rightCardSlot.PulsePanel(0);
        StartCoroutine(MaterialLerp.FloatLerp(armOnFD, new Material[1] { rightArm.materials[1] }));

        screenFD.variableReference = "_SecondFillCoef";
        screenFD.invert = true;
        StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));

        //Cable IndAlpha a 0
        cableIndAlphaFD.invert = true;
        tempMaterials.Clear();
        foreach (MeshRenderer mesh in rightCables)
        {
            tempMaterials.Add(mesh.material);
        }
        StartCoroutine(MaterialLerp.FloatLerp(cableIndAlphaFD, tempMaterials.ToArray()));
    }

    public void ActivateButton()
    {
        screenTransitionFD.invert = false;
        StartCoroutine(MaterialLerp.FloatLerp(screenTransitionFD, new Material[1] { screen.materials[1] }));
        screenButtonText.material.SetFloat("_ReplaceCoef", 1.0f);

        //lerp button out
        screenButton.transform.DOLocalMoveY(0.0f, 0.2f);
        buttonFD.invert = false;
        StartCoroutine(MaterialLerp.FloatLerp(buttonFD, new Material[1] { screenButtonOutline.materials[1] }));
    }

    public void DeactivateButton()
    {
        screenTransitionFD.invert = true;
        StartCoroutine(MaterialLerp.FloatLerp(screenTransitionFD, new Material[1] { screen.materials[1] }));
        screenButtonText.material.SetFloat("_ReplaceCoef", 0.0f);

        //lerp button in
        screenButton.transform.DOLocalMoveY(-0.082f, 0.2f);
        buttonFD.invert = true;
        StartCoroutine(MaterialLerp.FloatLerp(buttonFD, new Material[1] { screenButtonOutline.materials[1] }));
    }

    public void Replace()
    {
        if (OnReplaceStart != null) OnReplaceStart();

        rightCardSlot.PulsePanel(0);
        leftCardSlot.PulsePanel(0);
        screenCardSlot.PulsePanel(0);
        screenButtonText.material.SetFloat("_ReplaceCoef", 0.0f);

        //Button disappear
        screenButton.transform.DOLocalMoveY(-0.3f, 0.25f);
        screenButtonOutline.transform.DOLocalMoveY(-0.3f, 0.6f);
        buttonFD.invert = true;
        StartCoroutine(MaterialLerp.FloatLerp(buttonFD, new Material[1] { screenButtonOutline.materials[1] }));

        //Lights
        leftLight.gameObject.SetActive(false);
        rightLight.gameObject.SetActive(false);

        //Drag in the cards
        leftCardSlot.Insert();
        rightCardSlot.Insert();

        StartCoroutine(EnergyFill());
    }

    private IEnumerator EnergyFill()
    {
        //Cable TransitionCoef a 1
        tempMaterials.Clear();
        foreach (MeshRenderer mesh in rightCables)
        {
            tempMaterials.Add(mesh.material);
        }
        foreach (MeshRenderer mesh in leftCables)
        {
            tempMaterials.Add(mesh.material);
        }
        StartCoroutine(MaterialLerp.FloatLerp(cableTransitionCoefFD, tempMaterials.ToArray()));
        //yield return new WaitForSeconds(0.75f);

        //Cable Energy a 1
        yield return new WaitForSeconds(cableTransitionCoefFD.time);
        //cableIndAlphaFD.invert = false;
        tempMaterials.Clear();
        foreach (MeshRenderer mesh in rightCables)
        {
            tempMaterials.Add(mesh.material);
        }
        foreach (MeshRenderer mesh in leftCables)
        {
            tempMaterials.Add(mesh.material);
        }
        StartCoroutine(MaterialLerp.FloatLerp(cableEnergyCoefFD, tempMaterials.ToArray()));
        
        yield return new WaitForSeconds(1.5f);
        screenTransitionFD.invert = true;
        screenTransitionFD.time = 0.25f;
        StartCoroutine(MaterialLerp.FloatLerp(screenTransitionFD, new Material[1] { screen.materials[1] }));

        if (OnReplaceCardPrinted != null) OnReplaceCardPrinted();
        screenCardSlot.Extract();
    }
}
