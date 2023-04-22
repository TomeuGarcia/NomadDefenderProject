using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using DG.Tweening;

public class OWMapTutorialManager2 : MonoBehaviour
{

    [SerializeField] private ScriptedSequence scriptedSequence;
    [SerializeField] private OverworldMapGameManager owMapGameManager;

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private CinemachineVirtualCamera animationCamera;

    [SerializeField] private OWCameraMovement cameraMovement;

    [Header("Animations")]
    [SerializeField] [Range(0.0f, 10.0f)] private float animation1Time;
    [SerializeField] [Range(0.0f, 10.0f)] private float animation2Time;
    [SerializeField] [Range(0.0f, 10.0f)] private float animation3Time;

    [SerializeField] private GameObject wathcersEyes;

    [SerializeField] private Animator animator;
    [SerializeField] private CinemachineSmoothPath dollyTrack;

    [SerializeField] private List<Lerp> doorSides = new List<Lerp>();
    [SerializeField] private List<VolumeProfile> volumes = new List<VolumeProfile>();
    [SerializeField] private Volume globalVolume;


    [Header("Developer Test")] 
    [SerializeField] private bool testing = false;

    [SerializeField] private GameObject owMapTutorial1;
    [SerializeField] private GameObject camera;

    
    private OWMap_Node[] lastNodes;
    private int currentVolume = 0;

    private void Start()
    {
        wathcersEyes.SetActive(false);
    }

    public void StartTutorial()
    {
        Init();
        StartCoroutine(Tutorial());
    }

    private void Init()
    {
        lastNodes = owMapGameManager.GetMapNodes()[owMapGameManager.GetMapNodes().Length - 1];
    }

    private void EnableLastNodes()
    {
        foreach(OWMap_Node node in lastNodes)
        {
            if (!node.IsDestroyed()) node.EnableInteraction();
        }
    }

    private void DisableLastNodes()
    {
        foreach (OWMap_Node node in lastNodes)
        {
            node.DisableInteraction();
        }
    }
    IEnumerator Tutorial()
    {
        cameraMovement.CanDrag(false);
        yield return new WaitForSeconds(2.0f);

        if (testing)
        {
            camera.transform.localPosition = new Vector3(0.0f, 4.5f, 8.0f);
            for (int i = 0; i < 8; i++)
            {
                scriptedSequence.SkipLine();
            }
            StartCoroutine(TutorialAnimation());
            yield break;
        }

        DisableLastNodes();
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine(); //0 -> Computing Battle Result...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3.0f);

        //Wait until node animation is done

        scriptedSequence.NextLine(); //1
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //2
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);


