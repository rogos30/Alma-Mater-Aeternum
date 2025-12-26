using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : EnemyCharacter
{
    public Skeleton() : base()
    {
        NominativeName = "Szkielet - ¿o³nierz";
        DativeName = "Szkieletowi - ¿o³nierzowi";
        AccusativeName = "Szkieleta - ¿o³nierza";

        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 8000;
        DifficultyHealthChange = 4000;
        Attack = DefaultAttack = BaseAttack = 550;
        DifficultyAttackChange = 250;
        Defense = DefaultDefense = BaseDefense = 90;
        Accuracy = DefaultAccuracy = BaseAccuracy = 1f;
        Turns = DefaultTurns = BaseTurns = 1;

        Speed = 350;
        MoneyDropped = 500;
        XPDropped = 4000;
        HeavyPunch heavyPunch = new HeavyPunch();
        skillSet.Add(heavyPunch);
        ToxicGas toxicGas = new ToxicGas();
        skillSet.Add(toxicGas);
        Staredown staredown = new Staredown();
        skillSet.Add(staredown);
        BoneThrow boneThrow = new BoneThrow();
        skillSet.Add(boneThrow);
    }

    public Skeleton(string nameN, string nameD, string nameA) : base()
    {
        NominativeName = nameN;
        DativeName = nameD;
        AccusativeName = nameA;
        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 10000;
        DifficultyHealthChange = 5000;
        Attack = DefaultAttack = BaseAttack = 550;
        DifficultyAttackChange = 250;
        Defense = DefaultDefense = BaseDefense = 90;
        Accuracy = DefaultAccuracy = BaseAccuracy = 1f;
        Turns = DefaultTurns = BaseTurns = 1;
        Speed = 350;
        MoneyDropped = 150;
        XPDropped = 4000;
        HeavyPunch heavyPunch = new HeavyPunch();
        skillSet.Add(heavyPunch);
        ToxicGas toxicGas = new ToxicGas();
        skillSet.Add(toxicGas);
        Staredown staredown = new Staredown();
        skillSet.Add(staredown);
        BoneThrow boneThrow = new BoneThrow();
        skillSet.Add(boneThrow);
    }
}
