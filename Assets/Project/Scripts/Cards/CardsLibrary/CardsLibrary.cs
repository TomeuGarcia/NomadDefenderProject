using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsLibrary", menuName = "Cards/CardsLibrary")]
public class CardsLibrary : ScriptableObject
{
    private CardsLibraryContent content;


    public void SetContent(CardsLibraryContent newContent)
    {
        content = newContent;
    }

    public void InitSetup()
    {
        content.Init();
    }



    public SupportCardParts[] GetRandomSupportCardPartsSet(NodeEnums.ProgressionState progressionState, int amount)
    {
        return content.GetRandomSupportCards(progressionState, amount);
    }


    public TurretCardParts[] GetRandomByProgressionTurretCardPartsSet(NodeEnums.ProgressionState progressionState, int totalAmount, int amountPerfect)
    {
        return content.GetRandomTurretCards(progressionState, totalAmount, amountPerfect);
    }




    // ---- USE TO CREATE NEW CARDS (INPUT RANDOM GENERATED CARDPARTS FROM ABOVE) ----
    public TurretCardParts GetConsumableTurretCardParts(TurretCardParts holderParts)
    {
        TurretCardParts parts = ScriptableObject.CreateInstance("TurretCardParts") as TurretCardParts;
        parts.Init(holderParts);

        return parts;
    }

    public SupportCardParts GetConsumableSupportCardParts(SupportCardParts holderParts)
    {
        SupportCardParts parts = ScriptableObject.CreateInstance("SupportCardParts") as SupportCardParts;
        parts.Init(holderParts);

        return parts;
    }


    public TurretCardParts[] GetConsumableTurretCardPartsSet(TurretCardParts[] holderPartsSetArray)
    {
        TurretCardParts[] partsSet = new TurretCardParts[holderPartsSetArray.Length];
        for (int i = 0; i < holderPartsSetArray.Length; ++i)
        {
            partsSet[i] = GetConsumableTurretCardParts(holderPartsSetArray[i]);
        }

        return partsSet;
    }

    public SupportCardParts[] GetConsumableSupportCardPartsSet(SupportCardParts[] holderPartsSetArray)
    {
        SupportCardParts[] partsSet = new SupportCardParts[holderPartsSetArray.Length];
        for (int i = 0; i < holderPartsSetArray.Length; ++i)
        {
            partsSet[i] = GetConsumableSupportCardParts(holderPartsSetArray[i]);
        }

        return partsSet;
    }



}