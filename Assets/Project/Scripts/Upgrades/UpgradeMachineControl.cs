using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Device;

public class UpgradeMachineControl : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private MeshRenderer leftArm;
    [SerializeField] private MeshRenderer leftCardSlot;
    [SerializeField] private List<MeshRenderer> leftCables = new List<MeshRenderer>();

    [SerializeField] private MeshRenderer rightArm;
    [SerializeField] private MeshRenderer rightCardSlot;
    [SerializeField] private List<MeshRenderer> rightCables = new List<MeshRenderer>();

    [SerializeField] private MeshRenderer screen;
    [SerializeField] private MeshRenderer screenButton;
    [SerializeField] private MeshRenderer screenButtonOutline;
    [SerializeField] private MeshRenderer screenCardSlot;



    [Header("MATERIAL LERP DATA")]
    [SerializeField] private MaterialLerp.FloatData armOnFD;
    [SerializeField] private MaterialLerp.FloatData armOffFD;
    [SerializeField] private MaterialLerp.FloatData screenFD;
    [SerializeField] private MaterialLerp.FloatData slotFD;
    [SerializeField] private MaterialLerp.FloatData cableFD;



    private void Awake()
    {
        RenderSettings.reflectionIntensity = 0.0f;
    }
    private void OnDestroy()
    {
        RenderSettings.reflectionIntensity = 1.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectLeftCard();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            RetrieveLeftCard();
        }


        ////left arm
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    StartCoroutine(MaterialLerp.FloatLerp(armOnFD, new Material[1] { leftArm.materials[1] }));
        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    StartCoroutine(MaterialLerp.FloatLerp(armOffFD, new Material[1] { leftArm.materials[1] }));
        //}


        ////screen
        ////left
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    screenFD.variableReference = "_FirstFillCoef";
        //    screenFD.invert = false;
        //    screenFD.endGoal = 1;
        //    StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    screenFD.variableReference = "_FirstFillCoef";
        //    screenFD.invert = true;
        //    screenFD.endGoal = 0;
        //    StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));
        //}
        ////right
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    screenFD.variableReference = "_SecondFillCoef";
        //    screenFD.invert = false;
        //    screenFD.endGoal = 1;
        //    StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));
        //}
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    screenFD.variableReference = "_SecondFillCoef";
        //    screenFD.invert = true;
        //    screenFD.endGoal = 0;
        //    StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));
        //}
    }

    public void SelectLeftCard()
    {
        StartCoroutine(MaterialLerp.FloatLerp(armOffFD, new Material[1] { leftArm.materials[1] }));

        screenFD.variableReference = "_FirstFillCoef";
        screenFD.invert = false;
        screenFD.endGoal = 1;
        StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));
    }

    public void RetrieveLeftCard()
    {
        StartCoroutine(MaterialLerp.FloatLerp(armOnFD, new Material[1] { leftArm.materials[1] }));

        screenFD.variableReference = "_FirstFillCoef";
        screenFD.invert = true;
        screenFD.endGoal = 0;
        StartCoroutine(MaterialLerp.FloatLerp(screenFD, new Material[1] { screen.materials[2] }));
    }

    public void Replace()
    {

    }
}
