using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TutorialPopup : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;           // The popup background panel
    public TextMeshProUGUI messageText;     // The message text
    public TextMeshProUGUI continueText;    // "Press Space to continue" text
    
    [Header("Popup Settings")]
    public bool pauseGameDuringPopup = true;
    public bool fadeInOut = true;
    public float fadeSpeed = 2f;
    
    private bool isShowingPopup = false;
    private CanvasGroup canvasGroup;
    
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
        
        isShowingPopup = false;
    }
}
