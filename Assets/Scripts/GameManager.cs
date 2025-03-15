using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public enum GameStates
{
    LOBBY,
    WAITING,
    GAME_PHASE1,
    GAME_PHASE2,
    GAME_PHASE3
}

public class GameManager : NetworkBehaviour
{
    //Scene IDS
    static public int startSceneID = 0;
    static public int lobbySceneID = 1;
    static public int instructSceneID = 2;
    static public int mainGameSceneID = 3;
    static public int gameWinSceneID = 4;
    static public int gameLoseSceneID = 5;

    //Scene Names
    static public string startSceneName = "Menu";
    static public string lobbySceneName = "Lobby";
    static public string instructSceneName = "Instructions";
    static public string mainGameSceneName = "Main Game";
    static public string gameWinSceneName = "Game Win";
    static public string gameLoseSceneName = "Game Lose";


    [SerializeField] public static GameStates gameState;
    public static bool isEnemyDead = false; //varible to alert players when the enemy is "killed"

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameStates.LOBBY;//change back to waiting
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameState);
    }
}
