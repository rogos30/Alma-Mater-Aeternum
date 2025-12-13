using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracksuiter : EnemyCharacter
{
    public Tracksuiter() : base()
    {
        NominativeName = "Dresiarz";
        DativeName = "Dresiarzowi";
        AccusativeName = "Dresiarza";
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 2500;
        DifficultyHealthChange = 750;
        Attack = DefaultAttack = BaseAttack = 150;
        DifficultyAttackChange = 50;
        Defense = DefaultDefense = BaseDefense = 70;
        Accuracy = DefaultAccuracy = BaseAccuracy = 1f;
        Turns = DefaultTurns = 1;
        Speed = 350;
        MoneyDropped = 25;
        XPDropped = 500;
        BaseballStrike baseballStrike = new BaseballStrike();
        skillSet.Add(baseballStrike);
        skillSet.Add(baseballStrike);
        BottleThrow bottleThrow = new BottleThrow();
        skillSet.Add(bottleThrow);
        EnemyBalaclava enemyBalaclava = new EnemyBalaclava();
        skillSet.Add(enemyBalaclava);
        Chant chant = new Chant();
        skillSet.Add(chant);
    }

    public Tracksuiter(string nameN, string nameD, string nameA) : base()
    {
        NominativeName = nameN;
        DativeName = nameD;
        AccusativeName = nameA;
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 1000;
        DifficultyHealthChange = 350;
        Attack = DefaultAttack = BaseAttack = 80;
        DifficultyAttackChange = 30;
        Defense = DefaultDefense = BaseDefense = 0;
        Accuracy = DefaultAccuracy = BaseAccuracy = 1f;
        Turns = DefaultTurns = 1;
        Speed = 350;
        MoneyDropped = 25;
        XPDropped = 500;
        BaseballStrike baseballStrike = new BaseballStrike();
        skillSet.Add(baseballStrike);
        skillSet.Add(baseballStrike);
        BottleThrow bottleThrow = new BottleThrow();
        skillSet.Add(bottleThrow);
        skillSet.Add(bottleThrow);
        EnemyBalaclava enemyBalaclava = new EnemyBalaclava();
        skillSet.Add(enemyBalaclava);
        Chant chant = new Chant();
        skillSet.Add(chant);
    }
}
