using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopkeeperController : Interactable
{
    [SerializeField] string[] superToastLines;
    [SerializeField] AudioClip[] superToastVoiceLines;
    [SerializeField] int[] superToastSpeakersIndexes;

    public override void Interact()
    {
        if (GameManager.instance.inGameCanvas.enabled)
        {
            if (StoryManager.instance.superToastProgress == 3 && superToastLines.Length > 0)
            {
                DialogueManager.instance.onDialogueEnd.RemoveAllListeners();
                DialogueManager.instance.onDialogueEnd.AddListener(AfterDialogue);
                DialogueManager.instance.StartDialogue(superToastLines, superToastSpeakersIndexes, superToastVoiceLines);
            }
            else
            {
                ShopManager.instance.SetUpShop();
            }
        }
        Debug.Log("stp " + StoryManager.instance.superToastProgress);
    }

    void AfterDialogue()
    {
        DialogueManager.instance.onDialogueEnd.RemoveListener(AfterDialogue);
        StoryManager.instance.finishedSuperToast = true;
        StoryManager.instance.superToastProgress = 4;
        Inventory.instance.wearables[18].Add(1); //super toast
        GameManager.instance.lastPageOfWearablesUnlocked = true;
        List<string> gameInfoLines = new List<string>();
        gameInfoLines.Add("Otrzymujesz Super Tosta (atrybut defensywny)");
        DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
    }
}
