using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overrun : PlayableSkill
{
    float attackMultiplier = 1f;

    public Overrun()
    {
        Name = "Rozjazd";
        SkillDescription = "wzywa swojego konia i rozje¿d¿a nim wszystkich wrogów";
        InFightDescription = " traktuje wrogów jako tor jeŸdziecki i zadaje ";
        Cost = 0.5f;
        AccuracyMultiplier = 0.8f;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = true;
        TargetIsRandom = false;
        SkillSoundId = 20;
        MaxLevel = 3;
        levelsToUpgrades = new List<int> { 3, 4, 7 };
        tokensToUpgrades = new List<int> { 2, 1, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zwiêksz obra¿enia umiejêtnoœci " + Name, "Zmniejsz koszt umiejêtnoœci " + Name };
        upgradeDescriptions = new List<string> { "Zadaje spore obra¿enia wszystkim przeciwnikom", "+25% obra¿eñ", "50% -> 40% maxSP" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return source.NominativeName + " spada z konia.";
        }
        string finalDesc = source.NominativeName + InFightDescription + target.DativeName;
        int damage = (int)(source.Attack * attackMultiplier * Random.Range(0.8f, 1.2f)) - target.Defense;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            damage *= source.criticalDamageMultiplier;
        }
        damage = Mathf.Max(damage, 1);
        finalDesc = finalDesc + " " + damage + " obra¿eñ!";
        target.TakeDamage(damage);
        return finalDesc;
    }
    public override void upgrade()
    {
        if (Level == 0)
        {
            IsUnlocked = true;
        }
        if (Level == 1)
        {
            attackMultiplier = 1f;
        }
        if (Level == 2)
        {
            Cost = 0.4f;
        }
        Level++;
    }
}
