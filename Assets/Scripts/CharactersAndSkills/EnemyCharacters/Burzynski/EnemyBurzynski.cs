using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBurzynski : EnemyCharacter
{
    public EnemyBurzynski() : base()
    {
        NominativeName = "Burzyñski";
        DativeName = "Burzyñskiemu";
        AccusativeName = "Burzyñskiego";
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 40000;
        DifficultyHealthChange = 40000;
        Attack = DefaultAttack = BaseAttack = 350;
        DifficultyAttackChange = 150;
        Defense = DefaultDefense = BaseDefense = 100;
        Accuracy = DefaultAccuracy = BaseAccuracy = 0.9f;
        Turns = DefaultTurns = BaseTurns = 1;
        Speed = 200;
        MoneyDropped = 500;
        XPDropped = 10000;
        EnemyAttack attack = new EnemyAttack();
        skillSet.Add(attack);
        EnemyBalaclava balaclava = new EnemyBalaclava();
        skillSet.Add(balaclava);
        EnemySwordTwirl swordTwirl = new EnemySwordTwirl();
        skillSet.Add(swordTwirl);
        EnemyKorwinsPistol korwinsPistol = new EnemyKorwinsPistol();
        skillSet.Add(korwinsPistol);
        EnemyShitMonologue enemyShitMonologue = new EnemyShitMonologue();
        skillSet.Add(enemyShitMonologue);
        EnemyDinology dinology = new EnemyDinology();
        skillSet.Add(dinology);
    }
    protected override void AdditionalChangesOnReset()
    {
        NegativeEffectsImmunity = 9999;
    }
    protected override void AdditionalChangesOnDeath()
    {
        NegativeEffectsImmunity = 9999;
    }
}
