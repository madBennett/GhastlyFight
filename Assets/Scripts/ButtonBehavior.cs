using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonBehavior : MonoBehaviour
{
    public LobbyManager lobbyManager;

    public GameObject PlayGameUI;
    public GameObject LobbySetUpUI;

    public TMP_InputField lobbyCode;
    public TMP_InputField lobbyName;

    private void Start()
    {
        lobbyManager = LobbyManager.LobbyManagerInstance;
    }

    public void PlayGame()
    {
        //load a game Main scene
        if (lobbyManager.IsLobbyHost())
        {
            //change game state to start game
            GameManager.gameState = GameStates.GAME_PHASE1;

            //remove lobby as it is no longer needed
            lobbyManager.DeleteLobby();
            //load game scene
            NetworkManager.Singleton.SceneManager.LoadScene(GameManager.mainGameSceneName, LoadSceneMode.Single);
        }
    }

    public void OpenPlayGameMenu()
    {
        //open proper ui
        PlayGameUI.SetActive(true);
    }

    public void OpenCreateLobbyMenu()
    {
        //open proper ui
        LobbySetUpUI.SetActive(true);
    }

    public void LeaveLobby()
    {
        //remove the player from lobby
        lobbyManager.LeaveLobby();
        ReturnToStart();
    }

    public void LeaveLobbyExit()
    {
        //remove player from lobby and quit game
        lobbyManager.LeaveLobby();
        QuitGame();
    }

    public void ReturnToStart()
    {
        //load a game start scene
        SceneManager.LoadScene(GameManager.startSceneID);
    }

    public void QuitGame()
    {
        //exit app
        Application.Quit();
    }

    public void ClosePlayGameMenu()
    {
        //close proper ui
        PlayGameUI.SetActive(false);
    }

    public void CloseLobbyMenu()
    {
        //close proper ui
        LobbySetUpUI.SetActive(false);
    }

    public void QuickJoinLobby()
    {
        //quick join any public lobby
        lobbyManager.QuickJoin();
    }

    public void JoinLobbyWithCode()
    {
        //join a specic lobby
        lobbyManager.JoinLobbyWIthCode(lobbyCode.text);
    }

    public void CreateLobbyPublic()
    {
        //create a public lobby
        lobbyManager.CreateLobby(lobbyName.text, false);
    }
    public void CreateLobbyPrivate()
    {
        //create a private lobby
        lobbyManager.CreateLobby(lobbyName.text, true);
    }
}
