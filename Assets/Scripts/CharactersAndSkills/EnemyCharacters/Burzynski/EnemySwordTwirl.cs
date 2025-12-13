using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordTwirl : EnemySkill
{
    //skill = new Skill("Wir szabli", "krêci siê z szabl¹ miêdzy przeciwnikami, ka¿demu zadaj¹c obra¿enia", "rozkrêca wir szabli", 0.5f, 1, 1.75f, 0.9f, 0, false, false, false, true, statusEffects);

    public EnemySwordTwirl()
    {
        Name = "Wir szabli";
        InFightDescription = " rozkrêca wir szabli, zadaj¹c ";
        AccuracyMultiplier = 0.9f;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = true;
        SkillSoundId = 44;
    }

    public override string execute(EnemyCharacter source, Character target)
    {
        if (Random.Range(0, 1f) > source.Accuracy * AccuracyMultiplier)
        {
            return source.NominativeName + " nie trafia szabl¹ w " + target.AccusativeName;
        }
        string finalDesc = source.NominativeName + InFightDescription + target.DativeName;
        int damage = (int)(source.Attack * 1.75f * Random.Range(0.8f, 1.2f)) - target.Defense;
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
