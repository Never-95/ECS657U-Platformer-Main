using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PerkCoin : MonoBehaviour
{
    public enum PerkType { SpeedBoost, DoubleJump, Invisibility, LevelEnd, HealthRestore, OxygenRestore }
    public PerkType perkType;
    public float effectDuration = 5f;
    public float speedMultiplier = 2f;

    [Header ("Level End Settings")]
    public string nextSceneName = "";
    public float levelEndDelay = 1f;
    public TextMeshProUGUI levelCompleteText;
    public string completionMessage = "Congratulations! Level Complete!";

    [Header ("Health/Oxygen Restore Settings")]
    public float restoreAmount = 30f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered coin");
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                switch (perkType)
                {
                    case PerkType.SpeedBoost:
                        player.ActivateSpeedBoost(speedMultiplier, effectDuration);
                        break;
                    case PerkType.DoubleJump:
                        player.EnableDoubleJump(effectDuration);
                        break;
                    case PerkType.Invisibility:
                        player.ActivateInvisibility(effectDuration);
                        break;
                    case PerkType.LevelEnd:
                        StartCoroutine(EndLevelProcess());
                        return;
                    case PerkType.HealthRestore:
                        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.Heal(restoreAmount);
                        }
                        break;
                    case PerkType.OxygenRestore:
                        PlayerHealth playerOxygen = player.GetComponent<PlayerHealth>();
                        if (playerOxygen != null)
                        {
                            playerOxygen.RestoreOxygen(restoreAmount);
                        }
                        break;
                }

                //Call CoinController script within parent object to temporarily deactivate coin
                transform.parent.gameObject.GetComponent<CoinController>().DeactivateCoin(this.gameObject, effectDuration);


                //Destroy(gameObject, 0.1f);
            }
        }
    }


    private System.Collections.IEnumerator EndLevelProcess()
    {
        Debug.Log("Level Complete!");
        Destroy(gameObject);

        if (levelCompleteText != null)
        {
            levelCompleteText.gameObject.SetActive(true);
            levelCompleteText.text = completionMessage;
        }

        yield return new WaitForSeconds(levelEndDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}