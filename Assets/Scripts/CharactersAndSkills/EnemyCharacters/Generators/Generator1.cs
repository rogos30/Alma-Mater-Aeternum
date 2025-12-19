using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator1 : EnemyCharacter
{
    public Generator1() : base()
    {
        NominativeName = "Generator wuefistów";
        DativeName = "Generatorowi wuefistów";
        AccusativeName = "Generator wuefistów";
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 1500;
        DifficultyHealthChange = 800;
        Attack = DefaultAttack = BaseAttack = 150;
        DifficultyAttackChange = 50;
        Defense = DefaultDefense = BaseDefense = 40;
        Accuracy = DefaultAccuracy = BaseAccuracy = 0.90f;
        Turns = DefaultTurns = BaseTurns = 1;
        Speed = 250;
        MoneyDropped = 100;
        XPDropped = 3000;
        EnemyAttack enemyAttack = new EnemyAttack();
        skillSet.Add(enemyAttack);
        skillSet.Add(enemyAttack);
        DoublePunch doublePunch = new DoublePunch();
        skillSet.Add(doublePunch);
        skillSet.Add(doublePunch);
        PowerGeneration powerGeneration = new PowerGeneration();
        skillSet.Add(powerGeneration);
    }
}
