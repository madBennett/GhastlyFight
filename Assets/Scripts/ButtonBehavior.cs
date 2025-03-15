using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ButtonBehavior : MonoBehaviour
{
    public LobbyManager lobbyManager;
    public void PlayGame()
    {
        //load a game Main scene
        GameManager.gameState = GameStates.GAME_PHASE1;
        SceneManager.LoadScene(GameManager.mainGameSceneID);
    }

    public void OpenInstructions()
    {
        //load a game instructions scene
        SceneManager.LoadScene(GameManager.instructSceneID);
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

    public void StartLobby()
    {
        lobbyManager.CreateLobby("Lobby1", false);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
