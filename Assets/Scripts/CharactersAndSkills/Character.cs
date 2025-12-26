using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.VolumeComponent;

public abstract class Character
{
    protected const float atkAccStatBoostMultiplier = 1.25f;
    protected const float defStatBoostMultiplier = 2f;
    protected const float poisonAndRegenMultiplier = 0.1f;
    public int criticalDamageMultiplier = 2;
    public enum StatusEffects
    {
        ATTACK,
        DEFENSE,
        ACCURACY,
        HEALTH,
        TURNS
    }
    public string NominativeName { get; set; }
    public string DativeName { get; set; }
    public string AccusativeName { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public float Accuracy { get; set; }
    public int Turns { get; set; }
    public int DefaultMaxHealth { get; set; }
    public int DefaultAttack { get; set; }
    public int DefaultDefense { get; set; }
    public float DefaultAccuracy { get; set; }
    public int DefaultTurns { get; set; }
    public int BaseMaxHealth { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }
    public float BaseAccuracy { get; set; }
    public int BaseTurns { get; set; }
    public int NegativeEffectsImmunity { get; set; }
    public int Speed { get; set; }
    public bool KnockedOut { get; set; }
    public int[] StatusTimers = new int[5];


    abstract public void HandleTimers();


    public void Reset()
    {
        KnockedOut = false;
        Health = MaxHealth;
        Attack = DefaultAttack;
        Defense = DefaultDefense;
        Turns = DefaultTurns;
        Accuracy = DefaultAccuracy;
        ResetTimers();
        AdditionalChangesOnReset();
    }

    void ResetTimers()
    {
        for (int i = 0;i < StatusTimers.Length;i++)
        {
            StatusTimers[i] = 0;
        }
        NegativeEffectsImmunity = 0;
    }

    virtual public void Heal(int healing)
    {
        Health = Mathf.Min(Health + healing, MaxHealth);
    }

    virtual public void Heal(float healing)
    {
        Health = Mathf.Min(Health + (int)(MaxHealth * healing), MaxHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        damage = Mathf.Max(damage, 1);
        Health = Mathf.Max(Health - damage, 0);
        if (Health == 0)
        {
            Death();
        }
    }

    public virtual void TakeDamage(float damage)
    {
        Health = Mathf.Max(Health - (int)(Health * damage), 1);
    }

    virtual public void ApplyBuff(int effect, int duration)
    {
        if (StatusTimers[effect] >= 0)
        {
            StatusTimers[effect] = Math.Max(duration, StatusTimers[effect]);
            switch (effect)
            {
                case 0: //attack buff
                    Attack = (int)(DefaultAttack * atkAccStatBoostMultiplier);
                    break;
                case 1: //defense buff
                    Defense = (int)(DefaultDefense * defStatBoostMultiplier);
                    break;
                case 2: //accuracy buff
                    Accuracy = DefaultAccuracy * atkAccStatBoostMultiplier;
                    break;
                case 4: //more turns
                    Turns = DefaultTurns + 1;
                    break;
            }
        }
        else
        { //effects cancel each other out
            StatusTimers[effect] = 0;
            CancelStatusEffect(effect);
        }
    }

    virtual public void ApplyDebuff(int effect, int duration)
    {
        if (NegativeEffectsImmunity > 0)
        {
            return;
        }
        if (StatusTimers[effect] <= 0)
        {
            StatusTimers[effect] = Math.Min(-duration, StatusTimers[effect]);
            switch (effect)
            {
                case 0: //attack debuff
                    Attack = (int)(DefaultAttack / atkAccStatBoostMultiplier);
                    break;
                case 1: //defense debuff
                    Defense = (int)(DefaultDefense / defStatBoostMultiplier);
                    break;
                case 2: //accuracy debuff
                    Accuracy = DefaultAccuracy / atkAccStatBoostMultiplier;
                    break;
                case 4: //paralysis
                    Turns = 0;
                    break;
            }
        }
        else
        { //effects cancel each other out
            StatusTimers[effect] = 0;
            CancelStatusEffect(effect);
        }
    }

    virtual public void CancelStatusEffect(int effect)
    {
        switch (effect)
        {
            case 0: //attack
                Attack = DefaultAttack;
                break;
            case 1: //defense
                Defense = DefaultDefense;
                break;
            case 2: //accuracy
                Accuracy = DefaultAccuracy;
                break;
            case 4: //turns
                Turns = DefaultTurns;
                break;
        }
    }

    public void HandlePersistentStatusEffects()
    {
        if (StatusTimers[(int)StatusEffects.HEALTH] > 0)
        {
            Heal(poisonAndRegenMultiplier);
        }
        else if (StatusTimers[(int)StatusEffects.HEALTH] < 0)
        {
            TakeDamage(poisonAndRegenMultiplier);
        }
    }

    public void Death()
    {
        KnockedOut = true;
        ResetTimers();
        AdditionalChangesOnDeath();
    }

    virtual protected void AdditionalChangesOnReset()
    {

    }

    virtual protected void AdditionalChangesOnDeath()
    {

    }

}
