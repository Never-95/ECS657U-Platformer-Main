using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePopUp : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private Button continueButton;

    [Header("Pause Settings")]
    [SerializeField] private bool pauseTime = true;

    private bool isShowing;

    private void Awake()
    {
        // Safety: if panel isn't assigned, assume this GameObject is the panel.
        if (panel == null)
            panel = gameObject;

        // Hook up button click
        if (continueButton != null)
            continueButton.onClick.AddListener(Hide);

        // Start hidden
        panel.SetActive(false);
        isShowing = false;
    }

    public void Show(string message)
    {
        if (isShowing) return;

        isShowing = true;

        if (popupText != null)
            popupText.text = message;

        panel.SetActive(true);

        // Pause the game
        if (pauseTime)
            Time.timeScale = 0f;

        // Optional: unlock cursor if you want mouse clicking on PC
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        if (!isShowing) return;

        isShowing = false;

        // Unpause the game
        if (pauseTime)
            Time.timeScale = 1f;

        panel.SetActive(false);

        // Optional: relock cursor if your game uses mouse look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Optional: allow keyboard continue
        // Uses unscaled input so it still works while paused
        if (isShowing && Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    private void OnDestroy()
    {
        if (continueButton != null)
            continueButton.onClick.RemoveListener(Hide);

        // Safety: ensure time isn't stuck paused if object is destroyed
        if (Time.timeScale == 0f && pauseTime)
            Time.timeScale = 1f;
    }
}
