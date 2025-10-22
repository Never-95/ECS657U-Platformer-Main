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
    public Image healthBar;

    [Header("Oxygen Settings")]
    public float maxOxygen = 60f;
    public float currentOxygen;
    public float oxygenDepletionRate = 5f; 
    public float oxygenDamageRate = 10f;
    public Image oxygenBarFill;

    [Header("Damage Settings")]
    public float damageAmount = 20f;
    private float oxygenDepletionTimer = 0f;
    private bool oxygenDepleted = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentOxygen = maxOxygen;
        UpdateHealthBar();
        UpdateOxygenBar();
    }

    // Update is called once per frame
    void Update()
    {
        oxygenTimer += Time.deltaTime;
        if (oxygenTimer >= oxygenDepletionRate)
        {
            currentOxygen -= oxygenDepletionRate;
            oxygenTimer = 0f;
        
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
            TakeDamage(oxygenDamageAmount * Time.deltaTime);
        }    
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal (float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();
        Debug.log("Player healed by " + amount + " points.");
    }

    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        
        }
    }

    public void UpdateOxygenBar()
    {
        if (oxygenBarFill != null)
        {
            oxygenBarFill.fillAmount = currentOxygen / maxOxygen;
        }
    }

    public void Die()
    {
        Debug.Log("Player has died.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
