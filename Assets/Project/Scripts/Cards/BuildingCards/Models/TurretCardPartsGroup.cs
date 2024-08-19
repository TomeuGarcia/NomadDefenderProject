using UnityEngine;

[System.Serializable]
public class TurretCardPartsGroup
{
    [SerializeField] private TurretPartAttack _projectile;
    [SerializeField] private TurretPartBody _body;
    [SerializeField] private TurretPartBase _base;
    [SerializeField] private TurretPassiveBase[] _passives;
    
    public TurretPartAttack Projectile => _projectile;
    public TurretPartBody Body => _body;
    public TurretPartBase Base => _base;
    public TurretPassiveBase Passive => _passives[0];

    public TurretCardPartsGroup(TurretCardPartsGroup other)
    {
        _projectile = other._projectile;
        _body = other._body;
        _base = other._base;
        _passives = new TurretPassiveBase[other._passives.Length];
        other._passives.CopyTo(_passives, 0);
    }

    public void SetProjectile(TurretPartAttack projectile)
    {
        _projectile = projectile;
    }
    public void SetBody(TurretPartBody body)
    {
        _body = body;
    }
    public void SetBaseAbility(TurretPartBase basePart)
    {
        _base = basePart;
    }
    public void SetPassiveAbility(TurretPassiveBase passiveAbility)
    {
        _passives[0] = passiveAbility;
    }
}