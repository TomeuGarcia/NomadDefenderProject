using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTileChange : MonoBehaviour
{
    [SerializeField, Min(0)] private int _waveIndex = 0;
    [SerializeField] private GameObject[] _visibleObjectsPermanent;
    [SerializeField] private GameObject[] _visibleObjects;
    [SerializeField] private GameObject[] _hiddenObjectsPermanent;
    [SerializeField] private GameObject[] _hiddenObjects;
    [SerializeField] private GameObject[] _flickeringObjects;

    public int WaveIndex => _waveIndex;
    

    public void Init()
    {
        bool startActive = _waveIndex == 0;
        SetObjectsVisibility(_visibleObjectsPermanent, startActive);
        SetObjectsVisibility(_visibleObjects, startActive);
        SetObjectsVisibility(_hiddenObjectsPermanent, !startActive);
        SetObjectsVisibility(_hiddenObjects, !startActive);
        SetObjectsVisibility(_flickeringObjects, false);
    }

    public void Show()
    {
        SetObjectsVisibility(_visibleObjectsPermanent, true);
        SetObjectsVisibility(_visibleObjects, true);
        SetObjectsVisibility(_hiddenObjectsPermanent, false);
        SetObjectsVisibility(_hiddenObjects, false);
        StartCoroutine(ShowFlickering(_flickeringObjects, 0.2f, 0.8f,6));
    }
    public void Hide()
    {
        SetObjectsVisibility(_visibleObjects, false);
        SetObjectsVisibility(_hiddenObjects, true);
    }

    private void SetObjectsVisibility(GameObject[] objectsToToggle, bool visible)
    {
        foreach (GameObject toggleableObject in objectsToToggle)
        {
            toggleableObject.SetActive(visible);
        }
    }

    private IEnumerator ShowFlickering(GameObject[] objectsToFlicker, float flickInterval, float intervalMultiplier, int times)
    {
        for (int i = 0; i < times; ++i)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            
            SetObjectsVisibility(objectsToFlicker, true);
            yield return new WaitForSeconds(flickInterval);
            SetObjectsVisibility(objectsToFlicker, false);
            yield return new WaitForSeconds(flickInterval);

            flickInterval *= intervalMultiplier;
        }
    }

}
