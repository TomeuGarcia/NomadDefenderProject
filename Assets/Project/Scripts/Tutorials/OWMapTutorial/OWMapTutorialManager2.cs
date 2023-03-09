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

    [Header("Animations")]
    [SerializeField] [Range(0.0f, 10.0f)] private float animation1Time;
    [SerializeField] [Range(0.0f, 10.0f)] private float animation2Time;
    [SerializeField] [Range(0.0f, 10.0f)] private float animation3Time;

    [SerializeField] private GameObject wathcersEyes;

    [Header("Developer Test")] 
    [SerializeField] private bool testing = false;

    [SerializeField] private GameObject owMapTutorial1;
    [SerializeField] private GameObject camera;

    
    private OWMap_Node[] lastNodes;

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
        yield return new WaitForSeconds(2.0f);

        if (testing)
        {
            camera.transform.localPosition = new Vector3(0.0f, 4.5f, 8.0f);
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
        yield return new WaitForSeconds(3.0f); //TODO Wait until last node is clicked

        StartCoroutine(TestingTutorial());
        

        

        yield return new WaitForSeconds(1.0f);

       
        
    }

    IEnumerator TestingTutorial()
    {
        
        scriptedSequence.NextLine(); //8 -> Hmmm...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //9 -> So you want to get to the end of this
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

        float currentTime = 0.0f;
        float tParam;
        bool text1Shown = false;
        bool text2Shown = false;
        
        while (currentTime < animation1Time)
        {
            if (!text1Shown && currentTime > 2.0f)
            {
                text1Shown = true;
                scriptedSequence.NextLine(); //11 -> I
            }
            else if (!text2Shown && currentTime > 5.0f)
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

        yield return new WaitForSeconds(1.0f);
        //Starts Opening door
        
        yield return new WaitForSeconds(1.0f);
        scriptedSequence.NextLine(); //13 -> Be
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);
        
        scriptedSequence.NextLine(); //14 -> Watching
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        wathcersEyes.SetActive(true);
        yield return new WaitForSeconds(2.5f);

        scriptedSequence.Clear();
        
        yield return new WaitForSeconds(2.5f);

        currentTime = 0.0f;
        while (currentTime < animation2Time)
        {
            if(currentTime > 2.0f){ wathcersEyes.SetActive(false); }
            currentTime += Time.deltaTime;
            tParam = currentTime / animation2Time;
            animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1 +Mathf.Sin(tParam * (Mathf.PI / 2.0f));
            yield return null;
        }
        animationCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 2.0f;
    }
}
