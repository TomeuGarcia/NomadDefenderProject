using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSceneNotifier : MonoBehaviour
{

    public delegate void MapSceneNotifierAction();
    public static event MapSceneNotifierAction OnMapSceneFinished;


    public void InvokeOnSceneFinished()
    {
        if (OnMapSceneFinished != null) OnMapSceneFinished();
        // OverworldMapGameManager listens  ->  ResumeMapAfterNodeScene
    }


}
