using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthBarSlider;
    
    [Header("Oxygen Settings")]
    public float maxOxygen = 60f;
    public float currentOxygen;
    public float oxygenDepletionRate = 5f;
    public float oxygenDamageRate = 10f;
    public Slider oxygenBarSlider;
    
    [Header("Damage Settings")]
    public float damageAmount = 20f;
    
    private float oxygenDepletionTimer = 0f;  // You named it this
    private bool oxygenDepleted = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        currentOxygen = maxOxygen;
        UpdateHealthBar();
        UpdateOxygenBar();
    }
    
    void Update()
    {
        oxygenDepletionTimer += Time.deltaTime;  // FIXED: was "oxygenTimer"
        
        if (oxygenDepletionTimer >= oxygenDepletionRate)
        {
            currentOxygen -= oxygenDepletionRate;
            oxygenDepletionTimer = 0f;  // FIXED: was "oxygenTimer"
            
            if (currentOxygen <= 0)
            {
                currentOxygen = 0;
                if (!oxygenDepleted)
                {
                    oxygenDepleted = true;
                    Debug.Log("Oxygen depleted! Starting to take damage.");
                }
            }
            UpdateOxygenBar();
        }
        
        if (oxygenDepleted)
        {
            TakeDamage(oxygenDamageRate * Time.deltaTime);  // FIXED: was "oxygenDamageAmount"
        }
    }
    
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;  // FIXED: was "damage"
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();
        Debug.Log("Player healed by " + amount + " points.");  // FIXED: capital L in Log
    }
    
    public void RestoreOxygen(float amount)  // ADD THIS METHOD
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
        
        if (currentOxygen > 0f)
        {
            oxygenDepleted = false;
        }
        
        UpdateOxygenBar();
        Debug.Log("Oxygen restored by " + amount + " seconds.");
    }
    
    public void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth / maxHealth;
        }
    }
    
    public void UpdateOxygenBar()
    {
        if (oxygenBarSlider != null)
        {
            oxygenBarSlider.value = currentOxygen / maxOxygen;
        }
    }
    
    public void Die()
    {
        Debug.Log("Player has died.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}