using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BalanseManagerScript : MonoBehaviour
{
    public static BalanseManagerScript instance;

    [Header("Text outputs")]
    [SerializeField] private Text coinTxt;

    private int balanse;

    private string userId;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        userId = AuthManager.Instance.GetTokenPayload().nameid;
        StartCoroutine(UpdateBalanse());
    }
    public IEnumerator UpdateBalanse()
    {
        var token = AuthManager.Instance.GetToken();
        string url = $"http://localhost:5295/api/Coins/balance/{userId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<BalanseResponse>(request.downloadHandler.text);
                balanse = response.coins;
                coinTxt.text = response.coins.ToString();
            }
        }
    }

    public IEnumerator EarnCoins(int amount)
    {
        var token = AuthManager.Instance.GetToken();
        string url = $"http://localhost:5295/api/Coins/earn/{userId}/{amount}";

        Debug.Log($"Отправляем запрос: {url}");
        Debug.Log(coinTxt ? "coinTxt назначен" : "coinTxt НЕ назначыен!");

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            // ЛОГИРУЕМ ОБЯЗАТЕЛЬНО — даже если произошла ошибка
            Debug.Log("Статус запроса: " + request.result);
            Debug.Log("Код ответа: " + request.responseCode);
            Debug.Log("Ответ сервера: " + request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<BalanseResponse>(request.downloadHandler.text);
                    balanse = response.coins;
                    coinTxt.text = response.coins.ToString();
                    Debug.Log($"Монеты обновлены: {balanse}");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Ошибка при парсинге JSON: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("Ошибка запроса: " + request.error);
            }
        }
    }

    [Serializable]
    public class BalanseResponse
    {
        public int coins;
    }
 }
