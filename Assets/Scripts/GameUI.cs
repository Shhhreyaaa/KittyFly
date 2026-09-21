using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public TextMeshProUGUI controlsText;
    [SerializeField] public TextMeshProUGUI statusBannerText;
    [SerializeField] public TextMeshProUGUI healthText;
    [SerializeField] public TextMeshProUGUI systemStatusText;

    [Header("Telemetry References")]
    [SerializeField] public TextMeshProUGUI velocityText;
    [SerializeField] public TextMeshProUGUI altitudeText;
    [SerializeField] public TextMeshProUGUI thrustStatusText;

    [Header("Health Bar References")]
    [SerializeField] public Image healthFillImage;
    [SerializeField] public Image damageFlashImage;

    [Header("UI Panels")]
    [SerializeField] public GameObject statusPanel;
    [SerializeField] public GameObject winPanel;
    [SerializeField] public GameObject pausePanel;
    [SerializeField] public GameObject crashPanel;

    [Header("Action Buttons")]
    [SerializeField] public Button restartButton;
    [SerializeField] public Button nextLevelButton;
    [SerializeField] public Button pauseButton;
    [SerializeField] public Button resumeButton;
    [SerializeField] public Button pauseRestartButton;
    [SerializeField] public Button pauseExitButton;
    [SerializeField] public Button crashRestartButton;

    private Rigidbody playerRb;
    private Transform playerTransform;
    private float lastKnownHealth = 100f;
    private float damageFlashAlpha = 0f;
    private bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 1f; // Ensure normal speed on load
        isPaused = false;

        FindPlayerReferences();
        UpdateHUD();

        if (statusPanel != null) statusPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (crashPanel != null) crashPanel.SetActive(false);

        // Bind button listeners
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        if (pauseButton != null) pauseButton.onClick.AddListener(TogglePause);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (pauseRestartButton != null) pauseRestartButton.onClick.AddListener(OnRestartCurrentLevel);
        if (pauseExitButton != null) pauseExitButton.onClick.AddListener(OnReturnToSector1);
        if (crashRestartButton != null) crashRestartButton.onClick.AddListener(OnRestartCurrentLevel);
    }

    private void Update()
    {
        HandleKeyboardShortcuts();
        UpdateTelemetry();
        UpdateDamageFlash();
    }

    private void FindPlayerReferences()
    {
        GameObject rocket = GameObject.Find("Player Rocket");
        if (rocket == null)
        {
            var rh = FindAnyObjectByType<RocketHealth>();
            if (rh != null) rocket = rh.gameObject;
        }

        if (rocket != null)
        {
            playerRb = rocket.GetComponent<Rigidbody>();
            playerTransform = rocket.transform;
        }
    }

    private void HandleKeyboardShortcuts()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }
    }

    private void UpdateTelemetry()
    {
        if (playerRb == null || playerTransform == null)
        {
            FindPlayerReferences();
            return;
        }

        if (velocityText != null)
        {
            float speed = playerRb.linearVelocity.magnitude;
            velocityText.text = $"VEL: {speed:F1} M/S";
        }

        if (altitudeText != null)
        {
            float alt = Mathf.Max(0f, playerTransform.position.y);
            altitudeText.text = $"ALT: {alt:F1} M";
        }

        if (thrustStatusText != null)
        {
            bool isThrusting = false;
            if (Keyboard.current != null)
            {
                isThrusting = Keyboard.current.spaceKey.isPressed ||
                              Keyboard.current.wKey.isPressed ||
                              Keyboard.current.upArrowKey.isPressed;
            }

            if (isThrusting)
            {
                thrustStatusText.text = "THRUST: ACTIVE";
                thrustStatusText.color = new Color(0.1f, 1.0f, 0.5f);
            }
            else
            {
                thrustStatusText.text = "THRUST: IDLE";
                thrustStatusText.color = new Color(0.6f, 0.7f, 0.85f);
            }
        }
    }

    private void UpdateDamageFlash()
    {
        if (damageFlashImage != null)
        {
            if (damageFlashAlpha > 0.001f)
            {
                damageFlashAlpha = Mathf.MoveTowards(damageFlashAlpha, 0f, Time.unscaledDeltaTime * 2.2f);
                Color c = damageFlashImage.color;
                c.a = damageFlashAlpha;
                damageFlashImage.color = c;
            }
            else if (damageFlashImage.color.a > 0f)
            {
                Color c = damageFlashImage.color;
                c.a = 0f;
                damageFlashImage.color = c;
            }
        }
    }

    public void UpdateHUD()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int activeIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = Mathf.Max(3, SceneManager.sceneCountInBuildSettings);

        if (levelText != null)
        {
            string sectorLabel = (activeIndex == 0) ? "SECTOR 01: CANYON FLIGHT" :
                                 (activeIndex == 1) ? "SECTOR 02: REFINERY" :
                                 "SECTOR 03: DEFENSE NEXUS";
            levelText.text = sectorLabel;
        }

        if (controlsText != null)
        {
            controlsText.text = "▲ [SPACE / W] THRUST   •   ◄ ► [A / D] STABILIZE";
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float ratio = Mathf.Clamp01(currentHealth / maxHealth);

        // Check if health dropped to trigger holographic red damage flash
        if (currentHealth < lastKnownHealth)
        {
            damageFlashAlpha = 0.45f;
        }
        lastKnownHealth = currentHealth;

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = ratio;
            if (ratio > 0.5f) healthFillImage.color = new Color(0.1f, 0.9f, 0.65f);
            else if (ratio > 0.25f) healthFillImage.color = new Color(1.0f, 0.65f, 0.1f);
            else healthFillImage.color = new Color(1.0f, 0.2f, 0.2f);
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)} HP";
        }

        if (systemStatusText != null)
        {
            if (ratio > 0.5f)
            {
                systemStatusText.text = "[SYS: OPTIMAL]";
                systemStatusText.color = new Color(0.2f, 0.95f, 0.6f);
            }
            else if (ratio > 0.25f)
            {
                systemStatusText.text = "[SYS: CAUTION]";
                systemStatusText.color = new Color(1.0f, 0.75f, 0.15f);
            }
            else
            {
                systemStatusText.text = "[SYS: CRITICAL]";
                systemStatusText.color = new Color(1.0f, 0.25f, 0.25f);
            }
        }
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void ShowLevelComplete()
    {
        if (statusPanel != null) statusPanel.SetActive(true);
        if (statusBannerText != null)
        {
            statusBannerText.color = new Color(0.2f, 0.95f, 0.5f);
            statusBannerText.text = "// SECTOR OBJECTIVE COMPLETE //";
        }
    }

    public void ShowCrash()
    {
        if (crashPanel != null)
        {
            crashPanel.SetActive(true);
        }
        else if (statusPanel != null)
        {
            statusPanel.SetActive(true);
            if (statusBannerText != null)
            {
                statusBannerText.color = new Color(1f, 0.25f, 0.25f);
                statusBannerText.text = "[ ! ] HULL INTEGRITY LOST! [ ! ]";
            }
        }
    }

    public void ShowGameComplete()
    {
        if (winPanel != null) winPanel.SetActive(true);
        if (statusPanel != null) statusPanel.SetActive(false);
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    private void OnRestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnReturnToSector1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnNextLevelClicked()
    {
        Time.timeScale = 1f;
        CollisionHandler handler = FindAnyObjectByType<CollisionHandler>();
        if (handler != null)
        {
            handler.LoadNextLevel();
        }
        else
        {
            string current = SceneManager.GetActiveScene().name;
            if (current == "Level1") SceneManager.LoadScene("Level2");
            else if (current == "Level2") SceneManager.LoadScene("Level3");
            else SceneManager.LoadScene("MainMenu");
        }
    }
}
