using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;


public enum PauseState { MAIN, SETTINGS, INVENTORY_MAIN, INVENTORY_HEALING, INVENTORY_WEARABLES, INVENTORY_ARTIFACTS,
    CHARACTER_INFO, CHARACTER_STATS, CHARACTER_EQ_CATEGORY, CHARACTER_EQ_CHANGE, CHARACTER_SKILL_TREE }
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] AudioClip[] freeRoamMusic;
    [SerializeField] AudioClip chaseMusic;

    public Canvas pauseCanvas, inGameCanvas, artifactCanvas, passwordCanvas, lockpickCanvas, fadeToBlackCanvas;
    [SerializeField] Image blackImage;
    public PlayerController player;
    public GameObject movingLockpick;
    public Slider skillCheckSlider;
    public GameObject skillCheckGreenArea;
    public Image[] passwordClues;
    public TMP_Text passwordText;
    [SerializeField] TMP_Text gameFpsText;
    public TMP_Text currentLocationText;
    public int currentLocationIndex;
    [SerializeField] GameObject optionsColumn;
    [SerializeField] GameObject inventoryMainColumn;
    [SerializeField] GameObject inventoryItemsColumn;
    [SerializeField] GameObject inventoryEqCategoryColumn;
    [SerializeField] GameObject inventoryEqChangeColumn;
    [SerializeField] GameObject characterInfoColumn;
    [SerializeField] GameObject characterStatsColumn;
    [SerializeField] GameObject characterSkillTreeColumn;
    public Image[] skillTreeIconBorders;
    public Image[] skillTreeIcons;
    public TMP_Text targetIsEscapingText;
    [SerializeField] Image characterSprite;
    [SerializeField] TMP_Text characterNameText;

    [SerializeField] AudioMixer mixer;
    [NonSerialized] public AudioSource musicSource, sfxSource;
    public AudioMixerGroup musicMixerGroup, sfxMixerGroup;

    readonly string[] difficultyNames = {"£atwy", "Œredni", "Trudny", "Fatalny" };
    readonly string[] lockFpsNames = { "Tak", "Nie" };
    [NonSerialized] public bool canPause = true;
    [NonSerialized] public bool canSaveGame = true;
    int currentRow, maxCurrentRow, currentColumn, currentPage;
    int chosenMain, chosenInv, chosenChar, chosenCharOption, chosenEqCategory, chosenPage;
    int sfxVolume = 25, musicVolume = 25;
    bool lockFPS = false, showFPS = false;
    [NonSerialized] public int difficulty = 0;
    [NonSerialized] public int currentFreeroamMusicStage = 0;
    int[,] freeroamMusicIds = { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 1, 1 }, { 2, 2, 2 }, { 0, 1, 2 }, { 3, 3, 3 }, { 4, 4, 4 }, { 5, 5, 5 }, { 1, 2, 2 } };
    //beginning, start of recruitment, rand, working with Burzynski, rand, chase, outside, underground, return to school
    int frames;
    float framesTime = 0f, maxFramesTime = 0.5f;
    [SerializeField] TMP_Text[] mainColumnTexts;
    [SerializeField] TMP_Text[] optionsTexts;
    [SerializeField] TMP_Text[] optionValuesTexts;
    [SerializeField] TMP_Text[] inventoryMainTexts;
    [SerializeField] TMP_Text[] inventoryBrowseTexts;
    [SerializeField] TMP_Text inventoryBrowseTitle;
    [SerializeField] TMP_Text[] inventoryEqCategoryTexts;
    [SerializeField] TMP_Text inventoryEqCategoryDescriptionText;
    [SerializeField] TMP_Text[] inventoryEqChangeTexts;
    [SerializeField] TMP_Text[] characterInfoTexts;
    [SerializeField] TMP_Text[] characterStatsTexts;
    [SerializeField] TMP_Text[] characterSkillTreeTexts;
    [SerializeField] TMP_Text characterSkillTreeDescriptionText;
    [SerializeField] TMP_Text characterSkillTreeRequirementsText;
    [SerializeField] TMP_Text itemDescriptionText;
    [SerializeField] TMP_Text itemPageText;
    [SerializeField] TMP_Text eqDescriptionText;
    [SerializeField] Image itemImage;
    
    Color orange = new Color(0.976f, 0.612f, 0.007f);

    string dataDirPath;
    string dataFileName;

    [SerializeField] AudioClip navigationScrollSound;
    [SerializeField] AudioClip navigationCancelSound;
    [SerializeField] AudioClip navigationAcceptSound;
    [SerializeField] AudioClip actionForbiddenSound;

    public GameObject[] artifacts;
    [SerializeField] Sprite emptySprite;

    Vector3 defaultUpgradeRequirementsTextScale;
    [SerializeField] Sprite[] upgradeSprites;
    [SerializeField] Sprite[] characterInfoSprites;

    public bool lastPageOfWearablesUnlocked = false;
    private void Start()
    {
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadSettings();
        }
        SaveSettings();
        dataDirPath = Application.persistentDataPath;
        dataFileName = PlayerPrefs.GetString("lastSaveFile");
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        StoryManager.instance.DisableSideQuestNPCs();
        if (File.Exists(fullPath))
        {
            LoadGame();
        }
        else
        {
            List<string> gameInfoLines = new List<string>();
            gameInfoLines.Add("U¿yj WASD lub strza³ek, aby siê poruszaæ. Naciœnij Esc, aby otworzyæ menu pauzy.");
            DialogueManager.instance.StartGameInfo(gameInfoLines.ToArray());
        }
        HandleArtifacts();
        StoryManager.instance.HandleSuperToastIngredients();
    }
    private void Awake()
    {
        pauseCanvas.enabled = false;
        artifactCanvas.enabled = false;
        passwordCanvas.enabled = false;
        lockpickCanvas.enabled = false;
        fadeToBlackCanvas.enabled = false;
        inGameCanvas.enabled = true;
        Time.timeScale = 1;
        instance = this;
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicMixerGroup;
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        defaultUpgradeRequirementsTextScale = characterSkillTreeRequirementsText.transform.localScale;
        //Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseCanvas.enabled || inGameCanvas.enabled || passwordCanvas.enabled || lockpickCanvas.enabled || artifactCanvas.enabled)
        {
            if (pauseCanvas.enabled)
            {
                HandlePauseInput();
            }
            else
            {
                CountFPS();
            }
            if (!musicSource.isPlaying)
            {
                PlayFreeroamMusic();
            }
        }
        else
        {
            musicSource.Stop();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            showFPS = !showFPS;
        }
    }

    public void PlayFreeroamMusic()
    {
        int musicId = UnityEngine.Random.Range(0, 3); //select a random track from current stage
        musicSource.clip = freeRoamMusic[freeroamMusicIds[currentFreeroamMusicStage, musicId]];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayChaseMusic()
    {
        musicSource.clip = chaseMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void CountFPS()
    {
        if (framesTime < maxFramesTime)
        {
            framesTime += Time.deltaTime;
            frames++;
        }
        else
        {
            if (showFPS)
            //if (true)
            {
                gameFpsText.text = (float)frames / maxFramesTime + " fps";
                BattleManager.instance.battleFpsText.text = (float)frames / maxFramesTime + " fps";
            }
            else
            {
                gameFpsText.text = "";
                BattleManager.instance.battleFpsText.text = "";
            }
            framesTime -= maxFramesTime;
            frames = 0;
        }
    }

    public void Pause()
    {
        canPause = false;
        currentRow = 0;
        mainColumnTexts[currentRow].color = orange;
        maxCurrentRow = mainColumnTexts.Length;
        Time.timeScale = 0;
        pauseCanvas.enabled = true;
        if (canSaveGame)
        {
            SaveGame();
        }
    }

    void Unpause()
    {
        Time.timeScale = 1;
        pauseCanvas.enabled = false;
        StartCoroutine(AllowToPause());
        mainColumnTexts[currentRow].color = Color.white;
        if (canSaveGame)
        {
            SaveGame();
        }
    }


    void HandlePauseInput()
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
                case (int)PauseState.INVENTORY_MAIN:
                    inventoryMainColumn.SetActive(false);
                    currentColumn = (int)PauseState.MAIN;
                    inventoryMainTexts[currentRow].color = Color.white;
                    currentRow = chosenMain;
                    mainColumnTexts[currentRow].color = orange;
                    maxCurrentRow = mainColumnTexts.Length;
                    break;
                case (int)PauseState.INVENTORY_HEALING:
                    inventoryItemsColumn.SetActive(false);
                    currentColumn = (int)PauseState.INVENTORY_MAIN;
                    inventoryBrowseTexts[currentRow].color = Color.white;
                    currentRow = chosenInv;
                    inventoryMainTexts[currentRow].color = orange;
                    maxCurrentRow = 3;
                    break;
                case (int)PauseState.INVENTORY_WEARABLES:
                    inventoryItemsColumn.SetActive(false);
                    currentColumn = (int)PauseState.INVENTORY_MAIN;
                    inventoryBrowseTexts[currentRow].color = Color.white;
                    currentRow = chosenInv;
                    inventoryMainTexts[currentRow].color = orange;
                    maxCurrentRow = 3;
                    break;
                case (int)PauseState.INVENTORY_ARTIFACTS:
                    inventoryItemsColumn.SetActive(false);
                    currentColumn = (int)PauseState.INVENTORY_MAIN;
                    inventoryBrowseTexts[currentRow].color = Color.white;
                    currentRow = chosenInv;
                    inventoryMainTexts[currentRow].color = orange;
                    maxCurrentRow = 3;
                    break;
                case (int)PauseState.CHARACTER_INFO:
                    characterInfoColumn.SetActive(false);
                    currentColumn = (int)PauseState.MAIN;
                    characterInfoTexts[currentRow].color = Color.white;
                    currentRow = chosenMain;
                    mainColumnTexts[currentRow].color = orange;
                    maxCurrentRow = mainColumnTexts.Length;
                    if (canSaveGame)
                    {
                        SaveGame();
                    }
                    break;
                case (int)PauseState.CHARACTER_STATS:
                    characterStatsColumn.SetActive(false);
                    currentColumn = (int)PauseState.CHARACTER_INFO;
                    characterInfoTexts[currentRow].color = orange;
                    break;
                case (int)PauseState.CHARACTER_EQ_CATEGORY:
                    inventoryEqCategoryColumn.SetActive(false);
                    currentColumn = (int)PauseState.CHARACTER_INFO;
                    inventoryEqCategoryTexts[currentRow].color = Color.white;
                    currentRow = chosenCharOption;
                    characterInfoTexts[currentRow].color = orange;
                    maxCurrentRow = characterInfoTexts.Length;
                    break;
                case (int)PauseState.CHARACTER_EQ_CHANGE:
                    inventoryEqChangeColumn.SetActive(false);
                    currentColumn = (int)PauseState.CHARACTER_EQ_CATEGORY;
                    inventoryEqChangeTexts[currentRow].color = Color.white;
                    currentRow = chosenEqCategory;
                    inventoryEqCategoryTexts[currentRow].color = orange;
                    maxCurrentRow = inventoryEqCategoryTexts.Length;
                    if (BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[currentRow] != null)
                    {
                        inventoryEqCategoryDescriptionText.text = BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[currentRow].Name;
                    }
                    else
                    {
                        inventoryEqCategoryDescriptionText.text = "Brak";
                    }
                    break;
                case (int)PauseState.CHARACTER_SKILL_TREE:
                    characterSkillTreeColumn.SetActive(false);
                    currentColumn = (int)PauseState.CHARACTER_INFO;
                    characterSkillTreeTexts[currentRow].color = Color.white;
                    skillTreeIconBorders[currentPage].color = Color.white;
                    currentRow = chosenCharOption;
                    currentPage = chosenPage;
                    characterInfoTexts[currentRow].color = orange;
                    maxCurrentRow = characterInfoTexts.Length;
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
                case (int)PauseState.INVENTORY_MAIN: //main inventory column
                    inventoryMainTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    inventoryMainTexts[currentRow].color = orange;
                    break;
                case (int)PauseState.INVENTORY_HEALING: //items inventory column
                    inventoryBrowseTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    inventoryBrowseTexts[currentRow].color = orange;
                    itemDescriptionText.text = Inventory.instance.items[currentPage * 4 + currentRow].Description + ".\n\nMasz: " +
                        Inventory.instance.items[currentPage * 4 + currentRow].Amount;
                    itemImage.sprite = Inventory.instance.itemsImages[Inventory.instance.items[currentPage * 4 + currentRow].Id];
                    break;
                case (int)PauseState.INVENTORY_WEARABLES: //equipment column
                    inventoryBrowseTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    inventoryBrowseTexts[currentRow].color = orange;
                    itemDescriptionText.text = Inventory.instance.wearables[currentPage * 4 + currentRow].Description + ".\n\nMasz: " +
                        Inventory.instance.wearables[currentPage * 4 + currentRow].Amount;
                    itemImage.sprite = Inventory.instance.wearablesImages[Inventory.instance.wearables[currentPage * 4 + currentRow].Id];
                    break;
                case (int)PauseState.INVENTORY_ARTIFACTS:
                    inventoryBrowseTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    if (artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().wasSeen)
                    {
                        itemDescriptionText.text = artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().description;
                        itemImage.sprite = artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().artifact;
                    }
                    else
                    {
                        itemDescriptionText.text = "???";
                        itemImage.sprite = emptySprite;
                    }
                    inventoryBrowseTexts[currentRow].color = orange;
                    break;
                case (int)PauseState.CHARACTER_INFO:
                    characterInfoTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    characterInfoTexts[currentRow].color = orange;
                    break;
                case (int)PauseState.CHARACTER_EQ_CATEGORY:
                    inventoryEqCategoryTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    inventoryEqCategoryTexts[currentRow].color = orange;
                    if (BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[currentRow] != null)
                    {
                        inventoryEqCategoryDescriptionText.text = BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[currentRow].Name;
                    }
                    else
                    {
                        inventoryEqCategoryDescriptionText.text = "Brak";
                    }
                    break;
                case (int)PauseState.CHARACTER_EQ_CHANGE: //eq change column
                    inventoryEqChangeTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    inventoryEqChangeTexts[currentRow].color = orange;
                    if (currentRow > 0)
                    {
                        eqDescriptionText.text = Inventory.instance.wearables[(currentRow - 1) * 4 + chosenEqCategory].Description + ".\n\nMasz: " +
                        Inventory.instance.wearables[(currentRow - 1) * 4 + chosenEqCategory].Amount;
                    }
                    else
                    {
                        eqDescriptionText.text = "";
                    }
                    break;
                case (int)PauseState.CHARACTER_SKILL_TREE:
                    characterSkillTreeTexts[currentRow].color = Color.white;
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        currentRow = (currentRow + 1) % maxCurrentRow;
                    }
                    else
                    {
                        currentRow = (currentRow - 1 < 0) ? (maxCurrentRow - 1) : (currentRow - 1);
                    }
                    characterSkillTreeTexts[currentRow].color = orange;
                    PrintUpgradeDescription();
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
                    switch(currentRow)
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
                        case 1: //entered inventory
                            currentColumn = (int)PauseState.INVENTORY_MAIN;
                            currentPage = 0;
                            chosenMain = currentRow;
                            mainColumnTexts[chosenMain].color = Color.red;
                            inventoryMainTexts[currentRow = 0].color = orange;
                            maxCurrentRow = inventoryMainTexts.Length;
                            inventoryMainColumn.SetActive(true);
                            break;
                        case 2: //entered char info
                            currentColumn = (int)PauseState.CHARACTER_INFO;
                            chosenMain = currentRow;
                            mainColumnTexts[chosenMain].color = Color.red;
                            characterInfoTexts[currentRow = 0].color = orange;
                            currentPage = 0;
                            PrintCharInfo();
                            maxCurrentRow = characterInfoTexts.Length;
                            characterInfoColumn.SetActive(true);
                            break;
                        case 3: //entered settings
                            currentColumn = (int)PauseState.SETTINGS;
                            currentPage = 0;
                            chosenMain = currentRow;
                            mainColumnTexts[chosenMain].color = Color.red;
                            optionsTexts[currentRow = 0].color = orange;
                            optionValuesTexts[currentRow].color = orange;
                            maxCurrentRow = optionsTexts.Length;
                            optionsColumn.SetActive(true);
                            break;
                        case 4: //returned to main
                            SceneManager.LoadScene("start");
                            break;
                    }
                    break;
                case (int)PauseState.INVENTORY_MAIN: //currently in main inventory
                    switch (currentRow)
                    {
                        case 0: //healing items
                            currentColumn = (int)PauseState.INVENTORY_HEALING;
                            currentPage = 0;
                            PrintCurrentPageOfItems();
                            chosenInv = currentRow;
                            inventoryMainTexts[chosenInv].color = Color.red;
                            inventoryBrowseTexts[currentRow = 0].color = orange; itemPageText.text = "Strona: " + (currentPage + 1) + "/" + (ShopManager.instance.level + 1);
                            itemDescriptionText.text = Inventory.instance.items[currentPage * 4 + currentRow].Description + ".\n\nMasz: " +
                                Inventory.instance.items[currentPage * 4 + currentRow].Amount;
                            maxCurrentRow = inventoryBrowseTexts.Length;
                            itemImage.sprite = Inventory.instance.itemsImages[Inventory.instance.items[currentPage * 4 + currentRow].Id];
                            inventoryBrowseTitle.text = "PRZEDMIOTY LECZ¥CE";
                            inventoryItemsColumn.SetActive(true);
                            break;
                        case 1: //wearables
                            currentColumn = (int)PauseState.INVENTORY_WEARABLES;
                            currentPage = 0;
                            PrintCurrentPageOfWearables();
                            chosenInv = currentRow;
                            inventoryMainTexts[chosenInv].color = Color.red;
                            inventoryBrowseTexts[currentRow = 0].color = orange;
                            int maxCurrentPage = lastPageOfWearablesUnlocked ? 4 : ShopManager.instance.level;
                            itemPageText.text = "Strona: " + (currentPage + 1) + "/" + (maxCurrentPage + 1);
                            itemDescriptionText.text = Inventory.instance.wearables[currentPage * 4 + currentRow].Description + ".\n\nMasz: " +
                                Inventory.instance.wearables[currentPage * 4 + currentRow].Amount;
                            maxCurrentRow = inventoryBrowseTexts.Length;
                            itemImage.sprite = Inventory.instance.wearablesImages[Inventory.instance.wearables[currentPage * 4 + currentRow].Id];
                            inventoryBrowseTitle.text = "WYPOSA¯ENIE";
                            inventoryItemsColumn.SetActive(true);
                            break;
                        case 2: //artifacts
                            currentColumn = (int)PauseState.INVENTORY_ARTIFACTS;
                            currentPage = 0;
                            PrintCurrentPageOfArtifacts();
                            itemPageText.text = "Strona: " + (currentPage + 1) + "/" + ((artifacts.Length - 1) / 4 + 1);
                            chosenInv = currentRow;
                            inventoryMainTexts[chosenInv].color = Color.red;
                            inventoryBrowseTexts[currentRow = 0].color = orange;
                            if (artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().wasSeen)
                            {
                                itemDescriptionText.text = artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().description;
                                itemImage.sprite = artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().artifact;
                            }
                            else
                            {
                                itemDescriptionText.text = "???";
                                itemImage.sprite = emptySprite;
                            }
                            maxCurrentRow = inventoryBrowseTexts.Length;
                            inventoryBrowseTitle.text = "ARTEFAKTY";
                            inventoryItemsColumn.SetActive(true);
                            break;
                        /*default: //character's equipment
                            currentColumn = (int)PauseState.INVENTORY_WEARABLES;
                            chosenInv = currentRow;
                            inventoryMainTexts[chosenInv].color = Color.red;
                            inventoryEquipmentTexts[currentRow = 0].color = orange;
                            maxCurrentRow = inventoryEquipmentTexts.Length;
                            inventoryEquipmentColumn.SetActive(true);
                            break;*/
                    }
                    break;
                case (int)PauseState.CHARACTER_INFO:
                    switch(currentRow)
                    {
                        case 0: //character stats
                            currentColumn = (int)PauseState.CHARACTER_STATS;
                            characterInfoTexts[currentRow].color = Color.red;
                            characterStatsColumn.SetActive(true);
                            PrintCharStats(BattleManager.instance.currentPartyCharacters[currentPage]);
                            break;
                        case 1: //character eq category
                            if (BattleManager.instance.playableCharacters[BattleManager.instance.currentPartyCharacters[chosenChar]].CanBeUpgraded)
                            {
                                currentColumn = (int)PauseState.CHARACTER_EQ_CATEGORY;
                                chosenCharOption = currentRow;
                                chosenChar = BattleManager.instance.currentPartyCharacters[currentPage];
                                characterInfoTexts[chosenCharOption].color = Color.red;
                                inventoryEqCategoryTexts[currentRow = 0].color = orange;
                                maxCurrentRow = inventoryEqCategoryTexts.Length;
                                inventoryEqCategoryColumn.SetActive(true);
                                eqDescriptionText.text = "";
                                if (BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[currentRow] != null)
                                {
                                    inventoryEqCategoryDescriptionText.text = BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[currentRow].Name;
                                }
                                else
                                {
                                    inventoryEqCategoryDescriptionText.text = "Brak";
                                }
                            }
                            else
                            {
                                sfxSource.clip = actionForbiddenSound;
                                sfxSource.loop = false;
                                sfxSource.Play();
                            }
                            break;
                        case 2: //character skill tree
                            if (BattleManager.instance.playableCharacters[BattleManager.instance.currentPartyCharacters[chosenChar]].CanBeUpgraded)
                            {
                                currentColumn = (int)PauseState.CHARACTER_SKILL_TREE;
                                chosenCharOption = currentRow;
                                chosenChar = BattleManager.instance.currentPartyCharacters[currentPage];
                                chosenPage = currentPage;
                                characterInfoTexts[chosenCharOption].color = Color.red;
                                characterSkillTreeTexts[currentRow = 0].color = orange;
                                skillTreeIconBorders[currentPage = 0].color = orange;
                                maxCurrentRow = BattleManager.instance.playableCharacters[chosenChar].UpgradeLevel + 1;
                                maxCurrentRow = Math.Min(maxCurrentRow, BattleManager.instance.playableCharacters[chosenChar].MaxUpgradeLevel);
                                characterSkillTreeColumn.SetActive(true);
                                PrintUpgrades();
                                PrintUpgradeDescription();
                                /*characterSkillTreeDescriptionText.text = BattleManager.instance.playableCharacters[chosenChar].upgradeDescription;
                                characterSkillTreeRequirementsText.text =
                                    "Wymagane:\nPoziom: " + BattleManager.instance.playableCharacters[chosenChar].Level + " / " + BattleManager.instance.playableCharacters[chosenChar].levelsToUpgrades[0]
                                    + "\n Tokeny: " + BattleManager.instance.playableCharacters[chosenChar].UpgradeTokens + " / " + BattleManager.instance.playableCharacters[chosenChar].tokensToUpgrades[0];*/
                                skillTreeIcons[0].sprite = upgradeSprites[upgradeSprites.Length - 1];
                                for (int i = 1; i < skillTreeIcons.Length; i++)
                                {
                                    skillTreeIcons[i].sprite = upgradeSprites[chosenChar * 5 + i - 1];
                                }
                            }
                            else
                            {
                                sfxSource.clip = actionForbiddenSound;
                                sfxSource.loop = false;
                                sfxSource.Play();
                            }
                            break;
                    }
                    break;
                case (int)PauseState.CHARACTER_EQ_CATEGORY:
                    currentColumn = (int)PauseState.CHARACTER_EQ_CHANGE;
                    chosenEqCategory = currentRow;
                    inventoryEqCategoryTexts[chosenEqCategory].color = Color.red;
                    inventoryEqChangeTexts[currentRow = 0].color = orange;
                    maxCurrentRow = inventoryEqChangeTexts.Length - 1;
                    if (lastPageOfWearablesUnlocked) maxCurrentRow = inventoryEqChangeTexts.Length;
                    inventoryEqChangeColumn.SetActive(true);
                    eqDescriptionText.text = "";
                    PrintAvailableEquipment();
                    break;
                case (int)PauseState.CHARACTER_EQ_CHANGE:
                    switch (currentRow)
                    {
                        case 0: //taking off equipment
                            if (BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[chosenEqCategory] != null)
                            {
                                Debug.Log("zdejmuje");
                                BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[chosenEqCategory].TakeOff(BattleManager.instance.playableCharacters[chosenChar]);
                            }
                            break;
                        default: //replacing current equipment
                            if (Inventory.instance.wearables[(currentRow - 1) * 4 + chosenEqCategory].Amount > 0)
                            {
                                if (BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[chosenEqCategory] != null)
                                { //is currently wearing something
                                    BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[chosenEqCategory].TakeOff(BattleManager.instance.playableCharacters[chosenChar]);
                                }
                                Inventory.instance.wearables[(currentRow - 1) * 4 + chosenEqCategory].PutOn(BattleManager.instance.playableCharacters[chosenChar]);
                            }
                            break;
                    }
                    inventoryEqChangeColumn.SetActive(false);
                    currentColumn = (int)PauseState.CHARACTER_EQ_CATEGORY;
                    inventoryEqChangeTexts[currentRow].color = Color.white;
                    currentRow = chosenEqCategory;
                    inventoryEqCategoryTexts[currentRow].color = orange;
                    maxCurrentRow = inventoryEqCategoryTexts.Length;
                    if (BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[currentRow] != null)
                    {
                        inventoryEqCategoryDescriptionText.text = BattleManager.instance.playableCharacters[chosenChar].wearablesWorn[currentRow].Name;
                    }
                    else
                    {
                        inventoryEqCategoryDescriptionText.text = "Brak";
                    }
                    break;
                case (int)PauseState.CHARACTER_SKILL_TREE:
                    if (currentPage == 0)
                    { //upgrade character's stats
                        if (currentRow == maxCurrentRow - 1)
                        {
                            if (BattleManager.instance.playableCharacters[chosenChar].Level >= BattleManager.instance.playableCharacters[chosenChar].levelsToUpgrades[currentRow] &&
                            BattleManager.instance.playableCharacters[chosenChar].UpgradeTokens >= BattleManager.instance.playableCharacters[chosenChar].tokensToUpgrades[currentRow])
                            {
                                BattleManager.instance.playableCharacters[chosenChar].Upgrade();
                                BattleManager.instance.playableCharacters[chosenChar].UpgradeTokens -= BattleManager.instance.playableCharacters[chosenChar].tokensToUpgrades[currentRow];
                                PrintUpgrades();
                                PrintUpgradeDescription();
                                if (BattleManager.instance.playableCharacters[chosenChar].UpgradeLevel < BattleManager.instance.playableCharacters[chosenChar].MaxUpgradeLevel)
                                {
                                    maxCurrentRow++;
                                }
                            }
                            else
                            {
                                StartCoroutine(CantAffordToUpgrade());
                            }
                        }
                        
                        
                    }
                    else
                    { //upgrade character's skill
                        if (currentRow == maxCurrentRow - 1)
                        {
                            if (BattleManager.instance.playableCharacters[chosenChar].Level >= BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].levelsToUpgrades[currentRow] &&
                            BattleManager.instance.playableCharacters[chosenChar].UpgradeTokens >= BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].tokensToUpgrades[currentRow])
                            {
                                BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].upgrade();
                                BattleManager.instance.playableCharacters[chosenChar].UpgradeTokens -= BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].tokensToUpgrades[currentRow];
                                PrintUpgrades();
                                PrintUpgradeDescription();
                                if (BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].Level < BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].MaxLevel)
                                {
                                    maxCurrentRow++;
                                }
                            }
                            else
                            {
                                StartCoroutine(CantAffordToUpgrade());
                            }
                        }
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
                case (int)PauseState.INVENTORY_HEALING:
                    currentPage = (currentPage - 1 < 0) ? currentPage : currentPage - 1;
                    itemPageText.text = "Strona: " + (currentPage + 1) + "/" + (ShopManager.instance.level + 1);
                    itemDescriptionText.text = Inventory.instance.items[currentPage * 4 + currentRow].Description + ".\n\nMasz: " +
                        Inventory.instance.items[currentPage * 4 + currentRow].Amount;
                    itemImage.sprite = Inventory.instance.itemsImages[Inventory.instance.items[currentPage * 4 + currentRow].Id];
                    PrintCurrentPageOfItems();
                    break;
                case (int)PauseState.INVENTORY_WEARABLES:
                    currentPage = (currentPage - 1 < 0) ? currentPage : currentPage - 1;
                    itemPageText.text = "Strona: " + (currentPage + 1) + "/" + (ShopManager.instance.level + 1);
                    itemDescriptionText.text = Inventory.instance.wearables[currentPage * 4 + currentRow].Description + ".\n\nMasz: " +
                        Inventory.instance.wearables[currentPage * 4 + currentRow].Amount;
                    itemImage.sprite = Inventory.instance.wearablesImages[Inventory.instance.wearables[currentPage * 4 + currentRow].Id];
                    PrintCurrentPageOfWearables();
                    break;
                case (int)PauseState.INVENTORY_ARTIFACTS:
                    currentPage = (currentPage - 1 < 0) ? currentPage : currentPage - 1;
                    itemPageText.text = "Strona: " + (currentPage + 1) + "/" + ((artifacts.Length - 1) / 4 + 1);
                    if (artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().wasSeen)
                    {
                        itemDescriptionText.text = artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().description;
                        itemImage.sprite = GameManager.instance.artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().artifact;
                    }
                    else
                    {
                        itemDescriptionText.text = "???";
                        itemImage.sprite = emptySprite;
                    }
                    PrintCurrentPageOfArtifacts();
                    break;
                case (int)PauseState.CHARACTER_INFO:
                    currentPage = (currentPage - 1 < 0) ? currentPage : currentPage - 1;
                    PrintCharInfo();
                    break;
                case (int)PauseState.CHARACTER_SKILL_TREE:
                    skillTreeIconBorders[currentPage].color = Color.white;
                    currentPage = (currentPage - 1 < 0) ? currentPage : currentPage - 1;
                    skillTreeIconBorders[currentPage].color = orange;
                    if (currentPage == 0)
                    {
                        maxCurrentRow = BattleManager.instance.playableCharacters[chosenChar].UpgradeLevel + 1;
                        maxCurrentRow = Math.Min(maxCurrentRow, BattleManager.instance.playableCharacters[chosenChar].MaxUpgradeLevel);
                    }
                    else
                    {
                        maxCurrentRow = BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].Level + 1;
                        maxCurrentRow = Math.Min(maxCurrentRow, BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].MaxLevel);
                    }
                    characterSkillTreeTexts[currentRow].color = Color.white;
                    currentRow = 0;
                    characterSkillTreeTexts[currentRow].color = orange;
                    PrintUpgrades();
                    PrintUpgradeDescription();
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
                case (int)PauseState.INVENTORY_HEALING:
                    currentPage = (currentPage + 1 > ShopManager.instance.level) ? currentPage : currentPage + 1;
                    itemPageText.text = "Strona: " + (currentPage + 1) + "/" + (ShopManager.instance.level + 1);
                    itemDescriptionText.text = Inventory.instance.items[currentPage * 4 + currentRow].Description + ".\n\nMasz: " +
                        Inventory.instance.items[currentPage * 4 + currentRow].Amount;
                    itemImage.sprite = Inventory.instance.itemsImages[Inventory.instance.items[currentPage * 4 + currentRow].Id];
                    PrintCurrentPageOfItems();
                    break;
                case (int)PauseState.INVENTORY_WEARABLES:
                    int maxCurrentPage = lastPageOfWearablesUnlocked ? 4 : ShopManager.instance.level;
                    currentPage = (currentPage + 1 > maxCurrentPage) ? currentPage : currentPage + 1;
                    itemPageText.text = "Strona: " + (currentPage + 1) + "/" + (maxCurrentPage + 1);
                    itemDescriptionText.text = Inventory.instance.wearables[currentPage * 4 + currentRow].Description + ".\n\nMasz: " +
                        Inventory.instance.wearables[currentPage * 4 + currentRow].Amount;
                    itemImage.sprite = Inventory.instance.wearablesImages[Inventory.instance.wearables[currentPage * 4 + currentRow].Id];
                    PrintCurrentPageOfWearables();
                    break;
                case (int)PauseState.INVENTORY_ARTIFACTS:
                    currentPage = (currentPage + 1 > ((artifacts.Length - 1) / 4)) ? currentPage : currentPage + 1;
                    itemPageText.text = "Strona: " + (currentPage + 1) + "/" + ((artifacts.Length - 1) / 4 + 1);
                    if (artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().wasSeen)
                    {
                        itemDescriptionText.text = artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().description;
                        itemImage.sprite = GameManager.instance.artifacts[currentPage * 4 + currentRow].GetComponent<ArtifactController>().artifact;
                    }
                    else
                    {
                        itemDescriptionText.text = "???";
                        itemImage.sprite = emptySprite;
                    }
                    PrintCurrentPageOfArtifacts();
                    break;
                case (int)PauseState.CHARACTER_INFO:
                    currentPage = (currentPage + 1 >= BattleManager.instance.currentPartyCharacters.Count) ? currentPage : currentPage + 1;
                    PrintCharInfo();
                    break;
                case (int)PauseState.CHARACTER_SKILL_TREE:
                    skillTreeIconBorders[currentPage].color = Color.white;
                    currentPage = (currentPage + 1 >= 6) ? currentPage : currentPage + 1;
                    skillTreeIconBorders[currentPage].color = orange;
                    if (currentPage == 0)
                    {
                        maxCurrentRow = BattleManager.instance.playableCharacters[chosenChar].UpgradeLevel + 1;
                        maxCurrentRow = Math.Min(maxCurrentRow, BattleManager.instance.playableCharacters[chosenChar].MaxUpgradeLevel);
                    }
                    else
                    {
                        maxCurrentRow = BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].Level + 1;
                        maxCurrentRow = Math.Min(maxCurrentRow, BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].MaxLevel);
                    }
                    characterSkillTreeTexts[currentRow].color = Color.white;
                    currentRow = 0;
                    characterSkillTreeTexts[currentRow].color = orange;
                    PrintUpgrades();
                    PrintUpgradeDescription();
                    break;
            }
        }
    }

    void HandleArtifacts()
    {
        foreach (var artifact in artifacts)
        {
            artifact.SetActive(!artifact.GetComponent<ArtifactController>().wasSeen);
        }
    }

    void PrintCharStats(int index)
    {
        var currentChar = BattleManager.instance.playableCharacters[index];
        float accuracyModifier = currentChar.GetAccuracyFromWearables();
        float healingModifier = currentChar.GetHealingFromWearables();
        characterStatsTexts[0].text = "Poziom: " + currentChar.Level;
        characterStatsTexts[1].text = "XP do nast.: " + (currentChar.XPToNextLevel - currentChar.CurrentXP);
        characterStatsTexts[2].text = "HP: " + currentChar.MaxHealth;
        characterStatsTexts[3].text = "SP: " + currentChar.MaxSkill;
        characterStatsTexts[4].text = "Atak: " + currentChar.DefaultAttack;
        characterStatsTexts[5].text = "Obrona: " + currentChar.DefaultDefense;
        characterStatsTexts[6].text = "Szybkoœæ: " + currentChar.Speed;
        characterStatsTexts[7].text = "Ruchy w turze: " + currentChar.DefaultTurns;
        characterStatsTexts[8].text = "Lecz. otrz.: " + healingModifier * 100 + "%";
        characterStatsTexts[9].text = "Celnoœæ: " + currentChar.DefaultAccuracy * accuracyModifier * 100 + "%";
    }

    void PrintUpgrades()
    {
        foreach (var t in characterSkillTreeTexts)
        {
            t.text = "";
            var c = t.color;
            c.a = 1;
            t.color = c;
        }
        if (currentPage == 0)
        {
            for (int i = 0; i < BattleManager.instance.playableCharacters[chosenChar].MaxUpgradeLevel; i++)
            {
                characterSkillTreeTexts[i].text = BattleManager.instance.playableCharacters[chosenChar].upgradeName;
                if (i > BattleManager.instance.playableCharacters[chosenChar].UpgradeLevel)
                {
                    var transparent = characterSkillTreeTexts[i].color;
                    transparent.a = 0.1f;
                    characterSkillTreeTexts[i].color = transparent;
                }
            }
        }
        else
        {
            for (int i = 0; i < BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].MaxLevel; i++)
            {
                characterSkillTreeTexts[i].text = BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].upgradeNames[i];
                if (i > BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].Level)
                {
                    var transparent = characterSkillTreeTexts[i].color;
                    transparent.a = 0.1f;
                    characterSkillTreeTexts[i].color = transparent;
                }
            }
        }
    }

    void PrintUpgradeDescription()
    {
        if (currentPage == 0)
        {
            characterSkillTreeDescriptionText.text = BattleManager.instance.playableCharacters[chosenChar].upgradeDescription;
            if (currentRow < BattleManager.instance.playableCharacters[chosenChar].UpgradeLevel)
            {
                characterSkillTreeRequirementsText.text = "ULEPSZONO";
            }
            else
            {
                characterSkillTreeRequirementsText.text =
                "Wymagane:\nPoziom: " + BattleManager.instance.playableCharacters[chosenChar].Level + " / " + BattleManager.instance.playableCharacters[chosenChar].levelsToUpgrades[currentRow]
                + "\n Tokeny: " + BattleManager.instance.playableCharacters[chosenChar].UpgradeTokens + " / " + BattleManager.instance.playableCharacters[chosenChar].tokensToUpgrades[currentRow];
            }
            
        }
        else
        {
            characterSkillTreeDescriptionText.text = BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].upgradeDescriptions[currentRow];
            if (currentRow < BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].Level)
            {
                characterSkillTreeRequirementsText.text = "ULEPSZONO";
            }
            else
            {
                characterSkillTreeRequirementsText.text =
                    "Wymagane:\nPoziom: " + BattleManager.instance.playableCharacters[chosenChar].Level + " / " + BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].levelsToUpgrades[currentRow]
                    + "\n Tokeny: " + BattleManager.instance.playableCharacters[chosenChar].UpgradeTokens + " / " + BattleManager.instance.playableCharacters[chosenChar].skillSet[currentPage].tokensToUpgrades[currentRow];
            }
        }
    }

    void PrintCharInfo()
    {
        var currentChar = BattleManager.instance.playableCharacters[BattleManager.instance.currentPartyCharacters[currentPage]];
        characterNameText.text = currentChar.NominativeName;
        characterSprite.sprite = characterInfoSprites[BattleManager.instance.playableCharacters[BattleManager.instance.currentPartyCharacters[currentPage]].SpriteIndex];
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
    void UpdateSettings()
    {
        optionValuesTexts[0].text = sfxVolume.ToString();
        optionValuesTexts[1].text = musicVolume.ToString();
        optionValuesTexts[2].text = difficultyNames[difficulty];
        optionValuesTexts[3].text = lockFpsNames[lockFPS ? 1 : 0];
    }

    void PrintCurrentPageOfItems()
    {
        for (int i = 0; i < inventoryBrowseTexts.Length; i++)
        {
            inventoryBrowseTexts[i].text = Inventory.instance.items[currentPage * 4 + i].Name;
        }
    }

    void PrintCurrentPageOfWearables()
    {
        for (int i = 0; i < inventoryBrowseTexts.Length; i++)
        {
            inventoryBrowseTexts[i].text = Inventory.instance.wearables[currentPage * 4 + i].Name;
        }
    }

    void PrintCurrentPageOfArtifacts()
    {
        for (int i = 0; i < inventoryBrowseTexts.Length; i++)
        {
            if (artifacts[currentPage * 4 + i].GetComponent<ArtifactController>().wasSeen)
            {
                inventoryBrowseTexts[i].text = artifacts[currentPage * 4 + i].GetComponent<ArtifactController>().artifactName;
            }
            else
            {
                inventoryBrowseTexts[i].text = "???";
            }
        }
    }

    void PrintAvailableEquipment()
    {
        int maxToPrint = maxCurrentRow - 1;
        if (lastPageOfWearablesUnlocked) maxToPrint = maxCurrentRow;
        for (int i = 1; i <= maxToPrint; i++)
        {
            inventoryEqChangeTexts[i].text = Inventory.instance.wearables[(i-1) * 4 + chosenEqCategory].Name;
        }
    }

    IEnumerator AllowToPause()
    {
        yield return new WaitForSeconds(1);
        canPause = true;
    }

    IEnumerator CantAffordToUpgrade()
    {
        characterSkillTreeRequirementsText.color = Color.red;
        Vector3 newScale = characterSkillTreeRequirementsText.transform.localScale;
        newScale.x *= 1.2f;
        newScale.y *= 1.2f;
        characterSkillTreeRequirementsText.transform.localScale = newScale;

        sfxSource.clip = actionForbiddenSound;
        sfxSource.loop = false;
        sfxSource.Play();

        yield return new WaitForSecondsRealtime(0.2f);

        characterSkillTreeRequirementsText.color = Color.white;
        characterSkillTreeRequirementsText.transform.localScale = defaultUpgradeRequirementsTextScale;
    }

    public IEnumerator FadeToBlack(float blackoutTime)
    {
        player.PreventRandomEncounters();
        fadeToBlackCanvas.enabled = true;
        float transitionTime = 0.2f;

        float time = 0.0f;
        while (time < transitionTime)
        {
            blackImage.color = Color.Lerp(new Color(0,0,0,0), new Color(0, 0, 0, 1), time / transitionTime);
            time += Time.deltaTime;
            yield return null;
        }
        blackImage.color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(blackoutTime);

        time = 0.0f;
        while (time < transitionTime)
        {
            blackImage.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), time / transitionTime);
            time += Time.deltaTime;
            yield return null;
        }
        blackImage.color = new Color(0, 0, 0, 0);

        fadeToBlackCanvas.enabled = false;
        player.AllowRandomEncounters();
    }

    public void SaveGame()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(StoryManager.instance.currentMainQuest);
                writer.WriteLine(DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + DateTime.Today.Year);
                writer.WriteLine(StoryManager.instance.currentPingPongScamsQuest);
                writer.WriteLine(StoryManager.instance.currentFollowingRefereesQuest);
                writer.WriteLine(StoryManager.instance.currentDiversionAndSearchQuest);
                writer.WriteLine(StoryManager.instance.currentStrongStuffQuest);
                writer.WriteLine(StoryManager.instance.currentFaceTheCheaterQuest);
                writer.WriteLine(Inventory.instance.money);
                writer.WriteLine(ShopManager.instance.player.transform.position.x);
                writer.WriteLine(ShopManager.instance.player.transform.position.y);
                foreach (var character in BattleManager.instance.playableCharacters)
                {
                    writer.WriteLine(character.NominativeName);
                    writer.WriteLine(character.Level);
                    writer.WriteLine(character.CurrentXP);
                    writer.WriteLine(character.UpgradeLevel);
                    writer.WriteLine(character.UpgradeTokens);
                    foreach(var skill in character.skillSet)
                    {
                        writer.WriteLine(skill.Level);
                    }
                    foreach(var wearable in character.wearablesWorn)
                    {
                        if (wearable != null)
                        {
                            writer.WriteLine(wearable.Id);
                        }
                        else
                        {
                            writer.WriteLine(-1);
                        }
                    }
                }
                writer.WriteLine(ShopManager.instance.level);
                writer.WriteLine("items");
                foreach (var item in Inventory.instance.items)
                {
                    writer.WriteLine(item.Amount);
                }
                writer.WriteLine("wearables");
                foreach (var wearable in Inventory.instance.wearables)
                {
                    writer.WriteLine(wearable.Amount);
                }
                foreach (var artifact in artifacts)
                {
                    writer.WriteLine(artifact.GetComponent<ArtifactController>().wasSeen);
                }
                foreach (var ingredient in StoryManager.instance.SuperToastIngredients)
                {
                    writer.WriteLine(ingredient.GetComponent<ArtifactController>().wasSeen);
                }
                writer.WriteLine(StoryManager.instance.finishedSuperToast);
                writer.WriteLine(currentLocationText.text);
                writer.WriteLine(currentLocationIndex);
                writer.WriteLine(lastPageOfWearablesUnlocked);
                writer.WriteLine("end");
                writer.Close();
            }
        }
    }

    public void LoadGame()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        Debug.Log(fullPath);
        using (FileStream stream = new FileStream(fullPath, FileMode.Open))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                int currentMainQuest = int.Parse(reader.ReadLine());
                StoryManager.instance.currentMainQuest = 0;
                for (int i = 0; i < currentMainQuest; i++)
                {
                    StoryManager.instance.ProgressStory(false);
                }

                string date = reader.ReadLine();

                int currentPingPongScamsQuest = int.Parse(reader.ReadLine());
                StoryManager.instance.currentPingPongScamsQuest = 0;
                for (int i = 0; i < currentPingPongScamsQuest; i++)
                {
                    StoryManager.instance.ProgressSideQuest(0, false);
                }
                if (currentPingPongScamsQuest < StoryManager.instance.sideQuestLengths[0] && currentPingPongScamsQuest != 0)
                {
                    StoryManager.instance.canReturnToMainStory = false;
                }

                int currentFollowingRefereesQuest = int.Parse(reader.ReadLine());
                StoryManager.instance.currentFollowingRefereesQuest = 0;
                for (int i = 0; i < currentFollowingRefereesQuest; i++)
                {
                    StoryManager.instance.ProgressSideQuest(1, false);
                }
                if (currentFollowingRefereesQuest < StoryManager.instance.sideQuestLengths[1] && currentFollowingRefereesQuest != 0)
                {
                    StoryManager.instance.canReturnToMainStory = false;
                }

                int currentDiversionAndSearchQuest = int.Parse(reader.ReadLine());
                StoryManager.instance.currentDiversionAndSearchQuest = 0;
                for (int i = 0; i < currentDiversionAndSearchQuest; i++)
                {
                    StoryManager.instance.ProgressSideQuest(2, false);
                }
                if (currentDiversionAndSearchQuest < StoryManager.instance.sideQuestLengths[2] && currentDiversionAndSearchQuest != 0)
                {
                    StoryManager.instance.canReturnToMainStory = false;
                }

                int currentStrongStuffQuest = int.Parse(reader.ReadLine());
                StoryManager.instance.currentStrongStuffQuest = 0;
                for (int i = 0; i < currentStrongStuffQuest; i++)
                {
                    StoryManager.instance.ProgressSideQuest(3, false);
                }
                if (currentStrongStuffQuest < StoryManager.instance.sideQuestLengths[3] && currentStrongStuffQuest != 0)
                {
                    StoryManager.instance.canReturnToMainStory = false;
                }

                int currentFaceTheCheaterQuest = int.Parse(reader.ReadLine());
                StoryManager.instance.currentFaceTheCheaterQuest = 0;
                for (int i = 0; i < currentFaceTheCheaterQuest; i++)
                {
                    StoryManager.instance.ProgressSideQuest(4, false);
                }
                StoryManager.instance.canReturnToMainStory = true;

                Inventory.instance.money = int.Parse(reader.ReadLine());

                float playerX = float.Parse(reader.ReadLine());
                float playerY = float.Parse(reader.ReadLine());
                ShopManager.instance.player.transform.position = new Vector2(playerX, playerY);
                if (StoryManager.instance.currentMainQuest == 73 || StoryManager.instance.currentMainQuest == 110)
                {
                    //just saw kidnapping cutscene, progress story once more
                    StoryManager.instance.ProgressStory(false);
                }

                foreach (var character in BattleManager.instance.playableCharacters)
                {
                    string characterName = reader.ReadLine();
                    int characterLevel = int.Parse(reader.ReadLine());
                    if (character.Level == 1)
                    {
                        for (int i = 1; i < characterLevel; i++)
                        {
                            character.LevelUp();
                        }
                    }
                    character.CurrentXP = int.Parse(reader.ReadLine());
                    int upgradeLevel = int.Parse(reader.ReadLine());
                    if (character.UpgradeLevel == 0)
                    {
                        for (int i = 0; i < upgradeLevel; i++)
                        {
                            character.Upgrade();
                        }
                    }
                    character.UpgradeTokens = int.Parse(reader.ReadLine());

                    foreach (var skill in character.skillSet)
                    {
                        int level = int.Parse(reader.ReadLine());
                        for (int i = 0; i <  level; i++)
                        {
                            skill.upgrade();
                        }
                    }

                    for (int i = 0; i < character.wearablesWorn.Length; i++)
                    {
                        int wearableId = int.Parse(reader.ReadLine());
                        if (wearableId != -1)
                        {
                            Inventory.instance.wearables[wearableId].PutOn(character);
                        }
                    }
                }

                int shopLevel = int.Parse(reader.ReadLine());
                ShopManager.instance.level = 0;
                for (int i = 0; i < shopLevel; i++)
                {
                    ShopManager.instance.PerformUpgrade();
                }
                reader.ReadLine();
                foreach (var item in Inventory.instance.items)
                {
                    item.Amount = int.Parse(reader.ReadLine());
                }
                reader.ReadLine();
                foreach (var wearable in Inventory.instance.wearables)
                {
                    wearable.Amount = int.Parse(reader.ReadLine());
                }
                foreach (var artifact in artifacts)
                {
                    artifact.GetComponent<ArtifactController>().wasSeen = bool.Parse(reader.ReadLine());
                }
                foreach (var ingredient in StoryManager.instance.SuperToastIngredients)
                {
                    ingredient.GetComponent<ArtifactController>().wasSeen = bool.Parse(reader.ReadLine());
                }
                StoryManager.instance.finishedSuperToast = bool.Parse(reader.ReadLine());
                currentLocationText.text = reader.ReadLine();
                currentLocationIndex = int.Parse(reader.ReadLine());
                lastPageOfWearablesUnlocked = bool.Parse(reader.ReadLine());
                reader.ReadLine();
                reader.Close();
            }
        }
    }
}
