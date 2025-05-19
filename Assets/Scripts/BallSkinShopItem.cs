using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;

public class BallSkinShopItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text skinNameText;
    [SerializeField] private Text priceText;
    [SerializeField] private Image skinImage;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button setButton;


    [Header("Skin Data")]
    public int SkinId;
    public string skinName;
    public int price;
    public Sprite skinSprite;

    private int userId;
    private bool isOwned;
    private bool isSelected;    

    private void Awake()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        setButton.onClick.AddListener(OnSetButtonClicked);
    }

    public void Initialize(int userId, BallSkinData skinData, bool owned, bool selected)
    {
        userId = userId;
        SkinId = skinData.Id;
        skinName = skinData.Name;
        price = skinData.Price;

        skinNameText.text = skinName;
        priceText.text = price.ToString();
        skinImage.sprite = skinSprite;

        isOwned = owned;
        isSelected = selected;

        UpdateUI();
    }

    private void UpdateUI()
    {
        buyButton.gameObject.SetActive(!isOwned);

        if (!isOwned)
        {
            StartCoroutine(CheckUserBalance());
        }
        else
        {
            setButton.gameObject.SetActive(true); // например
            setButton.interactable = !isSelected;
        }
    }

    private IEnumerator GetAvailible()
    {
        var token = AuthManager.Instance.GetToken();
        string url = $"http://localhost:5295/api/BallSkins/GetAvailableSkins/{userId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<AvailableSkinsResponse>(
                    "{\"Skins\":" + request.downloadHandler.text + "}"
                );

                isOwned = response.Skins.Exists(skin => skin.BallSkinId == SkinId);
            }
        }

        UpdateUI(); // Вызываем UpdateUI только после завершения запроса
    }

    private IEnumerator CheckUserBalance()
    {
        string url = $"http://localhost:5295/api/Coins/GetBalance/{userId}";
        var token = AuthManager.Instance.GetToken();

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var balance = JsonUtility.FromJson<BalanceResponse>(request.downloadHandler.text);
                buyButton.interactable = balance.Coins >= price;
            }
        }
    }
    private void OnSetButtonClicked()
    {
        StartCoroutine( SetSkinRequest() );
    }

    private void OnBuyButtonClicked()
    {
        StartCoroutine(BuySkinRequest());
    }
    private IEnumerator SetSkinRequest()
    {
        yield return "";
        Debug.Log("avava");
    }

    private IEnumerator BuySkinRequest()
    {
        var token = AuthManager.Instance.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("User is not authenticated");
            yield break;
        }

        var payload = AuthManager.Instance.GetTokenPayload();
        if (payload == null)
        {
            Debug.LogError("Could not get user ID from token");
            yield break;
        }

        int UserId = int.Parse(payload.nameid);

        string url = $"http://localhost:5295/api/BallSkins/BuySkin/{UserId}/{SkinId}";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Successfully purchased skin: {skinName}");
                isOwned = true;
                UpdateUI();
            }
            else
            {
                Debug.LogError($"Purchase failed: {request.error}");

                try
                {
                    var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                    if (errorResponse != null)
                    {
                        if (errorResponse.ErrorCode == "INSUFFICIENT_FUNDS")
                        {
                            Debug.LogError($"Not enough coins. Needed: {errorResponse.Required}, Have: {errorResponse.Available}");
                        }
                        else
                        {
                            Debug.LogError($"Purchase error: {errorResponse.Error}");
                        }
                    }
                }
                catch
                {
                    Debug.LogError($"Raw error response: {request.downloadHandler.text}");
                }
            }
        }
    }
    [System.Serializable]
    private class AvailableSkinsResponse
    {
        public System.Collections.Generic.List<BuySkins> Skins;
    }

    [System.Serializable]
    private class BuySkins
    {
        public int UserId;
        public int BallSkinId;
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string Error;
        public string ErrorCode;
        public int Required;
        public int Available;
    }

    [System.Serializable]
    public class BallSkinData
    {
        public int Id;
        public string Name;
        public int Price;
        // Add other skin properties as needed
    }
    [System.Serializable]
    private class BalanceResponse
    {
        public int Coins;
    }
}