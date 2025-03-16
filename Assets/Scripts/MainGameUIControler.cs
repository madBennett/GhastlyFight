using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameUIControler : MonoBehaviour
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
        if (GameManager.gameState == GameStates.GAME_OVER)
        {
            GameOverScreen.SetActive(true);
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
}
