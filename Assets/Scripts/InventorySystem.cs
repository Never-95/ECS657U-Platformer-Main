using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [Header("Coin Counter")]
    public TextMeshProUGUI coinCountText;
    public int totalCoins = 0;
    
    [Header("Active Perks Display")]
    public GameObject perkDisplayPanel;
    public Transform perkContainer; // Parent object for perk icons
    
    [Header("Perk Prefab")]
    public GameObject perkIconPrefab; // Prefab for individual perk display
    
    private Dictionary<string, PerkDisplay> activePerks = new Dictionary<string, PerkDisplay>();
    
    // Singleton pattern for easy access
    public static InventorySystem Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        UpdateCoinDisplay();
    }
    
    // Call this when player collects a gold coin
    public void AddCoin(int amount = 1)
    {
        totalCoins += amount;
        UpdateCoinDisplay();
        Debug.Log("Coins collected: " + totalCoins);
    }
    
    private void UpdateCoinDisplay()
    {
        if (coinCountText != null)
        {
            coinCountText.text = "Coins: " + totalCoins;
        }
    }
    
    // Call this when a perk is activated
    public void AddPerk(string perkName, float duration, Sprite icon = null)
    {
        // If perk already exists, refresh its duration
        if (activePerks.ContainsKey(perkName))
        {
            activePerks[perkName].RefreshDuration(duration);
            return;
        }
        
        // Create new perk display
        if (perkIconPrefab != null && perkContainer != null)
        {
            GameObject perkObj = Instantiate(perkIconPrefab, perkContainer);
            PerkDisplay perkDisplay = perkObj.GetComponent<PerkDisplay>();
            
            if (perkDisplay != null)
            {
                perkDisplay.Initialize(perkName, duration, icon);
                activePerks.Add(perkName, perkDisplay);
                
                // Remove from dictionary when done
                StartCoroutine(RemovePerkAfterDuration(perkName, duration));
            }
        }
    }
    
    private IEnumerator RemovePerkAfterDuration(string perkName, float duration)
    {
        yield return new WaitForSeconds(duration);
        
        if (activePerks.ContainsKey(perkName))
        {
            Destroy(activePerks[perkName].gameObject);
            activePerks.Remove(perkName);
        }
    }
    
    // Get current coin count (useful for other scripts)
    public int GetCoinCount()
    {
        return totalCoins;
    }
}