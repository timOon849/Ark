using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnloadCurrentScene : MonoBehaviour
{
    public void UnloadCurrent()
    {
        Scene CurScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(CurScene);
    }
}
