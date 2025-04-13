// todo:
// special sound for special choice
// change maxStageTurns on stage change??
  // put VICTORY / DEFEAT in the stage name text and prevent sutff....?
// tint text?

// then configure the UI
// pick sprites

// then make a simple playtest to debug

// then figure out the hard coded stuff to balance the qame out
// introduce better choice generation..
// play test it...

// change stat prefab
// put in build
// change build /....

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    
    private BuildState     buildState;
    private MetaState      metaState;
    private ResourcesState resourcesState;

    private bool goNextTurn = false;
    private bool goGameOver = false;
    private bool goVictory  = false;
    
    private static int         maxBlock               = 9999;
    private static int         maxTurn                = 100;
    private static int         maxStageNumber         = 4;
    private static int         maxStageTurns          = 25;
    private static EnemyType[] stageMarshEnemies      = {EnemyType.Bat, EnemyType.Snail};
    private static EnemyType[] stageGraveyardEnemies  = {EnemyType.Zombie, EnemyType.Skeleton};
    private static EnemyType[] stageStrongholdEnemies = {EnemyType.Goblin, EnemyType.Orc};
    private static EnemyType[] stageNecropolisEnemies = {EnemyType.Ghost, EnemyType.Wraith};
    private StageType          currentStage           = StageType.Garden;
    private EnemyType[]        currentStageEnemies    = stageMarshEnemies;
    private Sprite             currentFrameSprite;
    
    private int           chosen;
    private ChoiceType    specialChosen            = ChoiceType.specialNone;
    private bool          toGenerateSpecialChoices = false;
    private ChoiceType[]  choices                  = new ChoiceType[maxChoiceNumber];
    private static int    maxChoiceNumber          = 3; // do I make this 2?
    private static int    nonSpecialChoiceNumber  = 6;
    
    private static int maxEnemiesInTurn     = 5;
    private Enemy[]    enemies              = new Enemy[maxEnemiesInTurn];
    private int        enemiesInCurrentTurn = 0;
    private int        enemyToAttack        = 0;
    private bool[]     willKillEnemy        = new bool[maxEnemiesInTurn];


    [SerializeField] private Image backgroundImage;
    
    // private Image enemyFrameImage = new Image();
    // private Image choiceFrameImage = new Image();
    // private Image statFrameImage = new Image();

    [SerializeField] private RectTransform buildPanel;
    [SerializeField] private RectTransform choicePanel;

    // set array sizes in inspector based on maxChoiceNumber
    [SerializeField] private Button[]   choiceButtons;
    [SerializeField] private Image[]   choiceSprites;
    [SerializeField] private TMP_Text[] choiceText;

    [SerializeField] private RectTransform enemyQueuePanel;
    [SerializeField] private GameObject enemyCardPrefab;
    
    [SerializeField] private Sprite batSprite;
    [SerializeField] private Sprite snailSprite;
    [SerializeField] private Sprite zombieSprite;
    [SerializeField] private Sprite skeletonSprite;
    [SerializeField] private Sprite goblinSprite;
    [SerializeField] private Sprite orcSprite;
    [SerializeField] private Sprite ghostSprite;
    [SerializeField] private Sprite wraithSprite;

    [SerializeField] private Sprite marshFrameSprite;
    [SerializeField] private Sprite graveyardFrameSprite;
    [SerializeField] private Sprite strongholdFrameSprite;
    [SerializeField] private Sprite necropolisFrameSprite;

    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text hpRegenText;
    [SerializeField] private TMP_Text blockText;
    [SerializeField] private TMP_Text attackPowerText;
    [SerializeField] private TMP_Text armorPenText;
    [SerializeField] private TMP_Text aoeText;

    [SerializeField] private Sprite boosthpSprite;
    [SerializeField] private Sprite boosthpRegenSprite;
    [SerializeField] private Sprite boostblockSprite;
    [SerializeField] private Sprite boostattackPowerSprite;
    [SerializeField] private Sprite boostarmorPenSprite;
    [SerializeField] private Sprite boostaoeSprite;

    [SerializeField] private Image specialChoiceImage;
    
    [SerializeField] private Sprite special0Sprite;
    [SerializeField] private Sprite special1Sprite;
    [SerializeField] private Sprite special2Sprite;

    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text dmgTakenText;
    [SerializeField] private TMP_Text stageText;

    // how do I set up the text?
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    
    [SerializeField] private Sprite marshBackgroundSprite;
    [SerializeField] private Sprite graveyardBackgroundSprite;
    [SerializeField] private Sprite strongholdBackgroundSprite;
    [SerializeField] private Sprite necropolisBackgroundSprite;


    [SerializeField] private AudioSource stageMusic = new AudioSource();
    
    [SerializeField] private AudioClip marshMusic;
    [SerializeField] private AudioClip graveyardMusic;
    [SerializeField] private AudioClip strongholdMusic;
    [SerializeField] private AudioClip necropolisMusic;

    [SerializeField] private AudioClip choiceSoundEffect;
    
    void Start()
    {
        buildState = new BuildState();
        resourcesState = new ResourcesState();
        metaState = new MetaState();
        
        InitGameState();

        for (int i = 0; i < maxChoiceNumber; i++)
        {
            int index = i;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => {
                chosen = index;
                goNextTurn = true;
                stageMusic.PlayOneShot(choiceSoundEffect);
            });
        }

        quitButton.onClick.AddListener(() => {
            Application.Quit();
            });

        restartButton.onClick.AddListener(() => {
            InitGameState();
            });
        
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            chosen = 0;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            chosen = 1;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            chosen = 2;
            goNextTurn = true;
        }
        
        if (goNextTurn && ((!goVictory) && (!goGameOver)))
        {
            // if (metaState.turn == maxTurn)
            // {
            //     goVictory = true;
            //     Logger.Log("Won game");
            //     stageText.text = "VICTORY";
            // }
            
            LogTurn();
            LogStage();
            // LogKillCount();
            // LogBuildState();
            // LogChoices();
            // LogChosen();

            ProcessChosen();
            
            // Logger.Log("UPDATING BUILD STATE");
            // LogBuildState();

            // Logger.Log("CURRENT ENEMIES");
            // Logger.Log("Total enemies in current turn: " + enemiesInCurrentTurn);
            // LogEnemyQueue();
            // Logger.Log("ATTACKING ENEMIES");

            ChooseTarget();
            WillKillEnemies();
            
            // LogEnemyQueue();
            
            // Logger.Log("UPDATING RESOURCES STATE");
            
            // LogResourcesState();
            
            UpdateResources();
            
            // LogResourcesState();
            
            // Logger.Log("GETTING ATTACKED BY ALIVE ENEMIES IN CURRENT TURN");
            // Logger.Log("HP before attack: " + resourcesState.hp);

            GetAttackedByEnemies();

            // Logger.Log("HP after attack: " + resourcesState.hp);
            
            // Logger.Log("CHECKING IF WON OR COMPLETED STAGE");

            UpdateStage();
            
            // Logger.Log("GENERATING NEW ENEMIES");

            GenerateEnemies();
            
            // LogEnemyQueue();
            
            // Logger.Log("GENERATING NEW CHOICES");

            GenerateChoices();
            
            // LogChoices();

            goNextTurn = false;
        }
    }

    void GenerateEnemies()
    {
        // delete prefabs here
        foreach (Transform child in enemyQueuePanel)
            Destroy(child.gameObject);
        
        enemiesInCurrentTurn = Random.Range(1,6);
        for (int i = 0; i < enemiesInCurrentTurn; i++)
        {
            Enemy newEnemy = new Enemy();
            int enemyType = Random.Range(0,2);
            newEnemy.type = currentStageEnemies[enemyType];
            newEnemy.isDead = false;
            Sprite newEnemySprite = batSprite;
            
            switch (newEnemy.type)
            {
                // @hardcoded
                case EnemyType.Bat:
                    {
                        newEnemy.hp = 10;
                        newEnemy.block = 0;
                        newEnemy.attack = 5;
                        newEnemySprite = batSprite;
                    } break;
                case EnemyType.Snail:
                    {
                        newEnemy.hp = 1;
                        newEnemy.block = 3;
                        newEnemy.attack = 7;
                        newEnemySprite = snailSprite;
                    } break;
                case EnemyType.Zombie:
                    {
                        newEnemy.hp = 15;
                        newEnemy.block = 0;
                        newEnemy.attack = 5;
                        newEnemySprite = zombieSprite;
                    } break;
                case EnemyType.Skeleton:
                    {
                        newEnemy.hp = 1;
                        newEnemy.block = 15;
                        newEnemy.attack = 15;
                        newEnemySprite = skeletonSprite;
                    } break;
                case EnemyType.Goblin:
                    {
                        newEnemy.hp = 5;
                        newEnemy.block = 0;
                        newEnemy.attack = 20;
                        newEnemySprite = goblinSprite;
                    } break;
                case EnemyType.Orc:
                    {
                        newEnemy.hp = 10;
                        newEnemy.block = 0;
                        newEnemy.attack = 20;
                        newEnemySprite = orcSprite;
                    } break;
                case EnemyType.Ghost:
                    {
                        newEnemy.hp = 1;
                        newEnemy.block = 30;
                        newEnemy.attack = 20;
                        newEnemySprite = ghostSprite;
                    } break;
                case EnemyType.Wraith:
                    {
                        newEnemy.hp = 15;
                        newEnemy.block = 15;
                        newEnemy.attack = 30;
                        newEnemySprite = wraithSprite;
                    } break;                    
                default:
                    {
                        Logger.LogError("Invalid default case for currentStage");
                    } break;
            }
            
            enemies[i] = newEnemy;
            
            // generate prefab here
            GameObject card = Instantiate(enemyCardPrefab, enemyQueuePanel);
            card.transform.Find("EnemySprite").GetComponent<Image>().sprite = newEnemySprite;
            card.transform.Find("EnemyCardFrameSprite").GetComponent<Image>().sprite = currentFrameSprite;
            card.transform.Find("EnemyStatText/HPtext").GetComponent<TMP_Text>().text = newEnemy.hp.ToString();
            card.transform.Find("EnemyStatText/BlockText").GetComponent<TMP_Text>().text = newEnemy.block.ToString();
            card.transform.Find("EnemyStatText/AttackText").GetComponent<TMP_Text>().text = newEnemy.attack.ToString();
        }

        for (int i = 0; i < maxEnemiesInTurn; i++)
        {
            willKillEnemy[i] = false;
        }
    }

    void GenerateChoices()
    {
        if (toGenerateSpecialChoices)
        {
            // @hardcoded
            choices[0] = ChoiceType.specialBlocksteal;
            choices[1] = ChoiceType.specialLifesteal;
            choices[2] = ChoiceType.specialBlockRegen;
            
            toGenerateSpecialChoices = false;
        }
        else
        {
            // @TODO generate choices better...
            
            for (int i = 0; i < maxChoiceNumber; i++)
            {
                // @FAILURE is this corrent inclusivity....?
                int choiceRNG = Random.Range(0, nonSpecialChoiceNumber);
                choices[i] = (ChoiceType) choiceRNG;
            }
        }

        for (int i = 0; i < maxChoiceNumber; i++)
        {
            switch (choices[i])
            {
                // @hardcoded
                case ChoiceType.boostMaxHp:
                    {
                        choiceSprites[i].sprite  = boosthpSprite;
                        choiceText[i].text = "increase by " + 5;
                    } break;
                case ChoiceType.boostHpRegen:
                    {
                        choiceSprites[i].sprite   = boosthpRegenSprite;
                        choiceText[i].text = "increase by " + 3;
                    } break;
                case ChoiceType.boostAttackPower:
                    {
                        choiceSprites[i].sprite   = boostattackPowerSprite;
                        choiceText[i].text = "increase by " + 1;
                    } break;
                case ChoiceType.boostArmorPen:
                    {
                        choiceSprites[i].sprite   = boostarmorPenSprite;
                        choiceText[i].text = "increase by " + 0.05;
                    } break;
                case ChoiceType.boostAOE:
                    {
                        choiceSprites[i].sprite   = boostaoeSprite;
                        choiceText[i].text = "increase by " + 0.05;
                    } break;
                case ChoiceType.boostBlock:
                    {
                        choiceSprites[i].sprite   = boostblockSprite;
                        choiceText[i].text = "increase by " + 3;
                    } break;                    
                case ChoiceType.specialBlocksteal:
                    {
                        choiceSprites[i].sprite   = special0Sprite;
                        choiceText[i].text = "Steak block from killed enemies";
                    } break;
                case ChoiceType.specialLifesteal:
                    {
                        choiceSprites[i].sprite = special1Sprite;
                        choiceText[i].text    = "Steal HP from enemies with lower DMG than you";
                    } break;
                case ChoiceType.specialBlockRegen:
                    {
                        choiceSprites[i].sprite   = special2Sprite;
                        choiceText[i].text = "HP Regen also gives block";
                    } break;
                default:
                    {
                        Logger.Log("Invalid default case for choices[i]");
                    } break;
            }
        }
    }

    void ProcessChosen()
    {
        // @TODO do multiplicative or opposite based on archetype detected
        
        switch (choices[chosen])
        {
            case ChoiceType.boostMaxHp:
                {
                    buildState.maxHp += 5;
                } break;
            case ChoiceType.boostHpRegen:
                {
                    buildState.hpRegen += 1;
                } break;
            case ChoiceType.boostAttackPower:
                {
                    buildState.attackPower += 3;
                } break;
            case ChoiceType.boostArmorPen:
                {
                    buildState.armorPen += 0.03f;
                } break;
            case ChoiceType.boostAOE:
                {
                    buildState.aoe += 0.03f;
                } break;
            case ChoiceType.boostBlock:
                {
                    resourcesState.block += 3;
                } break;                
            case ChoiceType.specialBlocksteal:
                 {
                    specialChosen = choices[chosen];
                    specialChoiceImage.sprite = special0Sprite;
                    specialChoiceImage.enabled = true;
                    // Logger.Log(" !!!!!! GENERATED SPECIAL CHOICES !!!!! ");
                } break;
            case ChoiceType.specialLifesteal:
                {
                    specialChosen = choices[chosen];
                    specialChoiceImage.sprite = special1Sprite;
                    specialChoiceImage.enabled = true;
                } break;
            case ChoiceType.specialBlockRegen:
                {
                    specialChosen = choices[chosen];
                    specialChoiceImage.sprite = special2Sprite;
                    specialChoiceImage.enabled = true;
                } break;
            default:
                {
                    Logger.LogError("Invalid default case for choices[chosen]");
                } break;
        }

        ClampStuff();
        
        UpdateUIText();
    }

    void UpdateResources()
    {
        resourcesState.hp = resourcesState.hp + buildState.hpRegen;

        switch (specialChosen)
        {
            case ChoiceType.specialNone:
                {
                    // do nothing
                } break;
            case ChoiceType.specialLifesteal:
                {
                    if (specialChosen == ChoiceType.specialLifesteal)
                    {
                        for (int i = 0; i < enemiesInCurrentTurn; i++)
                        {
                            if ((!enemies[i].isDead) && (enemies[i].attack < buildState.attackPower))
                            {
                                resourcesState.hp = resourcesState.hp + enemies[i].hp;
                            }
                        }
                    }
                } break;
            case ChoiceType.specialBlocksteal:
                {
                    if (specialChosen == ChoiceType.specialBlocksteal)
                    {
                        for (int i = 0; i < enemiesInCurrentTurn; i++)
                        {
                            if ((!enemies[i].isDead) && willKillEnemy[i])
                            {
                                resourcesState.block = resourcesState.block + enemies[i].block;
                            }
                        }
                    }
                } break;
            case ChoiceType.specialBlockRegen:
                {
                    if (specialChosen == ChoiceType.specialBlockRegen)
                    {
                        resourcesState.block = resourcesState.block + buildState.hpRegen;
                    }
                } break;
            default:
                {
                    Logger.Log("Invalid default case for specialChosen");
                } break;                
        }
        
        ClampStuff();
        
        UpdateUIText();
    }

    void ClampStuff()
    {
        
        if (buildState.aoe > 1) buildState.aoe = 1;
        if (buildState.armorPen > 1) buildState.armorPen = 1;
        if (buildState.hpRegen > 33) buildState.hpRegen = 33;
        if (buildState.attackPower > 99) buildState.attackPower = 99;
        if (buildState.maxHp > 99) buildState.maxHp = 99;
        if (resourcesState.block > 99) resourcesState.block = 99;
        if (resourcesState.hp > buildState.maxHp) resourcesState.hp = buildState.maxHp;        
    }

    void ChooseTarget()
    {
        enemyToAttack = Random.Range(0,maxEnemiesInTurn);
        Logger.Log("ATTACKING ENEMY: " + enemyToAttack);
    }

    int AttackEnemyAfterAOEandBlock(int i)
    {
        int dmg = 0;
        if (enemyToAttack == i) dmg = buildState.attackPower;
        else dmg = (int)Mathf.Floor(buildState.attackPower*buildState.aoe);
        
        Enemy enemy = enemies[i];
        enemy.block = (int)Mathf.Floor(enemy.block*(1 - buildState.armorPen));
        int result = (int)Mathf.Floor((float)(dmg - enemy.block));
        if (result < 0) result = 0;
        return result;
    }

    void WillKillEnemies()
    {
        for (int i = 0; i < enemiesInCurrentTurn; i++)
        {
            Enemy enemy = enemies[i];
            enemy.hp = enemy.hp - AttackEnemyAfterAOEandBlock(i);
            if (enemy.hp < 0)
            {
                enemy.hp = 0;
                enemy.isDead = true; // probably don't need both...
                willKillEnemy[i] = true;
                metaState.killCount = metaState.killCount + 1;
                if (metaState.killCount > 9999) metaState.killCount = 9999;
            }
        }
        
        UpdateUIText();
    }

    int DefendAfterBlock(int i)
    {
        int dmg = enemies[i].attack;
        metaState.dmgTaken = metaState.dmgTaken + dmg;
        if (metaState.dmgTaken > 9999) metaState.dmgTaken = 9999;
        resourcesState.block = resourcesState.block - dmg;
        if (resourcesState.block < 0)
        {
            dmg = - resourcesState.block;
            resourcesState.block = 0;
        }
        else
        {
            dmg = 0;
        }
        return dmg;
    }

    void GetAttackedByEnemies()
    {
        for (int i = 0; i < enemiesInCurrentTurn; i++)
        {
            if ((!willKillEnemy[i]) && (!enemies[i].isDead))
            {
                resourcesState.hp = resourcesState.hp - DefendAfterBlock(i);
                if (resourcesState.hp <= 0)
                {
                    resourcesState.hp = 0;
                    goGameOver = true;
                    Logger.Log("LOST GAME");
                    // how to tint text?
                    stageText.text = "DEFEAT";
                }
            }
        }

        UpdateUIText();
    }

    void UpdateStage()
    {
        metaState.turn = metaState.turn + 1;
        int relativeTurn = metaState.turn - maxStageTurns * (int)currentStage;
        if (metaState.turn == maxTurn)
        {
            goVictory = true;
            Logger.Log("Won game");
            // how to tint text?
            stageText.text = "VICTORY";
        }
        else if (relativeTurn == maxStageTurns)
        {
            Logger.Log(" !!!!! CHANGED STAGE !!!!! ");
            currentStage = currentStage + 1;
                
            switch (currentStage)
            {
                case StageType.Garden:
                    {
                        Logger.LogError("Switched to first stage");
                    } break;
                case StageType.Graveyard:
                    {
                        currentStageEnemies = stageGraveyardEnemies;
                        currentFrameSprite  = graveyardFrameSprite;
                        backgroundImage.sprite = graveyardBackgroundSprite;
                        stageMusic.clip = graveyardMusic;
                        stageMusic.loop = true;
                        stageMusic.Play();
                    } break;
                case StageType.Crypt:
                    {
                        currentStageEnemies = stageStrongholdEnemies;
                        currentFrameSprite  = strongholdFrameSprite;
                        toGenerateSpecialChoices = true;
                        backgroundImage.sprite = strongholdBackgroundSprite;
                        stageMusic.clip = strongholdMusic;
                        stageMusic.loop = true;
                        stageMusic.Play();                        
                    } break;
                case StageType.Necropolis:
                    {
                        currentStageEnemies = stageNecropolisEnemies;
                        currentFrameSprite  = necropolisFrameSprite;
                        backgroundImage.sprite = necropolisBackgroundSprite;
                        stageMusic.clip = necropolisMusic;
                        stageMusic.loop = true;
                        stageMusic.Play();                        
                    } break;                    
                default:
                    {
                        Logger.LogError("Invalid default case for currentStage");
                    } break;
            }

            for (int i = 0; i < choicePanel.childCount; i++)
            {
                Transform child = choicePanel.GetChild(i);
                Image img = child.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = currentFrameSprite;
                }
            }

            for (int i = 0; i < buildPanel.childCount; i++)
            {
                Transform child = buildPanel.GetChild(i);
                Image img = child.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = currentFrameSprite;
                }
            }
            
        }

        UpdateUIText();
    }

    void UpdateUIText()
    {
        hpText.text          = resourcesState.hp.ToString() + "/" + buildState.maxHp;
        hpRegenText.text     = buildState.hpRegen.ToString();
        blockText.text       = resourcesState.block.ToString();
        attackPowerText.text = buildState.attackPower.ToString();
        armorPenText.text    = buildState.armorPen.ToString();
        aoeText.text         = buildState.aoe.ToString();
        
        turnText.text        = "TURN " + metaState.turn.ToString() + "/" + maxTurn.ToString();
        killCountText.text   = "KILLS " + metaState.killCount.ToString();
        dmgTakenText.text    = "DAMAGE TAKEN " + metaState.dmgTaken.ToString();
        if (goVictory)
        {
            stageText.text       = "VICTORY";
            
        }
        else if (goGameOver)
        {
            stageText.text       = "DEFEAT";
        }
        else
        {
            stageText.text       = currentStage.ToString();
        }
    }

    void InitGameState()
    {
        currentStage = StageType.Garden;
        currentStageEnemies = stageMarshEnemies;
        currentFrameSprite = marshFrameSprite;
        specialChosen = ChoiceType.specialNone;
        toGenerateSpecialChoices = false;
        enemiesInCurrentTurn = 0;
        enemyToAttack = 0;

        stageMusic.clip = marshMusic;
        stageMusic.loop = true;
        stageMusic.Play();
        backgroundImage.sprite = marshBackgroundSprite;
        
        // @TODO invalidate complex choice generation later...

        for (int i = 0; i < maxEnemiesInTurn; i++)
        {
            willKillEnemy[i] = false;
        }

        // @hardcoded
        buildState.maxHp = 50;
        buildState.hpRegen = 10;
        buildState.attackPower = 7;
        buildState.armorPen = 0;
        buildState.aoe = 0;

        metaState.turn = 1;
        metaState.killCount = 0;
        metaState.dmgTaken = 0;

        resourcesState.hp = 50;
        resourcesState.block = 50;

        GenerateEnemies();
        GenerateChoices();

        specialChoiceImage.enabled = false;

        goNextTurn = false;
        goGameOver = false;
        goVictory  = false;


        for (int i = 0; i < choicePanel.childCount; i++)
        {
            Transform child = choicePanel.GetChild(i);
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = currentFrameSprite;
            }
        }

        for (int i = 0; i < buildPanel.childCount; i++)
        {
            Transform child = buildPanel.GetChild(i);
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = currentFrameSprite;
            }
        }
        
        UpdateUIText();
    }

    [System.Diagnostics.Conditional("DEBUG")]    
    private void LogResourcesState()
    {
        Logger.Log("[Resources state] HP: " + resourcesState.hp + " BLOCK: " + resourcesState.block);
    }

    [System.Diagnostics.Conditional("DEBUG")]        
    private void LogBuildState()
    {
        Logger.Log("[Build state] ATTACK-POWER: " + buildState.attackPower + " MAXHP: " + buildState.maxHp
                   + " HPREGEN: " + buildState.hpRegen + " ARMORPEN: " + buildState.armorPen + " AOE: " + buildState.aoe);
    }

    [System.Diagnostics.Conditional("DEBUG")]        
    private void LogEnemyQueue()
    {
        for (int i = 0; i < enemiesInCurrentTurn; i++)
        {
            Logger.Log("[Enemy " + i + "] TYPE: " + enemies[i].type + " IS-DEAD: " + enemies[i].isDead + " HP:" + enemies[i].hp + " ATTACK:" + enemies[i].attack);
        }        
    }

    [System.Diagnostics.Conditional("DEBUG")]        
    private void LogChoices()
    {
        for (int i = 0; i < maxChoiceNumber; i++)
        {
            Logger.Log("[Choice " + i + "] " + choices[i]);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]        
    private void LogChosen()
    {
        Logger.Log("[Choosen ] " + choices[chosen]);
    }

    [System.Diagnostics.Conditional("DEBUG")]        
    private void LogTurn()
    {
        Logger.Log("[Entered turn] " + metaState.turn +
                   " -------------------------------------------------");
    }

    [System.Diagnostics.Conditional("DEBUG")]        
    private void LogKillCount()
    {
        Logger.Log("[Kill Count] " + metaState.killCount);
    }

    [System.Diagnostics.Conditional("DEBUG")]        
    private void LogStage()
    {
        Logger.Log("[STAGE ] " + currentStage);
    }
}


public enum ChoiceType
{
    boostMaxHp,
    boostHpRegen,
    boostAttackPower,
    boostArmorPen,
    boostAOE,
    boostBlock,
    specialNone,
    specialBlocksteal,
    specialLifesteal,
    specialBlockRegen    
}

public enum EnemyType
{
    Bat,
    Snail,
    Zombie,
    Skeleton,
    Goblin,
    Orc,
    Ghost,
    Wraith
}

public enum StageType
{
    Garden,
    Graveyard,
    Crypt,
    Necropolis
}

public class Enemy
{
    public EnemyType type;
    public int hp;
    public int block;
    public int attack;
    public bool isDead;
}

public class BuildState
{
    public int maxHp;
    public int hpRegen;
    public int attackPower;
    public float armorPen;
    public float aoe;
}

public class ResourcesState
{
    public int hp;
    public int block;
}

public class MetaState
{
    public int turn;
    public int killCount;
    public int dmgTaken;
}


