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
    GAME_PHASE3,
    GAME_OVER
}

public enum AudioType
{
    HURT,
    ATTACK,
    DEATH,
    MOVE,
    HEAL,
    DASH
}

public class GameManager : NetworkBehaviour
{
    //Scene IDS
    static public int startSceneID = 0;
    static public int lobbySceneID = 1;
    static public int mainGameSceneID = 2;

    //Scene Names
    static public string startSceneName = "Menu";
    static public string lobbySceneName = "Lobby";
    static public string mainGameSceneName = "Main Game";


    [SerializeField] public static GameStates gameState;//varible to indiocate stat of game to all objects
    public static bool isEnemyDead = false; //varible to alert players when the enemy is "killed"

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameStates.LOBBY;
        DontDestroyOnLoad(this.gameObject);
    }
}
