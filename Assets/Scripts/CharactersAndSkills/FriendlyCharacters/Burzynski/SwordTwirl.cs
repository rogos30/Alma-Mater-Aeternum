using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTwirl : PlayableSkill
{
    bool criticalHitInflictsPoison = false;
    public SwordTwirl()
    {
        Name = "Wir szabli";
        SkillDescription = "krêci siê z szabl¹ miêdzy przeciwnikami, ka¿demu zadaj¹c obra¿enia.";
        InFightDescription = " rozkrêca wir szabli, zadaj¹c ";
        Cost = 0.5f;
        AccuracyMultiplier = 0.9f;
        TargetIsFriendly = false;
        TargetIsSelf = false;
        MultipleTargets = true;
        TargetIsRandom = false;
        MaxLevel = 3;
        SkillSoundId = 44;
        levelsToUpgrades = new List<int> { 1, 1, 12 };
        tokensToUpgrades = new List<int> { 2, 1, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Zmniejsz koszt umiejêtnoœci", "Wzmocnij krytyczne trafienia" };
        upgradeDescriptions = new List<string> { "Zadaje spore obra¿enia wszystkim przeciwnikom", "50% -> 40% maxSP", "Krytyczne trafienia nak³adaj¹ krwawienie na 1 turê" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            return source.NominativeName + " nie trafia szabl¹ w " + target.AccusativeName;
        }
        string finalDesc = source.NominativeName + InFightDescription + target.DativeName;
        int damage = (int)(source.Attack * 1.5f * Random.Range(0.8f, 1.2f)) - target.Defense;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc;
            damage *= source.criticalDamageMultiplier;
            if (criticalHitInflictsPoison)
            {
                target.ApplyDebuff((int)Character.StatusEffects.HEALTH, 2);
            }
        }
        damage = Mathf.Max(damage, 1);
        finalDesc = finalDesc + " " + damage + " obra¿eñ";
        target.TakeDamage(damage);
        if (skillPerformance == 2 && criticalHitInflictsPoison)
        {
            finalDesc = finalDesc + " i nak³adaj¹c krwawienie na 1 turê!";
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
            Cost = 0.4f;
        }
        if (Level == 2)
        {
            criticalHitInflictsPoison = true;
        }
        Level++;
    }
}
