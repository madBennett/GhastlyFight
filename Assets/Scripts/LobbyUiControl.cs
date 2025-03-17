using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyUiControl : MonoBehaviour
{
    public TMP_Text lobbyName;
    public TMP_Text lobbyCode;

    public LobbyManager lobbyManager;
    private Lobby joinedLobby;

    // Start is called before the first frame update
    void Start()
    {
        //get lobby manager
        lobbyManager = LobbyManager.LobbyManagerInstance;

        //get the joined lobby
        joinedLobby = lobbyManager.getJoinedLobby();

        //display lobby information
        lobbyName.text = "Lobby Name: " + joinedLobby.Name;
        lobbyCode.text = "Lobby Code: " + joinedLobby.LobbyCode;
    }
}
