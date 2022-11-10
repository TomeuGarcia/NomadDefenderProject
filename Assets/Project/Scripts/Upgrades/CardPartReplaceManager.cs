using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPartReplaceManager : MonoBehaviour
{
    [SerializeField] private UpgradeCardHolder upgradeCardHolder;
    [SerializeField] private CardPartHolder cardPartHolder;

    private enum PartType { ATTACK, BODY, BASE }
    [SerializeField] private PartType partType;



    private bool CanConfirm()
    {
        return upgradeCardHolder.AlreadyHasSelectedCard && cardPartHolder.AlreadyHasSelectedPart;
    }

    
    private void ReplacePartInCard()
    {
        switch (partType)
        {
            case PartType.ATTACK:
                {
                    upgradeCardHolder.selectedCard.SetNewPartAttack(cardPartHolder.selectedCardPart.GetComponent<CardPartAttack>().turretPartAttack);
                }
                break;
            case PartType.BODY:
                {
                    upgradeCardHolder.selectedCard.SetNewPartBody(cardPartHolder.selectedCardPart.GetComponent<CardPartBody>().turretPartBody);
                }
                break;
            case PartType.BASE:
                {
                    upgradeCardHolder.selectedCard.SetNewPartBase(cardPartHolder.selectedCardPart.GetComponent<CardPartBase>().turretPartBase);
                }
                break;
            default: break;
        }
    }


}
