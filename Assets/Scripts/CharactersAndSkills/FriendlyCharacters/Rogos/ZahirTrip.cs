using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZahirTrip : PlayableSkill
{
    bool giveaway = false;
    int defaultTripTurns = 2;
    int defaultKebabHealing = 500;
    public ZahirTrip()
    {
        MaxLevel = 3;
        levelsToUpgrades = new List<int> { 8, 9, 12 };
        tokensToUpgrades = new List<int> { 2, 1, 1 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ Podró¿ po kebaby", "Skróæ czas podró¿y", "Zwiêksz leczenie z kebabów"};
        upgradeDescriptions = new List<string> { "Pierwsze u¿ycie wysy³a po kebaby, a drugie rozdaje je dru¿ynie, wzmacniaj¹c j¹", "2 -> 1 tura", "500 -> 750 HP" };
        SetToMission();
    }

    public void SetToMission()
    {
        giveaway = false;
        Name = "Podró¿ po kebaby";
        SkillDescription = "jedzie rowerem po kebaby. Nie bêdzie go chwilê, ale kebaby pomog¹ wszystkim";
        InFightDescription = " wybywa na misjê";
        Cost = 0.45f;
        TargetIsFriendly = false;
        TargetIsSelf = true;
        MultipleTargets = false;
        TargetIsRandom = false;
        SkillSoundId = 23;

    }

    public void SetToGiveaway()
    {
        giveaway = true;
        Name = "Rozdanie kebabów";
        SkillDescription = "rozdaje dru¿ynie zdobyte kebaby. Leczy i buffuje wszystkich";
        InFightDescription = " mówi \"bierzcie i jedzcie\", lecz¹c i mega wzmacniaj¹c ";
        Cost = 0.45f;
        TargetIsFriendly = true;
        TargetIsSelf = false;
        MultipleTargets = true;
        TargetIsRandom = false;
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (!giveaway)
        {
            if (skillPerformance == 0)
            {
                return source.NominativeName + " spada z rowerka";
            }
            string finalDesc = source.NominativeName + InFightDescription;
            int turns = defaultTripTurns;
            if (skillPerformance == 2)
            {
                finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc + " na " + turns;
                if (turns == 1)
                {
                    finalDesc = finalDesc + " turê!";
                }
                else
                {
                    finalDesc = finalDesc + " tury!";
                }
            }
            else
            {
                turns = defaultTripTurns + 1;
                finalDesc = finalDesc + " na " + turns + " tury!";
            }
            target.ApplyDebuff((int)Character.StatusEffects.TURNS, turns);
            target.ApplyDebuff((int)Character.StatusEffects.TURNS, turns);
            SetToGiveaway();
            return finalDesc;
        }
        
        else
        {
            if (skillPerformance == 0)
            {
                SetToMission();
                return source.DativeName + " rozsypa³y siê kebaby! Ju¿ nikt ich nie zje :(";
            }
            string finalDesc = source.NominativeName + InFightDescription + target.AccusativeName;
            int turns = 3;
            int healing = defaultKebabHealing;
            if (skillPerformance == 2)
            {
                finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
                turns = 4;
                healing *= source.criticalDamageMultiplier;
            }
            finalDesc = finalDesc + " na " + (turns - 1) + " tury!";
            for (int i = 0; i < 5; i++)
            {
                target.ApplyBuff(i, turns);
            }
            target.Heal(healing);
            return finalDesc;
        }
    }
    public override void upgrade()
    {
        if (Level == 0)
        {
            IsUnlocked = true;
        }
        if (Level == 1)
        {
            defaultTripTurns = 1;
        }
        if (Level == 2)
        {
            defaultKebabHealing = 750;
        }
        Level++;
    }
}
