using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballStrike : EnemySkill
{
    public BaseballStrike() : base()
    {
        Name = "£omot baseballem";
        InFightDescription = " wyprowadza potê¿ny cios baseballem, ";
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = false;
        SkillSoundId = 26;
    }

    public override string execute(EnemyCharacter source, Character target)
    {
        if (Random.Range(0, 1f) > source.Accuracy * AccuracyMultiplier)
        {
            return source.NominativeName + " nie trafia baseballem w " + target.AccusativeName;
        }
        string finalDesc = source.NominativeName + InFightDescription + target.DativeName;
        int damage = (int)(source.Attack * 2 * Random.Range(0.8f, 1.2f)) - target.Defense;
        int turns = 1;
        if (Random.Range(0, 1f) < criticalChance)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc + " parali¿uj¹c na 1 turê i ";
            damage *= source.criticalDamageMultiplier;
            target.ApplyDebuff((int)Character.StatusEffects.TURNS, turns);
        }

        damage = Mathf.Max(damage, 1);
        target.TakeDamage(damage);
        if (((FriendlyCharacter)target).IsGuarding && damage != 1)
        {
            damage /= 2;
        }
        finalDesc = finalDesc + " zadaj¹c " + damage + " obra¿eñ!";
        return finalDesc;
    }
}
