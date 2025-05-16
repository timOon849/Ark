using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AuthManager;

public class GreetTextManager : MonoBehaviour
{
    public Text txt;
    private JwtPayload token;
    public string username;
    private AuthManager authManager;
    private void Awake()
    {
        authManager = GameObject.Find("AuthManager").GetComponent<AuthManager>();
        token = authManager.GetTokenPayload();
        username = token.unique_name;
        txt.text = $"Добро пожаловать, {username}!";
    }
}
