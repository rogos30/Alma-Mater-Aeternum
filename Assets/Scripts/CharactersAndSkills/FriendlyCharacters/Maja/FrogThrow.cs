using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogThrow : PlayableSkill
{
    float attackMultiplier = 1.67f;
    int defaultTurns = 3;
    public FrogThrow() : base()
    {
        Name = "Rzut ¿ab¹";
        SkillDescription = "rzuca w przeciwnika ¿ab¹. Krytyczne trafienie nak³ada truciznê.";
        InFightDescription = " rzuca ¿ab¹ w ";
        Cost = 0.2f;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = false;
        SkillSoundId = 25;
        MaxLevel = 3;
        levelsToUpgrades = new List<int> { 5, 7, 9 };
        tokensToUpgrades = new List<int> { 2, 1, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zwiêksz obra¿enia umiejêtnoœci", "Wyd³u¿ czas dzia³ania trucizny"};
        upgradeDescriptions = new List<string> { "Zadaje œrednie obra¿enia. Krytyczne trafienie nak³ada truciznê", "+20% obra¿eñ", "2 -> 3 tury" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        AnimationId = 1;
        if (skillPerformance == 0)
        {
            return source.NominativeName + " nie trafia ¿ab¹ w " + target.AccusativeName;
        }
        string finalDesc = source.NominativeName + InFightDescription + target.AccusativeName;
        int damage = (int)(source.Attack * attackMultiplier * Random.Range(0.8f, 1.2f)) - target.Defense;
        if (skillPerformance == 2)
        {
            int turns = defaultTurns;
            finalDesc = finalDesc + ", nak³adaj¹c truciznê na " + (turns-1) + " tury";
            target.ApplyDebuff((int)Character.StatusEffects.HEALTH, turns);
            AnimationId = 4;
        }
        damage = Mathf.Max(damage, 1);
        finalDesc = finalDesc + " i zadaje " + damage + " obra¿eñ!";
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
            defaultTurns = 4;
        }
        if (Level == 2)
        {
            attackMultiplier = 2;
        }
        Level++;
    }
}
