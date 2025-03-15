using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyUiControl : MonoBehaviour
{
    public TMP_Text lobbyName;
    public TMP_Text lobbyCode;
    public TMP_Text ReadyPlayers;

    public LobbyManager lobbyManager;
    private Lobby joinedLobby;

    // Start is called before the first frame update
    void Start()
    {
        lobbyManager = LobbyManager.LobbyManagerInstance;

        joinedLobby = lobbyManager.getJoinedLobby();

        lobbyName.text = "Lobby Name: " + joinedLobby.Name;
        lobbyCode.text = "Lobby Code: " + joinedLobby.LobbyCode;

        ReadyPlayers.text = ButtonBehavior.readyPlayers + " / " + PlayerBehavior.numPlayers;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState == GameStates.LOBBY)
            ReadyPlayers.text = ButtonBehavior.readyPlayers.Value + " / " + PlayerBehavior.numPlayers;
    }
}
