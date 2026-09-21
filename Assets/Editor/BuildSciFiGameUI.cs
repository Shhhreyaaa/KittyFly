using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BuildSciFiGameUI
{
    [MenuItem("Tools/Build Sci-Fi UI For All Scenes")]
    public static void BuildAllScenes()
    {
        string[] scenes = new string[] {
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };

        for (int i = 0; i < scenes.Length; i++)
        {
            string scenePath = scenes[i];
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            BuildUIForActiveScene(i + 1);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[BuildSciFiGameUI] Successfully built Sci-Fi UI for {scenePath}");
        }

        // Return to Level 1
        EditorSceneManager.OpenScene("Assets/Scenes/Level1.unity", OpenSceneMode.Single);
        Debug.Log("[BuildSciFiGameUI] All scenes updated with Sci-Fi UI suite!");
    }

    public static void BuildUIForActiveScene(int sectorNumber)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            var camGo = GameObject.FindWithTag("MainCamera");
            if (camGo != null) mainCam = camGo.GetComponent<Camera>();
        }

        // Clean existing UI Canvas
        GameObject existingCanvas = GameObject.Find("UI Canvas");
        if (existingCanvas != null)
        {
            Object.DestroyImmediate(existingCanvas);
        }

        // Ensure EventSystem
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");

        // 1. Root Canvas
        GameObject canvasObj = new GameObject("UI Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = mainCam;
        canvas.planeDistance = 2.0f;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
        GameUI gameUI = canvasObj.AddComponent<GameUI>();

        // Full-screen Damage Flash Overlay
        GameObject flashObj = CreateUIPanel("DamageFlash", canvasObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(1f, 0.15f, 0.15f, 0f));
        flashObj.GetComponent<Image>().raycastTarget = false;
        gameUI.damageFlashImage = flashObj.GetComponent<Image>();

        // 2. TOP-LEFT: HULL DIAGNOSTICS CARD
        GameObject hullFrame = CreateBorderedCard("HullCard", canvasObj.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -35f), new Vector2(430f, 95f), new Color(0.04f, 0.07f, 0.12f, 0.92f), new Color(0f, 0.94f, 1f, 0.65f), 1.5f);

        CreateUIText("HullTitle", hullFrame.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -14f), new Vector2(160f, 24f), "HULL INTEGRITY", 15, FontStyles.Bold, new Color(0.0f, 0.94f, 1.0f), TextAlignmentOptions.Left, font);

        GameObject sysStatusObj = CreateUIText("SysStatus", hullFrame.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(20f, -14f), new Vector2(140f, 24f), "[SYS: OPTIMAL]", 14, FontStyles.Bold, new Color(0.2f, 0.95f, 0.6f), TextAlignmentOptions.Center, font);
        gameUI.systemStatusText = sysStatusObj.GetComponent<TextMeshProUGUI>();

        GameObject hpNumObj = CreateUIText("HPNumber", hullFrame.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -14f), new Vector2(120f, 24f), "100 / 100 HP", 15, FontStyles.Bold, Color.white, TextAlignmentOptions.Right, font);
        gameUI.healthText = hpNumObj.GetComponent<TextMeshProUGUI>();

        GameObject healthTrack = CreateUIPanel("HealthTrack", hullFrame.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 18f), new Vector2(-40f, 26f), new Color(0.08f, 0.12f, 0.18f, 0.95f));

        GameObject healthFillObj = CreateUIPanel("HealthFill", healthTrack.transform, Vector2.zero, Vector2.one, new Vector2(0f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.12f, 0.92f, 0.65f, 1f));
        Image healthFillImg = healthFillObj.GetComponent<Image>();
        healthFillImg.type = Image.Type.Filled;
        healthFillImg.fillMethod = Image.FillMethod.Horizontal;
        healthFillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFillImg.fillAmount = 1.0f;
        gameUI.healthFillImage = healthFillImg;

        // 3. TOP-CENTER: FLIGHT TELEMETRY CONSOLE
        GameObject telemFrame = CreateBorderedCard("TelemetryConsole", canvasObj.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -35f), new Vector2(460f, 48f), new Color(0.04f, 0.07f, 0.12f, 0.88f), new Color(0f, 0.94f, 1f, 0.45f), 1.5f);

        GameObject velObj = CreateUIText("VelText", telemFrame.transform, new Vector2(0f, 0f), new Vector2(0.33f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, "VEL: 0.0 M/S", 14, FontStyles.Bold, new Color(0.1f, 0.85f, 1.0f), TextAlignmentOptions.Center, font);
        gameUI.velocityText = velObj.GetComponent<TextMeshProUGUI>();

        GameObject altObj = CreateUIText("AltText", telemFrame.transform, new Vector2(0.33f, 0f), new Vector2(0.66f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, "ALT: 1.4 M", 14, FontStyles.Bold, Color.white, TextAlignmentOptions.Center, font);
        gameUI.altitudeText = altObj.GetComponent<TextMeshProUGUI>();

        GameObject thrustObj = CreateUIText("ThrustStatus", telemFrame.transform, new Vector2(0.66f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, "THRUST: IDLE", 14, FontStyles.Bold, new Color(0.6f, 0.7f, 0.85f), TextAlignmentOptions.Center, font);
        gameUI.thrustStatusText = thrustObj.GetComponent<TextMeshProUGUI>();

        // 4. TOP-RIGHT: SECTOR & MISSION CARD + PAUSE BUTTON
        GameObject sectorFrame = CreateBorderedCard("SectorCard", canvasObj.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -35f), new Vector2(430f, 95f), new Color(0.04f, 0.07f, 0.12f, 0.92f), new Color(0f, 0.94f, 1f, 0.65f), 1.5f);

        string sectorTitle = (sectorNumber == 1) ? "SECTOR 01: CANYON FLIGHT" :
                             (sectorNumber == 2) ? "SECTOR 02: REFINERY" :
                             "SECTOR 03: DEFENSE NEXUS";

        string missionObjective = (sectorNumber == 1) ? "PRIMARY: REACH LZ-OMEGA" :
                                  (sectorNumber == 2) ? "PRIMARY: REACH REFINERY PAD" :
                                  "PRIMARY: SECURE EXTRACTION";

        Color titleColor = (sectorNumber == 1) ? new Color(0f, 0.94f, 1f) :
                           (sectorNumber == 2) ? new Color(1.0f, 0.65f, 0.2f) :
                           new Color(0.9f, 0.35f, 1.0f);

        GameObject sectorTitleObj = CreateUIText("SectorTitle", sectorFrame.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(-40f, -14f), new Vector2(-120f, 26f), sectorTitle, 16, FontStyles.Bold, titleColor, TextAlignmentOptions.Left, font);
        gameUI.levelText = sectorTitleObj.GetComponent<TextMeshProUGUI>();

        CreateUIText("ObjectiveText", sectorFrame.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(-40f, 18f), new Vector2(-120f, 24f), missionObjective, 13, FontStyles.Normal, new Color(0.85f, 0.88f, 0.95f), TextAlignmentOptions.Left, font);

        GameObject pauseBtnObj = CreateUIButton("PauseButton", sectorFrame.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-16f, 0f), new Vector2(65f, 55f), "||", 20, new Color(0.1f, 0.22f, 0.35f, 0.9f), new Color(0f, 0.9f, 1f), font);
        gameUI.pauseButton = pauseBtnObj.GetComponent<Button>();

        // 5. BOTTOM-CENTER: CONTROLS GUIDANCE PILL
        GameObject controlsFrame = CreateBorderedCard("ControlsPill", canvasObj.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 30f), new Vector2(540f, 42f), new Color(0.04f, 0.07f, 0.12f, 0.80f), new Color(0f, 0.94f, 1f, 0.40f), 1f);

        GameObject ctrlTextObj = CreateUIText("ControlsText", controlsFrame.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, "▲ [SPACE / W] THRUST   •   ◄ ► [A / D] STABILIZE   •   [ESC] PAUSE", 13, FontStyles.Bold, new Color(0.85f, 0.92f, 1.0f), TextAlignmentOptions.Center, font);
        gameUI.controlsText = ctrlTextObj.GetComponent<TextMeshProUGUI>();

        // 6. MODAL SYSTEM
        SetupSciFiModals(canvasObj, gameUI, font, sectorNumber);

        Canvas.ForceUpdateCanvases();
    }

    private static void SetupSciFiModals(GameObject canvasObj, GameUI gameUI, TMP_FontAsset font, int sectorNumber)
    {
        // 1. PAUSE TERMINAL
        GameObject pauseDimmer, pauseCard;
        CreateModal("PausePanel", canvasObj.transform, new Vector2(520f, 310f), new Color(0.03f, 0.06f, 0.10f, 0.96f), new Color(0f, 0.94f, 1f, 0.85f), out pauseDimmer, out pauseCard);
        
        CreateUIText("PauseHeader", pauseCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(480f, 36f), "// FLIGHT SYSTEMS PAUSED //", 22, FontStyles.Bold, new Color(0f, 0.94f, 1f), TextAlignmentOptions.Center, font);
        CreateUIText("PauseSub", pauseCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -58f), new Vector2(480f, 24f), "SIMULATION FROZEN • READY FOR COMMAND", 13, FontStyles.Normal, Color.white, TextAlignmentOptions.Center, font);

        GameObject resumeBtn = CreateUIButton("ResumeButton", pauseCard.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(280f, 44f), "RESUME FLIGHT", 15, new Color(0.12f, 0.55f, 0.85f), Color.cyan, font);
        GameObject restartBtn = CreateUIButton("RestartButton", pauseCard.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -32f), new Vector2(280f, 44f), "RESTART SECTOR", 15, new Color(0.25f, 0.32f, 0.42f), Color.white, font);
        GameObject abortBtn = CreateUIButton("AbortButton", pauseCard.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -84f), new Vector2(280f, 44f), "ABORT TO SECTOR 1", 15, new Color(0.35f, 0.18f, 0.22f), new Color(1f, 0.6f, 0.6f), font);

        gameUI.pausePanel = pauseDimmer;
        gameUI.resumeButton = resumeBtn.GetComponent<Button>();
        gameUI.pauseRestartButton = restartBtn.GetComponent<Button>();
        gameUI.pauseExitButton = abortBtn.GetComponent<Button>();
        pauseDimmer.SetActive(false);

        // 2. LEVEL COMPLETE DEBRIEF TERMINAL
        GameObject statusDimmer, statusCard;
        CreateModal("StatusPanel", canvasObj.transform, new Vector2(560f, 240f), new Color(0.03f, 0.06f, 0.10f, 0.96f), new Color(0.15f, 0.95f, 0.55f, 0.85f), out statusDimmer, out statusCard);

        GameObject bannerTextObj = CreateUIText("BannerTitle", statusCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -26f), new Vector2(520f, 38f), "// SECTOR OBJECTIVE COMPLETE //", 22, FontStyles.Bold, new Color(0.2f, 0.95f, 0.55f), TextAlignmentOptions.Center, font);
        CreateUIText("DebriefText", statusCard.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 15f), new Vector2(520f, 28f), "LANDING ZONE SECURED • INITIATING HYPER-JUMP", 15, FontStyles.Normal, Color.white, TextAlignmentOptions.Center, font);

        GameObject nextSectorBtn = CreateUIButton("NextSectorButton", statusCard.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 35f), new Vector2(280f, 46f), "PROCEED TO NEXT SECTOR", 15, new Color(0.12f, 0.70f, 0.40f), Color.white, font);

        gameUI.statusPanel = statusDimmer;
        gameUI.statusBannerText = bannerTextObj.GetComponent<TextMeshProUGUI>();
        gameUI.nextLevelButton = nextSectorBtn.GetComponent<Button>();
        statusDimmer.SetActive(false);

        // 3. MISSION FAILED / CRASH TERMINAL
        GameObject crashDimmer, crashCard;
        CreateModal("CrashPanel", canvasObj.transform, new Vector2(560f, 240f), new Color(0.08f, 0.03f, 0.05f, 0.96f), new Color(1f, 0.25f, 0.25f, 0.85f), out crashDimmer, out crashCard);

        CreateUIText("CrashTitle", crashCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -26f), new Vector2(520f, 38f), "[ ! ] HULL FAILURE DETECTED [ ! ]", 22, FontStyles.Bold, new Color(1f, 0.3f, 0.3f), TextAlignmentOptions.Center, font);
        CreateUIText("CrashSub", crashCard.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 15f), new Vector2(520f, 28f), "CRITICAL STRUCTURAL DAMAGE SUSTAINED", 15, FontStyles.Normal, Color.white, TextAlignmentOptions.Center, font);

        GameObject rebootBtn = CreateUIButton("RebootButton", crashCard.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 35f), new Vector2(280f, 46f), "REBOOT SIMULATION", 15, new Color(0.85f, 0.22f, 0.22f), Color.white, font);

        gameUI.crashPanel = crashDimmer;
        gameUI.crashRestartButton = rebootBtn.GetComponent<Button>();
        crashDimmer.SetActive(false);

        // 4. CAMPAIGN VICTORY TERMINAL
        GameObject winDimmer, winCard;
        CreateModal("WinPanel", canvasObj.transform, new Vector2(600f, 300f), new Color(0.03f, 0.06f, 0.10f, 0.96f), new Color(1f, 0.85f, 0.2f, 0.90f), out winDimmer, out winCard);

        CreateUIText("WinTitle", winCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -26f), new Vector2(560f, 40f), "=== CAMPAIGN ACCOMPLISHED ===", 24, FontStyles.Bold, new Color(1f, 0.88f, 0.25f), TextAlignmentOptions.Center, font);
        CreateUIText("WinSub", winCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -66f), new Vector2(560f, 26f), "ALL 3 HOSTILE SECTORS SECURED • PILOT RATING: ACE", 14, FontStyles.Normal, Color.white, TextAlignmentOptions.Center, font);

        CreateUIText("WinStats", winCard.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 12f), new Vector2(560f, 30f), "EXTRACTION COMPLETE • RICK'S POD OPERATIONAL", 14, FontStyles.Bold, new Color(0.3f, 0.85f, 1.0f), TextAlignmentOptions.Center, font);

        GameObject replayBtn = CreateUIButton("ReplayButton", winCard.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 36f), new Vector2(280f, 48f), "PLAY AGAIN", 16, new Color(0.15f, 0.65f, 0.40f), Color.white, font);

        gameUI.winPanel = winDimmer;
        gameUI.restartButton = replayBtn.GetComponent<Button>();
        winDimmer.SetActive(false);
    }

    private static GameObject CreateBorderedCard(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, Color bgColor, Color borderColor, float borderWidth)
    {
        GameObject borderObj = CreateUIPanel(name, parent, anchorMin, anchorMax, pivot, anchoredPos, sizeDelta, borderColor);
        Vector2 innerSizeDelta = new Vector2(-borderWidth * 2f, -borderWidth * 2f);
        CreateUIPanel("Background", borderObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, innerSizeDelta, bgColor);
        return borderObj;
    }

    private static void CreateModal(string name, Transform parent, Vector2 sizeDelta, Color bgColor, Color borderColor, out GameObject dimmerObj, out GameObject cardObj)
    {
        dimmerObj = CreateUIPanel(name, parent, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0f, 0f, 0f, 0.70f));
        cardObj = CreateBorderedCard("TerminalCard", dimmerObj.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, sizeDelta, bgColor, borderColor, 2f);
    }

    private static GameObject CreateUIPanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.SetParent(parent, false);

        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        Image img = panelObj.AddComponent<Image>();
        img.color = color;
        return panelObj;
    }

    private static GameObject CreateUIText(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, string text, float fontSize, FontStyles style, Color color, TextAlignmentOptions align, TMP_FontAsset font)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = align;
        if (font != null) tmp.font = font;

        return textObj;
    }

    private static GameObject CreateUIButton(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, string label, float fontSize, Color bgColor, Color textColor, TMP_FontAsset font)
    {
        GameObject btnObj = CreateUIPanel(name, parent, anchorMin, anchorMax, pivot, anchoredPos, sizeDelta, bgColor);
        Button btn = btnObj.AddComponent<Button>();

        ColorBlock colors = btn.colors;
        colors.normalColor = bgColor;
        colors.highlightedColor = bgColor * 1.3f;
        colors.pressedColor = bgColor * 0.8f;
        btn.colors = colors;

        CreateUIText("Label", btnObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, label, fontSize, FontStyles.Bold, textColor, TextAlignmentOptions.Center, font);

        return btnObj;
    }
}
