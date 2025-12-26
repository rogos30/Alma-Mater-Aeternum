using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capoeira : PlayableSkill
{
    int overcharge = 1;
    public Capoeira() : base()
    {
        Name = "Capoeira";
        SkillDescription = "tañczy capoeirê, ³aduj¹c mno¿nik ataku do pe³na (" + Welenc.MaxAttackMultiplier + ")";
        InFightDescription = " tañczy capoeirê i zwiêksza swój mno¿nik ataku ";
        Cost = 0.5f;
        TargetIsFriendly = false;
        TargetIsSelf = true;
        MultipleTargets = false;
        TargetIsRandom = false;
        SkillSoundId = 2;
        MaxLevel = 2;
        levelsToUpgrades = new List<int> { 7, 9 };
        tokensToUpgrades = new List<int> { 2, 2 };
        upgradeNames = new List<string> { "Odblokuj umiejêtnoœæ " + Name, "Wzmocnij trafienie krytyczne"};
        upgradeDescriptions = new List<string> { "Zwiêksza swój mno¿nik ataku", "Trafienie krytyczne zwiêksza mno¿nik o 2 ponad limit" };
    }

    public override string execute(FriendlyCharacter source, Character target, int skillPerformance)
    {
        if (skillPerformance == 0)
        {
            int damage = Random.Range(source.MaxHealth / 7, source.MaxHealth / 4);
            source.TakeDamage(damage);
            return source.NominativeName + " wywala siê na twarz, otrzymuj¹c " + damage + " obra¿eñ";
        }
        string finalDesc = source.NominativeName + InFightDescription;
        if (skillPerformance == 2)
        {
            finalDesc = "KRYTYCZNE TRAFIENIE! " + finalDesc + "ponad limit!";
            ((Welenc)source).MaxOutAttackMultiplier(overcharge);
        }
        else
        {
            finalDesc = finalDesc + "do limitu!";
            ((Welenc)source).MaxOutAttackMultiplier(0);
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
            overcharge = 2;
        }
        Level++;
    }
}
