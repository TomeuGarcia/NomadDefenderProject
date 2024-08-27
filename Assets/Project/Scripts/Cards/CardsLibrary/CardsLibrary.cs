using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsLibrary", 
    menuName = SOAssetPaths.CARDS_LIBRARIES + "CardsLibrary")]
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



    public SupportCardDataModel[] GetRandomSupportCardPartsSet(NodeEnums.ProgressionState progressionState, int amount)
    {
        return content.GetRandomSupportCards(progressionState, amount);
    }


    public TurretCardDataModel[] GetRandomByProgressionTurretCardPartsSet(NodeEnums.ProgressionState progressionState, int totalAmount, int amountPerfect)
    {
        return content.GetRandomTurretCards(progressionState, totalAmount, amountPerfect);
    }
    

}