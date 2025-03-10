using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance;
    
    [Header("Coin System")]
    public int totalCoins = 0;
    public int coinsCollected = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI coinText;
    
    [Header("Coin Effects")]
    public GameObject coinCollectedEffect;
    public float effectDuration = 1.5f;
    
    [Header("Audio")]
    public AudioSource musicAudioSource;
    public AudioClip backgroundMusic;
    public AudioClip coinCollectedSound;
    
    void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Set up audio source if needed
        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Set up background music
        if (backgroundMusic != null)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
        
        // Count coins in the scene
        CountCoinsInScene();
        
        // Update UI
        UpdateCoinUI();
    }
    
    public void AddCoin(int value = 1)
    {
        // Increase the coin count
        coinsCollected += value;
        
        // Play coin collected sound
        if (coinCollectedSound != null)
        {
            AudioSource.PlayClipAtPoint(coinCollectedSound, Camera.main.transform.position);
        }
        
        // Update UI
        UpdateCoinUI();
        
        // Check if all coins are collected
        CheckAllCoinsCollected();
    }
    
    public void ShowCoinCollectionEffect(Vector3 coinPosition)
    {
        // Show effect if available at the coin's position
        if (coinCollectedEffect != null)
        {
            GameObject effect = Instantiate(coinCollectedEffect, coinPosition, Quaternion.identity);
            
            // Destroy the effect after duration
            Destroy(effect, effectDuration);
        }
    }
    
    void CountCoinsInScene()
    {
        // Find all coins in the scene
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        totalCoins = coins.Length;
        
        Debug.Log("Found " + totalCoins + " coins in the scene");
    }
    
    public void UpdateCoinUI()
    {
        // Update the UI text if available
        if (coinText != null)
        {
            coinText.text = "Coins: " + coinsCollected + " / " + totalCoins;
        }
    }
    
    void CheckAllCoinsCollected()
    {
        if (coinsCollected >= totalCoins)
        {
            Debug.Log("All coins collected!");
            // You can trigger level completion, show a victory screen, etc.
        }
    }
}