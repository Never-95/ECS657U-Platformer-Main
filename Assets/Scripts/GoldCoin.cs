using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinID;  // Unique ID for this specific coin
    public int coinValue = 1;
    public bool rotateAnimation = true;
    public float rotationSpeed = 100f;
    
    [Header("Visual")]
    public Sprite coinSprite;  // Optional: custom sprite for this coin
    
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
            // Add to collection book
            if (CollectionBook.Instance != null)
            {
                CollectionBook.Instance.CollectCoin(coinID);
            }
            
            // Also add to old inventory system if you want to keep coin counter
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.AddCoin(coinValue);
            }
            
            // Destroy coin
            Destroy(gameObject);
        }
    }
}