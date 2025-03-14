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
    [SerializeField] private NetworkVariable<int> testFrameCount = new NetworkVariable<int>(0);
    public TMP_Text TestText;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameStates.GAME_PHASE1;
        testFrameCount.OnValueChanged += CountChanged;
    }

    // Update is called once per frame
    void Update()
    {
        testFrameCount.Value += 1;
    }
    private void CountChanged(int previousValue, int newValue)
    {
        TestText.text = "" + newValue;
    }
}
