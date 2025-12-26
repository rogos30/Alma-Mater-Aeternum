using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;
    public GameObject[] videoPlayer;
    int cutsceneId;


    public Canvas pauseCanvas;

    [SerializeField] AudioMixer mixer;
    [NonSerialized] public AudioSource masterSource, sfxSource, cutsceneSource;
    public AudioMixerGroup masterMixerGroup, sfxMixerGroup, cutsceneMixerGroup;

    readonly string[] difficultyNames = { "£atwy", "Œredni", "Trudny", "Fatalny" };
    readonly string[] lockFpsNames = { "Tak", "Nie" };
    [NonSerialized] public bool canPause = true;

    int currentRow, maxCurrentRow, currentColumn, currentPage;
    int chosenMain, chosenInv, chosenChar, chosenCharOption, chosenEqCategory, chosenPage;
    int sfxVolume = 25, musicVolume = 25;

    [SerializeField] TMP_Text[] mainColumnTexts;
    [SerializeField] GameObject optionsColumn;
    [SerializeField] TMP_Text[] optionsTexts;
    [SerializeField] TMP_Text[] optionValuesTexts;

    [SerializeField] AudioClip navigationScrollSound;
    [SerializeField] AudioClip navigationCancelSound;
    [SerializeField] AudioClip navigationAcceptSound;
    [SerializeField] AudioClip actionForbiddenSound;

    Color orange = new Color(0.976f, 0.612f, 0.007f);
    bool lockFPS = false;
    [NonSerialized] public int difficulty = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        masterSource = gameObject.AddComponent<AudioSource>();
        masterSource.outputAudioMixerGroup = masterMixerGroup;
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        cutsceneSource = gameObject.AddComponent<AudioSource>();
        cutsceneSource.outputAudioMixerGroup = cutsceneMixerGroup;
    }
    void Start()
    {
        Time.timeScale = 1;
        cutsceneId = PlayerPrefs.GetInt("cutsceneId");
        videoPlayer[cutsceneId].SetActive(true);
        //videoPlayer.clip = cutscenes[0];
    }

    // Update is called once per frame
    void Update()
    {
        /*if (pauseCanvas.enabled)
        {
            HandleInput();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }*/
    }


    public void Pause()
    {
        canPause = false;
        currentRow = 0;
        mainColumnTexts[currentRow].color = orange;
        maxCurrentRow = mainColumnTexts.Length;
        Time.timeScale = 0;
        pauseCanvas.enabled = true;
    }

    void Unpause()
    {
        Time.timeScale = 1;
        pauseCanvas.enabled = false;
        StartCoroutine(AllowToPause());
        mainColumnTexts[currentRow].color = Color.white;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sfxSource.clip = navigationCancelSound;
            sfxSource.loop = false;
            sfxSource.Play();
            switch (currentColumn)
            {
                case (int)PauseState.MAIN:
                    Unpause();
                    break;
                case (int)PauseState.SETTINGS:
                    SaveSettings();
                    optionsColumn.SetActive(false);
                    currentColumn = (int)PauseState.MAIN;
                    optionsTexts[currentRow].color = Color.white;
                    optionValuesTexts[currentRow].color = Color.white;
                    currentRow = chosenMain;
                    mainColumnTexts[currentRow].color = orange;
                    maxCurrentRow = mainColumnTexts.Length;
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            switch (currentColumn)
            {
                case (int)PauseState.MAIN: //main column
                    mainColumnTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    mainColumnTexts[currentRow].color = orange;
                    break;
                case (int)PauseState.SETTINGS: //settings column
                    optionsTexts[currentRow].color = Color.white;
                    optionValuesTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    optionsTexts[currentRow].color = orange;
                    optionValuesTexts[currentRow].color = orange;
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            switch (currentColumn)
            {
                case (int)PauseState.SETTINGS: //settings
                    switch (currentRow)
                    {
                        case 0:
                            sfxVolume = Mathf.Max(sfxVolume - 5, 0);
                            break;
                        case 1:
                            musicVolume = Mathf.Max(musicVolume - 5, 0);
                            break;
                        case 2:
                            difficulty = Mathf.Max(difficulty - 1, 0);
                            break;
                        case 3:
                            lockFPS = false;
                            break;
                    }
                    UpdateSettings();
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            switch (currentColumn)
            {
                case (int)PauseState.SETTINGS: //settings
                    switch (currentRow)
                    {
                        case 0:
                            sfxVolume = Mathf.Min(sfxVolume + 5, 100);
                            break;
                        case 1:
                            musicVolume = Mathf.Min(musicVolume + 5, 100);
                            break;
                        case 2:
                            difficulty = Mathf.Min(difficulty + 1, difficultyNames.Length - 1);
                            break;
                        case 3:
                            lockFPS = true;
                            break;
                    }
                    UpdateSettings();
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            sfxSource.clip = navigationAcceptSound;
            sfxSource.loop = false;
            sfxSource.Play();
            switch (currentColumn)
            {
                case (int)PauseState.MAIN: //currently in main column
                    switch (currentRow)
                    {
                        case 0: //resumed
                            Unpause();
                            break;
                        case 1: //entered settings
                            currentColumn = (int)PauseState.SETTINGS;
                            currentPage = 0;
                            chosenMain = currentRow;
                            mainColumnTexts[chosenMain].color = Color.red;
                            optionsTexts[currentRow = 0].color = orange;
                            optionValuesTexts[currentRow].color = orange;
                            maxCurrentRow = optionsTexts.Length;
                            optionsColumn.SetActive(true);
                            break;
                        case 2: //skipped
                            SceneManager.LoadScene("start");
                            break;
                    }
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            switch (currentColumn)
            {
                case (int)PauseState.SETTINGS:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            switch (currentColumn)
            {
                
            }
        }
    }

    void UpdateSettings()
    {
        optionValuesTexts[0].text = sfxVolume.ToString();
        optionValuesTexts[1].text = musicVolume.ToString();
        optionValuesTexts[2].text = difficultyNames[difficulty];
        optionValuesTexts[3].text = lockFpsNames[lockFPS ? 1 : 0];
    }

    void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetInt("musicVolume");
        if (musicVolume == 0)
        {
            mixer.SetFloat("musicVolume", Mathf.Log10(0.01f));
        }
        else
        {
            mixer.SetFloat("musicVolume", Mathf.Log10((float)musicVolume / 100) * 20);
        }
        sfxVolume = PlayerPrefs.GetInt("sfxVolume");
        if (sfxVolume == 0)
        {
            mixer.SetFloat("sfxVolume", Mathf.Log10(0.01f));
        }
        else
        {
            mixer.SetFloat("sfxVolume", Mathf.Log10((float)sfxVolume / 100) * 20);
        }
        difficulty = PlayerPrefs.GetInt("difficulty");
        lockFPS = Boolean.Parse(PlayerPrefs.GetString("lockFPS"));
        UpdateSettings();
    }
    void SaveSettings()
    {
        PlayerPrefs.SetInt("sfxVolume", sfxVolume);
        if (sfxVolume == 0)
        {
            mixer.SetFloat("sfxVolume", Mathf.Log10(0.0001f) * 20);
        }
        else
        {
            mixer.SetFloat("sfxVolume", Mathf.Log10((float)sfxVolume / 100) * 20);
        }
        PlayerPrefs.SetInt("musicVolume", musicVolume);
        if (musicVolume == 0)
        {
            mixer.SetFloat("musicVolume", Mathf.Log10(0.0001f) * 20);
        }
        else
        {
            mixer.SetFloat("musicVolume", Mathf.Log10((float)musicVolume / 100) * 20);
        }
        PlayerPrefs.SetInt("difficulty", difficulty);
        foreach (var enemy in BattleManager.instance.allEnemyCharacters)
        {
            enemy.UpdateDifficulty(difficulty);
        }
        foreach (var patrolNpc in StoryManager.instance.PatrolNPCs)
        {
            patrolNpc.GetComponent<PatrolNPCController>().UpdateDifficulty(difficulty);
        }
        PlayerPrefs.SetString("lockFPS", lockFPS.ToString());
        Debug.Log(lockFPS.ToString());
        QualitySettings.vSyncCount = lockFPS ? 0 : 1;
        PlayerPrefs.Save();
    }

    IEnumerator AllowToPause()
    {
        yield return new WaitForSeconds(1);
        canPause = true;
    }
}
