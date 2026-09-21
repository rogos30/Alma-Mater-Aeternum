using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMotivation : EnemySkill
{
    public CrowdMotivation() : base()
    {
        Name = "Motywacja od t³umu";
        InFightDescription = " pozyskuje motywacjê od t³umu ";
        TargetIsFriendly = false;
        TargetIsSelf = true;
        MultipleTargets = false;
        SkillSoundId = 40;
    }

    public override string execute(EnemyCharacter source, Character target)
    {
        if (Random.Range(0, 1f) > source.Accuracy * AccuracyMultiplier)
        {
            return source.NominativeName + " nie ws³uchuje siê w t³um";
        }
        string finalDesc = source.NominativeName + InFightDescription + target.DativeName + ", odnawiaj¹c sobie ";
        int healing = (int)(source.MaxHealth / 20 * Random.Range(0.8f, 1.2f));
        int turns = 3;
        if (Random.Range(0, 1f) < criticalChance)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            turns = 4;
            healing *= source.criticalDamageMultiplier;
        }
        target.ApplyBuff((int)Character.StatusEffects.ATTACK, turns);
        healing = Mathf.Max(healing, 1);
        target.Heal(healing);
        finalDesc = finalDesc + healing + " zdrowia i wzmacniaj¹c swój atak na " + (turns - 1) + " tury!";
        return finalDesc;
    }
}
