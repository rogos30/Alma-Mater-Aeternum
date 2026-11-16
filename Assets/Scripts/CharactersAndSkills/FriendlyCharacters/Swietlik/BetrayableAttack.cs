using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetrayableAttack : PlayableSkill
{
    public BetrayableAttack()
    {
        Name = "Atak";
        SkillDescription = "wyprowadza zwyk³y cios. Mo¿e zdradziæ!";
        InFightDescription = " wyprowadza zwyk³y cios i zadaje ";
        Cost = 0;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = false;
        IsUnlocked = true;
}

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return source.NominativeName + " nie trafia atakiem w " + target.AccusativeName;
        }
        string finalDesc = "";
        bool betrayed = false;
        if (((Swietlik)source).Betrayal > Random.Range(0, 100))
        {   //choose a new target from your companions
            int knockedOuts = 0, newTargetId;
            foreach (var companionId in BattleManager.instance.currentPartyCharacters)
            {
                if (BattleManager.instance.playableCharacters[companionId].KnockedOut)
                {
                    knockedOuts++;
                }
            }
            if (knockedOuts + 1 < BattleManager.instance.currentPartyCharacters.Count)
            { //there is someone else alive besides Swietlik
                do
                {
                    newTargetId = Random.Range(0, BattleManager.instance.currentPartyCharacters.Count);
                } while (BattleManager.instance.playableCharacters[BattleManager.instance.currentPartyCharacters[newTargetId]] == source || BattleManager.instance.playableCharacters[BattleManager.instance.currentPartyCharacters[newTargetId]].KnockedOut);
                target = BattleManager.instance.playableCharacters[BattleManager.instance.currentPartyCharacters[newTargetId]];
                finalDesc = "NAST¥PI£A ZDRADA! ";
                betrayed = true;
            }
        }
        finalDesc += source.NominativeName + InFightDescription + target.DativeName;
        int damage = (int)(source.Attack * Random.Range(0.8f, 1.2f) * (100 + ((Swietlik)source).Betrayal) / 100f) - target.Defense;
        if (betrayed && ((FriendlyCharacter)target).IsGuarding)
        {
            damage = (int)(damage * FriendlyCharacter.guardDamageMultiplier);
        }
        if (betrayed)
        {
            damage /= 2;
        }
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            damage *= source.criticalDamageMultiplier;
        }
        damage = Mathf.Max(damage, 1);
        finalDesc = finalDesc + " " + damage + " obra¿eñ!";
        target.TakeDamage(damage);
        ((Swietlik)source).ResetBetrayal();
        if (betrayed && ((FriendlyCharacter)target).IsGuarding && damage != 1)
        {
            damage /= 2;
        }
        return finalDesc;
    }
    public override void upgrade()
    {
        return;
    }
}
