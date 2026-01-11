using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TutorialPopup : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI continueText;
    
    [Header("Popup Settings")]
    public bool pauseGameDuringPopup = true;
    public bool fadeInOut = true;
    public float fadeSpeed = 2f;
    
    private bool isShowingPopup = false;
    private CanvasGroup canvasGroup;
    private PlayerController playerController;
    
    void Start()
    {
        // Setup canvas group for fading
        if (popupPanel != null && fadeInOut)
        {
            canvasGroup = popupPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = popupPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // Find player controller
        playerController = FindObjectOfType<PlayerController>();
        
        // Hide popup at start
        HidePopup();
    }
    
    void Update()
    {
        // Check for Space key to close popup (using new Input System)
        if (isShowingPopup && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HidePopup();
        }
    }
    
    public void ShowPopup(string message)
    {
        if (isShowingPopup) return;
        
        StartCoroutine(ShowPopupCoroutine(message));
    }
    
    private IEnumerator ShowPopupCoroutine(string message)
    {
        isShowingPopup = true;
        
        // Set message
        if (messageText != null)
        {
            messageText.text = message;
        }
        
        // IMPORTANT: Clear jump state BEFORE disabling
        if (playerController != null)
        {
            playerController.jumping = false; // Clear any queued jump
        }
        
        // Disable player controls
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Pause game if enabled
        if (pauseGameDuringPopup)
        {
            Time.timeScale = 0f;
        }
        
        // Show popup
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
        
        // Fade in
        if (fadeInOut && canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += fadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
    
    public void HidePopup()
    {
        if (!isShowingPopup) return;
        
        StartCoroutine(HidePopupCoroutine());
    }
    
    private IEnumerator HidePopupCoroutine()
    {
        // Fade out
        if (fadeInOut && canvasGroup != null)
        {
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= fadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }
        }
        
        // Hide popup
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        
        // Resume game
        if (pauseGameDuringPopup)
        {
            Time.timeScale = 1f;
        }
        
        // IMPORTANT: Clear jump state again before re-enabling
        if (playerController != null)
        {
            playerController.jumping = false; // Prevent jump after closing
        }
        
        // Small delay before re-enabling to let input clear
        yield return new WaitForSecondsRealtime(0.1f);
        
        // Re-enable player controls
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        isShowingPopup = false;
    }
}