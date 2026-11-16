using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogHat : PlayableSkill
{
    public FrogHat() : base()
    {
        Name = "¯abia czapka";
        SkillDescription = "zak³ada sojusznikowi ¿abi¹ czapkê, nak³adaj¹c na niego obronê i regeneracjê.";
        InFightDescription = " zak³ada ¿abê-czapkê ";
        Cost = 120;
        TargetIsFriendly = true;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = false;
        SkillSoundId = 25;
        MaxLevel = 2;
        levelsToUpgrades = new List<int> { 2, 4 };
        tokensToUpgrades = new List<int> { 1, 1 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zmniejsz koszt umiejêtnoœci"};
        upgradeDescriptions = new List<string> { "Zwiêksza sojusznikowi obronê i nadaje regeneracjê", "120 -> 90 SP" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return source.NominativeName + " nie trafia ¿ab¹ na g³owê " + target.AccusativeName + "...";
        }
        string finalDesc = source.NominativeName + InFightDescription;
        int turns = 3;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            turns = 5;
        }
        finalDesc = finalDesc + target.DativeName + " i nak³ada obronê i regeneracjê na " + (turns-1) + " tury!";
        target.ApplyBuff((int)Character.StatusEffects.DEFENSE, turns);
        target.ApplyBuff((int)Character.StatusEffects.HEALTH, turns);
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
            Cost = 90;
        }
        Level++;
    }
}
