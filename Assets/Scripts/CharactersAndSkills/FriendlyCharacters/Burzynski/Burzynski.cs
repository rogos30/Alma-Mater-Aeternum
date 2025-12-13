using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burzynski : FriendlyCharacter
{
    public Burzynski() : base()
    {
        NominativeName = "Burzyñski";
        DativeName = "Burzyñskiemu";
        AccusativeName = "Burzyñskiego";
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 450;
        Skill = MaxSkill = 100;
        Attack = DefaultAttack = BaseAttack = 100;
        Defense = DefaultDefense = BaseDefense = 30;
        Accuracy = DefaultAccuracy = BaseAccuracy = 0.8f;
        Turns = DefaultTurns = 1;
        Speed = 200;
        SpriteIndex = 4;
        levelsToUnlockSkill = 5;
        AbilityDescription = "Burzyñski jest odporny na negatywne efekty statusu";
        CharacterDescription = "Opis Burzyñskiego wip";
        Attack attack = new Attack();
        skillSet.Add(attack);
        Balaclava balaclava = new Balaclava();
        skillSet.Add(balaclava);
        SwordTwirl swordTwirl = new SwordTwirl();
        skillSet.Add(swordTwirl);
        KorwinsPistol korwinsPistol = new KorwinsPistol();
        skillSet.Add(korwinsPistol);
        MoraleDebuff moraleDebuff = new MoraleDebuff();
        skillSet.Add(moraleDebuff);
        Dinology dinology = new Dinology();
        skillSet.Add(dinology);
    }
    protected override void AdditionalChangesOnReset()
    {
        Skill = MaxSkill;
        IsGuarding = false;
        NegativeEffectsImmunity = 9999;
    }
    protected override void AdditionalChangesOnDeath()
    {
        IsGuarding = false;
        NegativeEffectsImmunity = 9999;
    }
}
