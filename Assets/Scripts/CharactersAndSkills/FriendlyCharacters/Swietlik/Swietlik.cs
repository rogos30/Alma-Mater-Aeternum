using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Swietlik : FriendlyCharacter
{
    //specialty - has a betrayal chance that goes down every turn in which an offensive skill is not used. Betrayal is a chance that a skill
    //will be used against a random friend instead of an enemy. The skill will be more powerful the more chance of betrayal there currently is.
    public int Betrayal {  get; set; }
    public Swietlik() : base()
    {
        NominativeName = "Œwietlik";
        DativeName = "Œwietlikowi";
        AccusativeName = "Œwietlika";
        Health = MaxHealth = DefaultMaxHealth = 500;
        Skill = MaxSkill = 80;
        Attack = DefaultAttack = BaseAttack = 100;
        Defense = DefaultDefense = BaseDefense = 35;
        Accuracy = DefaultAccuracy =BaseAccuracy = 0.9f;
        Turns = DefaultTurns = 1;
        Speed = 500;
        SpriteIndex = 1;
        AbilityDescription = "Œwietlik ma szansê na zdradê. Za ka¿dy % zdrady obra¿enia zwiêkszaj¹ siê o 2%. Obecnie: " + Betrayal + "% na zdradê";
        CharacterDescription = "Opis Œwietlika wip";
        BetrayableAttack attack = new BetrayableAttack();
        skillSet.Add(attack);
        BetrayableTriplePunch betrayableTriplePunch = new BetrayableTriplePunch();
        skillSet.Add(betrayableTriplePunch);
        BetrayableGrants betrayableGrants = new BetrayableGrants();
        skillSet.Add(betrayableGrants);
        TankStance tankStance = new TankStance();
        skillSet.Add(tankStance);
        BetrayableChangeOfRules betrayableChangeOfRules = new BetrayableChangeOfRules();
        skillSet.Add(betrayableChangeOfRules);
        ElectionPromise electionPromise = new ElectionPromise();
        skillSet.Add(electionPromise);
    }

    public void ResetBetrayal()
    {
        Betrayal = Random.Range(40, 50);
        AbilityDescription = "Œwietlik ma szansê na zdradê. Za ka¿dy % zdrady obra¿enia zwiêkszaj¹ siê o 2%. Obecnie: " + Betrayal + "% na zdradê";
    }

    public void ReduceBetrayal()
    {
        Betrayal = Mathf.Max(Betrayal - Random.Range(15, 25), 0);
        AbilityDescription = "Œwietlik ma szansê na zdradê. Za ka¿dy % zdrady obra¿enia zwiêkszaj¹ siê o 2%. Obecnie: " + Betrayal + "% na zdradê";
    }

    protected override void AdditionalChangesOnReset()
    {
        base.AdditionalChangesOnReset();
        ResetBetrayal();
    }

    public override void StartGuard()
    {
        base.StartGuard();
        ReduceBetrayal();
    }

    public override void OnItemUsed(Item item)
    {
        ReduceBetrayal();
    }
}
