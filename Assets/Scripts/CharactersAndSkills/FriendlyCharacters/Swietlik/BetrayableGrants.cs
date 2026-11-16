using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetrayableGrants : PlayableSkill
{    
    //skill = new Skill("Grant's", "proponuje przeciwnikowi Grant'sa. Mo¿e otruæ", "otruwa Grant'sem w¹trobê", 40, 1, 0, 0.5f, 0, false, false, false, false, statusEffects);
    bool dealDamage = false;
    int baseTurns = 3;
    public BetrayableGrants() : base()
    {
        Name = "Grant's";
        SkillDescription = "proponuje przeciwnikowi Grant'sa. Mo¿e otruæ";
        InFightDescription = " otruwa Grant'sem w¹trobê ";
        Cost = 90;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = false;
        MaxLevel = 3;
        levelsToUpgrades = new List<int> { 2, 2, 5 };
        tokensToUpgrades = new List<int> { 1, 1, 1 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Dodatkowo zadaj obra¿enia", "Wyd³u¿ czas dzia³ania trucizny" };
        upgradeDescriptions = new List<string> { "Proponuje przeciwnikowi Grant'sa. Nak³ada truciznê.", "Zadaje œrednie obra¿enia po trafieniu umiejêtnoœci¹", "1 -> 2 tury" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return target.NominativeName + "odmawia Grant'sa!";
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
        finalDesc = finalDesc + source.NominativeName + InFightDescription + target.DativeName + ", nak³adaj¹c truciznê na ";
        int damage = (int)(source.Attack * Random.Range(0.8f, 1.2f) * (100 + ((Swietlik)source).Betrayal) / 100f) - target.Defense;
        if (betrayed && ((FriendlyCharacter)target).IsGuarding)
        {
            damage = (int)(damage * FriendlyCharacter.guardDamageMultiplier);
        }
        if (betrayed)
        {
            damage /= 2;
        }
        int turns = baseTurns;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            turns = baseTurns + 2;
            damage *= source.criticalDamageMultiplier;
        }
        finalDesc = finalDesc + (turns-1) + " tury";
        target.ApplyDebuff((int)Character.StatusEffects.HEALTH, turns);
        damage = Mathf.Max(damage, 1);
        if (dealDamage)
        {
            target.TakeDamage(damage);
            finalDesc = finalDesc + " i zadaj¹c " + damage + " obra¿eñ!";
        }
        ((Swietlik)source).ResetBetrayal();
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
            dealDamage = true;
        }
        if (Level == 2)
        {
            baseTurns = 4;
        }
        Level++;
    }
}
