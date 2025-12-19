using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator3 : EnemyCharacter
{
    public Generator3() : base()
    {
        NominativeName = "Generator teleskopowy";
        DativeName = "Generatorowi teleskopowemu";
        AccusativeName = "Generator teleskopowy";
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 4000;
        DifficultyHealthChange = 2000;
        Attack = DefaultAttack = BaseAttack = 200;
        DifficultyAttackChange = 100;
        Defense = DefaultDefense = BaseDefense = 80;
        Accuracy = DefaultAccuracy = BaseAccuracy = 0.90f;
        Turns = DefaultTurns = BaseTurns = 1;
        Speed = 450;
        MoneyDropped = 100;
        XPDropped = 5000;
        DoublePunch doublePunch = new DoublePunch();
        skillSet.Add(doublePunch);
        skillSet.Add(doublePunch);
        skillSet.Add(doublePunch);
        PowerGeneration powerGeneration = new PowerGeneration();
        skillSet.Add(powerGeneration);
        GroupShock groupShock = new GroupShock();
        skillSet.Add(groupShock);
    }
}
