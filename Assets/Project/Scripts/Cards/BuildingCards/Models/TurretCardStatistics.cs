public class TurretCardStatistics
{
    public int TotalDamageDealt { get; private set; }
    public int TotalKills { get; private set; }


    public TurretCardStatistics()
    {
        TotalDamageDealt = 0;
        TotalKills = 0;
    }

    public void AddTotalDamageDealt(int damageDealt)
    {
        TotalDamageDealt += damageDealt;
    }
    public void IncrementTotalKills()
    {
        ++TotalKills;
    }
}