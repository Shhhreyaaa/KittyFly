using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BuildSciFiMainMenu
{
    private const string SCENE_PATH = "Assets/Scenes/MainMenu.unity";

    [MenuItem("Tools/Build Sci-Fi Main Menu Scene")]
    public static void BuildScene()
    {
        // 1. Create or open scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 2. Setup Environment Lighting & Sky
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.06f, 0.10f, 0.18f);

        // 3. Camera
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        camObj.AddComponent<AudioListener>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.015f, 0.03f, 0.06f);
        cam.fieldOfView = 52f;
        camObj.transform.position = new Vector3(0f, 2.6f, -9.2f);
        camObj.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
        camObj.tag = "MainCamera";

        // 4. Directional Lights
        GameObject lightObj = new GameObject("Main Rim Light");
        Light mainLight = lightObj.AddComponent<Light>();
        mainLight.type = LightType.Directional;
        mainLight.color = new Color(0.65f, 0.90f, 1f);
        mainLight.intensity = 1.1f;
        lightObj.transform.rotation = Quaternion.Euler(40f, -35f, 0f);

        GameObject fillLightObj = new GameObject("Warm Key Light");
        Light fillLight = fillLightObj.AddComponent<Light>();
        fillLight.type = LightType.Directional;
        fillLight.color = new Color(1.0f, 0.70f, 0.50f);
        fillLight.intensity = 0.55f;
        fillLightObj.transform.rotation = Quaternion.Euler(25f, 145f, 0f);

        // 5. 3D Showcase Diorama
        BuildDiorama();

        // 6. UI Canvas
        BuildMenuUI(cam);

        // 7. Save Scene
        EditorSceneManager.SaveScene(scene, SCENE_PATH);
        Debug.Log($"[BuildSciFiMainMenu] Saved MainMenu scene at {SCENE_PATH}");

        // 8. Update Build Settings
        UpdateBuildSettings();
    }

    private static void BuildDiorama()
    {
        GameObject diorama = new GameObject("Diorama");

        // Pedestal base
        GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pad.name = "Showcase_Pad";
        pad.transform.SetParent(diorama.transform);
        pad.transform.position = new Vector3(0f, 0.15f, 0f);
        pad.transform.localScale = new Vector3(4.0f, 0.25f, 4.0f);
        Material padMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/GameDevTV Assets/Materials and Textures/SciFi_LaunchPad_Blue.mat");
        if (padMat != null) pad.GetComponent<Renderer>().material = padMat;

        // Glowing outer ring
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "Pad_HaloRing";
        ring.transform.SetParent(pad.transform);
        ring.transform.localPosition = new Vector3(0f, 0.06f, 0f);
        ring.transform.localScale = new Vector3(1.06f, 0.15f, 1.06f);
        Material ringMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/GameDevTV Assets/Materials and Textures/SciFi_Gate_Cyan.mat");
        if (ringMat != null) ring.GetComponent<Renderer>().material = ringMat;

        // 4 Corner Landing Beacon Pylons
        Vector3[] beaconPositions = new Vector3[] {
            new Vector3(-2.2f, 0.15f, -2.2f),
            new Vector3(2.2f, 0.15f, -2.2f),
            new Vector3(-2.2f, 0.15f, 2.2f),
            new Vector3(2.2f, 0.15f, 2.2f)
        };

        Material beaconMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/GameDevTV Assets/Materials and Textures/SciFi_Beacon_Amber.mat");
        for (int i = 0; i < beaconPositions.Length; i++)
        {
            GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            b.name = $"Beacon_{i + 1}";
            b.transform.SetParent(diorama.transform);
            b.transform.position = beaconPositions[i] + new Vector3(0f, 0.45f, 0f);
            b.transform.localScale = new Vector3(0.18f, 0.45f, 0.18f);
            if (beaconMat != null) b.GetComponent<Renderer>().material = beaconMat;
        }

        // Showcase Spaceship
        GameObject shipPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameDevTV Assets/Prefabs/Rick's Weird Space Pod.prefab");
        if (shipPrefab != null)
        {
            GameObject ship = (GameObject)PrefabUtility.InstantiatePrefab(shipPrefab);
            ship.name = "Showcase_Ship";
            ship.transform.SetParent(diorama.transform);
            ship.transform.position = new Vector3(0f, 1.85f, 0f);
            ship.transform.rotation = Quaternion.Euler(0f, 205f, 0f);

            // Add smooth hover animation
            MenuHoverShip hover = ship.AddComponent<MenuHoverShip>();
            hover.bobSpeed = 1.6f;
            hover.bobHeight = 0.12f;
            hover.yawSpeed = 12f;

            // Idle Thruster Light
            GameObject lightObj = new GameObject("IdleThrusterLight");
            lightObj.transform.SetParent(ship.transform);
            lightObj.transform.localPosition = new Vector3(0.08f, 0.05f, -0.17f);
            Light tLight = lightObj.AddComponent<Light>();
            tLight.type = LightType.Point;
            tLight.color = new Color(0f, 0.95f, 1f);
            tLight.intensity = 3.2f;
            tLight.range = 5.0f;
        }

        // Atmospheric backdrop terrain
        Material bedrockMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/GameDevTV Assets/Materials and Textures/SciFi_Martian_Bedrock.mat");
        for (int i = -4; i <= 4; i++)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rock.name = $"Backdrop_Mountain_{i}";
            rock.transform.SetParent(diorama.transform);
            float dist = Mathf.Abs(i);
            rock.transform.position = new Vector3(i * 3.8f, -1.2f, 8.5f + dist * 1.2f);
            rock.transform.localScale = new Vector3(4.8f, 3.5f + (4 - dist) * 0.9f, 3.5f);
            rock.transform.rotation = Quaternion.Euler(dist * 2f, i * 14f, dist * 3f);
            if (bedrockMat != null) rock.GetComponent<Renderer>().material = bedrockMat;
        }

        // Ambient floating starfield / particles
        GameObject starfieldObj = new GameObject("AmbientStarfield");
        starfieldObj.transform.SetParent(diorama.transform);
        starfieldObj.transform.position = new Vector3(0f, 3f, 0f);
        ParticleSystem ps = starfieldObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 8f;
        main.startSpeed = 0.25f;
        main.startSize = 0.08f;
        main.startColor = new Color(0.5f, 0.85f, 1f, 0.45f);
        main.maxParticles = 80;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(14f, 6f, 12f);

        var em = ps.emission;
        em.rateOverTime = 10f;
    }

    private static void BuildMenuUI(Camera cam)
    {
        // EventSystem
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");

        // Root Canvas
        GameObject canvasObj = new GameObject("Menu Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 2.0f;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
        MainMenuUI menuUI = canvasObj.AddComponent<MainMenuUI>();

        // 1. TOP HEADER BRANDING
        GameObject headerCard = CreateBorderedCard("HeaderCard", canvasObj.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(-80f, 85f), new Color(0.03f, 0.06f, 0.10f, 0.92f), new Color(0f, 0.94f, 1f, 0.75f), 1.5f);

        CreateUIText("TitleText", headerCard.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(25f, 8f), new Vector2(400f, 40f), "KITTYFLY", 32, FontStyles.Bold, new Color(0f, 0.95f, 1f), TextAlignmentOptions.Left, font);
        CreateUIText("SubTitle", headerCard.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(25f, -20f), new Vector2(500f, 22f), "// ORBITAL FLIGHT PROTOCOL //   •   TACTICAL SIMULATOR", 13, FontStyles.Normal, new Color(0.8f, 0.9f, 1f), TextAlignmentOptions.Left, font);

        CreateUIText("StatusBadge", headerCard.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-25f, 0f), new Vector2(300f, 30f), "[TERMINAL: ONLINE]  SYS VER 2.4.0", 14, FontStyles.Bold, new Color(0.2f, 0.95f, 0.55f), TextAlignmentOptions.Right, font);

        // 2. MAIN PANEL
        GameObject mainPanel = CreateUIPanel("MainPanel", canvasObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.clear);
        menuUI.mainPanel = mainPanel;

        // 2A. LEFT NAVIGATION DECK
        GameObject navCard = CreateBorderedCard("NavCard", mainPanel.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(40f, -25f), new Vector2(400f, 460f), new Color(0.04f, 0.07f, 0.12f, 0.94f), new Color(0f, 0.94f, 1f, 0.65f), 1.5f);

        CreateUIText("NavHeader", navCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(360f, 28f), "// FLIGHT OPERATIONS //", 16, FontStyles.Bold, new Color(0f, 0.94f, 1f), TextAlignmentOptions.Center, font);

        GameObject launchBtn = CreateUIButton("LaunchBtn", navCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -75f), new Vector2(340f, 55f), "►  LAUNCH CAMPAIGN", 17, new Color(0.12f, 0.65f, 0.95f), Color.white, font);
        GameObject sectorBtn = CreateUIButton("SectorBtn", navCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -145f), new Vector2(340f, 50f), "[ = ]  SECTOR ARCHIVE", 15, new Color(0.18f, 0.28f, 0.40f), Color.white, font);
        GameObject manualBtn = CreateUIButton("ManualBtn", navCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -210f), new Vector2(340f, 50f), "[ ? ]  FLIGHT MANUAL", 15, new Color(0.18f, 0.28f, 0.40f), Color.white, font);
        GameObject settingsBtn = CreateUIButton("SettingsBtn", navCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -275f), new Vector2(340f, 50f), "[ # ]  SYSTEMS CONFIG", 15, new Color(0.18f, 0.28f, 0.40f), Color.white, font);
        GameObject quitBtn = CreateUIButton("QuitBtn", navCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -345f), new Vector2(340f, 48f), "[ X ]  ABORT PROTOCOL", 15, new Color(0.40f, 0.16f, 0.18f), new Color(1f, 0.6f, 0.6f), font);

        menuUI.launchButton = launchBtn.GetComponent<Button>();
        menuUI.sectorSelectButton = sectorBtn.GetComponent<Button>();
        menuUI.manualButton = manualBtn.GetComponent<Button>();
        menuUI.settingsButton = settingsBtn.GetComponent<Button>();
        menuUI.quitButton = quitBtn.GetComponent<Button>();

        // 2B. RIGHT INTEL / TELEMETRY CARD
        GameObject intelCard = CreateBorderedCard("IntelCard", mainPanel.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-40f, -25f), new Vector2(460f, 460f), new Color(0.04f, 0.07f, 0.12f, 0.94f), new Color(0f, 0.94f, 1f, 0.65f), 1.5f);

        CreateUIText("IntelHeader", intelCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(410f, 28f), "// CAMPAIGN DIRECTIVE //", 16, FontStyles.Bold, new Color(0f, 0.94f, 1f), TextAlignmentOptions.Left, font);

        GameObject intelTitleObj = CreateUIText("IntelTitle", intelCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -60f), new Vector2(410f, 32f), "CAMPAIGN DIRECTIVE: PROJECT KITTYFLY", 15, FontStyles.Bold, Color.white, TextAlignmentOptions.Left, font);
        menuUI.intelTitleText = intelTitleObj.GetComponent<TextMeshProUGUI>();

        GameObject intelThreatObj = CreateUIText("IntelThreat", intelCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -95f), new Vector2(410f, 24f), "SYSTEM STATUS: ALL SYSTEMS NOMINAL", 13, FontStyles.Bold, new Color(0f, 0.94f, 1f), TextAlignmentOptions.Left, font);
        menuUI.intelThreatText = intelThreatObj.GetComponent<TextMeshProUGUI>();

        GameObject intelDescObj = CreateUIText("IntelDesc", intelCard.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -190f), new Vector2(410f, 140f), "Pilot Rick's high-maneuverability prototype space pod across 3 planetary sectors. Master vertical thrust vectoring, maintain hull integrity, avoid kinetic hazards, and achieve touchdown extraction.", 14, FontStyles.Normal, new Color(0.85f, 0.90f, 0.98f), TextAlignmentOptions.Left, font);
        menuUI.intelDescText = intelDescObj.GetComponent<TextMeshProUGUI>();

        // Sector summary pill inside Intel Card
        GameObject sectorPill = CreateBorderedCard("SectorSummaryPill", intelCard.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 30f), new Vector2(410f, 80f), new Color(0.06f, 0.10f, 0.16f, 0.9f), new Color(0f, 0.94f, 1f, 0.4f), 1f);
        CreateUIText("PillText", sectorPill.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, "SECTOR 01: CANYON FLIGHT  [READY]\nSECTOR 02: REFINERY       [READY]\nSECTOR 03: DEFENSE NEXUS  [READY]", 12, FontStyles.Normal, new Color(0.25f, 0.95f, 0.65f), TextAlignmentOptions.Center, font);

        // 3. SECTOR SELECT PANEL
        GameObject sectorSelectPanel = CreateModalPanel("SectorSelectPanel", canvasObj.transform, new Vector2(800f, 520f), "// SECTOR ARCHIVE //", font);
        menuUI.sectorSelectPanel = sectorSelectPanel;
        Transform sCard = sectorSelectPanel.transform.Find("TerminalCard");

        GameObject s1Btn = CreateSectorRow("Sector1Row", sCard, new Vector2(0f, 110f), "SECTOR 01: CANYON FLIGHT", "Atmospheric introductory valley • Outpost Towers • 11m Gate", "DEPLOY SECTOR 01", new Color(0.12f, 0.55f, 0.85f), font);
        GameObject s2Btn = CreateSectorRow("Sector2Row", sCard, new Vector2(0f, 10f), "SECTOR 02: INDUSTRIAL REFINERY", "Non-linear ridge terrain • Moving Cargo Crane • Nano-Repair Bay", "DEPLOY SECTOR 02", new Color(0.85f, 0.55f, 0.12f), font);
        GameObject s3Btn = CreateSectorRow("Sector3Row", sCard, new Vector2(0f, -90f), "SECTOR 03: DEFENSE NEXUS", "Orbital gauntlet • Synchronized Pistons • 360° Rotating Turbine", "DEPLOY SECTOR 03", new Color(0.75f, 0.25f, 0.85f), font);

        GameObject sBackBtn = CreateUIButton("SectorBackBtn", sCard, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(280f, 44f), "◄  RETURN TO MENU", 15, new Color(0.25f, 0.32f, 0.42f), Color.white, font);

        menuUI.sector1Button = s1Btn.GetComponent<Button>();
        menuUI.sector2Button = s2Btn.GetComponent<Button>();
        menuUI.sector3Button = s3Btn.GetComponent<Button>();
        menuUI.sectorBackButton = sBackBtn.GetComponent<Button>();
        sectorSelectPanel.SetActive(false);

        // 4. FLIGHT MANUAL PANEL
        GameObject manualPanel = CreateModalPanel("ManualPanel", canvasObj.transform, new Vector2(820f, 540f), "// FLIGHT MANUAL & SYSTEMS SPECIFICATION //", font);
        menuUI.manualPanel = manualPanel;
        Transform mCard = manualPanel.transform.Find("TerminalCard");

        string manualContent = "<b>1. MAIN ENGINE PROPULSION:</b>\n" +
                               "Press <b>[SPACE]</b> or <b>[W]</b> to ignite main plasma thruster. Apply gentle bursts to manage vertical momentum.\n\n" +
                               "<b>2. RCS LATERAL STABILIZATION:</b>\n" +
                               "Press <b>[A]</b> / <b>[D]</b> to rotate spacecraft attitude. Maintain upright orientation for landing.\n\n" +
                               "<b>3. HULL INTEGRITY & DAMAGE:</b>\n" +
                               "Impacting structures or terrain depletes Hull HP. Retrieve green <b>Nano-Repair Crates</b> in Sectors 2 & 3 to recover +25 HP.\n\n" +
                               "<b>4. LANDING ZONE PROTOCOL:</b>\n" +
                               "Touchdown on the cyan Landing Pad upright to initiate hyper-jump extraction.";

        CreateUIText("ManualContent", mCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(740f, 320f), manualContent, 14, FontStyles.Normal, new Color(0.90f, 0.94f, 1f), TextAlignmentOptions.Left, font);

        GameObject mBackBtn = CreateUIButton("ManualBackBtn", mCard, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(280f, 44f), "◄  RETURN TO MENU", 15, new Color(0.25f, 0.32f, 0.42f), Color.white, font);
        menuUI.manualBackButton = mBackBtn.GetComponent<Button>();
        manualPanel.SetActive(false);

        // 5. SETTINGS PANEL
        GameObject settingsPanel = CreateModalPanel("SettingsPanel", canvasObj.transform, new Vector2(620f, 400f), "// SYSTEMS CONFIGURATION //", font);
        menuUI.settingsPanel = settingsPanel;
        Transform setCard = settingsPanel.transform.Find("TerminalCard");

        CreateUIText("VolumeLabel", setCard, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(500f, 26f), "MASTER AUDIO OUTPUT", 15, FontStyles.Bold, Color.white, TextAlignmentOptions.Left, font);

        GameObject sliderObj = CreateSlider("VolumeSlider", setCard, new Vector2(0.5f, 1f), new Vector2(0f, -125f), new Vector2(380f, 24f));
        menuUI.volumeSlider = sliderObj.GetComponent<Slider>();

        GameObject volValObj = CreateUIText("VolumeValue", setCard, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-60f, -125f), new Vector2(80f, 24f), "100%", 15, FontStyles.Bold, new Color(0f, 0.95f, 1f), TextAlignmentOptions.Right, font);
        menuUI.volumeValueText = volValObj.GetComponent<TextMeshProUGUI>();

        CreateUIText("DisplayLabel", setCard, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -180f), new Vector2(500f, 26f), "FULLSCREEN DISPLAY MODE", 15, FontStyles.Bold, Color.white, TextAlignmentOptions.Left, font);

        GameObject toggleObj = CreateToggle("FullscreenToggle", setCard, new Vector2(0.5f, 1f), new Vector2(-220f, -220f), "ENABLED");
        menuUI.fullscreenToggle = toggleObj.GetComponent<Toggle>();

        GameObject setBackBtn = CreateUIButton("SettingsBackBtn", setCard, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(280f, 44f), "◄  RETURN TO MENU", 15, new Color(0.25f, 0.32f, 0.42f), Color.white, font);
        menuUI.settingsBackButton = setBackBtn.GetComponent<Button>();
        settingsPanel.SetActive(false);

        Canvas.ForceUpdateCanvases();
    }

    private static GameObject CreateSectorRow(string name, Transform parent, Vector2 anchoredPos, string title, string sub, string btnText, Color btnColor, TMP_FontAsset font)
    {
        GameObject row = CreateBorderedCard(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPos, new Vector2(720f, 75f), new Color(0.06f, 0.10f, 0.16f, 0.92f), new Color(0f, 0.94f, 1f, 0.45f), 1f);

        CreateUIText("RowTitle", row.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(20f, 12f), new Vector2(450f, 24f), title, 16, FontStyles.Bold, Color.white, TextAlignmentOptions.Left, font);
        CreateUIText("RowSub", row.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(20f, -14f), new Vector2(450f, 20f), sub, 12, FontStyles.Normal, new Color(0.7f, 0.8f, 0.92f), TextAlignmentOptions.Left, font);

        GameObject btn = CreateUIButton("DeployBtn", row.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-15f, 0f), new Vector2(200f, 46f), btnText, 14, btnColor, Color.white, font);
        return btn;
    }

    private static GameObject CreateModalPanel(string name, Transform parent, Vector2 sizeDelta, string headerText, TMP_FontAsset font)
    {
        GameObject dimmer = CreateUIPanel(name, parent, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0f, 0f, 0f, 0.75f));
        GameObject card = CreateBorderedCard("TerminalCard", dimmer.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, sizeDelta, new Color(0.04f, 0.07f, 0.12f, 0.96f), new Color(0f, 0.94f, 1f, 0.85f), 2f);

        CreateUIText("ModalHeader", card.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(sizeDelta.x - 40f, 32f), headerText, 20, FontStyles.Bold, new Color(0f, 0.95f, 1f), TextAlignmentOptions.Center, font);
        return dimmer;
    }

    private static GameObject CreateBorderedCard(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, Color bgColor, Color borderColor, float borderWidth)
    {
        GameObject borderObj = CreateUIPanel(name, parent, anchorMin, anchorMax, pivot, anchoredPos, sizeDelta, borderColor);
        Vector2 innerSizeDelta = new Vector2(-borderWidth * 2f, -borderWidth * 2f);
        CreateUIPanel("Background", borderObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, innerSizeDelta, bgColor);
        return borderObj;
    }

    private static GameObject CreateUIPanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        Image img = obj.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = (color.a > 0.001f);
        return obj;
    }

    private static GameObject CreateUIText(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, string text, float fontSize, FontStyles style, Color color, TextAlignmentOptions alignment, TMP_FontAsset font)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        if (font != null) tmp.font = font;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.raycastTarget = false;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        return obj;
    }

    private static GameObject CreateUIButton(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, string buttonText, float fontSize, Color bgColor, Color textColor, TMP_FontAsset font)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        img.raycastTarget = true;

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = bgColor;
        colors.highlightedColor = bgColor * 1.35f;
        colors.pressedColor = bgColor * 0.75f;
        btn.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        if (font != null) tmp.font = font;
        tmp.text = buttonText;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;

        return btnObj;
    }

    private static GameObject CreateSlider(string name, Transform parent, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        RectTransform rt = sliderObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        Slider slider = sliderObj.AddComponent<Slider>();

        // Background
        GameObject bg = CreateUIPanel("Background", sliderObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.1f, 0.15f, 0.22f));

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform faRt = fillArea.AddComponent<RectTransform>();
        faRt.anchorMin = new Vector2(0f, 0.25f);
        faRt.anchorMax = new Vector2(1f, 0.75f);
        faRt.sizeDelta = Vector2.zero;

        GameObject fill = CreateUIPanel("Fill", fillArea.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0f, 0.95f, 1f));

        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.targetGraphic = bg.GetComponent<Image>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        return sliderObj;
    }

    private static GameObject CreateToggle(string name, Transform parent, Vector2 pivot, Vector2 anchoredPos, string label)
    {
        GameObject toggleObj = new GameObject(name);
        toggleObj.transform.SetParent(parent, false);
        RectTransform rt = toggleObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(180f, 30f);

        Toggle toggle = toggleObj.AddComponent<Toggle>();

        GameObject bg = CreateUIPanel("Background", toggleObj.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), Vector2.zero, new Vector2(26f, 26f), new Color(0.12f, 0.18f, 0.25f));
        GameObject checkmark = CreateUIPanel("Checkmark", bg.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(18f, 18f), new Color(0f, 0.95f, 1f));

        toggle.graphic = checkmark.GetComponent<Image>();
        toggle.targetGraphic = bg.GetComponent<Image>();
        toggle.isOn = true;

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        CreateUIText("Label", toggleObj.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(35f, 0f), new Vector2(140f, 24f), label, 14, FontStyles.Bold, Color.white, TextAlignmentOptions.Left, font);

        return toggleObj;
    }

    public static void UpdateBuildSettings()
    {
        var originalScenes = EditorBuildSettings.scenes;
        string[] allScenePaths = new string[] {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };

        EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[allScenePaths.Length];
        for (int i = 0; i < allScenePaths.Length; i++)
        {
            newScenes[i] = new EditorBuildSettingsScene(allScenePaths[i], true);
        }

        EditorBuildSettings.scenes = newScenes;
        Debug.Log("[BuildSciFiMainMenu] EditorBuildSettings updated: MainMenu (0), Level1 (1), Level2 (2), Level3 (3)");
    }
}
