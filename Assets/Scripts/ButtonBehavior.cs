using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ButtonBehavior : MonoBehaviour
{
    //Scene IDS
    private int startSceneID = 0;
    private int mainGameSceneID = 1;
    private int instructSceneID = 3;


    public void PlayGame()
    {
        //load a game Main scene
        SceneManager.LoadScene(mainGameSceneID);
    }

    public void OpenInstructions()
    {
        //load a game instructions scene
        SceneManager.LoadScene(instructSceneID);
    }

    public void ReturnToStart()
    {
        //load a game start scene
        SceneManager.LoadScene(startSceneID);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
