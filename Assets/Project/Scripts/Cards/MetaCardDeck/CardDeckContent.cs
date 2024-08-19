

using System.Collections.Generic;


public class CardDeckContent
{
    private readonly List<TurretCardData> _turretCardsData;
    private readonly List<SupportCardData> _supportCardsData;
    
    public TurretCardData[] TurretCardsData => _turretCardsData.ToArray();
    public SupportCardData[] SupportCardsData => _supportCardsData.ToArray();
    
    public CardDeckContent(TurretCardDataModel[] turretCardModels, SupportCardDataModel[] supportCardModels)
    {
        _turretCardsData = new List<TurretCardData>(turretCardModels.Length);
        foreach (TurretCardDataModel turretDataModel in turretCardModels)
        {
            AddTurretCard(turretDataModel);
        }
        
        _supportCardsData = new List<SupportCardData>(supportCardModels.Length);
        foreach (SupportCardDataModel supportCardModel in supportCardModels)
        {
            AddSupportCard(supportCardModel);
        }
    }

    public void AddTurretCard(TurretCardDataModel turretDataModel)
    {
        _turretCardsData.Add(new TurretCardData(turretDataModel));
    }
    public void AddSupportCard(SupportCardDataModel supportCardModel)
    {
        _supportCardsData.Add(new SupportCardData(supportCardModel));
    }
    
    
    
    
}