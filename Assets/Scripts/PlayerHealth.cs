using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBarSlider;
    
    [Header("Oxygen Settings")]
    public float maxOxygen = 60f;
    public float currentOxygen;
    public float oxygenDepletionRate = 5f;
    public float oxygenDamageRate = 10f;
    public Slider oxygenBarSlider;
    
    [Header("Damage Settings")]
    public float damageAmount = 20f;
    
    private float oxygenDepletionTimer = 0f;
    private bool oxygenDepleted = false;

    public float damageCooldown = 0.5f;
    private float lastDamageTime;
    
    void Start()
    {
        currentHealth = maxHealth;
        currentOxygen = maxOxygen;
        UpdateHealthBar();
        UpdateOxygenBar();
    }
    
    void Update()
    {
        oxygenDepletionTimer += Time.deltaTime;
        
        if (oxygenDepletionTimer >= oxygenDepletionRate)
        {
            currentOxygen -= oxygenDepletionRate;
            oxygenDepletionTimer = 0f;
            
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
            TakeDamage(damageAmount);
        }
    }
    
    public void TakeDamage(float amount)
    {
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            lastDamageTime = Time.time;
            currentHealth -= amount;
            Debug.Log("Player took " + amount + " damage.");
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();
            lastDamageTime = Time.time;
        }

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
        Debug.Log("Player healed by " + amount + " points.");
    }
    
    public void RestoreOxygen(float amount)
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
        StartCoroutine(RespawnDelay());
    }

    private System.Collections.IEnumerator RespawnDelay()
    {
        // Optional: Disable player controls during respawn
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;
        }
        
        yield return new WaitForSeconds(2f);  // 2 second delay
        
        // Re-enable controls
        if (pc != null)
        {
            pc.enabled = true;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}