using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeOfLuck : PlayableSkill
{
    float minHealing = 0.25f;
    public StrokeOfLuck()
    {
        Name = "£ut szczêœcia";
        SkillDescription = "przywraca wybranemu kompanowi losow¹ czêœæ jego hp oraz buffuje losow¹ statystykê.";
        InFightDescription = " przywraca ";
        Cost = 0.6f;
        AccuracyMultiplier = 0.75f;
        TargetIsFriendly = true;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = false;
        SkillSoundId = 12;
        MaxLevel = 2;
        levelsToUpgrades = new List<int> { 1, 6 };
        tokensToUpgrades = new List<int> { 1, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zwiêksz minimalne leczenie" };
        upgradeDescriptions = new List<string> { "Przywraca kompanowi losow¹ czêœæ hp i wzmacnia losow¹ statystykê", "25% -> 35% maxHP" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return source.NominativeName + " jednak ma pecha.";
        }
        string finalDesc = source.NominativeName + InFightDescription + target.DativeName;
        int turns = 3;
        int effect = UnityEngine.Random.Range(0, 5);
        float healing = UnityEngine.Random.Range(minHealing, 0.8f);
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            turns = 5;
            healing = Math.Min(healing * 2, 1);
        }
        healing = (float)Math.Round(healing, 4);
        finalDesc = finalDesc + " " + healing * 100 + "% max hp i wzmacnia ";
        switch (effect)
        {
            case 0:
                finalDesc = finalDesc + "atak!";
                break;
            case 1:
                finalDesc = finalDesc + "obronê!";
                break;
            case 2:
                finalDesc = finalDesc + "celnoœæ!";
                break;
            case 3:
                finalDesc = finalDesc + "regeneracjê!";
                break;
            case 4:
                finalDesc = finalDesc + "iloœæ tur!";
                break;
        }
        target.ApplyBuff(effect, turns);
        target.Heal(healing);
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
            minHealing = 0.35f;
        }
        Level++;
    }
}
