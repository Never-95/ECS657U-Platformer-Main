using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [TextArea(3, 10)]
    public string tutorialMessage = "Welcome! Use WASD to move.";
    
    [Header("Trigger Settings")]
    public bool triggerOnce = true;      // Only show once
    public float displayDelay = 0.5f;    // Delay before showing popup
    
    private bool hasTriggered = false;
    private TutorialPopup tutorialPopup;
    
    void Start()
    {
        // Find the TutorialPopup in the scene
        tutorialPopup = FindObjectOfType<TutorialPopup>();
        
        if (tutorialPopup == null)
        {
            Debug.LogError("TutorialPopup not found in scene! Please add one.");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (triggerOnce)
            {
                hasTriggered = true;
            }
            
            // Show popup after delay
            StartCoroutine(ShowTutorialAfterDelay());
        }
    }
    
    private System.Collections.IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(displayDelay);
        
        if (tutorialPopup != null)
        {
            tutorialPopup.ShowPopup(tutorialMessage);
        }
    }
    
    // Optional: Draw gizmo to see trigger area in editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}