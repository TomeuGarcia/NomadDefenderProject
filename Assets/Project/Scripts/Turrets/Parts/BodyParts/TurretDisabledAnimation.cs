using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDisabledAnimation : MonoBehaviour
{
    [Header("HEAD ANIMATION")]
    [SerializeField] private Transform _headTransform;
    [SerializeField] private TweenConfig _headLookDownTween;
    [SerializeField] private TweenConfig _headLookUpTween;

    [Header("MATERIAL")] 
    [SerializeField] private Material _removeColorMaterial;
    public Material RemoveColorMaterial => _removeColorMaterial;

    public void PlayDo()
    {
        _headTransform.LocalRotate(_headLookDownTween);
    }
    
    public void PlayUndo()
    {
        _headTransform.LocalRotate(_headLookUpTween);
    }
    
}
