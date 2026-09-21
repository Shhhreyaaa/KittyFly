using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BuildPolishedFlappyLevel1
{
    const string kModelsPath = "Assets/GameDevTV Assets/Models/";
    const string kMatsPath = "Assets/GameDevTV Assets/Materials and Textures/";
    const float kZ = 24.5f;

    [MenuItem("Tools/Build Polished Flappy Level 1")]
    public static void Build()
    {
        string level1Path = "Assets/Scenes/Level1.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Lighting & Atmosphere
        SetupLightingAndAtmosphere();

        // 2. Background Mountains (Properly scaled at Z = 38 so they NEVER block camera)
        SetupDistantCanyon();

        // 3. Terrain & Boundaries (Floor and Ceiling)
        SetupCanyonCorridor();

        // 4. Start Pad & Base
        Vector3 startPadPos = new Vector3(-18f, 1f, kZ);
        GameObject player = SetupStartBase(startPadPos);

        // 5. Flappy Gates (Sleek sci-fi pillars with neon gate beacons)
        BuildFlappyGate("Gate_01", 0f, 3.5f, 14.0f);
        BuildFlappyGate("Gate_02", 18f, 5.0f, 15.5f);
        BuildFlappyGate("Gate_03", 36f, 4.0f, 14.5f);

        // 6. Finish Landing Base: X = 52
        SetupFinishBase(new Vector3(52f, 1f, kZ));

        // 7. Camera & Cinemachine (Wide, cinematic 2.5D framing)
        Camera mainCam = SetupCamera(player);

        // 8. In-Game UI & Health Bar (Screen Space Camera for crisp in-game rendering)
        SetupUI(mainCam, "LEVEL 1: FLAPPY CANYON (EASY)");

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, level1Path);
        Debug.Log($"[BuildPolishedFlappyLevel1] Successfully rebuilt and saved {level1Path}!");
    }

    private static void SetupLightingAndAtmosphere()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.65f, 0.78f, 0.95f);
        RenderSettings.ambientEquatorColor = new Color(0.72f, 0.65f, 0.55f);
        RenderSettings.ambientGroundColor = new Color(0.35f, 0.3f, 0.25f);

        GameObject sunObj = new GameObject("Sun Light");
        Light sun = sunObj.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.35f;
        sun.color = new Color(1f, 0.94f, 0.85f);
        sun.shadows = LightShadows.Soft;
        sunObj.transform.rotation = Quaternion.Euler(42f, -32f, 0f);

        GameObject fillObj = new GameObject("Fill Light");
        Light fill = fillObj.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.45f;
        fill.color = new Color(0.55f, 0.7f, 0.9f);
        fillObj.transform.rotation = Quaternion.Euler(-30f, 150f, 0f);
    }

    private static void SetupDistantCanyon()
    {
        GameObject bgParent = new GameObject("Background Mountains");

        // Placed deep at Z = 55 and lower Y so ridges frame the canyon without ever reaching the corridor
        float[] xPositions = new float[] { -24f, -4f, 16f, 36f, 54f };
        for (int i = 0; i < xPositions.Length; i++)
        {
            float x = xPositions[i];
            float z = 55f;
            float y = -2.5f;

            GameObject cliff = SpawnModel("Rock_Cliffs_Env_01.fbx", new Vector3(x, y, z), new Vector3(1.1f, 1.2f, 0.8f), Quaternion.Euler(0, (i % 2 == 0) ? 0 : 180, 0), bgParent.transform);
            if (cliff != null)
            {
                Collider[] cols = cliff.GetComponentsInChildren<Collider>();
                foreach (var c in cols) Object.DestroyImmediate(c);
            }
        }
    }

    private static void SetupCanyonCorridor()
    {
        GameObject corridor = new GameObject("Canyon Corridor");

        // Ground Floor
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground Floor";
        ground.transform.SetParent(corridor.transform);
        ground.transform.position = new Vector3(17f, -1.0f, kZ);
        ground.transform.localScale = new Vector3(95f, 2f, 5f);
        SetSolidColor(ground, new Color(0.24f, 0.27f, 0.32f), 0.3f, 0.5f);

        // Ground Rock Details
        float[] rockX = new float[] { -8f, 9f, 27f, 44f };
        foreach (float rx in rockX)
        {
            GameObject rock = SpawnModel("Rock_Env.fbx", new Vector3(rx, 0.2f, kZ + 1.2f), new Vector3(1.2f, 1.0f, 1.2f), Quaternion.Euler(0, rx * 25f, 0), corridor.transform);
            if (rock != null)
            {
                Collider[] cols = rock.GetComponentsInChildren<Collider>();
                foreach (var c in cols) Object.DestroyImmediate(c);
            }
        }

        // Ceiling Boundary
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling Boundary";
        ceiling.tag = "Obstacle";
        ceiling.transform.SetParent(corridor.transform);
        ceiling.transform.position = new Vector3(17f, 21.5f, kZ);
        ceiling.transform.localScale = new Vector3(95f, 2f, 5f);
        SetSolidColor(ceiling, new Color(0.2f, 0.22f, 0.26f), 0.2f, 0.4f);
    }

    private static GameObject SetupStartBase(Vector3 pos)
    {
        GameObject baseParent = new GameObject("Start Base");

        // Launch Platform pedestal
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pedestal.name = "Launch Pedestal";
        pedestal.transform.SetParent(baseParent.transform);
        pedestal.transform.position = pos + new Vector3(0f, -0.6f, 0f);
        pedestal.transform.localScale = new Vector3(5.5f, 1.2f, 5f);
        SetSolidColor(pedestal, new Color(0.2f, 0.24f, 0.3f), 0.5f, 0.6f);

        // Blue Launch Pad
        GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pad.name = "launch pad";
        pad.tag = "LaunchPad";
        pad.transform.SetParent(baseParent.transform);
        pad.transform.position = pos;
        pad.transform.localScale = new Vector3(4.8f, 0.4f, 4.8f);
        SetSolidColor(pad, new Color(0.18f, 0.58f, 0.95f), 0.2f, 0.7f);

        // Flat BoxCollider
        Collider[] oldCols = pad.GetComponents<Collider>();
        foreach (var c in oldCols) Object.DestroyImmediate(c);
        BoxCollider box = pad.AddComponent<BoxCollider>();
        box.size = Vector3.one;

        // Decorative Rover
        SpawnModel("Rover_01_Neutral.fbx", pos + new Vector3(-3.8f, 0f, 0.8f), new Vector3(0.75f, 0.75f, 0.75f), Quaternion.Euler(0, 80, 0), baseParent.transform);

        // Crate
        SpawnModel("Crate_02_2_Glow.fbx", pos + new Vector3(3.4f, 0f, 0.8f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, baseParent.transform);

        // Player Rocket
        GameObject rocketPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player Rocket 1.prefab");
        GameObject rocket = (rocketPrefab != null) ? (GameObject)PrefabUtility.InstantiatePrefab(rocketPrefab) : new GameObject("Player Rocket");
        rocket.name = "Player Rocket";
        rocket.transform.position = pos + new Vector3(0f, 1.35f, 0f);
        rocket.transform.rotation = Quaternion.identity;

        Transform podMesh = rocket.transform.Find("Rick's Weird Space Pod");
        if (podMesh != null)
        {
            podMesh.localPosition = Vector3.zero;
            podMesh.localRotation = Quaternion.identity;
        }

        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb == null) rb = rocket.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;
        rb.linearDamping = 0.8f;
        rb.angularDamping = 2f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

        BoxCollider col = rocket.GetComponent<BoxCollider>();
        if (col == null) col = rocket.AddComponent<BoxCollider>();
        col.center = Vector3.zero;
        col.size = new Vector3(1.4f, 2.2f, 1.4f);

        if (rocket.GetComponent<movement>() == null) rocket.AddComponent<movement>();
        if (rocket.GetComponent<RocketHealth>() == null) rocket.AddComponent<RocketHealth>();
        if (rocket.GetComponent<CollisionHandler>() == null) rocket.AddComponent<CollisionHandler>();

        return rocket;
    }

    private static void BuildFlappyGate(string gateName, float x, float gapBottomY, float gapTopY)
    {
        GameObject gate = new GameObject(gateName);
        gate.tag = "Obstacle";

        float canyonFloorY = 0f;
        float canyonCeilingY = 20.5f;

        // 1. Bottom Pylon (Sleek dark pillar)
        float bottomHeight = gapBottomY - canyonFloorY;
        float bottomCenterY = canyonFloorY + (bottomHeight / 2f);

        GameObject bottomPylon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottomPylon.name = "Bottom_Pylon";
        bottomPylon.tag = "Obstacle";
        bottomPylon.transform.SetParent(gate.transform);
        bottomPylon.transform.position = new Vector3(x, bottomCenterY, kZ);
        bottomPylon.transform.localScale = new Vector3(2.2f, bottomHeight, 2.2f);
        SetSolidColor(bottomPylon, new Color(0.22f, 0.25f, 0.3f), 0.4f, 0.6f);

        // Neon warning rim on bottom pylon tip
        GameObject bottomCap = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottomCap.name = "Bottom_Cap";
        bottomCap.tag = "Obstacle";
        bottomCap.transform.SetParent(gate.transform);
        bottomCap.transform.position = new Vector3(x, gapBottomY - 0.2f, kZ);
        bottomCap.transform.localScale = new Vector3(2.5f, 0.4f, 2.5f);
        SetEmissiveColor(bottomCap, new Color(1f, 0.45f, 0.1f));

        // 2. Top Pylon (Sleek dark pillar coming down from ceiling)
        float topHeight = canyonCeilingY - gapTopY;
        float topCenterY = gapTopY + (topHeight / 2f);

        GameObject topPylon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topPylon.name = "Top_Pylon";
        topPylon.tag = "Obstacle";
        topPylon.transform.SetParent(gate.transform);
        topPylon.transform.position = new Vector3(x, topCenterY, kZ);
        topPylon.transform.localScale = new Vector3(2.2f, topHeight, 2.2f);
        SetSolidColor(topPylon, new Color(0.22f, 0.25f, 0.3f), 0.4f, 0.6f);

        // Neon warning rim on top pylon tip
        GameObject topCap = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topCap.name = "Top_Cap";
        topCap.tag = "Obstacle";
        topCap.transform.SetParent(gate.transform);
        topCap.transform.position = new Vector3(x, gapTopY + 0.2f, kZ);
        topCap.transform.localScale = new Vector3(2.5f, 0.4f, 2.5f);
        SetEmissiveColor(topCap, new Color(1f, 0.45f, 0.1f));

        // 3. Floating Glowing Guide Crate in the Center
        float centerGapY = (gapBottomY + gapTopY) / 2f;
        GameObject guideCrate = SpawnModel("Crate_02_2_Glow.fbx", new Vector3(x, centerGapY, kZ), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.Euler(15, 45, 15), gate.transform);
        if (guideCrate != null)
        {
            Collider[] cols = guideCrate.GetComponentsInChildren<Collider>();
            foreach (var c in cols) Object.DestroyImmediate(c);
        }
    }

    private static void SetupFinishBase(Vector3 pos)
    {
        GameObject baseParent = new GameObject("Finish Base");

        // Landing Pedestal
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pedestal.name = "Landing Pedestal";
        pedestal.transform.SetParent(baseParent.transform);
        pedestal.transform.position = pos + new Vector3(0f, -0.6f, 0f);
        pedestal.transform.localScale = new Vector3(5.5f, 1.2f, 5f);
        SetSolidColor(pedestal, new Color(0.2f, 0.26f, 0.24f), 0.4f, 0.6f);

        // Green Finish Pad
        GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pad.name = "finish pad";
        pad.tag = "Finish";
        pad.transform.SetParent(baseParent.transform);
        pad.transform.position = pos;
        pad.transform.localScale = new Vector3(4.8f, 0.4f, 4.8f);
        SetSolidColor(pad, new Color(0.2f, 0.88f, 0.42f), 0.2f, 0.7f);

        // Flat BoxCollider
        Collider[] oldCols = pad.GetComponents<Collider>();
        foreach (var c in oldCols) Object.DestroyImmediate(c);
        BoxCollider box = pad.AddComponent<BoxCollider>();
        box.size = Vector3.one;

        // Finish Beacons & Radar
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(-2.8f, 0f, 1.0f), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.identity, baseParent.transform);
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(2.8f, 0f, 1.0f), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.identity, baseParent.transform);
        SpawnModel("Mothership_02_Radar_Blue.fbx", pos + new Vector3(3.8f, 0.4f, 1.2f), new Vector3(1.0f, 1.0f, 1.0f), Quaternion.identity, baseParent.transform);
    }

    private static Camera SetupCamera(GameObject player)
    {
        GameObject mainCamObj = new GameObject("Camera");
        mainCamObj.tag = "MainCamera";
        Camera mainCam = mainCamObj.AddComponent<Camera>();
        mainCam.nearClipPlane = 0.1f;
        mainCam.farClipPlane = 1000f;
        mainCam.fieldOfView = 56f;
        mainCamObj.AddComponent<AudioListener>();

        var brain = mainCamObj.AddComponent<Unity.Cinemachine.CinemachineBrain>();
        brain.UpdateMethod = Unity.Cinemachine.CinemachineBrain.UpdateMethods.FixedUpdate;

        GameObject vcamObj = new GameObject("CinemachineCamera");
        var vcam = vcamObj.AddComponent<Unity.Cinemachine.CinemachineCamera>();

        Transform targetTransform = player.transform.Find("Rick's Weird Space Pod");
        if (targetTransform == null) targetTransform = player.transform;

        vcam.Follow = targetTransform;
        vcam.LookAt = targetTransform;
        vcam.Priority.Value = 100;

        var composer = vcamObj.AddComponent<Unity.Cinemachine.CinemachinePositionComposer>();
        composer.CameraDistance = 20f;
        composer.TargetOffset = new Vector3(3.0f, 2.0f, 0f); // Lead camera forward so player sees upcoming gates
        composer.Damping = Vector3.zero;

        vcamObj.transform.position = targetTransform.position + new Vector3(3.0f, 2.0f, -20f);
        vcamObj.transform.rotation = Quaternion.identity;

        mainCamObj.transform.position = vcamObj.transform.position;
        mainCamObj.transform.rotation = Quaternion.identity;

        return mainCam;
    }

    private static void SetupUI(Camera mainCam, string levelTitleText)
    {
        GameObject canvasObj = new GameObject("UI Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = mainCam;
        canvas.planeDistance = 2.0f;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        GameUI gameUI = canvasObj.AddComponent<GameUI>();

        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Top HUD Header Panel
        GameObject headerPanel = CreateUIPanel(canvasObj.transform, "HeaderPanel", new Vector2(0, 470), new Vector2(1820, 75), new Color(0.08f, 0.12f, 0.18f, 0.8f));

        GameObject titleObj = CreateUIText(headerPanel.transform, "LevelTitleText", levelTitleText, 28, TextAlignmentOptions.Left, new Vector2(-400, 0), new Vector2(800, 60), Color.yellow);
        gameUI.levelText = titleObj.GetComponent<TextMeshProUGUI>();

        GameObject ctrlObj = CreateUIText(headerPanel.transform, "ControlsText", "W / Space : Thrust   |   A / D : Steer   |   L : Skip", 22, TextAlignmentOptions.Right, new Vector2(400, 0), new Vector2(800, 60), Color.white);
        gameUI.controlsText = ctrlObj.GetComponent<TextMeshProUGUI>();

        // Health Bar UI (Top Left HUD)
        GameObject healthBarBg = CreateUIPanel(canvasObj.transform, "HealthBarBg", new Vector2(-680, 400), new Vector2(300, 32), new Color(0.12f, 0.14f, 0.16f, 0.9f));
        
        GameObject healthFillObj = CreateUIPanel(healthBarBg.transform, "HealthFill", Vector2.zero, new Vector2(292, 24), Color.green);
        Image healthFillImg = healthFillObj.GetComponent<Image>();
        healthFillImg.type = Image.Type.Filled;
        healthFillImg.fillMethod = Image.FillMethod.Horizontal;
        healthFillImg.fillAmount = 1f;
        gameUI.healthFillImage = healthFillImg;

        GameObject hpTextObj = CreateUIText(healthBarBg.transform, "HealthText", "HP: 100 / 100", 18, TextAlignmentOptions.Center, Vector2.zero, new Vector2(280, 26), Color.white);
        gameUI.healthText = hpTextObj.GetComponent<TextMeshProUGUI>();

        // Status Panel
        GameObject statusPanel = CreateUIPanel(canvasObj.transform, "StatusPanel", Vector2.zero, new Vector2(700, 180), new Color(0.05f, 0.08f, 0.12f, 0.9f));
        GameObject bannerObj = CreateUIText(statusPanel.transform, "BannerText", "LEVEL COMPLETE!", 40, TextAlignmentOptions.Center, Vector2.zero, new Vector2(650, 140), Color.green);
        gameUI.statusPanel = statusPanel;
        gameUI.statusBannerText = bannerObj.GetComponent<TextMeshProUGUI>();
        statusPanel.SetActive(false);

        // Victory Panel
        GameObject winPanel = CreateUIPanel(canvasObj.transform, "WinPanel", Vector2.zero, new Vector2(850, 420), new Color(0.06f, 0.1f, 0.18f, 0.95f));
        CreateUIText(winPanel.transform, "WinTitle", "🏆 VICTORY! ALL LEVELS CLEARED! 🏆", 38, TextAlignmentOptions.Center, new Vector2(0, 110), new Vector2(800, 90), Color.gold);
        CreateUIText(winPanel.transform, "WinSub", "You mastered the Rick's Space Pod Flappy Skyway!", 24, TextAlignmentOptions.Center, new Vector2(0, 30), new Vector2(750, 50), Color.cyan);

        GameObject restartBtnObj = CreateUIButton(winPanel.transform, "RestartButton", "Play Again", new Vector2(-140, -90), new Vector2(240, 65), new Color(0.2f, 0.7f, 0.35f));
        gameUI.restartButton = restartBtnObj.GetComponent<Button>();

        GameObject nextBtnObj = CreateUIButton(winPanel.transform, "NextButton", "Main Menu", new Vector2(140, -90), new Vector2(240, 65), new Color(0.2f, 0.55f, 0.95f));
        gameUI.nextLevelButton = nextBtnObj.GetComponent<Button>();

        gameUI.winPanel = winPanel;
        winPanel.SetActive(false);
    }

    private static GameObject CreateUIPanel(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.SetParent(parent, false);
        RectTransform rt = panelObj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Image img = panelObj.AddComponent<Image>();
        img.color = color;
        return panelObj;
    }

    private static GameObject CreateUIText(Transform parent, string name, string content, float fontSize, TextAlignmentOptions alignment, Vector2 pos, Vector2 size, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = color;
        return textObj;
    }

    private static GameObject CreateUIButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color color)
    {
        GameObject btnObj = CreateUIPanel(parent, name, pos, size, color);
        Button btn = btnObj.AddComponent<Button>();
        CreateUIText(btnObj.transform, "Label", label, 26, TextAlignmentOptions.Center, Vector2.zero, size, Color.white);
        return btnObj;
    }

    private static GameObject SpawnModel(string fbxName, Vector3 pos, Vector3 scale, Quaternion rot, Transform parent)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(kModelsPath + fbxName);
        if (prefab != null)
        {
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.transform.SetParent(parent);
            obj.transform.position = pos;
            obj.transform.localScale = scale;
            obj.transform.rotation = rot;
            return obj;
        }
        return null;
    }

    private static void SetSolidColor(GameObject go, Color color, float metallic = 0.2f, float smoothness = 0.5f)
    {
        Renderer ren = go.GetComponent<Renderer>();
        if (ren != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.color = color;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);
            ren.sharedMaterial = mat;
        }
    }

    private static void SetEmissiveColor(GameObject go, Color emissiveColor)
    {
        Renderer ren = go.GetComponent<Renderer>();
        if (ren != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.color = emissiveColor;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emissiveColor * 1.5f);
            ren.sharedMaterial = mat;
        }
    }
}
