using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOptionsController : MonoBehaviour
{
    [SerializeField] private Toggle _windowedToggle;
    [SerializeField] private Toggle _fixedResolutionToggle;

    private const int FIXED_WIDTH = 1920;
    private const int FIXED_HEIGHT = 1080;
    private Resolution _fixedResolution;

    private bool IsCurrentlyFullscreen => _windowedToggle.isOn;

    private void Awake()
    {
        _fixedResolution = new Resolution();
        _fixedResolution.width = FIXED_WIDTH;
        _fixedResolution.height = FIXED_HEIGHT;

        _windowedToggle.onValueChanged.AddListener(OnWidowedTogglePressed);
        _fixedResolutionToggle.onValueChanged.AddListener(OnFixedResolutionTogglePressed);

        OnWidowedTogglePressed(_windowedToggle.isOn);
        OnFixedResolutionTogglePressed(_fixedResolutionToggle.isOn);
    }


    private void OnWidowedTogglePressed(bool isMarked)
    {
        if (isMarked)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    private void OnFixedResolutionTogglePressed(bool isMarked)
    {
        Resolution[] availableResolutions = Screen.resolutions;

        if (!isMarked)
        {
            SetScreenResolution(availableResolutions[^1]);
            return;
        }


        bool foundFixedResoluton = false;
        int i = availableResolutions.Length - 1;

        while (!foundFixedResoluton && i > 0)
        {
            Resolution resolution = availableResolutions[i];
            if (resolution.width == FIXED_WIDTH &&
                resolution.height == FIXED_HEIGHT)
            {
                SetScreenResolution(resolution);
                foundFixedResoluton = true;
            }

            --i;
        }

        if (!foundFixedResoluton)
        {
            SetScreenResolution(_fixedResolution);
        }        
    }

    private void SetScreenResolution(Resolution resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, IsCurrentlyFullscreen);
    }

}
