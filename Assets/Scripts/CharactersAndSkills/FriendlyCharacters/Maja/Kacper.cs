using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kacper : PlayableSkill
{

    public Kacper()
    {
        Name = "Kacper";
        SkillDescription = "jest ponownie przyæmiewana przez swojego brata, który losowo rozdaje sojusznikom SP.";
        InFightDescription = " patrzy jak jej brat odnawia ";
        Cost = 0;
        Repetitions = 8;
        TargetIsFriendly = true;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = true;
        SkillSoundId = 9;
        MaxLevel = 2;
        levelsToUpgrades = new List<int> { 5, 7 };
        tokensToUpgrades = new List<int> { 2, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zwiêksz iloœæ rozdañ SP" };
        upgradeDescriptions = new List<string> { "Losowo rozdaje sojusznikom SP", "8 -> 10 rozdañ" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return "Kacper ma jednak wa¿niejsze rzeczy na g³owie";
        }
        string finalDesc = source.NominativeName + InFightDescription + target.DativeName;
        float sp = 0.1f;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            sp = 0.15f;
        }
        finalDesc = finalDesc + " " + sp * 100 + "% SP";
        ((FriendlyCharacter)target).RestoreSkill(sp);
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
            Repetitions = 10;
        }
        Level++;
    }
}
