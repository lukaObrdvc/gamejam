using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // reference to current choices
    // reference to enemy queue
    // build panel text (sprites like bars maybe later?)
    // (metadata panel maybe later?)

    private BuildState     buildState;
    // private MetaState      metaState;
    private ResourcesState resourcesState;

    private bool goNextTurn = false;
    private bool goMenu     = false;
    private bool goGameOver = false;
    private bool goVictory  = false;
    
    private static int maxTurn     = 100;
    private int        currentTurn = 0;

    private static int         maxStageNumber      = 2;
    private static int         maxStageTurns       = 50;
    private StageType          currentStage        = StageType.Basic;
    private static EnemyType[] stageBasicEnemies   = {EnemyType.Goblin};
    private static EnemyType[] stageVoidEnemies    = {EnemyType.Dragon, EnemyType.Goblin};
    private EnemyType[]        currentStageEnemies = stageBasicEnemies;
    
    private int                 chosen;
    private ChoiceType[]        choices             = new ChoiceType[maxChoiceNumber];
    private static int          minChoiceNumber     = 4;
    private static int          maxChoiceNumber     = 8;
    private int                 currentChoiceNumber = minChoiceNumber;
    // this is maxChoiceNumber - minChoiceNumber array size
    private int[] incrementChoiceNumberOnRelativeTurns = new int[4]{14, 22, 33, 41};
    
    private static int     maxEnemies                 = 10;
    private static int     maxEnemiesInTurn           = 10;
    private Enemy[]        enemies                    = new Enemy[maxEnemies];
    private int[]          enemyTurns                 = new int[maxEnemies];
    private int            enemiesInCurrentTurn       = 0;
    private int            enemiesKilledInCurrentTurn = 0;
    private int            latestEnemyTurn            = 0;

    private int totalEnemiesKilled = 0;

    [SerializeField] private RectTransform enemyQueuePanel;
    [SerializeField] private RectTransform choicePanel;
    // make a static array of enemy prefabs here


    // basic prototype:
    //   build attributes (text only): attack power, hp regen
    //   resources (text only): hp, block (should not be a bar anyway..)
    //   choices (text only): increase attack power, increase max hp
    //   enemies (text only): 2a + 6h, 6a + 2h
    
    void Start()
    {
        currentStageEnemies = stageBasicEnemies;

        buildState = new BuildState();
        buildState.hpRegen     = 1;
        buildState.attackPower = 2;
        buildState.maxHp = 10;

        // we probably do not need this anyway...
        // metaState.turn      = 0;
        // metaState.killCount = 0;

        resourcesState = new ResourcesState();
        resourcesState.hp    = 10;
        resourcesState.block = 0;

        int enemyTurnSpreadRNG1 = Random.Range(1, maxEnemies/2);
        Logger.Log("SLOTS: " + enemyTurnSpreadRNG1);
        int[] enemyTurnSlots = new int[enemyTurnSpreadRNG1];
        int currentEnemiesProcessed = 0;
        for (int i = 0; i < enemyTurnSpreadRNG1; i++)
        {
            int enemiesInTurnRNG = Random.Range(1, maxEnemies-enemyTurnSpreadRNG1-currentEnemiesProcessed + 1);
            if (i + 1 == enemyTurnSpreadRNG1)
            {
                enemiesInTurnRNG = maxEnemies - currentEnemiesProcessed;
            }
            Logger.Log("ENEMIES: " + enemiesInTurnRNG + " IN SLOT: " + i);
            enemyTurnSlots[i] = enemiesInTurnRNG;
            for (int j = 0; j < enemiesInTurnRNG; j++)
            {
                enemyTurns[currentEnemiesProcessed + j] = i;
            }
            currentEnemiesProcessed = currentEnemiesProcessed + enemiesInTurnRNG;
        }
        
        int currentTurn = 0;
        int enemiesInSlot = 0;
        int currentTurnSlots = enemyTurnSlots[currentTurn];
        for (int i = 0; i < maxEnemies; i++)
        {
            Enemy newEnemy = new Enemy();
                
            int enemyRNG = Random.Range(0, 1);
            EnemyType enemyType = currentStageEnemies[enemyRNG];
            newEnemy.type = enemyType;
            int enemyAttackRNG = Random.Range(1,2);
            newEnemy.attack = 2*enemyAttackRNG;

            int enemyHpRNG = Random.Range(2,3);
            newEnemy.hp = (int)Mathf.Ceil(0.4f * enemyHpRNG);

            newEnemy.isDead = false;
                
            int enemiesArrOffset = maxEnemies-enemiesInCurrentTurn;
            enemies[enemiesArrOffset + i] = newEnemy;
            enemyTurns[enemiesArrOffset + i] = currentTurn;

            enemiesInSlot = enemiesInSlot + 1;
            if (enemiesInSlot == enemyTurnSlots[currentTurn])
            {
                currentTurn = currentTurn + 1;
                enemiesInSlot = 0;
            }
            
        }
        
        
    }

    void Update()
    {
        // @TODO add sound everywhere?

        // - check if menu button is pressed
        //   (with mouse raycast, or ESC or something)
        // - check which choice is picked
        //   (with mouse raycast, or number keys or something)
        //   (if none is picked then just update some metadata and skip)
        // - update states based on choice
        // - update certain resources
        // - deal damage to enemies (in current turn?)
        // - update certain resources
        // - remove dead enemies
        // - take damage from enemies in current turn
        // - update certain resources
        // - check if game over
        //   (if yes put a game over overlay)
        // - increment turn (metadata?)
        // - check if won game
        //   (if yes put a victory overlay)
        // - check if going to new biome or whatever
        //   (if yes revert build state, change enemy types)
        // - make new enemies
        // - generate new choices

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            chosen = 1;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            chosen = 2;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            chosen = 3;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            chosen = 4;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            chosen = 5;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            chosen = 6;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            chosen = 7;
            goNextTurn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            chosen = 8;
            goNextTurn = true;
        }        
        
        if (goMenu)
        {

            
            
        }
        
        if (goNextTurn)
        {

            Logger.Log("Entered turn: " + currentTurn);

            Logger.Log("Current build state: " + buildState.attackPower + " " + buildState.maxHp);

            for (int i = 0; i < currentChoiceNumber; i++)
            {
                Logger.Log("Choice i: " + choices[i]);
            }

            Logger.Log("Choosen: " + choices[chosen]);
            
            // process chosen choice
            switch (choices[chosen])
            {
                case ChoiceType.IncreaseAttackPower:
                    {
                        buildState.attackPower += 1;
                    } break;
                case ChoiceType.IncreaseHP:
                    {
                        buildState.maxHp += 10;
                    } break;
                default:
                    {
                        Logger.LogError("Invalid default case for chosenChoice");
                    } break;
            }

            Logger.Log("Updated build state: " + buildState.attackPower + " " + buildState.maxHp);

            // update resources
            resourcesState.hp = resourcesState.hp + buildState.hpRegen;
            if (resourcesState.hp > buildState.maxHp)
            {
                resourcesState.hp = buildState.maxHp;
            }

            Logger.Log("Updated resources state: " + resourcesState.hp);

            // logging...
            for (int i = 0; i < maxEnemies; i++)
            {
                Logger.Log("Enemy: " + i + " is dead:" + enemies[i].isDead + " hp:" + enemies[i].hp + " attack:" + enemies[i].attack);
            }
            
            // attack enemies in current turn
            for (int i = 0; i < maxEnemies; i++)
            {
                if (enemyTurns[i] != 0)
                {
                    enemiesInCurrentTurn = i + 1;
                    Logger.Log("Total enemies in current turn: " + enemiesInCurrentTurn);
                    break;
                }
                enemies[i].hp -= buildState.attackPower;
                if (enemies[i].hp <= 0)
                {
                    enemies[i].isDead = true;
                    enemiesKilledInCurrentTurn = enemiesKilledInCurrentTurn + 1;
                }
            }

            for (int i = 0; i < maxEnemies; i++)
            {
                Logger.Log("Enemy: " + i + " is dead:" + enemies[i].isDead + " hp:" + enemies[i].hp + " attack:" + enemies[i].attack);
            }
            
            // remove dead enemies
            int enemiesLeft = 0;
            Enemy[] newEnemies = new Enemy[maxEnemies];
            for (int i = 0; i < maxEnemies; i++)
            {
                if (!enemies[i].isDead)
                {
                    newEnemies[enemiesLeft] = enemies[i];
                    enemiesLeft = enemiesLeft + 1;
                }
            }
            enemies = newEnemies;

            for (int i = 0; i < enemiesLeft; i++)
            {
                Logger.Log("Enemy: " + i + " is dead:" + enemies[i].isDead + " hp:" + enemies[i].hp + " attack:" + enemies[i].attack);
            }

            Logger.Log("HP before attack: " + resourcesState.hp);

            for (int i = 0; i < enemiesLeft; i++)
            {
                Logger.Log("Enemy: " + i + " Turn: " + enemyTurns[i]);
            }
            
            // get attacked by alive enemies in current turn
            int enemiesThatAttack = enemiesInCurrentTurn - enemiesKilledInCurrentTurn;
            for (int i = 0; i < enemiesThatAttack; i++)
            {
                if (enemyTurns[i] == 0)
                {
                    resourcesState.hp -= enemies[i].attack;
                    if (resourcesState.hp <= 0)
                    {
                        goGameOver = true;

                        Logger.Log("Lost game");
                        // @TODO go to game over scene...
                    }
                }
                else
                {
                    enemyTurns[i] = enemyTurns[i] - 1;
                }
            }

            for (int i = 0; i < enemiesThatAttack; i++)
            {
                Logger.Log("Enemy: " + i + " Turn: " + enemyTurns[i]);
            }
            
            // update turn and see if won game or changed stage, so to change enemies
            // in stage and generate full new enemies
            currentTurn = currentTurn + 1;
            int relativeTurn = currentTurn - (int)currentStage * maxStageTurns;
            if (currentTurn == maxTurn)
            {
                goVictory = true;
                
                Logger.Log("Won game");
                // @TODO go to victory scene...
            }
            else if (relativeTurn == maxStageTurns)
            {
                Logger.Log("Changed stage");
                currentStage = currentStage + 1;
                // currentChoiceNumber = minChoiceNumber; // I quess this is not needed..
                
                // this is so that the for loop after can spawn all the
                // enemies for the stage
                enemiesInCurrentTurn = maxEnemies;
                
                switch (currentStage)
                {
                    case StageType.Basic:
                        {
                            currentStageEnemies = stageBasicEnemies;
                        } break;
                    case StageType.Void:
                        {
                            currentStageEnemies = stageVoidEnemies;
                        } break;
                    default:
                        {
                            Logger.LogError("Invalid default case for currentStage");
                        } break;
                }
            }

            // generate new enemies to max them out
            for (int i = 0; i < enemiesInCurrentTurn; i++)
            {
                // @TODO move queue and append these sorted by turn on which
                // they attack; also update enemyTurn array

                // @TODO you have to not spawn a new enemy if it's relative
                // turn would exceed the stage turn
                
                Enemy newEnemy = new Enemy();
                int newEnemyTurn;
                
                newEnemyTurn = latestEnemyTurn + Random.Range(0, 1);
                
                int enemyRNG = Random.Range(0, 1);
                EnemyType enemyType = currentStageEnemies[enemyRNG];
                newEnemy.type = enemyType;
                int enemyAttackRNG = Random.Range(1,2);
                newEnemy.attack = 2*enemyAttackRNG;

                int enemyHpRNG = Random.Range(2,3);
                newEnemy.hp = (int)Mathf.Ceil(0.4f * enemyHpRNG);

                newEnemy.isDead = false;
                
                int enemiesArrOffset = maxEnemies-enemiesInCurrentTurn;
                enemies[enemiesArrOffset + i] = newEnemy;
                enemyTurns[enemiesArrOffset + i] = newEnemyTurn;
            }

            for (int i = 0; i < maxEnemies; i++)
            {
                Logger.Log("Enemy: " + i + " is dead:" + enemies[i].isDead + " hp:" + enemies[i].hp + " attack:" + enemies[i].attack);
            }

            for (int i = 0; i < maxEnemies; i++)
            {
                Logger.Log("Enemy: " + i + " Turn:" + enemyTurns[i]);
            }
            
            // update metadata
            latestEnemyTurn = enemyTurns[maxEnemies-1];
            
            totalEnemiesKilled = totalEnemiesKilled + enemiesKilledInCurrentTurn;
            
            enemiesInCurrentTurn       = 0;
            enemiesKilledInCurrentTurn = 0;

            // increase choice number
            currentChoiceNumber = minChoiceNumber;
            for (int i = 0; i < maxChoiceNumber; i++)
            {
                if (relativeTurn >= incrementChoiceNumberOnRelativeTurns[i])
                {
                    Logger.Log("Changed choice number to: " + currentChoiceNumber);
                    currentChoiceNumber = currentChoiceNumber + 1;
                }
                else
                {
                    break;
                }
            }

            // generate new choice
            for (int i = 0; i < currentChoiceNumber; i++)
            {
                int choiceRNG = Random.Range(0,1);
                ChoiceType newChoice = (ChoiceType) choiceRNG;
                choices[i] = newChoice;

                // @TODO add button listeners
            }

            for (int i = 0; i < currentChoiceNumber; i++)
            {
                Logger.Log("Choice i: " + choices[i]);
            }
            
            goNextTurn = false;

        }
        
    }

}


public enum EnemyType
{
    Goblin,
    Dragon
}

// @TODO figure out if I need this or how use..
public class Enemy
{
    public EnemyType type;
    public int hp;
    public int attack;
    public bool isDead;
    // @TODO figure out what else I would want...
}


public class BuildState
{

    public int attackPower;
    public int maxHp;
    public int hpRegen;
    // @TODO add resistances here
}

// @TODO figure out how use this...
// public class SpellData
// {
//     public SpellType spell;
//     public int counter;
// }

// public enum SpellType
// {
//     None,
//     Fireball,
//     Freeze
// }


public enum ChoiceType
{
    IncreaseAttackPower,
    IncreaseHP
}


public class MetaState
{
    public int turn;
    public int killCount;
    
    // @TODO add time tracker
    // @TODO add maybe some build stats tracking, or similar
    
}


public class ResourcesState
{
    public int hp;
    public int block;

    // @TODO add summons and traps here, how exactly?
    
    // @TODO can I use flags in C# instead?
    // @TODO add more artifacts
    // public bool hasMegaSword;
    // public bool hasGigaAmulet;
}


public enum StageType
{
    Basic,
    Void
}
