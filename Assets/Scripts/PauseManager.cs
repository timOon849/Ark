using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPause = false;
    public GameObject pauseScreen;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    public void TogglePause()
    {
        isPause = !isPause;
        ToggleVisibility();
        Time.timeScale = isPause ? 0 : 1;
    }
    
    public void ToggleVisibility()
    {
        if (pauseScreen != null)
        {
            pauseScreen.SetActive(isPause);
        }
    }
}
