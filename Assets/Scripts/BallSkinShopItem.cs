using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
        userId = GetId();
        StartCoroutine(GetAvailible());
        StartCoroutine(GetCurrentSkin());
    }

    public void Initialize(BallSkinData skinData, bool owned, bool selected)
    {
        SkinId = skinData.Id;
        skinName = skinData.Name;
        price = skinData.Price;

        skinNameText.text = skinName;
        priceText.text = price.ToString();
        skinImage.sprite = skinSprite;

        isSelected = selected;

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

    private IEnumerator GetCurrentSkin()
    { 
        var token = AuthManager.Instance.GetToken();
        string url = $"http://localhost:5295/api/BallSkins/GetCurrentSkin/{userId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<CurrentSkinResponse>(request.downloadHandler.text);
                Debug.Log(response.skin.ballSkinId);
                isSelected = response.skin.ballSkinId == SkinId;
                Debug.Log($"{skinName}: {isSelected}");
            }
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
                var response = JsonUtility.FromJson<AvailableSkinsWrapper>(request.downloadHandler.text);
                isOwned = response.skins.Exists(skin => skin.ballSkinId == SkinId);
            }
            else
            {
                Debug.LogError($"Error fetching skins: {request.error}, Code: {request.responseCode}");
            }
        }
        UpdateUI();
    }

    private int GetId()
    { 
        var payload = AuthManager.Instance.GetTokenPayload();
        return int.Parse(payload.nameid);
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
    public class CurrentSkinResponse
    {
        public BuySkinResponse skin;
    }

    [System.Serializable]
    public class BuySkinResponse
    {
        public int Id;
        public int userId;
        public object User; // Можно оставить как object, или сделать отдельный класс User
        public int ballSkinId;
        public object BallSkin; // Аналогично
    }

    [System.Serializable]
    public class AvailableSkinsWrapper
    {
        public List<BuySkinResponse> skins;
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