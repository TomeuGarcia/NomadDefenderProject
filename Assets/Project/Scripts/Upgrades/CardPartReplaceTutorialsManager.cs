using System.Collections;
using UnityEngine;

public class CardPartReplaceTutorialsManager : MonoBehaviour
{
    [SerializeField] private OptionalTutorial_UpgradeRoom_CardLevelMeaning _cardLevelMeaningTutorialPrefab;
    private const OptionalTutorialTypes CardLevelMeaning_TutorialType = OptionalTutorialTypes.UpgradeRoom_CardLevelMeaning;

    private IOptionalTutorialsStateManager OptionalTutorialsStateManager => 
        ServiceLocator.GetInstance().OptionalTutorialsStateManager;
    

    public IEnumerator PlayCardResultTutorials(BuildingCard card)
    {
        //if (ShouldPlay_CardLevelTutorial())
        {
            yield return StartCoroutine(Play_CardLevelTutorial(card));
        }
    }
    
    
    
    private bool ShouldPlay_CardLevelTutorial()
    {
        return !OptionalTutorialsStateManager.IsTutorialDone(CardLevelMeaning_TutorialType);
    }

    private IEnumerator Play_CardLevelTutorial(BuildingCard card)
    {
        OptionalTutorial_UpgradeRoom_CardLevelMeaning cardLevelMeaningTutorial =
            Instantiate(_cardLevelMeaningTutorialPrefab, transform);
        
        card.RootCardTransform.SetParent(transform);
        
        yield return StartCoroutine(cardLevelMeaningTutorial.Play(card));
        OptionalTutorialsStateManager.SetTutorialAsDone(CardLevelMeaning_TutorialType);
    }
    
}
