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
    }

    public void Initialize(int userId, BallSkinData skinData, bool owned, bool selected)
    {
        userId = userId;
        SkinId = skinData.id;
        skinName = skinData.name;
        price = skinData.price;

        skinNameText.text = skinName;
        priceText.text = price.ToString();
        skinImage.sprite = skinSprite; // You'll need to load this from resources or assign in inspector

        isOwned = owned;
        isSelected = selected;

        UpdateUI();
    }

    private void UpdateUI()
    {
        GetAvailible();
        buyButton.gameObject.SetActive(!isOwned);


        if (!isOwned)
        {
            StartCoroutine(CheckUserBalance());
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
                UpdateUI();
            }
        }

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

    private void OnBuyButtonClicked()
    {
        StartCoroutine(BuySkinRequest());
    }

    private IEnumerator BuySkinRequest()
    {
        // Get the JWT token from AuthManager
        var token = AuthManager.Instance.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("User is not authenticated");
            yield break;
        }

        // Get user ID from token payload
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
                // Purchase successful
                Debug.Log($"Successfully purchased skin: {skinName}");

                // Update UI to show as owned
                isOwned = true;
                UpdateUI();

                // You might want to refresh the player's coin balance here
                // by making another request to get the updated balance
            }
            else
            {
                // Handle error
                Debug.LogError($"Purchase failed: {request.error}");

                try
                {
                    // Try to parse the error response
                    var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                    if (errorResponse != null)
                    {
                        if (errorResponse.ErrorCode == "INSUFFICIENT_FUNDS")
                        {
                            // Show "Not enough coins" message to player
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
        public int id;
        public string name;
        public int price;
        // Add other skin properties as needed
    }
    [System.Serializable]
    private class BalanceResponse
    {
        public int Coins;
    }
}