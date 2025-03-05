using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FacilityUIHelper : MonoBehaviour
{
    public void ButtonHovered()
    {
        GameAudioManager.GetInstance().PlayCardInfoShown();
    }

    public void ButtonUnhovered()
    {
        GameAudioManager.GetInstance().PlayCardInfoHidden();
    }
    
    public void ButtonClickedPunch(RectTransform animationTransform)
    {
        Sequence punchSequence = DOTween.Sequence();
        punchSequence.Append(animationTransform.DOPunchScale(Vector3.one * 0.4f, 0.5f, 6));

        GameAudioManager.GetInstance().PlayCardSelected();
    }

}