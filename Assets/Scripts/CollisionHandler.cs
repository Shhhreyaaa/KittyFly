using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 1.5f;
    [SerializeField] float collisionDamage = 25f;

    bool isTransitioning = false;
    bool collisionDisabled = false;

    private void Update()
    {
        RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                LoadNextLevel();
            }
            else if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                collisionDisabled = !collisionDisabled;
                Debug.Log($"[CollisionHandler] Collision Disabled: {collisionDisabled}");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionDisabled) return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
            case "LaunchPad":
                // Safe starting zone
                break;
            case "Finish":
            case "LandingPad":
                StartSuccessSequence();
                break;
            default:
                ProcessHit();
                break;
        }
    }

    private void ProcessHit()
    {
        RocketHealth health = GetComponent<RocketHealth>();
        if (health != null)
        {
            bool isDead = health.TakeDamage(collisionDamage);
            if (isDead)
            {
                StartCrashSequence();
            }
        }
        else
        {
            StartCrashSequence();
        }
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;

        movement mov = GetComponent<movement>();
        if (mov != null) mov.enabled = false;

        GameUI gameUI = FindAnyObjectByType<GameUI>();
        if (gameUI != null)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int totalScenes = SceneManager.sceneCountInBuildSettings;
            if (currentSceneIndex + 1 >= totalScenes && totalScenes > 0)
            {
                gameUI.ShowGameComplete();
                return;
            }
            else
            {
                gameUI.ShowLevelComplete();
            }
        }

        Invoke(nameof(LoadNextLevel), levelLoadDelay);
    }

    private void StartCrashSequence()
    {
        isTransitioning = true;

        movement mov = GetComponent<movement>();
        if (mov != null) mov.enabled = false;

        GameUI gameUI = FindAnyObjectByType<GameUI>();
        if (gameUI != null)
        {
            gameUI.ShowCrash();
        }

        Invoke(nameof(ReloadLevel), levelLoadDelay);
    }

    public void LoadNextLevel()
    {
        string currentName = SceneManager.GetActiveScene().name;
        if (currentName == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (currentName == "Level2")
        {
            SceneManager.LoadScene("Level3");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


