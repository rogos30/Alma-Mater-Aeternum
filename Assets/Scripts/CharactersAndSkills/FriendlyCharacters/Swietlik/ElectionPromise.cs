using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectionPromise : PlayableSkill
{
    bool hitAllies = true;
    bool buffAllies = false;
    public ElectionPromise()
    {
        Name = "Obietnica wyborcza";
        SkillDescription = "spe³nia swoj¹ obietnicê wyborcz¹.";
        InFightDescription = "";
        Cost = 1;
        TargetIsFriendly = true;
        TargetIsSelf = false;
        MultipleTargets = true;
        TargetIsRandom = false;
        MaxLevel = 4;
        levelsToUpgrades = new List<int> { 10, 12, 12, 14 };
        tokensToUpgrades = new List<int> { 2, 1, 1, 1 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Nie atakuj sojuszników", "Dodatkowo wzmocnij atak sojuszników", "Zmniejsz koszt umiejêtnoœci" };
        upgradeDescriptions = new List<string> { "Obieca³, ¿e nie zrobi nic.", "Po ocuceniu nie atakuje sojuszników", "Nak³ada buff do ataku na ka¿dego sojusznika na 2 tury", "100% -> 80% maxSP" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 1 || skillPerformance == 2)
        {
            return source.NominativeName + " nie robi nic";
        }
        float criticalChance = 0.1f;
        string finalDesc = source.AccusativeName + " POJEBA£O! ";
        if (target.KnockedOut)
        {
            if (!hitAllies && !buffAllies)
            {
                finalDesc += "Cuci " + target.AccusativeName;
            }
            else
            {
                finalDesc += "Cuci i ";
            }
            target.KnockedOut = false;
            target.Health = target.MaxHealth / 2;
        }
        if (hitAllies)
        {
            finalDesc += "wali bombê " + target.DativeName + " za ";
            int damage = (int)(source.Attack * Random.Range(0.8f, 1.2f)) - target.Defense;
            if (((FriendlyCharacter)target).IsGuarding)
            {
                damage = (int)(damage * FriendlyCharacter.guardDamageMultiplier);
            }
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
        }
        if (buffAllies)
        {
            finalDesc += "wzmacnia atak " + target.DativeName + " na ";
            int turns = 3;
            if (Random.Range(0, 1f) < criticalChance)
            {
                finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
                turns = 4;
            }
            target.ApplyBuff((int)Character.StatusEffects.ATTACK, turns);
            finalDesc = finalDesc + (turns-1) + " tury!";
        }
        
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
            hitAllies = false;
        }
        if (Level == 2)
        {
            buffAllies = true;
        }
        if (Level == 3)
        {
            Cost = 0.8f;
        }
        Level++;
    }
}
