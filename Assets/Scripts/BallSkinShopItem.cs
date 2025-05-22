using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class BallSkinShopItem : MonoBehaviour
{

    [Header("Red buttons")]
    public Button redBuyButton;
    public Button redSetButton;
    public UnityEngine.UI.Text redPriceTxt;

    [Header("Blue buttons")]
    public Button blueBuyButton;
    public Button blueSetButton;
    public UnityEngine.UI.Text bluePriceTxt;

    [Header("Brown buttons")]
    public Button brownBuyButton;
    public Button brownSetButton;
    public UnityEngine.UI.Text brownPriceTxt;

    [Header("Green buttons")]
    public Button greenBuyButton;
    public Button greenSetButton;
    public UnityEngine.UI.Text greenPriceTxt;


    [Header("Text objects")]
    [SerializeField] private UnityEngine.UI.Text coinTxt;
    [SerializeField] private UnityEngine.UI.Text errorTxt;

    [Header("Sprites")]
    public Sprite[] sprites;

    private int userId;

    private bool redOwned = false;
    private bool redSelected = false;
    private int redPrice = 0;

    private bool blueOwned = false;
    private bool blueSelected = false;
    private int bluePrice = 100;

    private bool brownOwned = false;
    private bool brownSelected = false;
    private int brownPrice = 100;

    private bool greenOwned = false;
    private bool greenSelected = false;
    private int greenPrice = 100;
    private int coinsBalanse = 0;

    private void Awake()
    {
        userId = GetId();
        StartCoroutine(UpdateBalance());
        StartCoroutine(GetAvailible());
        StartCoroutine(GetCurrentSkin());
        redPriceTxt.text = $"{redPrice} coins";
        bluePriceTxt.text = $"{bluePrice} coins";
        brownPriceTxt.text = $"{brownPrice} coins";
        greenPriceTxt.text = $"{greenPrice} coins";
    }



    private void UpdateUI()
    {
        redBuyButton.gameObject.SetActive(!redOwned);
        blueBuyButton.gameObject.SetActive(!blueOwned);
        greenBuyButton.gameObject.SetActive(!greenOwned);
        brownBuyButton.gameObject.SetActive(!brownOwned);
        redSetButton.gameObject.SetActive(redOwned);
        redSetButton.interactable = !redSelected;
        blueSetButton.gameObject.SetActive(blueOwned);
        blueSetButton.interactable = !blueSelected;
        greenSetButton.gameObject.SetActive(greenOwned);
        greenSetButton.interactable = !greenSelected;
        brownSetButton.gameObject.SetActive(brownOwned);
        brownSetButton.interactable = !brownSelected;
    }

    private IEnumerator UpdateBalance()
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
                coinsBalanse = response.coins;
                coinTxt.text = coinsBalanse.ToString();
            }
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
                switch (response.skin.ballSkinId)
                {
                    case 1:
                        redSelected = true;
                        blueSelected = false;
                        greenSelected = false;
                        brownSelected = false;
                        break;
                    case 2:
                        redSelected = false;
                        blueSelected = true;
                        greenSelected = false;
                        brownSelected = false;
                        break;
                    case 3:
                        redSelected = false;
                        blueSelected = false;
                        greenSelected = false;
                        brownSelected = true;
                        break;
                    case 4:
                        redSelected = false;
                        blueSelected = false;
                        greenSelected = true;
                        brownSelected = false;
                        break;
                }
            }
            UpdateUI();
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
                foreach (BuySkinResponse bs in response.skins)
                {
                    switch (bs.ballSkinId)
                    {
                        case 1:
                            redOwned = true;
                            continue;
                        case 2:
                            blueOwned = true;
                            continue;
                        case 3:
                            brownOwned = true;
                            continue;
                        case 4:
                            greenOwned = true;
                            continue;
                    }
                }
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

    public void SetButton(string sender)
    {
        StartCoroutine(SetSkinRequest(sender));
    }

    private IEnumerator SetSkinRequest(string sender)
    {
        int SkinId = 0;
        switch (sender)
        {
            case "red":
                SkinId = 1; break;
            case "blue":
                SkinId = 2; break;
            case "brown":
                SkinId = 3; break;
            case "green":
                SkinId = 4; break;
            case "pink":
                SkinId = 5; break;
            case "greenBlock":
                SkinId = 6 ; break;
        }
        Debug.Log(sprites[SkinId - 1].name);
        SkinManagerScript.Instance.SetCurrentSkin(sprites[SkinId - 1]);
        var token = AuthManager.Instance.GetToken();
        string url = $"http://localhost:5295/api/BallSkins/SetCurrentSkin/{userId}/{SkinId}";
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var result = JsonUtility.FromJson<CurrentSkinResponse>(request.downloadHandler.text);
                Debug.Log(result.skin.ballSkinId);
                StartCoroutine(GetCurrentSkin());
                UpdateUI();
            }
        }
    }

    public void BuyButton(string sender)
    {
        StartCoroutine(BuySkinRequest(sender));
    }

    private IEnumerator BuySkinRequest(string Sender)
    {
        bool enough = true;
        int SkinId = 0;
        switch (Sender)
        {
            case "red":
                SkinId = 1; break;
            case "blue":
                SkinId = 2;
                if (coinsBalanse < bluePrice)
                {
                    enough = false;
                }
                break;
            case "brown":
                SkinId = 3;
                if (coinsBalanse < brownPrice)
                {
                    enough = false;
                }
                break;
            case "green":
                SkinId = 4;
                if (coinsBalanse < greenPrice)
                {
                    enough = false;
                }
                break;
            case "pink":
                SkinId = 5;
                break;
            case "greenBlock": 
                SkinId = 6;
                break;
        }
        if (enough)
        {
            var token = AuthManager.Instance.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("User is not authenticated");
                yield break;
            }

            if (userId == 0)
            {
                Debug.Log($"Can't load user's ID!");
            }

            string url = $"http://localhost:5295/api/BallSkins/BuySkin/{userId}/{SkinId}";

            using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
            {
                request.SetRequestHeader("Authorization", $"Bearer {token}");
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"Successfully purchased skin: {SkinId}");
                    errorTxt.text = "¿…  –¿—¿¬¿!";
                    errorTxt.color = Color.green;
                    StartCoroutine(UpdateBalance());
                    StartCoroutine(GetAvailible());
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
        else 
        {
            errorTxt.text = "Õ≈ƒŒ—“¿“Œ◊ÕŒ —–≈ƒ—“¬!";
            errorTxt.color = Color.red;
        }
    }

    [System.Serializable]
    public class BalanseResponse
    {
        public int coins;
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
        public object User; // ÃÓÊÌÓ ÓÒÚ‡‚ËÚ¸ Í‡Í object, ËÎË Ò‰ÂÎ‡Ú¸ ÓÚ‰ÂÎ¸Ì˚È ÍÎ‡ÒÒ User
        public int ballSkinId;
        public object BallSkin; // ¿Ì‡ÎÓ„Ë˜ÌÓ
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
}