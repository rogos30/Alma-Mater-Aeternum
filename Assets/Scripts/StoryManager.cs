using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;
    [SerializeField] Camera cam;
    [SerializeField] PlayerController player;
    [SerializeField] TMP_Text currentQuestText;
    [SerializeField] GameObject[] storyNPCs;
    [SerializeField] GameObject[] storyTeleports;
    [SerializeField] GameObject[] sideQuestNPCs;
    [SerializeField] GameObject[] followerNPCs;
    [SerializeField] GameObject[] storyTriggers;
    [SerializeField] GameObject[] sideQuestTriggers;
    [SerializeField] public GameObject[] PatrolNPCs;
    [SerializeField] GameObject[] Marlboros;
    int marlborosCollected;
    [NonSerialized] public int currentMainQuest = 0;
    [SerializeField] string[] questDescriptions;

    [NonSerialized] public int currentPingPongScamsQuest = 0;
    [SerializeField] string[] pingPongScamsQuestDescriptions;

    [NonSerialized] public int currentFollowingRefereesQuest = 0;
    [SerializeField] string[] followingRefereesQuestDescriptions;

    [NonSerialized] public int currentDiversionAndSearchQuest = 0;
    [SerializeField] string[] diversionAndSearchQuestDescriptions;

    [SerializeField] GameObject[] PonyStickers;
    int ponyStickersCollected;
    [NonSerialized] public int currentStrongStuffQuest = 0;
    [SerializeField] string[] strongStuffQuestDescriptions;

    [NonSerialized] public int currentFaceTheCheaterQuest = 0;
    [SerializeField] string[] faceTheCheaterQuestDescriptions;

    public int[] sideQuestLengths = new int[5];
    bool jaronaldMentioned = false, jaronaldFightAvailable = false;

    public bool canReturnToMainStory = true;

    public GameObject[] SuperToastIngredients;
    public bool finishedSuperToast;
    public int superToastProgress;

    [SerializeField] AudioClip[] additionalVoiceLines;
    void Awake()
    {
        sideQuestLengths[0] = pingPongScamsQuestDescriptions.Length;
        sideQuestLengths[1] = followingRefereesQuestDescriptions.Length;
        sideQuestLengths[2] = diversionAndSearchQuestDescriptions.Length;
        sideQuestLengths[3] = strongStuffQuestDescriptions.Length;
        sideQuestLengths[4] = faceTheCheaterQuestDescriptions.Length;
        instance = this;
        currentQuestText.text = questDescriptions[currentMainQuest];
        HandleAllNPCs(); //initializing all npcs
        marlborosCollected = 0;
        ponyStickersCollected = 0;
        superToastProgress = 0;
        finishedSuperToast = false;
    }

    void Update()
    {

    }

    public void CollectMarlboro()
    {
        if (++marlborosCollected == Marlboros.Length)
        {
            ProgressStory();
        }
    }

    void EnableMarlboros()
    {
        Debug.Log("enabling marlboros");
        foreach (var marlboro in Marlboros)
        {
            marlboro.SetActive(true);
        }
    }

    void DisableMarlboros()
    {
        Debug.Log("disabling marlboros");
        foreach (var marlboro in Marlboros)
        {
            marlboro.SetActive(false);
        }
    }
    public void CollectPonySticker()
    {
        ponyStickersCollected++;
        if (ponyStickersCollected == PonyStickers.Length)
        {
            string[] lines = {
                    "Dobra, to chyba wszystkie"
                    };
            int[] speakerIndexes = { 3 };
            AudioClip[] voiceLines = new AudioClip[1];
            voiceLines[0] = additionalVoiceLines[8];

            DialogueManager.instance.StartDialogue(lines, speakerIndexes, voiceLines); DialogueManager.instance.onDialogueEnd.RemoveAllListeners();
            DialogueManager.instance.onDialogueEnd.AddListener(() => {
                ProgressSideQuest(3);
            });
        }
    }

    void EnablePonyStickers()
    {
        Debug.Log("enabling pony stickers");
        foreach (var sticker in PonyStickers)
        {
            sticker.SetActive(true);
        }
    }

    void DisablePonyStickers()
    {
        Debug.Log("disabling pony stickers");
        foreach (var sticker in PonyStickers)
        {
            sticker.SetActive(false);
        }
    }
    public void HandleAllNPCs()
    {
        HandleStoryNPCs();
        HandleStoryTeleports();
        HandleFollowerNPCs();
        HandleSideQuestNPCs();
        HandleSuperToastIngredients();
    }

    public void HandleSuperToastIngredients()
    {
        superToastProgress = 0;
        SuperToastIngredients[0].SetActive(!SuperToastIngredients[0].GetComponent<ArtifactController>().wasSeen);
        SuperToastIngredients[1].SetActive(!SuperToastIngredients[1].GetComponent<ArtifactController>().wasSeen);
        foreach (var ingredient in SuperToastIngredients)
        {
            if (ingredient.GetComponent<ArtifactController>().wasSeen)
            {
                superToastProgress++;
            }
        }
        SuperToastIngredients[2].SetActive(superToastProgress == 2 && currentMainQuest >= 109);
        if (finishedSuperToast) superToastProgress = 4;
        Debug.Log(superToastProgress);
    }

    public void DisableAllNPCs()
    {
        DisableStoryNPCs();
        DisableStoryTeleports();
        DisableSideQuestNPCs();
        DisableFollowerNPCs();
    }

    public void DisablePatrolNPCs()
    {
        foreach (var npc in PatrolNPCs)
        {
            npc.SetActive(false);
        }
    }

    public void HandlePatrolNPCs()
    {
        foreach (var npc in PatrolNPCs)
        {
            if (currentMainQuest >= npc.GetComponent<Interactable>().appearanceAtQuest && currentMainQuest < npc.GetComponent<Interactable>().disappearanceAtQuest && npc.GetComponent<PatrolNPCController>().sideQuestId == -1)
            {
                npc.SetActive(true);
            }
            else
            {
                npc.SetActive(false);
            }
        }
        foreach (var pnpc in PatrolNPCs)
        {
            var sqpnpc = pnpc.GetComponent<PatrolNPCController>();
            switch (sqpnpc.sideQuestId)
            {
                case 0:
                    if (currentPingPongScamsQuest >= sqpnpc.appearanceAtQuest && currentPingPongScamsQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
                case 1:
                    if (currentFollowingRefereesQuest >= sqpnpc.appearanceAtQuest && currentFollowingRefereesQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
                case 2:
                    if (currentDiversionAndSearchQuest >= sqpnpc.appearanceAtQuest && currentDiversionAndSearchQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
                case 3:
                    if (currentStrongStuffQuest >= sqpnpc.appearanceAtQuest && currentStrongStuffQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
                case 4:
                    if (currentFaceTheCheaterQuest >= sqpnpc.appearanceAtQuest && currentFaceTheCheaterQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
            }
        }
    }

    public void HandleStoryNPCs()
    {
        foreach (var npc in storyNPCs)
        {
            if (currentMainQuest >= npc.GetComponent<Interactable>().appearanceAtQuest && currentMainQuest < npc.GetComponent<Interactable>().disappearanceAtQuest)
            {
                npc.SetActive(true);
            }
            else
            {
                npc.SetActive(false);
            }
            if (npc.GetComponent<Interactable>().appearanceAtQuest == 112 && jaronaldFightAvailable)
            {
                npc.SetActive(false);
            }
        }
        foreach (var trigger in storyTriggers)
        {
            if (currentMainQuest >= trigger.GetComponent<Interactable>().appearanceAtQuest && currentMainQuest < trigger.GetComponent<Interactable>().disappearanceAtQuest)
            {
                trigger.SetActive(true);
            }
            else
            {
                trigger.SetActive(false);
            }
        }
        foreach (var npc in PatrolNPCs)
        {
            if (currentMainQuest >= npc.GetComponent<Interactable>().appearanceAtQuest && currentMainQuest < npc.GetComponent<Interactable>().disappearanceAtQuest && npc.GetComponent<PatrolNPCController>().sideQuestId == -1)
            {
                npc.SetActive(true);
            }
            else
            {
                npc.SetActive(false);
            }
        }
    }

    public void DisableStoryNPCs()
    {
        foreach(var npc in storyNPCs)
        {
            npc.SetActive(false);
        }
        foreach(var trigger in storyTriggers)
        {
            trigger.SetActive(false);
        }
    }

    public void HandleStoryTeleports()
    {
        foreach (var tp in storyTeleports)
        {
            if (currentMainQuest >= tp.GetComponent<Interactable>().appearanceAtQuest && currentMainQuest < tp.GetComponent<Interactable>().disappearanceAtQuest)
            {
                tp.SetActive(true);
            }
            else
            {
                tp.SetActive(false);
            }
        }
    }

    public void DisableStoryTeleports()
    {
        foreach (var tp in storyTeleports)
        {
            tp.SetActive(false);
        }
    }

    public void HandleFollowerNPCs()
    {
        foreach (var npc in followerNPCs)
        {
            if (currentMainQuest >= npc.GetComponent<Interactable>().appearanceAtQuest && currentMainQuest < npc.GetComponent<Interactable>().disappearanceAtQuest)
            {
                npc.SetActive(true);
            }
            else
            {
                npc.SetActive(false);
            }
        }
    }

    public void DisableFollowerNPCs()
    {
        foreach (var npc in followerNPCs)
        {
            npc.SetActive(false);
        }
    }

    bool IsSideQuestCompleted(int sideQuest)
    {
        switch (sideQuest)
        {
            case -1: //mati vs jaronald override
                return true;
            case 0:
                return currentPingPongScamsQuest >= pingPongScamsQuestDescriptions.Length;
            case 1:
                return currentFollowingRefereesQuest >= followingRefereesQuestDescriptions.Length;
            case 2:
                return currentDiversionAndSearchQuest >= diversionAndSearchQuestDescriptions.Length;
            case 3:
                return currentStrongStuffQuest >= strongStuffQuestDescriptions.Length;
            case 4:
                return currentFaceTheCheaterQuest >= faceTheCheaterQuestDescriptions.Length;
        }
        return false;
    }

    public void HandleSideQuestNPCs(int sideQuestId = -1)
    {
        foreach (var npc in sideQuestNPCs)
        {
            var sqnpc = npc.GetComponent<SidequestNPCController>();
            if (sqnpc.appearsBasedOnMainStoryProgress)
            {
                if (currentMainQuest >= sqnpc.appearanceAtQuest && currentMainQuest < sqnpc.disappearanceAtQuest && sqnpc.GetProgressInSidequest() == 0
                    && (!sqnpc.appearsBasedOnOtherSideStoryProgress || sqnpc.appearsBasedOnOtherSideStoryProgress && IsSideQuestCompleted(sqnpc.otherSideQuestRequiredId))
                    && (sqnpc.sideQuestId == sideQuestId || sideQuestId == -1))
                {
                    npc.SetActive(true);
                }
                else
                {
                    npc.SetActive(false);
                }
            }
            else
            {
                switch (sqnpc.sideQuestId) 
                {
                    case 0:
                        if (currentPingPongScamsQuest >= sqnpc.appearanceAtQuest && currentPingPongScamsQuest < sqnpc.disappearanceAtQuest)
                        {
                            npc.SetActive(true);
                        }
                        else
                        {
                            npc.SetActive(false);
                        }
                        break;
                    case 1:
                        if (currentFollowingRefereesQuest >= sqnpc.appearanceAtQuest && currentFollowingRefereesQuest < sqnpc.disappearanceAtQuest)
                        {
                            npc.SetActive(true);
                        }
                        else
                        {
                            npc.SetActive(false);
                        }
                        break;
                    case 2:
                        if (currentDiversionAndSearchQuest >= sqnpc.appearanceAtQuest && currentDiversionAndSearchQuest < sqnpc.disappearanceAtQuest)
                        {
                            npc.SetActive(true);
                        }
                        else
                        {
                            npc.SetActive(false);
                        }
                        break;
                    case 3:
                        if (currentStrongStuffQuest >= sqnpc.appearanceAtQuest && currentStrongStuffQuest < sqnpc.disappearanceAtQuest)
                        {
                            npc.SetActive(true);
                        }
                        else
                        {
                            npc.SetActive(false);
                        }
                        break;
                    case 4:
                        if (currentFaceTheCheaterQuest >= sqnpc.appearanceAtQuest && currentFaceTheCheaterQuest < sqnpc.disappearanceAtQuest)
                        {
                            npc.SetActive(true);
                        }
                        else
                        {
                            npc.SetActive(false);
                        }
                        break;
                }
            }
        }
        foreach (var trigger in sideQuestTriggers)
        {
            var sqtrigger = trigger.GetComponent<SidequestTriggerController>();
            if (sqtrigger.appearsBasedOnMainStoryProgress)
            {
                if (currentMainQuest >= sqtrigger.appearanceAtQuest && currentMainQuest < sqtrigger.disappearanceAtQuest && sqtrigger.GetProgressInSidequest() == 0
                    && (!sqtrigger.appearsBasedOnOtherSideStoryProgress || sqtrigger.appearsBasedOnOtherSideStoryProgress && IsSideQuestCompleted(sqtrigger.otherSideQuestRequiredId))
                    && (sqtrigger.sideQuestId == sideQuestId || sideQuestId == -1))
                {
                    trigger.SetActive(true);
                }
                else
                {
                    trigger.SetActive(false);
                }
            }
            else
            {
                switch (sqtrigger.sideQuestId)
                {
                    case 0:
                        if (currentPingPongScamsQuest >= sqtrigger.appearanceAtQuest && currentPingPongScamsQuest < sqtrigger.disappearanceAtQuest)
                        {
                            trigger.SetActive(true);
                        }
                        else
                        {
                            trigger.SetActive(false);
                        }
                        break;
                    case 1:
                        if (currentFollowingRefereesQuest >= sqtrigger.appearanceAtQuest && currentFollowingRefereesQuest < sqtrigger.disappearanceAtQuest)
                        {
                            trigger.SetActive(true);
                        }
                        else
                        {
                            trigger.SetActive(false);
                        }
                        break;
                    case 2:
                        if (currentDiversionAndSearchQuest >= sqtrigger.appearanceAtQuest && currentDiversionAndSearchQuest < sqtrigger.disappearanceAtQuest)
                        {
                            trigger.SetActive(true);
                        }
                        else
                        {
                            trigger.SetActive(false);
                        }
                        break;
                    case 3:
                        if (currentStrongStuffQuest >= sqtrigger.appearanceAtQuest && currentStrongStuffQuest < sqtrigger.disappearanceAtQuest)
                        {
                            trigger.SetActive(true);
                        }
                        else
                        {
                            trigger.SetActive(false);
                        }
                        break;
                    case 4:
                        if (currentFaceTheCheaterQuest >= sqtrigger.appearanceAtQuest && currentFaceTheCheaterQuest < sqtrigger.disappearanceAtQuest)
                        {
                            trigger.SetActive(true);
                        }
                        else
                        {
                            trigger.SetActive(false);
                        }
                        break;
                }
            }
        }
        foreach (var pnpc in PatrolNPCs)
        {
            var sqpnpc = pnpc.GetComponent<PatrolNPCController>();
            switch (sqpnpc.sideQuestId)
            {

                case 0:
                    if (currentPingPongScamsQuest >= sqpnpc.appearanceAtQuest && currentPingPongScamsQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
                case 1:
                    if (currentFollowingRefereesQuest >= sqpnpc.appearanceAtQuest && currentFollowingRefereesQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
                case 2:
                    if (currentDiversionAndSearchQuest >= sqpnpc.appearanceAtQuest && currentDiversionAndSearchQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
                case 3:
                    if (currentStrongStuffQuest >= sqpnpc.appearanceAtQuest && currentStrongStuffQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
                case 4:
                    if (currentFaceTheCheaterQuest >= sqpnpc.appearanceAtQuest && currentFaceTheCheaterQuest < sqpnpc.disappearanceAtQuest)
                    {
                        pnpc.SetActive(true);
                    }
                    else
                    {
                        pnpc.SetActive(false);
                    }
                    break;
            }
        }
    }

    public void DisableSideQuestNPCs()
    {
        foreach (var npc in sideQuestNPCs)
        {
            npc.SetActive(false);
        }
        foreach (var trigger in sideQuestTriggers)
        {
            trigger.SetActive(false);
        }
    }

    void AdjustFollowerNPCsDistance(bool keepClose)
    {
        float distance;
        if (keepClose)
        {
            distance = 0.4f;
        }
        else
        {
            distance = 1.2f;
        }
        foreach (var npc in followerNPCs)
        {
            npc.GetComponent<PlayerFollowerController>().distanceFromTarget = distance;
        }
    }

    public void ProgressStory(bool isPlayingGameNormally = true)
    {
        Debug.Log("progressing story");
        currentQuestText.text = questDescriptions[++currentMainQuest];
        //FriendlyCharacter character;
        switch (currentMainQuest)
        {
            case 2:
                //zdjecie kurtki
                player.ChangeAnimator(1);
                break;
            case 4:
                //pobity uczen
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Zwiêkszanie poziomu postaci daje tokeny, które mo¿esz wykorzystaæ na ulepszenie postaci. Drzewo rozwoju postaci znajdziesz w menu pauzy. Zagl¹daj tam czêsto!");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 5:
                //odwiedzony sklep
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("U toœciary mo¿esz kupowaæ przedmioty lecz¹ce oraz wyposa¿enie kluczowe do zwyciêstwa w walkach. Zajrzyj tu, jak zaczniesz mieæ trudnoœci z pokonywaniem przeciwników.");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 9:
                //dodanie Welenca
                BattleManager.instance.currentPartyCharacters.Add(1);
                GameManager.instance.currentFreeroamMusicStage = 1;
                if (isPlayingGameNormally) GameManager.instance.PlayFreeroamMusic();
                break;
            case 10:
                GameManager.instance.currentFreeroamMusicStage = 2;
                if (isPlayingGameNormally) GameManager.instance.PlayFreeroamMusic();
                break;
            case 14:
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Misja poboczna dostêpna! Udaj siê na salê od ping ponga (2. piêtro, blok A), aby j¹ wykonaæ");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 19:
                //dodanie Stasiaka;
                BattleManager.instance.currentPartyCharacters.Add(2);
                player.AllowRandomEncounters();
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("PRZYPOMNIENIE - Zwiêkszanie poziomu postaci daje tokeny, które mo¿esz wykorzystaæ na ulepszenie postaci. Drzewo rozwoju postaci znajdziesz w menu pauzy. Zagl¹daj tam czêsto!");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 24:
                //dodanie Mai
                BattleManager.instance.currentPartyCharacters.Add(3);
                break;
            case 25:
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Misja poboczna dostêpna! Udaj siê na 3. piêtro w bloku D, aby j¹ wykonaæ");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 27:
                //skradanka elektrotechnik
                AdjustFollowerNPCsDistance(true);
                break;
            case 29:
                //koniec skradanki elektrotechnik
                AdjustFollowerNPCsDistance(false);
                GameManager.instance.artifacts[5].GetComponent<ArtifactController>().wasSeen = true;
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Zdobyto nowy artefakt. Artefakty mo¿esz przegl¹daæ w menu pauzy.");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 30:
                EnableMarlboros();
                break;
            case 31:
                DisableMarlboros();
                GameManager.instance.artifacts[1].GetComponent<ArtifactController>().wasSeen = true;
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Zdobyto nowy artefakt. Artefakty mo¿esz przegl¹daæ w menu pauzy.");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 34:
                //Maja zostaje pod lazienka
                BattleManager.instance.currentPartyCharacters.Remove(3);
                break;
            case 36:
                //Maja wraca do druzyny
                BattleManager.instance.currentPartyCharacters.Add(3);
                break;
            case 40:
                if (isPlayingGameNormally && IsSideQuestCompleted(0))
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Misja poboczna dostêpna! Udaj siê miêdzy kanapy na 2. piêtrze w bloku B, aby j¹ wykonaæ");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 43:
                //first lockpick
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Aby otworzyæ zamkniête drzwi, przekrêcaj wytrych. Dr¿enie wytrychu oznacza, ¿e jesteœ blisko dobrego ustawienia. Im bli¿ej dobrego ustawienia, tym szersze pole na pasku szybkiego trafienia.");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 50:
                //dodanie Burzyñskiego
                player.currentRandomEncounterStage = 1;
                BattleManager.instance.currentPartyCharacters.Add(4);
                if (isPlayingGameNormally)
                {
                    while (BattleManager.instance.playableCharacters[4].Level < 8)
                    {
                        BattleManager.instance.playableCharacters[4].HandleLevel(500);
                    }
                }
                break;
            case 51:
                GameManager.instance.currentFreeroamMusicStage = 3;
                if (isPlayingGameNormally) GameManager.instance.PlayFreeroamMusic();
                if (isPlayingGameNormally && IsSideQuestCompleted(1))
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Misja poboczna dostêpna! Udaj siê na 1. piêtro do bloku D, aby j¹ wykonaæ");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 52:
                GameManager.instance.currentFreeroamMusicStage = 4;
                if (isPlayingGameNormally) GameManager.instance.PlayFreeroamMusic();
                break;
            case 60:
                //przejœcie do Lory
                player.PreventRandomEncounters();
                if (isPlayingGameNormally)
                {
                    StartCoroutine(GameManager.instance.FadeToBlack(0.7f));
                    StartCoroutine(TransitionToLora());
                }
                else
                {
                    player.ChangeAnimator(2);
                    player.transform.position = new Vector2(-207, 52);
                }
                BattleManager.instance.currentPartyCharacters.RemoveAll(x => x >= 0);
                BattleManager.instance.currentPartyCharacters.Add(6); //Janek
                BattleManager.instance.currentPartyCharacters.Add(5); //Lora
                break;
            case 69:
                //ucieczka przed caryca
                player.ChangeAnimator(1);
                BattleManager.instance.currentPartyCharacters.RemoveAll(x => x >= 0);
                BattleManager.instance.currentPartyCharacters.Add(0); //Rogos
                BattleManager.instance.currentPartyCharacters.Add(1); //Welenc
                BattleManager.instance.currentPartyCharacters.Add(2); //Stasiak
                BattleManager.instance.currentPartyCharacters.Add(3); //Kaja
                AdjustFollowerNPCsDistance(true);
                GameManager.instance.currentFreeroamMusicStage = 5;
                cam.orthographicSize = 5f;
                if (isPlayingGameNormally)
                {
                    GameManager.instance.PlayFreeroamMusic();
                }
                break;
            case 73:
                //koniec ucieczki
                if (isPlayingGameNormally)
                {
                    PlayerPrefs.SetInt("cutsceneId", 1);
                    SceneManager.LoadScene("cutscenes");
                }
                break;
            case 74:
                //poczatek outside'u
                player.transform.position = new Vector2(-414, 185);
                cam.orthographicSize = 5f;
                AdjustFollowerNPCsDistance(false);
                GameManager.instance.currentFreeroamMusicStage = 6;
                if (isPlayingGameNormally) GameManager.instance.PlayFreeroamMusic();
                GameManager.instance.currentLocationText.text = "Za szko³¹";
                break;
            case 81:
                player.moveSpeed = 8;
                foreach (var follower in followerNPCs)
                {
                    follower.GetComponent<PlayerFollowerController>().moveSpeed = 9;
                }
                break;
            case 82:
                player.moveSpeed = 3.5f;
                foreach (var follower in followerNPCs)
                {
                    follower.GetComponent<PlayerFollowerController>().moveSpeed = 3.5f;
                }
                break;
            case 83:
                player.moveSpeed = 8;
                foreach (var follower in followerNPCs)
                {
                    follower.GetComponent<PlayerFollowerController>().moveSpeed = 9;
                }
                break;
            case 84:
                player.moveSpeed = 3.5f;
                foreach (var follower in followerNPCs)
                {
                    follower.GetComponent<PlayerFollowerController>().moveSpeed = 3.5f;
                }
                break;
            case 87:
                player.moveSpeed = 8;
                foreach (var follower in followerNPCs)
                {
                    follower.GetComponent<PlayerFollowerController>().moveSpeed = 9;
                }
                break;
            case 88:
                player.moveSpeed = 3.5f;
                foreach (var follower in followerNPCs)
                {
                    follower.GetComponent<PlayerFollowerController>().moveSpeed = 3.5f;
                }
                break;
            case 90:
                player.moveSpeed = 8;
                foreach (var follower in followerNPCs)
                {
                    follower.GetComponent<PlayerFollowerController>().moveSpeed = 9;
                }
                break;
            case 91:
                //wejscie do podziemi
                player.currentRandomEncounterStage = 2;
                GameManager.instance.currentFreeroamMusicStage = 7;
                GameManager.instance.currentLocationText.text = "Podziemia szko³y";
                cam.orthographicSize = 3.5f;
                player.moveSpeed = 3.5f;
                foreach (var follower in followerNPCs)
                {
                    follower.GetComponent<PlayerFollowerController>().moveSpeed = 3.5f;
                }
                player.AllowRandomEncounters();
                if (isPlayingGameNormally)
                {
                    StartCoroutine(GameManager.instance.FadeToBlack(0.7f));
                    StartCoroutine(TransitionToUnderground());
                    GameManager.instance.PlayFreeroamMusic();
                }
                else
                {
                    player.transform.position = new Vector2(-258, 175);
                }
                break;
            case 101:
                //poczatek patrolujacych szkieletow
                AdjustFollowerNPCsDistance(true);
                break;
            case 108:
                //koniec patrolujacych szkieletow
                AdjustFollowerNPCsDistance(false);
                break;
            case 110:
                //wyjscie z podziemi
                if (isPlayingGameNormally)
                {
                    PlayerPrefs.SetInt("cutsceneId", 2);
                    SceneManager.LoadScene("cutscenes");
                }
                break;
            case 111:
                player.currentRandomEncounterStage = 1;
                player.transform.position = new Vector2(-300.5f, 45);
                GameManager.instance.currentFreeroamMusicStage = 8;
                if (isPlayingGameNormally) GameManager.instance.PlayFreeroamMusic();
                GameManager.instance.currentLocationText.text = "Parter, blok D";
                break;
            case 121:
                player.PreventRandomEncounters();
                break;
            case 124:
                //burzynski chase
                BattleManager.instance.currentPartyCharacters.Remove(1);
                BattleManager.instance.currentPartyCharacters.Remove(2);
                AdjustFollowerNPCsDistance(true);
                GameManager.instance.currentFreeroamMusicStage = 5;
                cam.orthographicSize = 5f;
                if (isPlayingGameNormally)
                {
                    GameManager.instance.PlayFreeroamMusic();
                }
                break;
            case 130:
                GameManager.instance.currentFreeroamMusicStage = 3;
                AdjustFollowerNPCsDistance(false);
                break;
            case 131:
                cam.orthographicSize = 3.5f;
                break;
            case 137:
                GameManager.instance.artifacts[8].GetComponent<ArtifactController>().wasSeen = true;
                if (isPlayingGameNormally)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Zdobyto nowy artefakt. Artefakty mo¿esz przegl¹daæ w menu pauzy.");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
                break;
            case 141:
                if (isPlayingGameNormally)
                {
                    PlayerPrefs.SetInt("cutsceneId", 3);
                    SceneManager.LoadScene("cutscenes");
                }
                break;
                //koniec gry

        }
        if (isPlayingGameNormally)
        {
            foreach (int i in ShopManager.instance.storyRequirementsForUpgrades)
            {
                if (i == currentMainQuest)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Ulepszenie sklepu dostêpne!");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
            }
        }
        HandleAllNPCs();
    }

    void FinishSideQuest()
    {
        Debug.Log("koniec side questa");
        //showStoryNPCs jest true gdy gra nie jest wczytywana aka jest grana normalnie
        if (canReturnToMainStory) //gdy gra nie jest wczytywana LUB nie jestesmy w side quescie
        {
            HandleAllNPCs();
            currentQuestText.text = questDescriptions[currentMainQuest];
        }
        else //gry gra jest wczytywana i jestesmy w side questach
        {
            HandleSideQuestNPCs();
        }
        player.AllowRandomEncounters();
    }

    public void ProgressSideQuest(int sideQuest, bool readDialogue = true)
    {
        DisableStoryNPCs();
        switch (sideQuest)
        {
            case 0:
                ProgressPingPongScams(readDialogue);
                break;
            case 1:
                ProgressFollowingReferees(readDialogue);
                break;
            case 2:
                ProgressDiversionAndSearch(readDialogue);
                break;
            case 3:
                ProgressStrongStuff(readDialogue);
                break;
            case 4:
                ProgressFaceTheCheater(readDialogue);
                break;
        }
    }

    void ProgressPingPongScams(bool readDialogue)
    {
        if (currentPingPongScamsQuest + 1 >= pingPongScamsQuestDescriptions.Length)
        {
            currentPingPongScamsQuest++;
            FinishSideQuest();
        }
        else
        {
            currentPingPongScamsQuest++;
            if (canReturnToMainStory)
            {
                currentQuestText.text = pingPongScamsQuestDescriptions[currentPingPongScamsQuest];
            }
            HandleSideQuestNPCs(0);
        }
    }

    void ProgressFollowingReferees(bool readDialogue)
    {
        if (currentFollowingRefereesQuest + 1 >= followingRefereesQuestDescriptions.Length)
        {
            currentFollowingRefereesQuest++;
            if (readDialogue)
            {
                StartCoroutine(GameManager.instance.FadeToBlack(0.7f));
                StartCoroutine(FinishFollowingReferees());
                if (currentMainQuest >= 51 && currentMainQuest < 57)
                {
                    List<string> gameInfoLines = new List<string>();
                    gameInfoLines.Add("Misja poboczna dostêpna! Udaj siê na 1. piêtro do bloku D, aby j¹ wykonaæ");
                    DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                }
            }
            else
            {
                player.ChangeAnimator(1);
                player.transform.position = new Vector2(-300, 32);
            }
            FinishSideQuest();
        }
        else
        {
            currentFollowingRefereesQuest++;
            if (canReturnToMainStory)
            {
                currentQuestText.text = followingRefereesQuestDescriptions[currentFollowingRefereesQuest];
            }
            switch (currentFollowingRefereesQuest)
            {
                case 1:
                    DisableFollowerNPCs();
                    if (readDialogue)
                    {
                        StartCoroutine(GameManager.instance.FadeToBlack(0.7f));
                        StartCoroutine(TransitionToManiaFollowingReferees());
                    }
                    else
                    {
                        player.ChangeAnimator(3);
                        player.transform.position = new Vector2(-309, -112);
                    }
                    player.PreventRandomEncounters();
                    cam.orthographicSize = 5f;
                    break;
                case 6:
                    cam.orthographicSize = 3.5f;
                    break;
            }
            HandleSideQuestNPCs(1);
        }
    }

    void ProgressDiversionAndSearch(bool readDialogue)
    {
        if (currentDiversionAndSearchQuest + 1 >= diversionAndSearchQuestDescriptions.Length)
        {
            if (jaronaldMentioned)
            {
                if (readDialogue)
                {
                    string[] lines = {
                    "Czekajcie! Jaronald... Gdzieœ s³ysza³em to imiê.",
                    "To nie o nim mówili dresiarze?",
                    "Cholera.",
                    "Trzeba daæ znaæ Matiemu.",
                    "Napisa³em do niego, ¿eby na niego uwa¿a³."
                };
                    int[] speakerIndexes = { 1, 0, 2, 3, 0 };
                    AudioClip[] voiceLines = new AudioClip[5];
                    voiceLines[0] = additionalVoiceLines[0];
                    voiceLines[1] = additionalVoiceLines[1];
                    voiceLines[2] = additionalVoiceLines[2];
                    voiceLines[3] = additionalVoiceLines[3];
                    voiceLines[4] = additionalVoiceLines[4];
                    DialogueManager.instance.StartDialogue(lines, speakerIndexes, voiceLines);
                }
                jaronaldFightAvailable = true;
            }
            else
            {
                jaronaldMentioned = true;
            }
            currentDiversionAndSearchQuest++;
            FinishSideQuest();
        }
        else
        {
            currentDiversionAndSearchQuest++;
            if (canReturnToMainStory)
            {
                currentQuestText.text = diversionAndSearchQuestDescriptions[currentDiversionAndSearchQuest];
            }
            switch (currentDiversionAndSearchQuest)
            {
                case 1:
                    DisableFollowerNPCs();
                    if (readDialogue)
                    {
                        StartCoroutine(GameManager.instance.FadeToBlack(0.7f));
                        StartCoroutine(TransitionToManiaDiversionAndSearch());
                    }
                    else
                    {
                        player.ChangeAnimator(3);
                        player.transform.position = new Vector2(-174, 11.5f);
                    }
                    player.PreventRandomEncounters();
                    break;
                case 2:
                    if (readDialogue)
                    {
                        StartCoroutine(GameManager.instance.FadeToBlack(0.7f));
                        StartCoroutine(TransitionBackToRogosDiversionAndSearch());
                    }
                    else
                    {
                        player.ChangeAnimator(1);
                        player.transform.position = new Vector2(-300, 35);
                    }
                    HandleFollowerNPCs();
                    followerNPCs[4].SetActive(false);
                    break;
                case 4:
                    GameManager.instance.artifacts[2].GetComponent<ArtifactController>().wasSeen = true;
                    if (readDialogue)
                    {
                        List<string> gameInfoLines = new List<string>();
                        gameInfoLines.Add("Zdobyto nowy artefakt. Artefakty mo¿esz przegl¹daæ w menu pauzy.");
                        DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                    }
                    break;
            }
            HandleSideQuestNPCs(2);
        }
    }

    void ProgressStrongStuff(bool readDialogue)
    {
        if (currentStrongStuffQuest + 1 >= strongStuffQuestDescriptions.Length)
        {
            if (jaronaldMentioned)
            {
                if (readDialogue)
                {
                    string[] lines = {
                    "Czekajcie! Jaronald... Gdzieœ s³ysza³em to imiê.",
                    "To nie on by³ zaznaczony na liœcie zawodników turnieju?",
                    "Cholera.",
                    "Trzeba daæ znaæ Matiemu.",
                    "Napisa³em do niego, ¿eby na niego uwa¿a³."
                };
                    int[] speakerIndexes = { 1, 0, 2, 3, 0 };
                    AudioClip[] voiceLines = new AudioClip[5];
                    voiceLines[0] = additionalVoiceLines[0];
                    voiceLines[1] = additionalVoiceLines[5];
                    voiceLines[2] = additionalVoiceLines[2];
                    voiceLines[3] = additionalVoiceLines[3];
                    voiceLines[4] = additionalVoiceLines[4];
                    DialogueManager.instance.StartDialogue(lines, speakerIndexes, voiceLines);
                }
                jaronaldFightAvailable = true;
            }
            else
            {
                jaronaldMentioned = true;
            }
            currentStrongStuffQuest++;
            FinishSideQuest();
        }
        else
        {
            currentStrongStuffQuest++;
            if (canReturnToMainStory)
            {
                currentQuestText.text = strongStuffQuestDescriptions[currentStrongStuffQuest];
            }
            switch (currentStrongStuffQuest)
            {
                case 1:
                    EnablePonyStickers();
                    break;
                case 3:
                    DisablePonyStickers();
                    if (readDialogue)
                    {
                        string[] lines = {
                            "Ej, Wy! Co to ma kurna byæ, co?!",
                            "Oho..."
                        };
                        int[] speakerIndexes = { 19, 0 };
                        AudioClip[] voiceLines = new AudioClip[2];
                        voiceLines[0] = additionalVoiceLines[6];
                        voiceLines[1] = additionalVoiceLines[7];
                        DialogueManager.instance.onDialogueEnd.RemoveAllListeners();
                        DialogueManager.instance.onDialogueEnd.AddListener(() =>
                        {
                            DialogueManager.instance.onDialogueEnd.RemoveAllListeners();
                            List<string> gameInfoLines = new List<string>();
                            gameInfoLines.Add("Zdobyto nowy artefakt. Artefakty mo¿esz przegl¹daæ w menu pauzy.");
                            DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
                        });
                        DialogueManager.instance.StartDialogue(lines, speakerIndexes, voiceLines);
                    }
                    GameManager.instance.artifacts[9].GetComponent<ArtifactController>().wasSeen = true;
                    
                    break;
            }
            HandleSideQuestNPCs(3);
        }
    }

    void ProgressFaceTheCheater(bool readDialogue)
    {
        if (currentFaceTheCheaterQuest + 1 >= faceTheCheaterQuestDescriptions.Length)
        {
            currentFaceTheCheaterQuest++;
            FinishSideQuest();
            ProgressStory();

            Inventory.instance.wearables[19].Add(1); //gold medal
            GameManager.instance.lastPageOfWearablesUnlocked = true;
            List<string> gameInfoLines = new List<string>();
            gameInfoLines.Add("Otrzymujesz Z³oty Medal Matiego (atrybut ofensywny)");
            DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
        }
        else
        {
            currentFaceTheCheaterQuest++;
            if (canReturnToMainStory)
            {
                currentQuestText.text = faceTheCheaterQuestDescriptions[currentFaceTheCheaterQuest];
            }
            switch (currentFaceTheCheaterQuest)
            {
                case 1:
                    DisableFollowerNPCs();
                    player.PreventRandomEncounters();
                    if (readDialogue)
                    {
                        StartCoroutine(GameManager.instance.FadeToBlack(0.7f));
                        StartCoroutine(TransitionToMatiFaceTheCheater());
                    }
                    else
                    {
                        player.ChangeAnimator(4);
                        player.transform.position = new Vector2(-193, -80);
                    }
                    break;
                case 3:
                    if (readDialogue)
                    {
                        StartCoroutine(GameManager.instance.FadeToBlack(0.7f));
                        StartCoroutine(TransitionBackToRogosFaceTheCheater());
                    }
                    else
                    {
                        player.ChangeAnimator(1);
                        player.transform.position = new Vector2(-290, -117);
                    }

                    break;
            }
            HandleSideQuestNPCs(4);
        }
    }

    IEnumerator TransitionToLora()
    {
        yield return new WaitForSeconds(0.4f);
        player.ChangeAnimator(2);
        player.transform.position = new Vector2(-207, 52);
    }
    IEnumerator TransitionToUnderground()
    {
        yield return new WaitForSeconds(0.4f);
        player.transform.position = new Vector2(-258, 175);
    }
    IEnumerator TransitionToManiaFollowingReferees()
    {
        yield return new WaitForSeconds(0.4f);
        player.ChangeAnimator(3);
        player.transform.position = new Vector2(-309, -112);
    }
    IEnumerator FinishFollowingReferees()
    {
        yield return new WaitForSeconds(0.4f);
        player.ChangeAnimator(1);
        player.transform.position = new Vector2(-300, 32);
    }

    IEnumerator TransitionToManiaDiversionAndSearch()
    {
        yield return new WaitForSeconds(0.4f);
        player.ChangeAnimator(3);
        player.transform.position = new Vector2(-174, 11.5f);
    }

    IEnumerator TransitionBackToRogosDiversionAndSearch()
    {
        yield return new WaitForSeconds(0.4f);
        player.ChangeAnimator(1);
        player.transform.position = new Vector2(-300, 35);
    }

    IEnumerator TransitionToMatiFaceTheCheater()
    {
        yield return new WaitForSeconds(0.4f);
        player.ChangeAnimator(4);
        player.transform.position = new Vector2(-193, -80);
    }
    IEnumerator TransitionBackToRogosFaceTheCheater()
    {
        yield return new WaitForSeconds(0.4f);
        player.ChangeAnimator(1);
        player.transform.position = new Vector2(-290, -117);
    }
}
