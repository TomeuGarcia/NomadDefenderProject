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
    [SerializeField] private TMP_Text screenButtonText;
    [SerializeField] private Color32 screenButtonActiveTextColor;
    private Color32 screenButtonUnactiveTextColor;
    [SerializeField] private Material offScreenMat;



    [Header("MATERIAL LERP DATA")]
    [SerializeField] private MaterialLerp.FloatData armOnFD;
    [SerializeField] private MaterialLerp.FloatData armOffFD;
    [SerializeField] private MaterialLerp.FloatData screenFD;
    [SerializeField] private MaterialLerp.FloatData buttonFD;
    [SerializeField] private MaterialLerp.FloatData cableIndAlphaFD;
    [SerializeField] private MaterialLerp.FloatData cableTransitionCoefFD;
    [SerializeField] private MaterialLerp.FloatData cableEnergyCoefFD;



    private List<Material> tempMaterials = new List<Material>();



    private void Awake()
    {
        RenderSettings.reflectionIntensity = 0.0f;

        StartCoroutine(ActivateSlots());
        StartCoroutine(LightBlink(rightLight));
        //SCREEN TURN ON
    }
    private void OnDestroy()
    {
        RenderSettings.reflectionIntensity = 1.0f;
    }

    private IEnumerator ActivateSlots()
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
        screenButtonText.color = screenButtonActiveTextColor;

        //lerp button out
        screenButton.transform.DOLocalMoveY(0.0f, 0.2f);
        buttonFD.invert = false;
        StartCoroutine(MaterialLerp.FloatLerp(buttonFD, new Material[1] { screenButtonOutline.materials[1] }));
    }

    public void DeactivateButton()
    {
        screenButtonText.color = screenButtonUnactiveTextColor;

        //lerp button in
        screenButton.transform.DOLocalMoveY(-0.082f, 0.2f);
        buttonFD.invert = true;
        StartCoroutine(MaterialLerp.FloatLerp(buttonFD, new Material[1] { screenButtonOutline.materials[1] }));
    }

    public void Replace()
    {
        screenButtonText.color = screenButtonUnactiveTextColor;
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
        //yield return new WaitForSeconds(0.75f);
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
        
        yield return new WaitForSeconds(1.0f);
        screenCardSlot.Extract();
    }
}
