using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetrayableChangeOfRules : PlayableSkill
{

    public BetrayableChangeOfRules()
    {
        Name = "Zmiana zasad";
        SkillDescription = "uznaje, ¿e przegrywa, wiêc usuwa turê przeciwnika.";
        InFightDescription = " parali¿uje ";
        Cost = 0.5f;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = false;
        MaxLevel = 2;
        levelsToUpgrades = new List<int> { 9, 12 };
        tokensToUpgrades = new List<int> { 2, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zmniejsz koszt" };
        upgradeDescriptions = new List<string> { "Usuwa turê przeciwnika", "50% -> 40% maxSP" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return source.NominativeName + " jednak gra wed³ug regu³";
        }
        string finalDesc = source.NominativeName + InFightDescription + target.AccusativeName;
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
            }
        }
        int turns = 2;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            turns = 3;
        }
        finalDesc = finalDesc + " na " + turns + " tur!";
        target.ApplyDebuff(4, turns);
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
            Cost = 0.4f;
        }
    }
}
