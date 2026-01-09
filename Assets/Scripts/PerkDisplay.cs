using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PerkDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public Image perkIcon;
    public TextMeshProUGUI perkNameText;
    public TextMeshProUGUI timerText;
    public Image timerBar; // Optional progress bar
    
    private string perkName;
    private float duration;
    private float timeRemaining;
    private bool isActive = false;
    
    public void Initialize(string name, float dur, Sprite icon = null)
    {
        perkName = name;
        duration = dur;
        timeRemaining = dur;
        isActive = true;
        
        // Set UI elements
        if (perkNameText != null)
        {
            perkNameText.text = name;
        }
        
        if (perkIcon != null && icon != null)
        {
            perkIcon.sprite = icon;
        }
        else if (perkIcon != null)
        {
            // Set default color based on perk type
            perkIcon.color = GetPerkColor(name);
        }
        
        StartCoroutine(UpdateTimer());
    }
    
    public void RefreshDuration(float newDuration)
    {
        duration = newDuration;
        timeRemaining = newDuration;
    }
    
    private IEnumerator UpdateTimer()
    {
        while (timeRemaining > 0 && isActive)
        {
            timeRemaining -= Time.deltaTime;
            
            // Update timer text
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timeRemaining).ToString() + "s";
            }
            
            // Update progress bar
            if (timerBar != null)
            {
                timerBar.fillAmount = timeRemaining / duration;
            }
            
            yield return null;
        }
    }
    
    private Color GetPerkColor(string name)
    {
        switch (name.ToLower())
        {
            case "speed boost":
                return new Color(1f, 0.92f, 0.016f); // Yellow
            case "double jump":
                return new Color(0.3f, 1f, 0.3f); // Green
            case "invisibility":
                return new Color(0.7f, 0.7f, 1f); // Light Blue
            default:
                return Color.white;
        }
    }
}