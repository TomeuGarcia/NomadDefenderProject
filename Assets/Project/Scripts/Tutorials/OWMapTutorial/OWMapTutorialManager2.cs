using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class OWMapTutorialManager2 : MonoBehaviour
{

    [SerializeField] private ScriptedSequence scriptedSequence;
    [SerializeField] private OverworldMapGameManager owMapGameManager;

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private CinemachineVirtualCamera animationCamera;

    [Header("Animations Time")]
    [SerializeField] [Range(0.0f, 10.0f)] private float animation1Time;
    [SerializeField] [Range(0.0f, 10.0f)] private float animation2Time;
    [SerializeField] [Range(0.0f, 10.0f)] private float animation3Time;

    [Header("Developer Test")] 
    [SerializeField] private bool testing = false;

    
    private OWMap_Node[] lastNodes;

    private void Start()
    {
        if (testing)
        {
            StartCoroutine(TestingTutorial());
        }
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

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine(); //7 -> Don't you want to get to the end of this?
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine(); //8 -> I won't be always arround
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //9 -> But...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //10 -> I
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //11 -> Will
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //12 -> Be
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //13 -> Watching
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);

        EnableLastNodes();

        //Camera Animation

        animationCamera.gameObject.transform.position = mainCamera.transform.position;
        animationCamera.gameObject.transform.rotation = mainCamera.transform.rotation;

        animationCamera.gameObject.SetActive(true);

        //Start animation

        float currentTime = 0;
        float tParam;
        
        while (currentTime < animation1Time)
        {
            currentTime += Time.deltaTime;
            tParam = currentTime / animation1Time;
            animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = Mathf.Lerp(0.0f, 1.0f, tParam);
            yield return null;
        }
        
        animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1.0f;


    }

    IEnumerator TestingTutorial()
    {
        yield return new WaitForSeconds(5.0f);
        //Camera Animation

        animationCamera.gameObject.transform.position = mainCamera.transform.position;
        animationCamera.gameObject.transform.rotation = mainCamera.transform.rotation;

        animationCamera.gameObject.SetActive(true);

        //Start animation

        float currentTime = 0.0f;
        float tParam;
        
        while (currentTime < animation1Time)
        {
            currentTime += Time.deltaTime;
            tParam = currentTime / animation1Time;
            animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = Mathf.Sin(tParam * (Mathf.PI / 2.0f));
            yield return null;
        }
        
        animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1.0f;

        yield return new WaitForSeconds(2.0f);

        currentTime = 0.0f;
        while (currentTime < animation2Time)
        {
            currentTime += Time.deltaTime;
            tParam = currentTime / animation2Time;
            animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1 +Mathf.Sin(tParam * (Mathf.PI / 2.0f));
            yield return null;
        }
        animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 2.0f;
    }
}
