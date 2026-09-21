using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class LockpickController : Interactable
{
    public UnityEvent onSkillCheckFinished;
    bool isLockpicking = false, isSuccessful = false;
    float currentAngle, optimalAngle;
    const int skillCheckSliderWidth = 500;
    const float defaultSkillCheckTime = 1.75f;
    bool skillCheckGoingRight, skillCheckAcceptsInput;
    float skillCheckTime = defaultSkillCheckTime, defaultGreenAreaScale;
    [SerializeField] int attemptsToUnlock = 1;
    int currentAttempt = 0;
    [SerializeField] AudioClip doorUnlockSound;
    [SerializeField] AudioClip lockpickProgressSound;

    [SerializeField] AudioMixer mixer;
    AudioSource sfxSource;
    public AudioMixerGroup sfxMixerGroup;

    [Header("Dialogue on success")]
    [SerializeField] string[] successLines;
    [SerializeField] AudioClip[] successVoiceLines;
    [SerializeField] int[] successSpeakersIndexes;

    [Header("Dialogue on failure")]
    [SerializeField] string[] failureLines;
    [SerializeField] AudioClip[] failureVoiceLines;
    [SerializeField] int[] failureSpeakersIndexes;

    GameObject lockpick;
    GameObject[] locks;

    private void Start()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        defaultGreenAreaScale = GameManager.instance.skillCheckGreenArea.transform.localScale.x;
    }
    public override void Interact()
    {
        isLockpicking = true;
        isSuccessful = false;
        optimalAngle = Random.Range(-270f, -90f);
        Debug.Log(optimalAngle);
        currentAngle = -90;
        skillCheckTime = defaultSkillCheckTime - 0.25f * GameManager.instance.difficulty;

        lockpick = GameManager.instance.movingLockpick;
        lockpick.transform.eulerAngles = new Vector3(0, 0, currentAngle);
        locks = GameManager.instance.locks;
        foreach(GameObject l in locks)
        {
            l.GetComponent<Image>().sprite = GameManager.instance.lockedLock;
            l.SetActive(false);
        }
        attemptsToUnlock = Mathf.Min(3, attemptsToUnlock);
        for (int i = 0; i < attemptsToUnlock; i++)
        {
            locks[i].SetActive(true);
        }
        for (int i = 0; i < currentAttempt; i++)
        {
            locks[i].GetComponent<Image>().sprite = GameManager.instance.unlockedLock;
        }
        GameManager.instance.lockpickCanvas.enabled = true;
        GameManager.instance.inGameCanvas.enabled = false;
        GameManager.instance.skillCheckSlider.gameObject.SetActive(false);
    } 

    void Update()
    {
        base.Update();
        if (skillCheckAcceptsInput)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                skillCheckAcceptsInput = false;
            }
        }
        if (isLockpicking)
        {
            HandleInput();
            int performance = Mathf.Max(10 - (int)Mathf.Abs(optimalAngle - currentAngle), 0);
            lockpick.transform.eulerAngles = new Vector3(0, 0, currentAngle + performance * Mathf.PerlinNoise(3 * performance * Time.time, 1) / 2);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        { //check password
            isLockpicking = false;
            int performance = Mathf.Max(10 - (int)Mathf.Abs(optimalAngle - currentAngle), 0);
            HandleSkillCheck(performance);
            onSkillCheckFinished.AddListener(HandlePostSkillCheckLogic);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            currentAngle -= Time.deltaTime * 60;
            currentAngle = Mathf.Max(currentAngle, -270);
            lockpick.transform.eulerAngles = new Vector3(0, 0, currentAngle);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            currentAngle += Time.deltaTime * 60;
            currentAngle = Mathf.Min(currentAngle, -90);
            lockpick.transform.eulerAngles = new Vector3(0, 0, currentAngle);
        }
    }

    void HandleSkillCheck(int performance)
    {
        GameObject greenArea = GameManager.instance.skillCheckGreenArea;
        int greenPosition = (int)Random.Range(-skillCheckSliderWidth * (1 - greenArea.transform.localScale.x) / 2, skillCheckSliderWidth * (1 - greenArea.transform.localScale.x) / 2);
        Vector2 newPos = greenArea.transform.localPosition;
        Vector3 newScale = greenArea.transform.localScale;
        newPos.x = greenPosition;
        newScale.x = defaultGreenAreaScale * performance / 7;
        greenArea.transform.localPosition = newPos;
        greenArea.transform.localScale = newScale;
        GameManager.instance.skillCheckSlider.value = 0;
        GameManager.instance.skillCheckSlider.gameObject.SetActive(true);
        skillCheckGoingRight = true;
        skillCheckAcceptsInput = true;
        StartCoroutine(PerformSkillCheck(greenPosition));
    }

    IEnumerator PerformSkillCheck(int greenPos)
    {
        Slider slider = GameManager.instance.skillCheckSlider;
        float totalSkillCheckDistance = 0f;
        while (totalSkillCheckDistance <= 200)
        {
            if (skillCheckAcceptsInput)
            {
                if (skillCheckGoingRight)
                {
                    slider.value += 2 * slider.maxValue * Time.deltaTime / skillCheckTime;
                    if (slider.value == slider.maxValue)
                    {
                        skillCheckGoingRight = false;
                    }
                }
                else
                {
                    slider.value -= 2 * slider.maxValue * Time.deltaTime / skillCheckTime;
                }
                totalSkillCheckDistance += 2 * slider.maxValue * Time.deltaTime / skillCheckTime;
            }
            else
            {
                break;
            }
            //yield return new WaitForSeconds(skillCheckTime / (2 * skillCheckSlider.maxValue));
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        slider.gameObject.SetActive(false);
        skillCheckAcceptsInput = false;
        float value = -skillCheckSliderWidth / 2 + skillCheckSliderWidth * slider.value / slider.maxValue;
        Debug.Log("value: " + value);
        Debug.Log("green: " + (greenPos - skillCheckSliderWidth * GameManager.instance.skillCheckGreenArea.transform.localScale.x / 2) + " - " + (greenPos + skillCheckSliderWidth * GameManager.instance.skillCheckGreenArea.transform.localScale.x / 2));
        if (value >= greenPos - skillCheckSliderWidth * GameManager.instance.skillCheckGreenArea.transform.localScale.x / 2
            && value <= greenPos + skillCheckSliderWidth * GameManager.instance.skillCheckGreenArea.transform.localScale.x / 2)
        { //hit green
            isSuccessful = true;
        }
        else
        {
            isSuccessful = false;
        }
        onSkillCheckFinished.Invoke();
    }

    void HandlePostSkillCheckLogic()
    {
        onSkillCheckFinished.RemoveAllListeners();
        Debug.Log(isSuccessful + " " + currentAttempt);
        if (isSuccessful)
        {
            currentAttempt++;
            sfxSource.clip = lockpickProgressSound;
            sfxSource.loop = false;
            sfxSource.Play();
            for (int i = 0; i < currentAttempt; i++)
            {
                locks[i].GetComponent<Image>().sprite = GameManager.instance.unlockedLock;
            }
            if (currentAttempt >= attemptsToUnlock)
            {
                StartCoroutine(FinalizeAndExit());
            }
            else
            {
                Interact();
            }
        }
        else
        {
            StartCoroutine(FinalizeAndExit());
        }
    }

    IEnumerator FinalizeAndExit()
    {
        currentAttempt = 0;
        if (isSuccessful)
        {
            sfxSource.clip = doorUnlockSound;
            sfxSource.loop = false;
            sfxSource.Play();
        }
        yield return new WaitForSeconds(1);
        if (isSuccessful)
        {
            DialogueManager.instance.onDialogueEnd.RemoveAllListeners();
            DialogueManager.instance.StartDialogue(successLines, successSpeakersIndexes, successVoiceLines);
            if (interactionProgressesStory)
            {
                DialogueManager.instance.onDialogueEnd.AddListener(() => StoryManager.instance.ProgressStory()); //after dialogue progress story
            }
            if (interactionSavesGame)
            {
                DialogueManager.instance.onDialogueEnd.AddListener(GameManager.instance.SaveGame); //after dialogue save game
            }
            if (interactionBlocksSavingGame)
            {
                DialogueManager.instance.onDialogueEnd.AddListener(() => GameManager.instance.canSaveGame = false);
            }
            else
            {
                DialogueManager.instance.onDialogueEnd.AddListener(() => GameManager.instance.canSaveGame = true);
            }
        }
        else
        {
            DialogueManager.instance.onDialogueEnd.RemoveAllListeners();
            DialogueManager.instance.StartDialogue(failureLines, failureSpeakersIndexes, failureVoiceLines);
        }
        
        Exit();
    }

    void Exit()
    {
        GameManager.instance.lockpickCanvas.enabled = false;
        GameManager.instance.inGameCanvas.enabled = true;
        isLockpicking = false;
    }
}
