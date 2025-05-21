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

        Debug.Log($"���������� ������: {url}");
        Debug.Log(coinTxt ? "coinTxt ��������" : "coinTxt �� ���������!");

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            // �������� ����������� � ���� ���� ��������� ������
            Debug.Log("������ �������: " + request.result);
            Debug.Log("��� ������: " + request.responseCode);
            Debug.Log("����� �������: " + request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<BalanseResponse>(request.downloadHandler.text);
                    balanse = response.coins;
                    coinTxt.text = response.coins.ToString();
                    Debug.Log($"������ ���������: {balanse}");
                }
                catch (Exception ex)
                {
                    Debug.LogError("������ ��� �������� JSON: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("������ �������: " + request.error);
            }
        }
    }

    [Serializable]
    public class BalanseResponse
    {
        public int coins;
    }
 }
