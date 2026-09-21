using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject player;
    public Canvas shopCanvas;
    public TMP_Text[] categories;
    public TMP_Text[] items;
    public TMP_Text pageText;
    public TMP_Text moneyText;
    public TMP_Text upgradeText;
    public TMP_Text shopLevelText;
    public TMP_Text paymentText;
    public TMP_Text itemDescriptionText;
    public TMP_Text controlsText;
    public Canvas warningCanvas;
    public TMP_Text warningText;

    Vector3 upgradeTextDefaultScale;


    [SerializeField] AudioMixer mixer;
    AudioSource musicSource, sfxSource;
    public AudioMixerGroup musicMixerGroup, sfxMixerGroup;
    [SerializeField] AudioClip shopMusic;

    Color orange = new Color(0.976f, 0.612f, 0.007f);
    int currentColumn, currentRow, amountToBuy, maxAmountToBuy, currentPage;
    public int level;
    readonly int[] upgradeCosts = { 50, 100, 250 };
    const string defaultControlsText = "W/S - nawigacja, A/D - iloœæ, Z/X - kategoria, Q/E - strona\r\nENTER - zakup, ESC - wyjœcie",
                controlsTextAddOn = ", LSHIFT - ulepszenie";
    const int maxLevel = 3;
    readonly public int[] storyRequirementsForUpgrades = { 33, 49, 82};

    [SerializeField] AudioClip navigationScrollSound;
    [SerializeField] AudioClip navigationCancelSound;
    [SerializeField] AudioClip navigationAcceptSound;
    [SerializeField] AudioClip actionForbiddenSound;
    [SerializeField] AudioClip purchaseSound;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicMixerGroup;
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        shopCanvas.enabled = false;
        warningCanvas.enabled = false;
        upgradeTextDefaultScale = upgradeText.transform.localScale;
        level = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (shopCanvas.enabled)
        {
            if (!warningCanvas.enabled)
            {
                HandleInput();
            }
            else
            {
                HandleWarningInput();
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sfxSource.clip = navigationCancelSound;
            sfxSource.loop = false;
            sfxSource.Play();
            ExitShop();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            items[currentRow].color = Color.white;
            currentRow = (currentRow - 1 < 0) ? 3 : (currentRow - 1);
            items[currentRow].color = orange;
            amountToBuy = 0;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            items[currentRow].color = Color.white;
            currentRow = (currentRow + 1) % 4;
            items[currentRow].color = orange;
            amountToBuy = 0;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            amountToBuy = (amountToBuy - 1 < 0) ? 0 : (amountToBuy - 1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            amountToBuy = (amountToBuy + 1 > maxAmountToBuy) ? maxAmountToBuy : (amountToBuy + 1);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            sfxSource.clip = navigationAcceptSound;
            sfxSource.loop = false;
            sfxSource.Play();
            HandlePurchase();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sfxSource.clip = navigationAcceptSound;
            sfxSource.loop = false;
            sfxSource.Play();
            HandleUpgrade();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            amountToBuy = 0;
            categories[currentColumn].color = Color.white;
            currentColumn = 0;
            categories[currentColumn].color = Color.red;
            PrintItems();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            amountToBuy = 0;
            categories[currentColumn].color = Color.white;
            currentColumn = 1;
            categories[currentColumn].color = Color.red;
            PrintEquipment();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            currentPage = (currentPage - 1 < 0) ? currentPage : currentPage - 1;
            pageText.text = "Strona: " + (currentPage + 1) + "/" + (ShopManager.instance.level + 1);
            if (currentColumn == 0)
            {
                PrintItems();
            }
            else
            {
                PrintEquipment();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            sfxSource.clip = navigationScrollSound;
            sfxSource.loop = false;
            sfxSource.Play();
            currentPage = (currentPage + 1 > ShopManager.instance.level) ? currentPage : currentPage + 1;
            pageText.text = "Strona: " + (currentPage + 1) + "/" + (ShopManager.instance.level + 1);
            if (currentColumn == 0)
            {
                PrintItems();
            }
            else
            {
                PrintEquipment();
            }
        }
        if (Input.anyKey)
        {
            if (currentColumn == 0)
            {
                maxAmountToBuy = Inventory.instance.maxOwnedItems - Inventory.instance.items[currentPage * 4 + currentRow].Amount;
                paymentText.text = amountToBuy + " x " + Inventory.instance.items[currentPage * 4 + currentRow].Cost + "PLN = " +
                    amountToBuy * Inventory.instance.items[currentPage * 4 + currentRow].Cost + "PLN";
                itemDescriptionText.text = Inventory.instance.items[currentPage * 4 + currentRow].Description + ". Masz: " + Inventory.instance.items[currentPage * 4 + currentRow].Amount;
            }
            else
            {
                maxAmountToBuy = Inventory.instance.maxOwnedItems - Inventory.instance.wearables[currentPage * 4 + currentRow].Amount;
                paymentText.text = amountToBuy + " x " + Inventory.instance.wearables[currentPage * 4 + currentRow].Cost + "PLN = " +
                    amountToBuy * Inventory.instance.wearables[currentPage * 4 + currentRow].Cost + "PLN";
                itemDescriptionText.text = Inventory.instance.wearables[currentPage * 4 + currentRow].Description + ". Masz: " + Inventory.instance.wearables[currentPage * 4 + currentRow].Amount;
            }
        }
    }

    void EnableWarningCanvas()
    {
        warningCanvas.enabled = true;
        warningText.text = "Tej operacji nie mo¿na cofn¹æ, a sporo kosztuje.\r\nCzy chcesz kontynuowaæ?";
    }

    void HandleWarningInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            warningCanvas.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            warningCanvas.enabled = false;
            Inventory.instance.money -= upgradeCosts[level];
            PerformUpgrade();
        }
    }

    void PrintItems()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].text = Inventory.instance.items[currentPage * 4 + i].Name + "\r\n(koszt: " + Inventory.instance.items[currentPage * 4 + i].Cost + "PLN)";
        }
    }

    void PrintEquipment()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].text = Inventory.instance.wearables[currentPage * 4 + i].Name + "\r\n(koszt: " + Inventory.instance.wearables[currentPage * 4 + i].Cost + "PLN)";
        }
    }

    void HandlePurchase()
    {
        if (currentColumn == 0)
        {
            if (Inventory.instance.money >= amountToBuy * Inventory.instance.items[currentPage * 4 + currentRow].Cost)
            {
                Inventory.instance.money -= amountToBuy * Inventory.instance.items[currentPage * 4 + currentRow].Cost;
                Inventory.instance.items[currentPage * 4 + currentRow].Add(amountToBuy);

                sfxSource.clip = purchaseSound;
                sfxSource.loop = false;
                sfxSource.Play();
            }
            else
            {
                StartCoroutine(CantAfford());
            }
        }
        else
        {
            if (Inventory.instance.money >= amountToBuy * Inventory.instance.wearables[currentPage * 4 + currentRow].Cost)
            {
                Inventory.instance.money -= amountToBuy * Inventory.instance.wearables[currentPage * 4 + currentRow].Cost;
                Inventory.instance.wearables[currentPage * 4 + currentRow].Add(amountToBuy);

                sfxSource.clip = purchaseSound;
                sfxSource.loop = false;
                sfxSource.Play();
            }
            else
            {
                StartCoroutine(CantAfford());
            }
        }
        moneyText.text = "Kasa: " + Inventory.instance.money + " PLN";
    }

    void HandleUpgrade()
    {
        if (level < maxLevel && StoryManager.instance.currentMainQuest >= storyRequirementsForUpgrades[level])
        {
            if (Inventory.instance.money >= upgradeCosts[level])
            {
                EnableWarningCanvas();
            }
            else
            {
                StartCoroutine(CantAfford());
            }
        }
        else
        {
            StartCoroutine(NotEnoughStoryProgress());
            sfxSource.clip = actionForbiddenSound;
            sfxSource.loop = false;
            sfxSource.Play();
        }
    }

    public void PerformUpgrade()
    {
        level++;
        moneyText.text = "Kasa: " + Inventory.instance.money + " PLN";
        pageText.text = "Strona: " + (currentPage + 1) + "/" + (ShopManager.instance.level + 1);
        controlsText.text = defaultControlsText;
        if (level < maxLevel && StoryManager.instance.currentMainQuest >= storyRequirementsForUpgrades[level])
        {
            upgradeText.text = "Ulepszenie: " + upgradeCosts[level] + " PLN";
            controlsText.text += controlsTextAddOn;
        }
        else if (level < maxLevel && StoryManager.instance.currentMainQuest < storyRequirementsForUpgrades[level])
        {
            upgradeText.text = "Kontynuuj fabu³ê, aby móc ulepszyæ";
        }
        else
        {
            upgradeText.text = "Osi¹gniêto maksymalny poziom sklepu";
        }
        shopLevelText.text = "Poziom sklepu: " + (level + 1) + " / " + (maxLevel + 1);
        if (currentColumn == 0)
        {
            PrintItems();
        }
        else
        {
            PrintEquipment();
        }
    }

    public void SetUpShop()
    {
        GameManager.instance.inGameCanvas.enabled = false;
        ShopManager.instance.shopCanvas.enabled = true;
        musicSource.clip = shopMusic;
        musicSource.loop = true;
        musicSource.Play();
        player.SetActive(false);
        PrintItems();
        moneyText.text = "Kasa: " + Inventory.instance.money + " PLN";
        shopLevelText.text = "Poziom sklepu: " + (level + 1) + " / " + (maxLevel + 1);
        amountToBuy = 0;
        items[currentRow].color = Color.white;
        currentRow = 0;
        currentPage = 0;
        items[currentRow].color = orange;
        paymentText.text = "0 x " + Inventory.instance.items[currentRow].Cost + "PLN = 0 PLN";
        currentColumn = 0;
        categories[0].color = Color.red;
        categories[1].color = Color.white;
        controlsText.text = defaultControlsText;
        if (level < maxLevel && StoryManager.instance.currentMainQuest >= storyRequirementsForUpgrades[level])
        {
            upgradeText.text = "Ulepszenie: " + upgradeCosts[level] + " PLN";
            controlsText.text += controlsTextAddOn;
        }
        else if (level < maxLevel && StoryManager.instance.currentMainQuest < storyRequirementsForUpgrades[level])
        {
            upgradeText.text = "Kontynuuj fabu³ê, aby móc ulepszyæ";
        }
        else
        {
            upgradeText.text = "Osi¹gniêto maksymalny poziom sklepu";
        }
    }

    void ExitShop()
    {
        GameManager.instance.inGameCanvas.enabled = true;
        shopCanvas.enabled = false;
        player.SetActive(true);
        musicSource.Stop();
        shopCanvas.enabled = false;
        if (GameManager.instance.canSaveGame)
        {
            GameManager.instance.SaveGame();
        }
    }

    IEnumerator CantAfford()
    {
        moneyText.color = Color.red;
        moneyText.transform.localScale = new Vector3(1.25f, 1.25f);

        sfxSource.clip = actionForbiddenSound;
        sfxSource.loop = false;
        sfxSource.Play();

        yield return new WaitForSeconds(0.2f);

        moneyText.color = Color.white;
        moneyText.transform.localScale = Vector3.one;
    }

    IEnumerator NotEnoughStoryProgress()
    {
        upgradeText.color = Color.red;
        Vector3 newScale = upgradeTextDefaultScale;
        newScale.x *= 1.15f;
        newScale.y *= 1.15f;
        upgradeText.transform.localScale = newScale;

        sfxSource.clip = actionForbiddenSound;
        sfxSource.loop = false;
        sfxSource.Play();

        yield return new WaitForSeconds(0.2f);

        upgradeText.color = Color.white;
        upgradeText.transform.localScale = upgradeTextDefaultScale;
    }
}
