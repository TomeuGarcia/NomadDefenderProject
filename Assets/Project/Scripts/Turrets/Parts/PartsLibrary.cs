using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartLibrary", menuName = "TurretParts/TurretPartLibrary")]
public class PartsLibrary : ScriptableObject
{
    public TurretPartAttack[] attackParts;
    public TurretPartBody[] bodyParts;
    public TurretPartBase[] baseParts;
    public TurretPassiveBase[] basePassive;


    public TurretPartAttack GetRandomTurretPartAttack()
    {
        return attackParts[Random.Range(0, attackParts.Length)];
    }
    public TurretPartBody GetRandomTurretPartBody()
    {
        return bodyParts[Random.Range(0, bodyParts.Length)];
    }
    public TurretPartBase GetRandomTurretPartBase()
    {
        return baseParts[Random.Range(0, baseParts.Length)];
    }
    public TurretPassiveBase GetRandomTurretPassiveBase()
    {
        return basePassive[Random.Range(0, basePassive.Length)];
    }

    public TurretPartAttack[] GetRandomTurretPartAttacks(int amount)
    {
        if (amount > attackParts.Length) amount = attackParts.Length;

        HashSet<TurretPartAttack> partsSet = new HashSet<TurretPartAttack>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomTurretPartAttack());
        }

        return partsSet.ToArray();
    }

    public TurretPartBody[] GetRandomTurretPartBodies(int amount)
    {
        if (amount > bodyParts.Length) amount = bodyParts.Length;

        HashSet<TurretPartBody> partsSet = new HashSet<TurretPartBody>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomTurretPartBody());
        }

        return partsSet.ToArray();
    }

    public TurretPartBase[] GetRandomTurretPartBases(int amount)
    {
        if (amount > baseParts.Length) amount = baseParts.Length;

        HashSet<TurretPartBase> partsSet = new HashSet<TurretPartBase>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomTurretPartBase());
        }

        return partsSet.ToArray();
    }

    public TurretPassiveBase[] GetRandomTurretPassiveBases(int amount)
    {
        if (amount > basePassive.Length) amount = basePassive.Length;

        HashSet<TurretPassiveBase> passiveSet = new HashSet<TurretPassiveBase>();
        while (passiveSet.Count < amount)
        {
            passiveSet.Add(GetRandomTurretPassiveBase());
        }

        return passiveSet.ToArray();
    }

}
