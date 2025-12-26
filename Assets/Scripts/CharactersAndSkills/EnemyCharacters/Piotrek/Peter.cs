using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peter : EnemyCharacter
{
    public Peter() : base()
    {
        NominativeName = "Piotrek";
        DativeName = "Piotrkowi";
        AccusativeName = "Piotrka";
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 3000;
        DifficultyHealthChange = 2000;
        Attack = DefaultAttack = BaseAttack = 200;
        DifficultyAttackChange = 70;
        Defense = DefaultDefense = BaseDefense = 75;
        Accuracy = DefaultAccuracy = BaseAccuracy = 0.90f;
        Turns = DefaultTurns = BaseTurns = 1;
        Speed = 150;
        MoneyDropped = 0;
        XPDropped = 0;
        DoublePunch doublePunch = new DoublePunch();
        skillSet.Add(doublePunch);
        TriplePunch triplePunch = new TriplePunch();
        skillSet.Add(triplePunch);
        HeavyPunch heavyPunch = new HeavyPunch();
        skillSet.Add(heavyPunch);
        CrowdIntimidation crowdIntimidation = new CrowdIntimidation();
        skillSet.Add(crowdIntimidation);
        CrowdMotivation crowdMotivation = new CrowdMotivation();
        skillSet.Add(crowdMotivation);
    }
}
