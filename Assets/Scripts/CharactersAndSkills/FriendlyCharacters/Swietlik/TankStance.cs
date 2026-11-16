using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankStance : PlayableSkill
{
    float healPercent = 0.25f;
    public TankStance()
    {
        Name = "Postawa tanka";
        SkillDescription = "zwiêksza swoj¹ obronê i odnawia czêœæ HP.";
        InFightDescription = " wchodzi w postawê tanka. Zwiêksza sobie obronê na ";
        Cost = 140f;
        TargetIsFriendly = false;
        TargetIsSelf = true;
        MultipleTargets = false;
        TargetIsRandom = false;
        MaxLevel = 2;
        levelsToUpgrades = new List<int> { 6, 12 };
        tokensToUpgrades = new List<int> { 1, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zwiêksz leczenie" };
        upgradeDescriptions = new List<string> { "Zwiêksza w³asn¹ obronê i odnawia 25% HP", "25% -> 40% HP" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return source.NominativeName + " nie zostaje tankiem.";
        }
        string finalDesc = source.NominativeName + InFightDescription;
        int turns = 3;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            turns = 5;
        }
        finalDesc = finalDesc + (turns-1) + " tury i odnawia " + (healPercent * 100) + "% HP.";
        target.Heal(healPercent);
        target.ApplyBuff((int)Character.StatusEffects.DEFENSE, turns);
        ((Swietlik)source).ReduceBetrayal();
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
            healPercent = 0.4f;
        }
        Level++;
    }
}
