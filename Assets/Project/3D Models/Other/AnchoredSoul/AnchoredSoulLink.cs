using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchoredSoulLink : MonoBehaviour
{
    private void OnMouseDown()
    {
        PauseMenu.GetInstance().Pause();
        Application.OpenURL("https://dhelpra-games.itch.io/anchoredsoul");
    }
}
