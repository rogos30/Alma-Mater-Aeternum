using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.AudioSettings;

public class Lora : FriendlyCharacter
{

    public Lora() : base()
    {
        NominativeName = "Lora";
        DativeName = "Lorze";
        AccusativeName = "Lory";

        Health = MaxHealth = DefaultMaxHealth = BaseMaxHealth = 350;
        Skill = MaxSkill = 80;
        Attack = DefaultAttack = BaseAttack = 60;
        Defense = DefaultDefense = BaseDefense = 30;
        Accuracy = DefaultAccuracy = BaseAccuracy = 1f;

        CanBeUpgraded = false;
        Turns = DefaultTurns = 2;
        Speed = 400;
        SpriteIndex = 6;
        AbilityDescription = "Lora jest chwilowo hiperpobudzona przez kawê i ma 2 ruchy w ka¿dej turze";
        Attack attack = new Attack();
        skillSet.Add(attack);
        DontCare dontCare = new DontCare();
        skillSet.Add(dontCare);
        NaturalBeauty naturalBeauty = new NaturalBeauty();
        skillSet.Add(naturalBeauty);
        BasketballStar basketballStar = new BasketballStar();
        skillSet.Add(basketballStar);
    }
}
