using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaughingStock : PlayableSkill
{
    int defaultTurns = 2;
    public LaughingStock() : base()
    {
        Name = "Poœmiewisko";
        SkillDescription = "œmieszkuje i przedrzeŸnia przeciwników, zmniejszaj¹c ich obronê";
        InFightDescription = " zmniejsza obronê ";
        Cost = 0.33f;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = true;
        TargetIsRandom = false;
        AnimationId = 4;
        SkillSoundId = 19;
        MaxLevel = 2;
        levelsToUpgrades = new List<int> { 1, 5 };
        tokensToUpgrades = new List<int> { 1, 1 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Wyd³u¿ czas dzia³ania efektu umiejêtnoœci" };
        upgradeDescriptions = new List<string> { "Zmniejsza obronê przeciwników", "1 -> 2 tury"  };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        int turns;
        if (skillPerformance == 0)
        {
            turns = 3;
            source.ApplyDebuff((int)Character.StatusEffects.DEFENSE, turns);
            return source.NominativeName + " robi poœmiewisko z siebie, zmniejszaj¹c swoj¹ obronê na " + (turns-1) + "tury";
        }
        string finalDesc = source.NominativeName + InFightDescription + target.AccusativeName;
        turns = defaultTurns;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            turns = defaultTurns + 1;
        }
        finalDesc = finalDesc + " na " + (turns - 1) + " tury!";
        target.ApplyDebuff((int)Character.StatusEffects.DEFENSE, turns);
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
            defaultTurns = 3;
        }
        Level++;
    }
}
