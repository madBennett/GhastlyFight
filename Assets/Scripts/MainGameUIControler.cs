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
        //hide ui screens
        GameOverScreen.SetActive(false);
        OptionsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerBehavior.numPlayers == 1 && GameManager.gameState == GameStates.GAME_PHASE3)
        {
            //open screen and update gamestate to prevent errors
            OpenGameOverScreenClientRPC();
            GameManager.gameState = GameStates.GAME_OVER;
        }
    }

    public void OpenOptionsMenu()
    {
        //open propper menu
        OptionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        //close propper menu
        OptionsMenu.SetActive(false);
    }

    [ClientRpc]
    public void OpenGameOverScreenClientRPC()
    {
        //open menu for all players
        GameOverScreen.SetActive(true);
    }
}
