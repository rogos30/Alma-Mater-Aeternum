using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriplePunch : EnemySkill
{
    public TriplePunch()
    {
        Name = "Potrójny atak";
        InFightDescription = " wyprowadza 3 zwyk³e ciosy, zadaj¹c ";
        Repetitions = 3;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = false;
    }
    public override string execute(EnemyCharacter source, Character target)
    {
        if (Random.Range(0, 1f) > source.Accuracy * AccuracyMultiplier)
        {
            return source.NominativeName + " nie trafia atakiem w " + target.AccusativeName;
        }
        string finalDesc = source.NominativeName + InFightDescription + target.DativeName;
        int damage = (int)(source.Attack * Random.Range(0.8f, 1.2f)) - target.Defense;
        if (Random.Range(0, 1f) < criticalChance)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            damage *= source.criticalDamageMultiplier;
        }
        damage = Mathf.Max(damage, 1);
        target.TakeDamage(damage);
        if (((FriendlyCharacter)target).IsGuarding && damage != 1)
        {
            damage /= 2;
        }
        finalDesc = finalDesc + " " + damage + " obra¿eñ!";
        return finalDesc;
    }
}
