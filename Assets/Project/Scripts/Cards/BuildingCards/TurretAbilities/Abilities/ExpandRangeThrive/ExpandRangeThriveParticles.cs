using System;
using System.Collections;
using UnityEngine;

public class ExpandRangeThriveParticles : MonoBehaviour
{
    [SerializeField] private RecyclableParticles _recyclableParticles;
    [SerializeField] private ParticleSystem _mainParticles;
    [SerializeField] private ParticleSystem _sourceParticles;


    public void Play(Vector3 sourcePosition, Vector3 ownerPosition, float newRadius)
    {
        transform.position = ownerPosition;
        transform.rotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);

        Vector3 size = Vector3.one * (newRadius + 1);
        _mainParticles.transform.localScale = size;
        _mainParticles.transform.GetChild(0).localScale = size;

        
        ParticleSystem.ShapeModule shapeModule = _sourceParticles.shape;
        shapeModule.position = (ownerPosition - sourcePosition) + Vector3.up;
        _sourceParticles.Play();
        
        
        StartCoroutine(DoPlay());
    }

    private IEnumerator DoPlay()
    {
        yield return new WaitForSeconds(0.4f);
        _mainParticles.Play();

        yield return new WaitUntil(() => !_mainParticles.isEmitting);
        _recyclableParticles.Recycle();
    }
}