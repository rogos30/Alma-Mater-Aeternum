using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuiler : EnemyCharacter
{
    public MainBuiler() : base()
    {
        NominativeName = "Brygadzista";
        DativeName = "Brygadziœcie";
        AccusativeName = "Brygadzisty";
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 6000;
        DifficultyHealthChange = 3000;
        Attack = DefaultAttack = BaseAttack = 300;
        DifficultyAttackChange = 150;
        Defense = DefaultDefense = BaseDefense = 90;
        Accuracy = DefaultAccuracy = BaseAccuracy = 1f;
        Turns = DefaultTurns = BaseTurns = 1;
        Speed = 450;
        MoneyDropped = 150;
        XPDropped = 4500;
        EnemyMoraleBoost enemyMoraleBoost = new EnemyMoraleBoost();
        skillSet.Add(enemyMoraleBoost);
        ShovelStrike shovelStrike = new ShovelStrike();
        skillSet.Add(shovelStrike);
        Wheelbarrow wheelbarrow = new Wheelbarrow();
        skillSet.Add(wheelbarrow);
        BottleThrow bottleThrow = new BottleThrow();
        skillSet.Add(bottleThrow);
    }
}
