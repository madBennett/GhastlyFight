using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainGameUIControler : NetworkBehaviour
{
    public GameObject GameOverScreen;
    public GameObject OptionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        GameOverScreen.SetActive(false);
        OptionsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerBehavior.numPlayers == 1 && GameManager.gameState == GameStates.GAME_PHASE3)
        {
            OpenGameOverScreenClientRPC();
            GameManager.gameState = GameStates.GAME_OVER;
        }
    }

    public void OpenOptionsMenu()
    {
        OptionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        OptionsMenu.SetActive(false);
    }

    [ClientRpc]
    public void OpenGameOverScreenClientRPC()
    {
        GameOverScreen.SetActive(true);
    }
}
