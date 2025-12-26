using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.AudioSettings;

public class Franek : FriendlyCharacter
{

    public Franek() : base()
    {
        NominativeName = "Franek";
        DativeName = "Frankowi";
        AccusativeName = "Franka";

        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 400;
        Skill = MaxSkill = 80;
        Attack = DefaultAttack = BaseAttack = 75;
        Defense = DefaultDefense = BaseDefense = 50;
        Accuracy = DefaultAccuracy = BaseAccuracy = 1.2f;

        CanBeUpgraded = false;
        Turns = DefaultTurns = 1;
        Speed = 500;
        SpriteIndex = 5;
        AbilityDescription = "Franek zaczyna walkê z buffem do ataku, obrony i celnoœci na 3 tury";
        Attack attack = new Attack();
        skillSet.Add(attack);
        PerfectGoalkeeper perfectGoalkeeper = new PerfectGoalkeeper();
        skillSet.Add(perfectGoalkeeper);
        Crosswords crosswords = new Crosswords();
        skillSet.Add(crosswords);
        Counterattack counterattack = new Counterattack();
        skillSet.Add(counterattack);
    }

    protected override void AdditionalChangesOnReset()
    {
        base.AdditionalChangesOnReset();
        ApplyBuff((int)Character.StatusEffects.ATTACK, 4);
        ApplyBuff((int)Character.StatusEffects.DEFENSE, 4);
        ApplyBuff((int)Character.StatusEffects.ACCURACY, 4);
    }
}
