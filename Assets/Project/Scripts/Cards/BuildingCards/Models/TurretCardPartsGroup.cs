using UnityEngine;

[System.Serializable]
public class TurretCardPartsGroup
{
    [SerializeField] private TurretPartProjectileDataModel _projectile;
    [SerializeField] private TurretPartBody _body;
    [SerializeField] private TurretPassiveBase[] _passives;
    
    public TurretPartProjectileDataModel Projectile => _projectile;
    public TurretPartBody Body => _body;
    public TurretPassiveBase Passive => _passives[0];

    
    public TurretCardPartsGroup(TurretCardPartsGroup other)
    {
        _projectile = other._projectile;
        _body = other._body;
        _passives = new TurretPassiveBase[other._passives.Length];
        other._passives.CopyTo(_passives, 0);
    }

    public void SetProjectile(TurretPartProjectileDataModel projectile)
    {
        _projectile = projectile;
    }
    public void SetBody(TurretPartBody body)
    {
        _body = body;
    }
    public void SetPassiveAbility(TurretPassiveBase passiveAbility)
    {
        _passives[0] = passiveAbility;
    }
}