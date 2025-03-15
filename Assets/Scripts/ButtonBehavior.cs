using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonBehavior : NetworkBehaviour
{
    public LobbyManager lobbyManager;

    public GameObject PlayGameUI;
    public GameObject LobbySetUpUI;
    public GameObject LobbyUI;
    public GameObject ReadyUpButtons;

    public TMP_InputField lobbyCode;
    public TMP_InputField lobbyName;

    public static NetworkVariable<int> readyPlayers = new NetworkVariable<int>(0);

    private void Start()
    {
        lobbyManager = LobbyManager.LobbyManagerInstance;
    }
    [ServerRpc]
    public void PlayGameServerRPC()
    {
        //load a game Main scene
        if (lobbyManager.IsLobbyHost())// && (PlayerBehavior.numPlayers == ButtonBehavior.readyPlayers.Value))
        {
            LobbyUI.SetActive(false);
            GameManager.gameState = GameStates.GAME_PHASE1;
            NetworkManager.Singleton.SceneManager.LoadScene(GameManager.mainGameSceneName, LoadSceneMode.Additive);
        }
    }

    public void PlayerReady()
    {
        if (!lobbyManager.IsLobbyHost())
            LobbyUI.SetActive(false);
        readyPlayers.Value += 1;
        ReadyUpButtons.SetActive(false);
    }

    public void OpenInstructions()
    {
        //load a game instructions scene
        SceneManager.LoadScene(GameManager.instructSceneID);
    }

    public void OpenPlayGameMenu()
    {
        PlayGameUI.SetActive(true);
    }

    public void OpenCreateLobbyMenu()
    {
        LobbySetUpUI.SetActive(true);
    }

    public void ReturnToStart()
    {
        //load a game start scene
        SceneManager.LoadScene(GameManager.startSceneID);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ClosePlayGameMenu()
    {
        PlayGameUI.SetActive(false);
    }

    public void CloseLobbyMenu()
    {
        LobbySetUpUI.SetActive(false);
    }

    public void QuickJoinLobby()
    {
        lobbyManager.QuickJoin();
    }

    public void JoinLobbyWithCode()
    {
        lobbyManager.JoinLobbyWIthCode(lobbyCode.text);
    }

    public void CreateLobbyPublic()
    {
        lobbyManager.CreateLobby(lobbyName.text, false);
    }
    public void CreateLobbyPrivate()
    {
        lobbyManager.CreateLobby(lobbyName.text, true);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
