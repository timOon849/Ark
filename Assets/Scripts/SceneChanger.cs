using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SceneByIndexAsync(int index)
    {
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
    }

    public void OpenShopScene()
    {
        SceneManager.LoadScene("ShopScene");
    }
}
