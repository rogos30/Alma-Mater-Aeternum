using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0, 10f)]public float moveSpeed = 3.5f;
    float timeToFight;
    Animator animator;
    [SerializeField] RuntimeAnimatorController[] controllers;
    bool isFacingRight = false, isMoving = false, isMovingDiagonally = false;
    public int currentRandomEncounterStage = 0;
    [SerializeField] bool allowRandomEncounters = false;
    [SerializeField] bool enableCheats = false;
    int[] randomEncounterBackgroundIndexes = { 5, 21, 5 };

    [SerializeField] GameObject interactionPrompt;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        ResetTimeToRandomEcounter();
        interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (GameManager.instance.inGameCanvas.enabled && !DialogueManager.instance.dialogueCanvas.enabled && !DialogueManager.instance.gameInfoCanvas.enabled
            && !GameManager.instance.artifactCanvas.enabled && !GameManager.instance.passwordCanvas.enabled && !GameManager.instance.fadeToBlackCanvas.enabled)
        {
            isMoving = false;
            HandleInput();
            if (isMoving && allowRandomEncounters)
            {
                timeToFight -= Time.deltaTime;
            }
            if (timeToFight <= 0)
            {
                ResetTimeToRandomEcounter();
                InitiateRandomEncounter();
            }
        }
        else
        {
            animator.SetInteger("isWalking", 0);
        }
    }

    void HandleInput()
    {
        bool pressedMovementKey = false;
        isMovingDiagonally = false;
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
        {
            isMovingDiagonally = true;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            pressedMovementKey = true;
            MoveHorizontal(true);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            pressedMovementKey = true;
            MoveHorizontal(false);
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            pressedMovementKey = true;
            MoveVertical(true);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            pressedMovementKey = true;
            MoveVertical(false);
        }
        if (enableCheats)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Inventory.instance.money += 1000;
                Debug.Log(Inventory.instance.money);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                for (int i = 0; i < BattleManager.instance.currentPartyCharacters.Count; i++)
                {
                    int index = BattleManager.instance.currentPartyCharacters[i];
                    BattleManager.instance.playableCharacters[index].HandleLevel(5000);
                }
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                StoryManager.instance.ProgressStory();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                for (int i = 0; i < 5; i++)
                {
                    StoryManager.instance.ProgressStory();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.instance.canPause)
            {
                GameManager.instance.Pause();
            }
        }
        if (!pressedMovementKey)
        {
            animator.SetInteger("isWalking", 0);
        }
    }

    public void ChangeAnimator(int animatorId)
    {
        animator.runtimeAnimatorController = controllers[animatorId];
    }

    public void AllowRandomEncounters()
    {
        allowRandomEncounters = true;
        ResetTimeToRandomEcounter();
    }

    public void PreventRandomEncounters()
    {
        allowRandomEncounters = false;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        interactionPrompt.transform.localScale = theScale;
    }

    void MoveHorizontal(bool right)
    {
        float speed = moveSpeed;
        if (isMovingDiagonally)
        {
            speed = moveSpeed / Mathf.Sqrt(2);
        }
        if (right)
        {
            if (!isFacingRight)
            {
                Flip();
            }
            transform.Translate(speed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }
        else
        {
            if (isFacingRight)
            {
                Flip();
            }
            transform.Translate(-speed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }
        animator.SetInteger("isWalking", 3);

        var pos = transform.position;
        pos.z = pos.y;
        transform.position = pos;
        isMoving = true;
    }

    void MoveVertical(bool up)
    {
        float speed = moveSpeed;
        if (isMovingDiagonally)
        {
            speed = moveSpeed / Mathf.Sqrt(2);
        }
        if (up)
        {
            transform.Translate(0.0f, speed * Time.deltaTime, 0.0f, Space.World);
            animator.SetInteger("isWalking", 1);
        }
        else
        {
            transform.Translate(0.0f, -speed * Time.deltaTime, 0.0f, Space.World);
            animator.SetInteger("isWalking", 2);
        }
        var pos = transform.position;
        pos.z = pos.y;
        transform.position = pos;
        isMoving = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            interactionPrompt.SetActive(true);
        }
        else if (other.CompareTag("Marlboro"))
        {
            StoryManager.instance.CollectMarlboro();
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("PonySticker"))
        {
            StoryManager.instance.CollectPonySticker();
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("VisionCone"))
        {
            //Debug.Log("AAAAAAAAA");
            other.GetComponentInParent<PatrolNPCController>().CheckIfPlayerInSight();
        }
        else if (other.CompareTag("Trigger"))
        {
            other.GetComponent<Interactable>().Interact();
        }
        else if (other.CompareTag("PursuitKillZone"))
        {
            other.GetComponentInParent<PatrolNPCController>().killZoneEngaged = true;
        }
        else if (other.CompareTag("PursuitNPC"))
        {
            other.GetComponentInParent<PatrolNPCController>().GameOver();
        }
        else if (other.CompareTag("Teleport"))
        {
            if (other.GetComponent<TeleportController>().isAutomatic)
            {
                other.GetComponent<TeleportController>().Interact();
            }
            else
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("LocationTrigger"))
        {
            GameManager.instance.currentLocationText.text = other.GetComponent<LocationChangeTriggerController>().newLocationName;
            GameManager.instance.currentLocationIndex = other.GetComponent<LocationChangeTriggerController>().newLocationIndex;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            interactionPrompt.SetActive(false);
        }
        else if (other.CompareTag("VisionCone"))
        {
            //Debug.Log("BBBBBBBBB");
            other.GetComponentInParent<PatrolNPCController>().PlayableLeaveSight();
        }
        else if (other.CompareTag("Teleport"))
        {
            interactionPrompt.SetActive(false);
        }
    }

    void ResetTimeToRandomEcounter()
    {
        timeToFight = Random.Range(55, 70);
        //timeToFight = Random.Range(5, 10);
    }

    void InitiateRandomEncounter()
    {
        int[] playables = new int[BattleManager.instance.currentPartyCharacters.Count];
        int[,] enemyConfigurations = { { 0, 0, 0, 1 }, { 0, 0, 1, 0 }, { 0, 0, 1, 1 }, { 0, 1, 0, 0 }, { 0, 1, 0, 1 }, { 0, 1, 1, 0 }, { 0, 1, 1, 1 },
                                        { 1, 0, 0, 0 }, { 1, 0, 0, 1 }, { 1, 0, 1, 0 }, { 1, 0, 1, 1 }, { 1, 1, 0, 0 }, { 1, 1, 0, 1 }, { 1, 1, 1, 0 }, { 1, 1, 1, 1 } };
        int enemySet = Random.Range(0, 15);
        for (int i = 0; i < playables.Length; i++)
        {
            playables[i] = i;
        }
        bool[] enemiesFirstDraw = new bool[4];
        int amountOfEnemies = 0;
        for (int i = 0; i < 4; i++)
        {
            enemiesFirstDraw[i] = enemyConfigurations[enemySet,i] == 1;
            if (enemiesFirstDraw[i])
            {
                amountOfEnemies++;
            }
        }
        int[] enemies = new int[amountOfEnemies];
        int j = 0;
        for (int i = 0; i < 4; i++)
        {
            if (enemiesFirstDraw[i])
            {
                enemies[j++] = BattleManager.instance.randomEncounterEnemyIndexes[currentRandomEncounterStage, i];
            }
        }
        if (GameManager.instance.currentLocationIndex == -1) return;
        BattleManager.instance.InitiateBattle(playables, enemies, GameManager.instance.currentLocationIndex, true, true);
    }
}
