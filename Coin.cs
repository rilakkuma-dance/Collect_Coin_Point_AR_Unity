using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public float rotationSpeed = 100f;
    public bool enableBobbing = true;
    public float bobSpeed = 1f;
    public float bobHeight = 0.2f;
    
    [Header("Coin Value")]
    public int value = 1;
    
    [Header("AR Settings")]
    public int coinLayer = 8; // Default to layer 8 (can be set to whatever you use for coins)
    
    // Position variables
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;
    private Collider coinCollider;
    
    void Awake()
    {
        // Store original transform data
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        
        // Cache components
        rb = GetComponent<Rigidbody>();
        coinCollider = GetComponent<Collider>();
        
        // Make sure this object has the "Coin" tag
        gameObject.tag = "Coin";
        
        // Set the proper layer for AR detection
        gameObject.layer = coinLayer;
        
        // Make sure we have a collider for raycasting
        if (coinCollider == null)
        {
            // If no collider, add a sphere collider
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.5f;
            sphereCollider.isTrigger = true;
            coinCollider = sphereCollider;
        }
        
        // Disable physics completely for AR
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.freezeRotation = true;
        }
    }
    
    void Start()
    {
        // Reset position to be sure
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
    
    void Update()
    {
        // Rotate the coin around its Y axis
        //transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Apply bobbing motion if enabled
        if (enableBobbing)
        {
            float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            
            // Keep X and Z positions fixed, only modify Y
            transform.position = new Vector3(
                originalPosition.x, 
                originalPosition.y + yOffset, 
                originalPosition.z
            );
        }
        else
        {
            // Ensure coin stays at original position
            transform.position = new Vector3(
                originalPosition.x,
                originalPosition.y,
                originalPosition.z
            );
        }
    }
    
    // If you need to reposition the coin, call this to update the base position
    public void UpdateBasePosition(Vector3 newPosition)
    {
        originalPosition = newPosition;
    }
}