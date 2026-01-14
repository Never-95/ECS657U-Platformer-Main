using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CollectionBook : MonoBehaviour
{
    [Header("UI References")]
    public GameObject collectionBookPanel;  // Main panel
    public TextMeshProUGUI collectionText;  // "X/Y Gold Coins Collected"
    public Transform coinGridContainer;     // Parent for coin slots
    public GameObject coinSlotPrefab;       // Prefab for each coin slot
    
    [Header("Collection Settings")]
    public int totalCoinsInLevel = 10;  // How many coins exist in the level
    public Sprite coinSprite;           // Default gold coin image
    public Color collectedColor = Color.yellow;
    public Color uncollectedColor = Color.gray;
    
    [Header("Book Toggle")]
    public KeyCode toggleKey = KeyCode.Tab;  // Key to open/close book
    
    private HashSet<int> collectedCoins = new HashSet<int>();  // Track collected coin IDs
    private Dictionary<int, Image> coinSlots = new Dictionary<int, Image>();  // UI slots
    private bool isBookOpen = false;
    
    // Singleton
    public static CollectionBook Instance { get; private set; }
    
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
        CreateCoinSlots();
        
        if (collectionBookPanel != null)
        {
            collectionBookPanel.SetActive(false);
        }
        
        UpdateCollectionDisplay();
        
        // Setup close button click - ADD THIS SECTION
        Button closeBtn = collectionBookPanel.GetComponentInChildren<Button>();
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(CloseBook);
            Debug.Log("Close button listener added!");
        }
        else
        {
            Debug.LogWarning("Close button not found!");
        }
    }
    
    void Update()
    {
        // Toggle book with Tab key (or your chosen key)
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleBook();
        }
    }
    
    private void CreateCoinSlots()
    {
        if (coinSlotPrefab == null || coinGridContainer == null) return;
        
        for (int i = 0; i < totalCoinsInLevel; i++)
        {
            // Create slot
            GameObject slot = Instantiate(coinSlotPrefab, coinGridContainer);
            Image coinImage = slot.GetComponent<Image>();
            
            if (coinImage != null)
            {
                coinImage.sprite = coinSprite;
                coinImage.color = uncollectedColor;  // Start gray
                coinSlots.Add(i, coinImage);
            }
            
            // Optional: Add coin number text
            TextMeshProUGUI numberText = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (numberText != null)
            {
                numberText.text = (i + 1).ToString();
            }
        }
    }
    
    public void CollectCoin(int coinID)
    {
        if (collectedCoins.Contains(coinID))
        {
            Debug.Log("Coin " + coinID + " already collected!");
            return;
        }
        
        collectedCoins.Add(coinID);
        Debug.Log("Collected coin " + coinID + "! Total: " + collectedCoins.Count + "/" + totalCoinsInLevel);
        
        // Update UI slot color
        if (coinSlots.ContainsKey(coinID))
        {
            coinSlots[coinID].color = collectedColor;  // Turn yellow
        }
        
        UpdateCollectionDisplay();
    }
    
    private void UpdateCollectionDisplay()
    {
        if (collectionText != null)
        {
            collectionText.text = collectedCoins.Count + "/" + totalCoinsInLevel + " Gold Coins Collected";
        }
    }
    
    public void ToggleBook()
    {
        isBookOpen = !isBookOpen;
        
        if (collectionBookPanel != null)
        {
            collectionBookPanel.SetActive(isBookOpen);
        }
        
        // Pause/unpause game
        Time.timeScale = isBookOpen ? 0f : 1f;
        
        // Show/hide cursor - ADD THIS
        if (isBookOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Disable/enable player controls
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = !isBookOpen;
        }
    }

    public void OpenBook()
    {
        isBookOpen = true;
        if (collectionBookPanel != null)
        {
            collectionBookPanel.SetActive(true);
        }
        
        // Show cursor - ADD THIS
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseBook()
    {
        isBookOpen = false;
        if (collectionBookPanel != null)
        {
            collectionBookPanel.SetActive(false);
        }
        
        Time.timeScale = 1f;
        
        // Hide cursor again - ADD THIS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = true;
        }
    }
        
    // Check if all coins collected
    public bool AllCoinsCollected()
    {
        return collectedCoins.Count >= totalCoinsInLevel;
    }
}
