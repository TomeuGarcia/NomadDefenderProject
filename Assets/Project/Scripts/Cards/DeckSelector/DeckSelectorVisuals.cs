using DG.Tweening;
using UnityEngine;

public class DeckSelectorVisuals : MonoBehaviour
{
    [SerializeField] private DeckSelector _deckSelector;
    [SerializeField] private MeshRenderer _runButtonMesh;
    private Material _runButtonMat;
    [SerializeField] private Transform runSimulationButtonTransform;
    private Vector3 positionButtonWires;

    [SerializeField] private MeshRenderer floorMesh;
    private Material floorMaterial;

    [SerializeField, Range(0f, 5f)] private float showDuration = 0.5f;
    [SerializeField, Range(0f, 5f)] private float maxWireStep = 1.65f;
    private float currentWiresStep = 0.0f;
    private int errorWiresStepPropertyId;

    private int errorOriginOffset1PropertyId;
    private int errorOriginOffset2PropertyId;

    private Vector3 hiddenWiresPosition = Vector3.right * 10000.0f;


    [SerializeField] float positionOffsetWires1 = 0.15f;
    [SerializeField] float positionOffsetWires2 = -0.70f;


    public void Init()
    {
        floorMaterial = floorMesh.material;

        errorWiresStepPropertyId = Shader.PropertyToID("_ErrorWiresStep");
        errorOriginOffset1PropertyId = Shader.PropertyToID("_ErrorOriginOffset");
        errorOriginOffset2PropertyId = Shader.PropertyToID("_ErrorOriginOffset2");

        positionButtonWires = runSimulationButtonTransform.position + Vector3.forward * positionOffsetWires2;

        _runButtonMat = _runButtonMesh.material;

        HideWires();
    }



    public void ShowWires(Vector3 position)
    {
        Vector3 wiresPosition1 = position;
        wiresPosition1.z += positionOffsetWires1;
        floorMaterial.SetVector(errorOriginOffset1PropertyId, wiresPosition1);

        
        //Vector3 wiresPosition2 = position;
        //wiresPosition2.z -= positionOffsetWires2;
        //floorMaterial.SetVector(errorOriginOffset2PropertyId, wiresPosition2);
        floorMaterial.SetVector(errorOriginOffset2PropertyId, positionButtonWires);
        

        SetCurrentWiresStep(0f);

        _runButtonMat.DOFloat(1.0f, "_EnableCoef", 0.1f);
        DOTween.To(() => currentWiresStep, x => SetCurrentWiresStep(x), maxWireStep, showDuration);
    }
    
    public void HideWires()
    {
        _runButtonMat.DOFloat(0.0f, "_EnableCoef", 0.1f);
        floorMaterial.SetVector(errorOriginOffset1PropertyId, hiddenWiresPosition);
        floorMaterial.SetVector(errorOriginOffset2PropertyId, hiddenWiresPosition);
        floorMaterial.SetFloat(errorWiresStepPropertyId, 0.0f);
    }

    private void SetCurrentWiresStep(float wiresStep)
    {
        currentWiresStep = wiresStep; 
        floorMaterial.SetFloat(errorWiresStepPropertyId, wiresStep);
    }


    public void OnButtonEnter()
    {
        if(!_deckSelector.StartButtonInteractable)
            return;

        GameAudioManager.GetInstance().PlayCardInfoShown();
        _runButtonMat.DOFloat(1.0f, "_HoverCoef", 0.1f);
    }
    public void OnButtonExit()
    {
        if (!_deckSelector.StartButtonInteractable)
            return;

        GameAudioManager.GetInstance().PlayCardInfoHidden();
        _runButtonMat.DOFloat(0.0f, "_HoverCoef", 0.1f);
    }

}
