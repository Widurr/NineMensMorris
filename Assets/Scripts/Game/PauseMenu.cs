using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public RectTransform costam;

    public GameObject pauseMenuUI;

    GameObject helper;
    // Update is called once per frame
    public void PauseTheGame()
    {
        if (GameIsPaused)
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
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    void Pause()
    {
        GameIsPaused = true;
        pauseMenuUI.SetActive(true);
        pauseMenuUI.transform.SetAsLastSibling();
        Time.timeScale = 0f;
    }
}
