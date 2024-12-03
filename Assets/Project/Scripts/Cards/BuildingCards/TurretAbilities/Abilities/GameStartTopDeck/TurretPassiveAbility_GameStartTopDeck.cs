

public class TurretPassiveAbility_GameStartTopDeck : ATurretPassiveAbility
{
    public TurretPassiveAbility_GameStartTopDeck(TPADataModel_GameStartTopDeck originalModel) 
        : base(originalModel)
    {

    }
    
    public override void OnTDGameStart(BuildingCard ownerCard, DeckBuildingCards deck)
    {
        deck.MoveCardToTop(ownerCard);
    }

}