using System;
using System.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    private static AuthManager _instance;
    private const string TOKEN_KEY = "auth_token";
    public static AuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AuthManager");
                _instance = go.AddComponent<AuthManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private string _jwtToken;
    public string GetToken() => _jwtToken;

    public bool IsLoggedIn
    {
        get
        {
            if (string.IsNullOrEmpty(_jwtToken)) return false;

            var payload = GetTokenPayload();
            return payload != null && !payload.IsExpired();
        }
    }

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        LoadToken(); // <-- Автоматически загружаем токен из PlayerPrefs
    }
    public void SaveToken()
    {
        if (!string.IsNullOrEmpty(_jwtToken))
        {
            PlayerPrefs.SetString(TOKEN_KEY, _jwtToken);
            PlayerPrefs.Save(); // Не забываем сохранить
        }
    }

    // Загрузить токен
    public void LoadToken()
    {
        if (PlayerPrefs.HasKey(TOKEN_KEY))
        {
            _jwtToken = PlayerPrefs.GetString(TOKEN_KEY);
        }
    }

    // Очистить токен (например, при выходе)
    public void ClearToken()
    {
        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.Save();
        _jwtToken = null;
    }
    // Авторизация
    public IEnumerator Login(string email, string password, System.Action<bool> onResult)
    {
        LoginData loginData = new LoginData() { Username = email, Password = password };
        string jsonBody = JsonUtility.ToJson(loginData);
        string json = JsonUtility.ToJson(loginData);
        Debug.Log($"json:{json}");

        using (UnityWebRequest request = new UnityWebRequest("http://localhost:5295/api/User/login ", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                _jwtToken = response.token;
                SaveToken();
                Debug.Log("Авторизация успешна!");
                onResult(true);
            }
            else
            {
                Debug.LogError("Ошибка авторизации: " + request.error);
                onResult(false);
            }
        }
    }

    public JwtPayload GetTokenPayload()
    {
        if (string.IsNullOrEmpty(_jwtToken)) return null;

        string[] parts = _jwtToken.Split('.');
        if (parts.Length < 2) return null;

        string payloadPart = parts[1];
        payloadPart = Base64UrlDecode(payloadPart);

        try
        {
            return JsonUtility.FromJson<JwtPayload>(payloadPart);
        }
        catch
        {
            Debug.LogError("Не удалось десериализовать payload");
            return null;
        }
    }
    private string Base64UrlDecode(string input)
    {
        string padded = input.Replace('-', '+').Replace('_', '/');
        int padLength = padded.Length % 4;
        if (padLength != 0) padded += new string('=', 4 - padLength);
        byte[] decoded = System.Convert.FromBase64String(padded);
        return System.Text.Encoding.UTF8.GetString(decoded);
    }

    [System.Serializable]
    private class LoginResponse
    {
        public string token;
    }
    [System.Serializable]
    public class LoginData
    {
        public string Username;
        public string Password;
    }
    [System.Serializable]
    public class JwtPayload
    {
        public string unique_name; // Логин пользователя
        public string nameid;      // ID пользователя (string)
        public int exp;            // Unix timestamp: время истечения
        public int nbf;            // Unix timestamp: начало действия
        public bool IsExpired()
        {
            return UnixTimeStampToDateTime(exp) < DateTime.UtcNow;
        }

        private DateTime UnixTimeStampToDateTime(int unixTimestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(unixTimestamp).ToLocalTime();
        }
    }
}