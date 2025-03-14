using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public enum GameStates
{
    WAITING,
    GAME_PHASE1,
    GAME_PHASE2,
    GAME_PHASE3
}

public class GameManager : NetworkBehaviour
{
    public static GameStates gameState;
    public static bool isEnemyDead = false; //varible to alert players when the enemy is "killed"

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameStates.GAME_PHASE1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CountChanged(int previousValue, int newValue)
    {
        
    }
}
