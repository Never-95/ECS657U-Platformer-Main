using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PerkCoin : MonoBehaviour
{
    public enum PerkType { SpeedBoost, DoubleJump, Invisibility, LevelEnd }
    public PerkType perkType;
    public float effectDuration = 5f;
    public float speedMultiplier = 2f;

    [Header ("Level End Settings")]
    public string nextSceneName = "";
    public float levelEndDelay = 1f;
    public TextMeshProUGUI levelCompleteText;
    public string completionMessage = "Congratulations! Level Complete!";

    private void OnTriggerEnter(Collider other)
    {
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
                }

                Destroy(gameObject, 0.1f);
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