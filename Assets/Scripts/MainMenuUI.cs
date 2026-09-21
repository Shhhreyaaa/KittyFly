using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] public GameObject mainPanel;
    [SerializeField] public GameObject sectorSelectPanel;
    [SerializeField] public GameObject manualPanel;
    [SerializeField] public GameObject settingsPanel;

    [Header("Main Navigation Buttons")]
    [SerializeField] public Button launchButton;
    [SerializeField] public Button sectorSelectButton;
    [SerializeField] public Button manualButton;
    [SerializeField] public Button settingsButton;
    [SerializeField] public Button quitButton;

    [Header("Sector Select Buttons")]
    [SerializeField] public Button sector1Button;
    [SerializeField] public Button sector2Button;
    [SerializeField] public Button sector3Button;
    [SerializeField] public Button sectorBackButton;

    [Header("Back Buttons")]
    [SerializeField] public Button manualBackButton;
    [SerializeField] public Button settingsBackButton;

    [Header("Settings Controls")]
    [SerializeField] public Slider volumeSlider;
    [SerializeField] public TextMeshProUGUI volumeValueText;
    [SerializeField] public Toggle fullscreenToggle;

    [Header("Intel Preview")]
    [SerializeField] public TextMeshProUGUI intelTitleText;
    [SerializeField] public TextMeshProUGUI intelDescText;
    [SerializeField] public TextMeshProUGUI intelThreatText;

    private void Start()
    {
        Time.timeScale = 1f;

        // Ensure main panel active, others closed
        ShowMainPanel();

        // Bind main buttons
        if (launchButton != null) launchButton.onClick.AddListener(OnLaunchClicked);
        if (sectorSelectButton != null) sectorSelectButton.onClick.AddListener(ShowSectorSelect);
        if (manualButton != null) manualButton.onClick.AddListener(ShowManual);
        if (settingsButton != null) settingsButton.onClick.AddListener(ShowSettings);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);

        // Bind sector select buttons
        if (sector1Button != null) sector1Button.onClick.AddListener(() => LoadSector("Level1"));
        if (sector2Button != null) sector2Button.onClick.AddListener(() => LoadSector("Level2"));
        if (sector3Button != null) sector3Button.onClick.AddListener(() => LoadSector("Level3"));
        if (sectorBackButton != null) sectorBackButton.onClick.AddListener(ShowMainPanel);

        // Bind back buttons
        if (manualBackButton != null) manualBackButton.onClick.AddListener(ShowMainPanel);
        if (settingsBackButton != null) settingsBackButton.onClick.AddListener(ShowMainPanel);

        // Bind settings
        if (volumeSlider != null)
        {
            float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            volumeSlider.value = savedVol;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            UpdateVolumeLabel(savedVol);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        }
    }

    public void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (sectorSelectPanel != null) sectorSelectPanel.SetActive(false);
        if (manualPanel != null) manualPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        SetIntelDefault();
    }

    public void ShowSectorSelect()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (sectorSelectPanel != null) sectorSelectPanel.SetActive(true);
        if (manualPanel != null) manualPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowManual()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (sectorSelectPanel != null) sectorSelectPanel.SetActive(false);
        if (manualPanel != null) manualPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (sectorSelectPanel != null) sectorSelectPanel.SetActive(false);
        if (manualPanel != null) manualPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    private void OnLaunchClicked()
    {
        LoadSector("Level1");
    }

    public void LoadSector(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnVolumeChanged(float val)
    {
        PlayerPrefs.SetFloat("MasterVolume", val);
        AudioListener.volume = val;
        UpdateVolumeLabel(val);
    }

    private void UpdateVolumeLabel(float val)
    {
        if (volumeValueText != null)
        {
            volumeValueText.text = $"{Mathf.RoundToInt(val * 100)}%";
        }
    }

    private void OnFullscreenToggled(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    public void SetSectorIntel(int sectorNum)
    {
        if (intelTitleText == null || intelDescText == null || intelThreatText == null) return;

        switch (sectorNum)
        {
            case 1:
                intelTitleText.text = "SECTOR 01: CANYON FLIGHT";
                intelDescText.text = "Atmospheric entry valley. Static outpost towers and wide 11m vertical flight gate. Primary objective: Reach Landing Pad LZ-Omega.";
                intelThreatText.text = "THREAT LEVEL: LOW [STANDARD]";
                intelThreatText.color = new Color(0.2f, 0.95f, 0.55f);
                break;
            case 2:
                intelTitleText.text = "SECTOR 02: INDUSTRIAL REFINERY";
                intelDescText.text = "Elevated ridge terrain and deep pipeline trench. Moving overhead cargo crane oscillating horizontally. Nano-repair pickup available.";
                intelThreatText.text = "THREAT LEVEL: MEDIUM [ELEVATED]";
                intelThreatText.color = new Color(1.0f, 0.75f, 0.15f);
                break;
            case 3:
                intelTitleText.text = "SECTOR 03: DEFENSE NEXUS";
                intelDescText.text = "Orbital defense perimeter. Counter-phase synchronized pistons and a 360-degree rotating generator turbine hazard. Critical pilot reflexes required.";
                intelThreatText.text = "THREAT LEVEL: CRITICAL [DANGER]";
                intelThreatText.color = new Color(1.0f, 0.25f, 0.25f);
                break;
        }
    }

    private void SetIntelDefault()
    {
        if (intelTitleText == null || intelDescText == null || intelThreatText == null) return;

        intelTitleText.text = "CAMPAIGN DIRECTIVE: PROJECT KITTYFLY";
        intelDescText.text = "Pilot Rick's high-maneuverability prototype space pod across 3 planetary sectors. Master vertical thrust vectoring, maintain hull integrity, and reach extraction.";
        intelThreatText.text = "SYSTEM STATUS: ALL SYSTEMS NOMINAL";
        intelThreatText.color = new Color(0f, 0.94f, 1f);
    }
}
