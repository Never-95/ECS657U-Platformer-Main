using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;
    public bool rotateAnimation = true;
    public float rotationSpeed = 100f;
    
    [Header("Effects")]
    public AudioClip collectSound;
    public ParticleSystem collectEffect;
    
    void Update()
    {
        if (rotateAnimation)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add to inventory
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.AddCoin(coinValue);
            }
            
            // Play effects
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }
            
            // Destroy coin
            Destroy(gameObject);
        }
    }
}