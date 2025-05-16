using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour
{
    public TMP_InputField usernameTxt;
    public TMP_InputField passwordTxt;

    public void OnLoginClick()
    {
            if (string.IsNullOrEmpty(usernameTxt.text))
        {
            Debug.Log("Введите имя пользователя!");
            return;
        }
        if (string.IsNullOrEmpty(passwordTxt.text))
        {
            Debug.Log("Введите пароль!");
            return;
        }
        string email = usernameTxt.text;
        string password = passwordTxt.text;

        StartCoroutine(AuthManager.Instance.Login(email, password, (success) =>
        {
            if (success)
            {
                Debug.Log("Вы вошли в систему");
                if (AuthManager.Instance.IsLoggedIn)
                {
                    var payload = AuthManager.Instance.GetTokenPayload();

                    if (payload != null && !payload.IsExpired())
                    {
                        Debug.Log($"Пользователь: {payload.unique_name}, ID: {payload.nameid}");
                        SceneChanger sceneChanger = new SceneChanger();
                        sceneChanger.SceneByIndexAsync(0);
                    }
                    else
                    {
                        Debug.LogWarning("Токен отсутствует или истёк");
                    }
                }
            }
            else
            {
                Debug.Log("Неверный логин или пароль");
            }
        }));
    }
}