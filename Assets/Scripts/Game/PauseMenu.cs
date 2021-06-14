using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    GameObject controller;

    string[] scriptsToDisable = new string[] { "Piece", "MovePlate" };

    public GameObject pauseMenuUI;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
    }
    public void PauseTheGame()
    {
        if (gameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    void Resume()
    {
        DisableScripts();
        gameIsPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    void Pause()
    {
        DisableScripts();
        gameIsPaused = true;
        pauseMenuUI.SetActive(true);
        pauseMenuUI.transform.SetAsLastSibling();
        Time.timeScale = 0f;
        
    }

    void DisableScripts()
    {
        for (int i = 0; i < scriptsToDisable.Length; i++)
        {
            var objects = GameObject.FindGameObjectsWithTag(scriptsToDisable[i]);

            foreach (GameObject piece in objects)
            {
                piece.GetComponent<CircleCollider2D>().enabled = gameIsPaused;
            }
        }
    }
    
}
