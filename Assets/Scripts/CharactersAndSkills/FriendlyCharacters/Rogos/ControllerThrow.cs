using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerThrow : PlayableSkill
{
    float attackMultiplier = 1.2f;
    public ControllerThrow() : base()
    {
        Name = "Rzut padem";
        SkillDescription = "rzuca padem jak bumerangiem. Zadaje spore obra¿enia 2 razy.";
        InFightDescription = " rzuca padem jak bumerangiem i zadaje ";
        Cost = 100;
        Repetitions = 2;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = true;
        SkillSoundId = 21;
        MaxLevel = 3;
        levelsToUpgrades = new List<int> {1, 5, 7 };
        tokensToUpgrades = new List<int> {1, 1, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zmniejsz koszt umiejêtnoœci", "Zwiêksz obra¿enia umiejêtnoœci" };
        upgradeDescriptions = new List<string> { "Zadaje spore obra¿enia 2 razy", "100 -> 80 SP", "+25% obra¿eñ" };
    }
    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return source.NominativeName + " nie trafia padem w " + target.AccusativeName;
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
            Cost = 80;
        }
        if (Level == 2)
        {
            attackMultiplier = 1.5f;
        }
        Level++;
    }
}
