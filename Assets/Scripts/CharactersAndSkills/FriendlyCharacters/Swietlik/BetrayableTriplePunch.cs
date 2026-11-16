using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetrayableTriplePunch : PlayableSkill
{

    public BetrayableTriplePunch()
    {
        Name = "Wielokrotny atak";
        Cost = 0.3f;
        Repetitions = 2;
        SkillDescription = "wyprowadza 2 zwyk³e ciosy. Mo¿e zdradziæ!";
        InFightDescription = " wyprowadza zwyk³y cios, zadaj¹c ";
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = false;
        TargetIsRandom = true;
        MaxLevel = 3;
        levelsToUpgrades = new List<int> { 0, 3, 8 };
        tokensToUpgrades = new List<int> { 1, 1, 3 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zmniejsz koszt umiejêtnoœci", "WyprowadŸ wiêcej ciosów" };
        upgradeDescriptions = new List<string> { "Wyprowadza 2 zwyk³e ciosy", "30% -> 25% maxSP", "2 -> 3 ciosy" };
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
        if (betrayed && ((FriendlyCharacter)target).IsGuarding && damage != 1)
        {
            damage /= 2;
        }
        target.TakeDamage(damage);
        finalDesc = finalDesc + " " + damage + " obra¿eñ!";
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
            Cost = 0.25f;
        }
        if (Level == 2)
        {
            Repetitions = 3;
            SkillDescription = "wyprowadza 3 zwyk³e ciosy. Mo¿e zdradziæ!";
            InFightDescription = " wyprowadza 3 zwyk³e ciosy, zadaj¹c ";
        }
        Level++;
    }
}
