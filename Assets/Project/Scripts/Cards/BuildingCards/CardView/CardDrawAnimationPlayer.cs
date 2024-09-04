using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawAnimationPlayer : MonoBehaviour
{
    [SerializeField] protected GameObject[] _objectsToShow;
    
    private Material _borderMaterial;

    private int _allBordersAreActivePropertyId;
    private int _bordersAreSegmentedPropertyId;
    private int _currentActiveBorderIndexPropertyId;
    
    
    public void Configure(Material borderMaterial)
    {
        _borderMaterial = borderMaterial;
        _allBordersAreActivePropertyId = Shader.PropertyToID("_AllBordersAreActive");
        _bordersAreSegmentedPropertyId = Shader.PropertyToID("_BordersAreSegmented");
        _currentActiveBorderIndexPropertyId = Shader.PropertyToID("_CurrentActiveBorderIndex");
    }
    

    public virtual IEnumerator PlayDrawAnimation()
    {
        SetupHideBeforeShowing();
        yield return StartCoroutine(ShowLoopingBorder());
        yield return StartCoroutine(ShowObjects());
    }

    private void SetupHideBeforeShowing()
    {
        foreach (GameObject objectToShow in _objectsToShow)
        {
            objectToShow.SetActive(false);
        }
        
        _borderMaterial.SetFloat(_allBordersAreActivePropertyId, 0);
        _borderMaterial.SetFloat(_bordersAreSegmentedPropertyId, 1);
        //_borderMaterial.SetFloat("_TimeStartBorderLoop", Time.time);
    }

    private IEnumerator ShowLoopingBorder()
    {
        const float t1 = 0.1f;
        const int numberOfLoops = 2;
        const int totalBorderIndices = 4 * numberOfLoops;
        for (int i = 0; i < totalBorderIndices; ++i)
        {
            _borderMaterial.SetFloat(_currentActiveBorderIndexPropertyId, i);
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
            yield return new WaitForSeconds(t1);
        }
        
        _borderMaterial.SetFloat(_bordersAreSegmentedPropertyId, 0);
        
        _borderMaterial.SetFloat(_allBordersAreActivePropertyId, 1);
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        yield return new WaitForSeconds(t1);
        
        _borderMaterial.SetFloat(_allBordersAreActivePropertyId, 0);
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        yield return new WaitForSeconds(t1);
        
        _borderMaterial.SetFloat(_allBordersAreActivePropertyId, 1);
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        yield return new WaitForSeconds(t1);
    }
    
    protected virtual IEnumerator ShowObjects()
    {
        const float t1 = 0.05f;
        foreach (var objectToShow in _objectsToShow)
        {
            yield return StartCoroutine(ShowObjectBlinking(objectToShow, t1, GameAudioManager.GetInstance().PlayCardInfoShown));
        }
    }

    protected IEnumerator ShowObjectBlinking(GameObject objectToShow, float blinkDuration, Action audioFunction)
    {
        objectToShow.SetActive(true);
        audioFunction();
        yield return new WaitForSeconds(blinkDuration);
            
        objectToShow.SetActive(false);
        audioFunction();
        yield return new WaitForSeconds(blinkDuration);
            
        objectToShow.SetActive(true);
    }
    
    protected IEnumerator ShowObjectsBlinking(GameObject[] objectsToShow, float blinkDuration, Action audioFunction)
    {
        foreach (GameObject objectToShow in objectsToShow)
        {
            objectToShow.SetActive(true);
        }
        audioFunction();
        yield return new WaitForSeconds(blinkDuration);
            
        
        foreach (GameObject objectToShow in objectsToShow)
        {
            objectToShow.SetActive(false);
        }
        audioFunction();
        yield return new WaitForSeconds(blinkDuration);
            
        
        foreach (GameObject objectToShow in objectsToShow)
        {
            objectToShow.SetActive(true);
        }
    }
}
