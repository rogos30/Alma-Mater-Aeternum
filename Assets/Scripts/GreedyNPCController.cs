using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyNPCController : Interactable
{
    [SerializeField] int[] playables;
    [SerializeField] int[] enemies;
    [SerializeField] string[] lines;
    [SerializeField] AudioClip[] voiceLines;
    [SerializeField] int[] speakersIndexes;
    [SerializeField] string[] failedConditionLines;
    [SerializeField] AudioClip[] failedConditionVoiceLines;
    [SerializeField] int[] failedConditionSpeakersIndexes;
    [SerializeField] int requiredMoneyAmount;



    public override void Interact()
    {
        if (Inventory.instance.money < requiredMoneyAmount)
        {
            DialogueManager.instance.onDialogueEnd.RemoveAllListeners();
            DialogueManager.instance.onDialogueEnd.AddListener(AfterFirstDialogue);
            DialogueManager.instance.StartDialogue(failedConditionLines, failedConditionSpeakersIndexes, failedConditionVoiceLines);
            return;
        }
        DialogueManager.instance.onDialogueEnd.RemoveAllListeners();
        DialogueManager.instance.onDialogueEnd.AddListener(AfterFirstDialogue);
        DialogueManager.instance.StartDialogue(lines, speakersIndexes, voiceLines);
    }

    void AfterFirstDialogue()
    {
        DialogueManager.instance.onDialogueEnd.RemoveListener(AfterFirstDialogue);
        BattleManager.instance.onBattleWon.RemoveAllListeners();

        if (interactionProgressesStory && Inventory.instance.money >= requiredMoneyAmount)
        { //dialogue end progresses story
            StoryManager.instance.ProgressStory();
            Inventory.instance.money -= requiredMoneyAmount;
        }
        if (interactionSavesGame)
        { //dialogue end saves game
            GameManager.instance.SaveGame();
        }
        if (interactionBlocksSavingGame)
        {
            GameManager.instance.canSaveGame = false;
        }
        else
        {
            GameManager.instance.canSaveGame = true;
        }
    }
    
}
