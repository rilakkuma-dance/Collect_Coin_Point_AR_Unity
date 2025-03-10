using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCoinCollector : MonoBehaviour
{
    [Header("AR Settings")]
    public Camera arCamera;
    public float collectionDistance = 1.5f;
    public LayerMask coinLayer;
    
    [Header("Collection Settings")]
    public float collectCooldown = 0.2f;
    public AudioClip coinPickupSound;
    
    private AudioSource audioSource;
    private float lastCollectTime;
    
    void Start()
    {
        // Get the audio source component
        audioSource = GetComponent<AudioSource>();
        
        // If no audio source attached, create one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // If AR camera not set, try to find it
        if (arCamera == null)
        {
            arCamera = GetComponent<Camera>();
            if (arCamera == null)
            {
                arCamera = Camera.main;
                Debug.LogWarning("AR Camera not assigned. Using Main Camera.");
            }
        }
        
        // Make sure coin layer is properly set up
        if (coinLayer.value == 0)
        {
            coinLayer = LayerMask.GetMask("Default");
            Debug.LogWarning("Coin layer not set. Using Default layer.");
        }
    }
    
    void Update()
    {
        // Check for tap input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryCollectCoin();
        }
        
        // For testing in editor with mouse
        if (Input.GetMouseButtonDown(0))
        {
            TryCollectCoin();
        }
    }
    
    void TryCollectCoin()
    {
        // Check cooldown
        if (Time.time - lastCollectTime < collectCooldown)
            return;
            
        Ray ray;
        
        // Create a ray from the camera through the touch/click point
        if (Input.touchCount > 0)
        {
            ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
        }
        else
        {
            ray = arCamera.ScreenPointToRay(Input.mousePosition);
        }
        
        // Cast the ray to check for coins
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, collectionDistance, coinLayer))
        {
            // Check if we hit a coin
            if (hit.collider.CompareTag("Coin"))
            {
                CollectCoin(hit.collider.gameObject);
                lastCollectTime = Time.time;
            }
        }
        
        // Alternative: Check for coins in the center of the screen
        CheckCoinsInFrontOfCamera();
    }
    
    void CheckCoinsInFrontOfCamera()
    {
        // Cast a ray directly from the center of the camera
        Ray centerRay = new Ray(arCamera.transform.position, arCamera.transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(centerRay, 0.5f, collectionDistance, coinLayer);
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Coin"))
            {
                CollectCoin(hit.collider.gameObject);
                lastCollectTime = Time.time;
                break; // Only collect one coin at a time
            }
        }
    }
    
    void CollectCoin(GameObject coinObject)
    {
        // Store the coin's position before destroying it
        Vector3 coinPosition = coinObject.transform.position;
        
        // Get coin value if available
        int coinValue = 1;
        Coin coinComponent = coinObject.GetComponent<Coin>();
        if (coinComponent != null)
        {
            coinValue = coinComponent.value;
        }
        
        // Play pickup sound
        if (coinPickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(coinPickupSound);
        }
        
        // Inform the game manager about the collected coin
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin(coinValue);
            
            // Show the coin collection effect at the coin's position
            GameManager.Instance.ShowCoinCollectionEffect(coinPosition);
        }
        
        // Destroy the coin
        Destroy(coinObject);
    }
}