        scriptedSequence.NextLine(); //3 -> Simulation will FAIL if no nodes are available
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //4
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);


        scriptedSequence.NextLine(); //5
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);


        scriptedSequence.NextLine(); //6 -> It better not happen again
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.5f);

        scriptedSequence.NextLine(); //7 -> I won't be always around...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);
        
        EnableLastNodes();
    }

    public IEnumerator TutorialAnimation(TutorialGameManager tutorialGame = null)
    {
        animator.SetBool("StartAnim", true);

        StartCoroutine(GlitchScreen(0.0f, 0.2f, true, 2));
        StartCoroutine(GlitchScreen(0.4f, 0.1f, false));
        //yield return StartCoroutine(GlitchScreen(0.0f, 1000.5f, true));

        GameAudioManager.GetInstance().PlayDroneBuildUp(25.0f, 0.0f, 0.0f);

        scriptedSequence.NextLine(); //9 -> So you want to get to the end of this...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3.0f);
        scriptedSequence.Clear();
        
        scriptedSequence.NextLine(); //10 -> In that case...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.Clear();
        
        //yield return new WaitForSeconds(5.0f);
        //Camera Animation

        animationCamera.gameObject.transform.position = mainCamera.transform.position;
        animationCamera.gameObject.transform.rotation = mainCamera.transform.rotation;

        animationCamera.gameObject.SetActive(true);



        //Start animation
        StartCoroutine(GlitchScreen(0.0f, 0.2f, false, 1));

        float currentTime = 0.0f;
        float tParam;
        bool text1Shown = false;
        bool text2Shown = false;
        
        while (currentTime < animation1Time)
        {
            if (!text1Shown && currentTime > 1.0f)
            {
                text1Shown = true;
                scriptedSequence.NextLine(); //11 -> I
            }
            else if (!text2Shown && currentTime > 3.5f)
            {
                text2Shown = true;
                scriptedSequence.NextLine(); //12 -> Will
            }
            currentTime += Time.deltaTime;
            tParam = currentTime / animation1Time;
            animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = Mathf.Sin(tParam * (Mathf.PI / 2.0f));
            yield return null;
        }

        animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1.0f;

        yield return new WaitForSeconds(1.85f);
        GameAudioManager.GetInstance().PlayDoorSound(0);
        yield return new WaitForSeconds(0.15f);

        //yield return new WaitForSeconds(1.0f);
        scriptedSequence.NextLine(); //13 -> Be
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());

        GameAudioManager.GetInstance().PlayDroneBuildUp(0.0f, 10.0f, 7.5f);
        //Starts Opening door
        float doorLerpTime = 4.0f;
        float offset = 1.0f;
        doorSides[0].LerpPosition(new Vector3(10046.1f + offset, 4.5f, 41.64484f), doorLerpTime);
        doorSides[1].LerpPosition(new Vector3(10063.94f - offset, 4.5f, 41.64484f), doorLerpTime);
        //CAMERA SHAKE
        StartCoroutine(CameraShake(doorLerpTime));
        StartCoroutine(LastCameraMovement());
        NextVolume();
        NextVolume();
        yield return new WaitForSeconds(5.5f);
        
        scriptedSequence.NextLine(); //14 -> Watching
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        StartCoroutine(GlitchScreen(0.0f, 0.2f, false, 0));
        wathcersEyes.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(GlitchScreen(0.0f, 0.2f, false, 2));
        StartCoroutine(GlitchScreen(0.4f, 0.1f, false));
        StartCoroutine(GlitchScreen(0.6f, 0.1f, false));
        scriptedSequence.Clear();
        wathcersEyes.SetActive(false);

        yield return new WaitUntil(() => animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition >= 2.0f);
        cameraMovement.CanDrag(true);

        if (tutorialGame != null)
        {
            tutorialGame.LoadRegularGame();
        }
    }

    private IEnumerator GlitchScreen(float delay, float glitchTime, bool transitionForward, int soundIndex = -2)
    {
        if(delay > 0.0f) { yield return new WaitForSeconds(delay); }
        if (soundIndex == -1) { GameAudioManager.GetInstance().PlayRandomGlitchSound(); }
        else if (soundIndex >= 0) { GameAudioManager.GetInstance().PlayGlitchSound(soundIndex); }
        NextVolume();
        yield return new WaitForSeconds(glitchTime);
        if(transitionForward) { NextVolume(); }
        else { PreviousVolume(); }
    }

    private void NextVolume()
    {
        currentVolume++;
        globalVolume.profile = volumes[currentVolume];
    }

    private void PreviousVolume()
    {
        currentVolume--;
        globalVolume.profile = volumes[currentVolume];
    }

    private IEnumerator CameraShake(float doorOpeningTime)
    {
        GameAudioManager.GetInstance().PlayDoorSound(1);

        float offset = 0.2f;
        CinemachineBasicMultiChannelPerlin shake = animationCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        shake.m_AmplitudeGain = 3.0f;
        yield return new WaitForSeconds(offset);
        shake.m_AmplitudeGain = 0.5f;
        yield return new WaitForSeconds(doorOpeningTime - offset * 3.0f);
        GameAudioManager.GetInstance().PlayDoorSound(2);
        yield return new WaitForSeconds(offset * 2.0f);
        shake.m_AmplitudeGain = 3.0f;
        yield return new WaitForSeconds(offset);
        shake.m_AmplitudeGain = 2.0f;
        yield return new WaitForSeconds(offset);
        shake.m_AmplitudeGain = 1.0f;
        yield return new WaitForSeconds(offset);
        shake.m_AmplitudeGain = 0.5f;
        yield return new WaitForSeconds(offset);
        shake.m_AmplitudeGain = 0.0f;

        StartCoroutine(LerpDollyTrackPoint());
    }

    private IEnumerator LerpDollyTrackPoint()
    {
        float totalTime = 4.0f;
        float currentTime = 0.0f;
        float tParam = 0.0f;

        float animStart = 8.0f;
        float animEnd = 4.0f;
        float diff = animStart- animEnd;

        while (currentTime < totalTime)
        {
            currentTime += Time.deltaTime;
            tParam = currentTime / totalTime;
            dollyTrack.m_Waypoints[2].position.y = animStart - (diff * tParam);
            yield return null;
        }
    }

    private IEnumerator LastCameraMovement()
    {
        yield return new WaitForSeconds(7.5f);

        float currentTime = 0.0f;
        float tParam;

        while (currentTime < animation2Time)
        {
            //if(currentTime > 2.0f){ wathcersEyes.SetActive(false); }
            currentTime += Time.deltaTime;
            tParam = currentTime / animation2Time;
            animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1 + Mathf.Sin(tParam * (Mathf.PI / 2.0f));
            yield return null;
        }
        animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 2.0f;
    }

    private void OnDestroy()
    {
        globalVolume.profile = volumes[0];
    }
}
