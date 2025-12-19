using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aniela : EnemyCharacter
{
    public Aniela() : base()
    {
        NominativeName = "Aniela";
        DativeName = "Anieli";
        AccusativeName = "Anielê";
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 500;
        DifficultyHealthChange = 300;
        Attack = DefaultAttack = BaseAttack = 120;
        DifficultyAttackChange = 60;
        Defense = DefaultDefense = BaseDefense = 45;
        Accuracy = DefaultAccuracy = BaseAccuracy = 0.9f;
        Turns = DefaultTurns = BaseTurns = 1;
        Speed = 450;
        MoneyDropped = 50;
        XPDropped = 1500;
        EnemyAttack attack = new EnemyAttack();
        skillSet.Add(attack);
        skillSet.Add(attack);
        Grunt grunt = new Grunt();
        skillSet.Add(grunt);
    }
}
