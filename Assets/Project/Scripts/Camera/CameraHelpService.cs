using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHelpService : ICameraHelpService
{
    public Camera CardsCamera { get; private set; }

    public void SetCardsCamera(Camera cardsCamera)
    {
        CardsCamera = cardsCamera;
    }
}
