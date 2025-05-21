using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManagerScript : MonoBehaviour
{
    public static SkinManagerScript Instance { get; private set; }

    [SerializeField] private GameObject currentBall;
    private SpriteRenderer currentSpriteRenderer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        currentSpriteRenderer = currentBall.GetComponent<SpriteRenderer>();
    }

    public void SetCurrentSkin(Sprite skin)
    {
        if (currentBall != null)
        {
            currentSpriteRenderer.sprite = skin;
            Debug.Log($"new sprite: {currentSpriteRenderer.sprite.name}");
        }
    }
}
