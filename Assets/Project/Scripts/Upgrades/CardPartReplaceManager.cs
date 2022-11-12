using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPartReplaceManager : MonoBehaviour
{
    [SerializeField] private UpgradeCardHolder upgradeCardHolder;
    [SerializeField] private CardPartHolder cardPartHolder;

    private enum PartType { ATTACK, BODY, BASE }
    [SerializeField] private PartType partType;


    [Header("Components")]
    [SerializeField] private Animator buttonAnimator;
    [SerializeField] private MouseOverNotifier buttonMouseOverNotifier;

    private bool replacementDone = false;


    private void OnEnable()
    {
        buttonMouseOverNotifier.OnMousePressed += ProceedReplace;
    }

    private void OnDisable()
    {
        buttonMouseOverNotifier.OnMousePressed -= ProceedReplace;
    }



    public void ProceedReplace()
    {
        if (replacementDone) return;

        if (CanConfirm())
        {
            buttonAnimator.SetTrigger("Pressed");
            ReplacePartInCard();

            replacementDone = true;

            upgradeCardHolder.RetrieveCard(upgradeCardHolder.selectedCard);
            cardPartHolder.RetrieveCard(cardPartHolder.selectedCardPart);

            upgradeCardHolder.StopInteractions();
            cardPartHolder.StopInteractions();
        }
    }

    private bool CanConfirm()
    {
        Debug.Log(upgradeCardHolder.AlreadyHasSelectedCard && cardPartHolder.AlreadyHasSelectedPart);
        return upgradeCardHolder.AlreadyHasSelectedCard && cardPartHolder.AlreadyHasSelectedPart;
    }

    
    private void ReplacePartInCard()
    {
        switch (partType)
        {
            case PartType.ATTACK:
                {
                    upgradeCardHolder.selectedCard.SetNewPartAttack(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartAttack>().turretPartAttack);
                }
                break;
            case PartType.BODY:
                {
                    upgradeCardHolder.selectedCard.SetNewPartBody(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartBody>().turretPartBody);
                }
                break;
            case PartType.BASE:
                {
                    upgradeCardHolder.selectedCard.SetNewPartBase(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartBase>().turretPartBase);
                }
                break;
            default: break;
        }
    }


}
