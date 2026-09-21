using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BuildBeautifulSciFiLevel1
{
    const string kModelsPath = "Assets/GameDevTV Assets/Models/";
    const string kMatsPath = "Assets/GameDevTV Assets/Materials and Textures/";
    const float kZ = 24.5f;

    [MenuItem("Tools/Build Beautiful Sci-Fi Level 1")]
    public static void Build()
    {
        string level1Path = "Assets/Scenes/Level1.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Core Materials
        Material terrainMat = CreateOrGetMaterial("Assets/GameDevTV Assets/Materials and Textures/SciFi_Terrain_Base.mat", new Color(0.14f, 0.16f, 0.20f), 0.2f, 0.35f);
        Material metalMat = CreateOrGetMaterial("Assets/GameDevTV Assets/Materials and Textures/SciFi_Structure_Metal.mat", new Color(0.20f, 0.23f, 0.28f), 0.4f, 0.6f);
        Material launchPadMat = CreateOrGetMaterial("Assets/GameDevTV Assets/Materials and Textures/SciFi_LaunchPad_Blue.mat", new Color(0.15f, 0.55f, 0.98f), 0.2f, 0.85f, new Color(0.08f, 0.45f, 0.95f) * 1.8f);
        Material finishPadMat = CreateOrGetMaterial("Assets/GameDevTV Assets/Materials and Textures/SciFi_FinishPad_Green.mat", new Color(0.18f, 0.88f, 0.42f), 0.2f, 0.85f, new Color(0.1f, 0.9f, 0.35f) * 1.8f);
        Material glowRimCyan = CreateOrGetMaterial("Assets/GameDevTV Assets/Materials and Textures/SciFi_Gate_Cyan.mat", new Color(0.1f, 0.85f, 1.0f), 0.1f, 0.9f, new Color(0.1f, 0.85f, 1.0f) * 2.5f);
        Material glowRimAmber = CreateOrGetMaterial("Assets/GameDevTV Assets/Materials and Textures/SciFi_Beacon_Amber.mat", new Color(1.0f, 0.55f, 0.1f), 0.1f, 0.9f, new Color(1.0f, 0.55f, 0.1f) * 2.5f);

        // 1. Lighting & Deep Atmospheric Sky
        SetupAtmosphereAndLighting();

        // 2. Cinematic Mountain Backdrop (Z = 48 to 62)
        SetupDistantCanyonBackdrop();

        // 3. Valley Floor & Ceiling Boundaries
        SetupValleyCorridor(terrainMat, metalMat);

        // 4. Start Base (LZ-Alpha at X = -12)
        Vector3 startPadPos = new Vector3(-12f, 1.0f, kZ);
        GameObject player = SetupStartBase(startPadPos, metalMat, launchPadMat);

        // 5. Flappy Sci-Fi Gates (Clean, clear 11-unit openings for easy flight)
        // Gate 1: Outpost Tower Gate (X = 1) - Gap Y = 3.5 to Y = 14.5
        BuildSciFiGate("Gate_01_Outpost", 1f, 3.5f, 14.5f, "Building_17.fbx", 0.36f, glowRimCyan, metalMat);

        // Gate 2: Observation Tower Gate (X = 15) - Gap Y = 4.8 to Y = 15.8 (gentle climb)
        BuildSciFiGate("Gate_02_Observation", 15f, 4.8f, 15.8f, "Building_02.fbx", 0.28f, glowRimAmber, metalMat);

        // Gate 3: Relay Station Gate (X = 29) - Gap Y = 3.8 to Y = 14.8 (gentle descent)
        BuildSciFiGate("Gate_03_Relay", 29f, 3.8f, 14.8f, "Building_17.fbx", 0.36f, glowRimCyan, metalMat);

        // 6. Finish Landing Base (LZ-Omega at X = 43)
        SetupFinishBase(new Vector3(43f, 1.0f, kZ), metalMat, finishPadMat);

        // 7. Camera & Cinemachine (Framing 2.5D with smooth tracking)
        Camera mainCam = SetupCamera(player);

        // 8. Modern Glassmorphic Sci-Fi HUD
        SetupHUD(mainCam);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, level1Path);
        Debug.Log($"[BuildBeautifulSciFiLevel1] Successfully built and saved {level1Path}!");
    }

    private static Material CreateOrGetMaterial(string path, Color albedo, float metallic, float smoothness, Color? emission = null)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }

        mat.color = albedo;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", albedo);
        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emission.Value);
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", Color.black);
        }

        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void SetupAtmosphereAndLighting()
    {
        // Deep space twilight color palette
        Color skyColor = new Color(0.09f, 0.13f, 0.22f);
        Color fogColor = new Color(0.12f, 0.17f, 0.27f);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.40f, 0.55f, 0.80f);
        RenderSettings.ambientEquatorColor = new Color(0.48f, 0.44f, 0.42f);
        RenderSettings.ambientGroundColor = new Color(0.16f, 0.15f, 0.15f);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.007f;
        RenderSettings.fogColor = fogColor;

        // Key Sun Light (Warm golden twilight)
        GameObject sunObj = new GameObject("Sun Light");
        Light sun = sunObj.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.35f;
        sun.color = new Color(1f, 0.94f, 0.86f);
        sun.shadows = LightShadows.Soft;
        sunObj.transform.rotation = Quaternion.Euler(32f, -36f, 0f);

        // Fill Light (Cool cyan reflection)
        GameObject fillObj = new GameObject("Fill Light");
        Light fill = fillObj.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.45f;
        fill.color = new Color(0.45f, 0.65f, 0.92f);
        fillObj.transform.rotation = Quaternion.Euler(-25f, 144f, 0f);
    }

    private static void SetupDistantCanyonBackdrop()
    {
        GameObject bgParent = new GameObject("Background Sci-Fi Canyon");

        // Majestic distant mountain ridge (Z = 50-56, Y = -8)
        float[] xCoords = new float[] { -22f, -4f, 14f, 32f, 50f };
        for (int i = 0; i < xCoords.Length; i++)
        {
            float x = xCoords[i];
            float z = 52f + (i % 2 == 0 ? 0f : 3f);
            float y = -7.5f;

            string cliffModel = (i % 2 == 0) ? "Rock_Cliff_Env_01.fbx" : "Rock_Cliffs_Env_01.fbx";
            float scaleX = (i % 2 == 0) ? 0.72f : 0.80f;
            float scaleY = (i % 2 == 0) ? 0.68f : 0.85f;

            GameObject cliff = SpawnModel(cliffModel, new Vector3(x, y, z), new Vector3(scaleX, scaleY, 0.65f), Quaternion.Euler(0, (i * 80) % 360, 0), bgParent.transform);
            if (cliff != null)
            {
                Collider[] cols = cliff.GetComponentsInChildren<Collider>();
                foreach (var c in cols) Object.DestroyImmediate(c);
            }
        }

        // Distant Outpost on the Ridge (Z = 56)
        GameObject distantOutpost = SpawnModel("Building_02.fbx", new Vector3(8f, 2.5f, 56f), new Vector3(0.45f, 0.45f, 0.45f), Quaternion.Euler(0, -25f, 0), bgParent.transform);
        if (distantOutpost != null)
        {
            Collider[] cols = distantOutpost.GetComponentsInChildren<Collider>();
            foreach (var c in cols) Object.DestroyImmediate(c);
        }
    }

    private static void SetupValleyCorridor(Material terrainMat, Material metalMat)
    {
        GameObject corridor = new GameObject("Canyon Corridor");

        // 1. Deep Valley Floor (Extends from Y = -6 to Y = 0 so it's deeply grounded)
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Valley Floor";
        floor.tag = "Obstacle";
        floor.transform.SetParent(corridor.transform);
        floor.transform.position = new Vector3(16f, -3.0f, kZ);
        floor.transform.localScale = new Vector3(84f, 6.0f, 10f);
        ApplyMaterial(floor, terrainMat);

        // 2. Foundation Trench Plates along the flight path
        float[] plateX = new float[] { -12f, 1f, 15f, 29f, 43f };
        foreach (float px in plateX)
        {
            GameObject plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "Foundation Plate";
            plate.transform.SetParent(corridor.transform);
            plate.transform.position = new Vector3(px, 0.08f, kZ);
            plate.transform.localScale = new Vector3(8.0f, 0.25f, 6.5f);
            ApplyMaterial(plate, metalMat);
        }

        // 3. Sci-Fi Pipelines running along the corridor bed
        float[] pipeX = new float[] { -6f, 8f, 22f, 36f };
        foreach (float px in pipeX)
        {
            SpawnModel("Fighter_08_Pipe_Red.fbx", new Vector3(px, 0.15f, kZ + 1.8f), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.Euler(0, 90f, 0), corridor.transform);
        }

        // 4. Natural Martian Rocks embedded into the valley floor
        float[] rockX = new float[] { -18f, -4f, 9f, 23f, 37f, 49f };
        for (int i = 0; i < rockX.Length; i++)
        {
            float rx = rockX[i];
            GameObject rock = SpawnModel("Rock_Env.fbx", new Vector3(rx, 0.1f, kZ + 1.6f), new Vector3(1.0f, 0.85f, 1.0f), Quaternion.Euler(0, (i * 70f) % 360, 0), corridor.transform);
            if (rock != null)
            {
                Collider[] cols = rock.GetComponentsInChildren<Collider>();
                foreach (var c in cols) Object.DestroyImmediate(c);
            }
        }

        // 5. Overhead Ceiling Boundary (Y = 19.5 to 22.0)
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling Girders";
        ceiling.tag = "Obstacle";
        ceiling.transform.SetParent(corridor.transform);
        ceiling.transform.position = new Vector3(16f, 21.0f, kZ);
        ceiling.transform.localScale = new Vector3(84f, 3.0f, 8f);
        ApplyMaterial(ceiling, metalMat);
    }

    private static GameObject SetupStartBase(Vector3 pos, Material metalMat, Material launchPadMat)
    {
        GameObject baseParent = new GameObject("Start Base (LZ-Alpha)");

        // 1. Launch Platform Base Pedestal
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pedestal.name = "Launch Pedestal";
        pedestal.transform.SetParent(baseParent.transform);
        pedestal.transform.position = pos + new Vector3(0f, -0.6f, 0f);
        pedestal.transform.localScale = new Vector3(5.8f, 1.2f, 5.8f);
        ApplyMaterial(pedestal, metalMat);

        // 2. Blue Launch Pad (Cylinder with flat BoxCollider)
        GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pad.name = "launch pad";
        pad.tag = "LaunchPad";
        pad.transform.SetParent(baseParent.transform);
        pad.transform.position = pos;
        pad.transform.localScale = new Vector3(5.0f, 0.35f, 5.0f);
        ApplyMaterial(pad, launchPadMat);

        Collider[] oldCols = pad.GetComponents<Collider>();
        foreach (var c in oldCols) Object.DestroyImmediate(c);
        BoxCollider box = pad.AddComponent<BoxCollider>();
        box.size = Vector3.one;

        // 3. Staging Equipment: Rover parked cleanly to the left, tech crates
        SpawnModel("Rover_01_Neutral.fbx", pos + new Vector3(-4.0f, 0.05f, 1.2f), new Vector3(0.75f, 0.75f, 0.75f), Quaternion.Euler(0, 80f, 0), baseParent.transform);
        SpawnModel("Crate_02_2_Glow.fbx", pos + new Vector3(-3.2f, 0f, -1.2f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.Euler(0, 25f, 0), baseParent.transform);

        // 4. Perimeter Guidance Beacons
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(-2.6f, 0.1f, 2.0f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, baseParent.transform);
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(2.6f, 0.1f, 2.0f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, baseParent.transform);

        // 5. Player Rocket
        GameObject rocketPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player Rocket 1.prefab");
        GameObject rocket = (rocketPrefab != null) ? (GameObject)PrefabUtility.InstantiatePrefab(rocketPrefab) : new GameObject("Player Rocket");
        rocket.name = "Player Rocket";
        rocket.transform.position = pos + new Vector3(0f, 1.4f, 0f);
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
        rb.angularDamping = 2.0f;
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

    private static void BuildSciFiGate(string gateName, float x, float gapBottomY, float gapTopY, string modelName, float modelScale, Material glowMat, Material metalMat)
    {
        GameObject gate = new GameObject(gateName);
        gate.tag = "Obstacle";

        // 1. LOWER SCI-FI TOWER
        // Spawn actual sci-fi building model
        GameObject lowerModel = SpawnModel(modelName, new Vector3(x, 0f, kZ), new Vector3(modelScale, modelScale * (gapBottomY / 4.5f), modelScale), Quaternion.Euler(0, 45f, 0), gate.transform);

        // Accurate BoxCollider for lower obstacle
        GameObject lowerColObj = new GameObject("Lower_Collider");
        lowerColObj.tag = "Obstacle";
        lowerColObj.transform.SetParent(gate.transform);
        lowerColObj.transform.position = new Vector3(x, gapBottomY / 2f, kZ);
        BoxCollider lowerCol = lowerColObj.AddComponent<BoxCollider>();
        lowerCol.size = new Vector3(3.4f, gapBottomY, 3.4f);

        // Lower Glowing Hazard Rim
        GameObject lowerRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerRim.name = "Lower_Rim_Beacon";
        lowerRim.tag = "Obstacle";
        lowerRim.transform.SetParent(gate.transform);
        lowerRim.transform.position = new Vector3(x, gapBottomY - 0.15f, kZ);
        lowerRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(lowerRim, glowMat);

        // Glowing guidance poles on lower rim
        SpawnModel("Light_Glow_13.fbx", new Vector3(x - 1.5f, gapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, gate.transform);
        SpawnModel("Light_Glow_13.fbx", new Vector3(x + 1.5f, gapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, gate.transform);

        // 2. UPPER SCI-FI HANGING STRUCTURE
        float topHeight = 21.0f - gapTopY;
        GameObject upperModel = SpawnModel(modelName, new Vector3(x, 21.0f, kZ), new Vector3(modelScale, modelScale * (topHeight / 4.5f), modelScale), Quaternion.Euler(180f, 45f, 0), gate.transform);

        // Accurate BoxCollider for upper obstacle
        GameObject upperColObj = new GameObject("Upper_Collider");
        upperColObj.tag = "Obstacle";
        upperColObj.transform.SetParent(gate.transform);
        upperColObj.transform.position = new Vector3(x, gapTopY + (topHeight / 2f), kZ);
        BoxCollider upperCol = upperColObj.AddComponent<BoxCollider>();
        upperCol.size = new Vector3(3.4f, topHeight, 3.4f);

        // Upper Glowing Hazard Rim
        GameObject upperRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upperRim.name = "Upper_Rim_Beacon";
        upperRim.tag = "Obstacle";
        upperRim.transform.SetParent(gate.transform);
        upperRim.transform.position = new Vector3(x, gapTopY + 0.15f, kZ);
        upperRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(upperRim, glowMat);
    }

    private static void SetupFinishBase(Vector3 pos, Material metalMat, Material finishPadMat)
    {
        GameObject baseParent = new GameObject("Finish Base (LZ-Omega)");

        // 1. Landing Pedestal
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pedestal.name = "Landing Pedestal";
        pedestal.transform.SetParent(baseParent.transform);
        pedestal.transform.position = pos + new Vector3(0f, -0.6f, 0f);
        pedestal.transform.localScale = new Vector3(6.2f, 1.2f, 6.2f);
        ApplyMaterial(pedestal, metalMat);

        // 2. Green Finish Landing Pad (Cylinder with flat BoxCollider)
        GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pad.name = "finish pad";
        pad.tag = "Finish";
        pad.transform.SetParent(baseParent.transform);
        pad.transform.position = pos;
        pad.transform.localScale = new Vector3(5.4f, 0.35f, 5.4f);
        ApplyMaterial(pad, finishPadMat);

        Collider[] oldCols = pad.GetComponents<Collider>();
        foreach (var c in oldCols) Object.DestroyImmediate(c);
        BoxCollider box = pad.AddComponent<BoxCollider>();
        box.size = Vector3.one;

        // 3. Recovery Station Dish & Guidance Beacons
        SpawnModel("Mothership_02_Radar_Blue.fbx", pos + new Vector3(3.8f, 0.4f, 1.4f), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.identity, baseParent.transform);
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(-2.8f, 0.1f, 2.0f), new Vector3(0.95f, 0.95f, 0.95f), Quaternion.identity, baseParent.transform);
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(2.8f, 0.1f, 2.0f), new Vector3(0.95f, 0.95f, 0.95f), Quaternion.identity, baseParent.transform);

        // 4. Cargo Crates
        SpawnModel("Crate_02_2_Glow.fbx", pos + new Vector3(3.2f, 0f, -1.2f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, baseParent.transform);
    }

    private static Camera SetupCamera(GameObject player)
    {
        GameObject mainCamObj = new GameObject("Camera");
        mainCamObj.tag = "MainCamera";
        Camera mainCam = mainCamObj.AddComponent<Camera>();
        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = new Color(0.09f, 0.13f, 0.22f);
        mainCam.nearClipPlane = 0.1f;
        mainCam.farClipPlane = 1000f;
        mainCam.fieldOfView = 56f;
        mainCamObj.AddComponent<AudioListener>();

        var brain = mainCamObj.AddComponent<Unity.Cinemachine.CinemachineBrain>();
        brain.UpdateMethod = Unity.Cinemachine.CinemachineBrain.UpdateMethods.FixedUpdate;

        // Cinemachine Virtual Camera
        GameObject vcamObj = new GameObject("CinemachineCamera");
        var vcam = vcamObj.AddComponent<Unity.Cinemachine.CinemachineCamera>();
        vcam.Priority = 10;
        vcam.Follow = player.transform;
        vcam.LookAt = null;

        var follow = vcamObj.AddComponent<Unity.Cinemachine.CinemachineFollow>();
        // Follow offset slightly forward (+X = 3) so player sees more ahead of them!
        follow.FollowOffset = new Vector3(4f, 2.2f, -18.5f);
        follow.TrackerSettings.PositionDamping = new Vector3(0.8f, 0.8f, 0.1f);

        vcamObj.transform.position = player.transform.position + follow.FollowOffset;
        vcamObj.transform.rotation = Quaternion.identity;

        mainCamObj.transform.position = vcamObj.transform.position;
        mainCamObj.transform.rotation = Quaternion.identity;

        return mainCam;
    }

    private static void SetupHUD(Camera mainCam)
    {
        // EventSystem
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSysObj = new GameObject("EventSystem");
            eventSysObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSysObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // ScreenSpaceCamera Canvas ensures perfect crisp overlay across any camera motion and captures
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

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");

        // 1. TOP-LEFT HEALTH CARD
        // Anchor top-left (0, 1), pivot top-left (0, 1) -> Pos (40, -35), size (360, 85)
        GameObject healthCard = CreateUIPanel("HealthCard", canvasObj.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -35f), new Vector2(360f, 85f), new Color(0.06f, 0.09f, 0.14f, 0.85f));

        // Subtitle: HULL INTEGRITY
        CreateUIText("HullLabel", healthCard.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, -12f), new Vector2(200f, 24f), "HULL INTEGRITY", 15, FontStyles.Bold, new Color(0.3f, 0.85f, 1f), TextAlignmentOptions.Left, font);

        // Numeric Readout (Right aligned)
        GameObject healthTextObj = CreateUIText("HealthText", healthCard.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-18f, -12f), new Vector2(140f, 24f), "100 / 100 HP", 16, FontStyles.Bold, Color.white, TextAlignmentOptions.Right, font);
        gameUI.healthText = healthTextObj.GetComponent<TextMeshProUGUI>();

        // Health Bar Track
        GameObject healthBg = CreateUIPanel("HealthBarBg", healthCard.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(-36f, 24f), new Color(0.12f, 0.16f, 0.22f, 0.95f));

        // Health Bar Fill
        GameObject healthFillObj = CreateUIPanel("HealthBarFill", healthBg.transform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.12f, 0.9f, 0.65f, 1f));
        Image healthFillImg = healthFillObj.GetComponent<Image>();
        healthFillImg.type = Image.Type.Filled;
        healthFillImg.fillMethod = Image.FillMethod.Horizontal;
        healthFillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFillImg.fillAmount = 1.0f;
        gameUI.healthFillImage = healthFillImg;

        // 2. TOP-RIGHT MISSION CARD
        // Anchor top-right (1, 1), pivot top-right (1, 1) -> Pos (-40, -35), size (360, 85)
        GameObject missionCard = CreateUIPanel("MissionCard", canvasObj.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -35f), new Vector2(360f, 85f), new Color(0.06f, 0.09f, 0.14f, 0.85f));

        GameObject levelTitleObj = CreateUIText("LevelTitle", missionCard.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -12f), new Vector2(-24f, 26f), "SECTOR 01: CANYON FLIGHT", 16, FontStyles.Bold, new Color(0.3f, 0.85f, 1.0f), TextAlignmentOptions.Center, font);
        gameUI.levelText = levelTitleObj.GetComponent<TextMeshProUGUI>();

        CreateUIText("MissionSubtitle", missionCard.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(-24f, 24f), "OBJECTIVE: REACH LZ-OMEGA", 13, FontStyles.Normal, new Color(0.8f, 0.85f, 0.9f), TextAlignmentOptions.Center, font);

        // 3. BOTTOM-CENTER CONTROLS BADGE
        GameObject controlsCard = CreateUIPanel("ControlsBadge", canvasObj.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 30f), new Vector2(500f, 40f), new Color(0.06f, 0.09f, 0.14f, 0.75f));

        GameObject controlsTextObj = CreateUIText("ControlsText", controlsCard.transform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, "▲ [SPACE / W] THRUST   •   ◄ ► [A / D] STABILIZE", 14, FontStyles.Bold, new Color(0.85f, 0.92f, 1f), TextAlignmentOptions.Center, font);
        gameUI.controlsText = controlsTextObj.GetComponent<TextMeshProUGUI>();

        // 4. MODALS (Status / Win)
        SetupModals(canvasObj, gameUI, font);
    }

    private static void SetupModals(GameObject canvasObj, GameUI gameUI, TMP_FontAsset font)
    {
        // Status Modal (Crash / Level Complete)
        GameObject statusPanel = CreateUIPanel("StatusPanel", canvasObj.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(480f, 180f), new Color(0.05f, 0.08f, 0.12f, 0.95f));
        GameObject bannerTextObj = CreateUIText("StatusBanner", statusPanel.transform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(-40f, 50f), "STATUS BANNER", 26, FontStyles.Bold, Color.white, TextAlignmentOptions.Center, font);
        gameUI.statusPanel = statusPanel;
        gameUI.statusBannerText = bannerTextObj.GetComponent<TextMeshProUGUI>();
        statusPanel.SetActive(false);

        // Win Modal (Game Complete)
        GameObject winPanel = CreateUIPanel("WinPanel", canvasObj.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(520f, 250f), new Color(0.05f, 0.08f, 0.12f, 0.95f));
        CreateUIText("WinTitle", winPanel.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(-40f, 40f), "★ MISSION ACCOMPLISHED! ★", 24, FontStyles.Bold, new Color(0.2f, 0.9f, 0.4f), TextAlignmentOptions.Center, font);
        CreateUIText("WinDesc", winPanel.transform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(-40f, 30f), "All sectors cleared successfully.", 15, FontStyles.Normal, Color.white, TextAlignmentOptions.Center, font);

        GameObject restartBtnObj = CreateUIButton("RestartButton", winPanel.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f), new Vector2(0f, 48f), new Vector2(210f, 46f), "PLAY AGAIN", font);
        gameUI.winPanel = winPanel;
        gameUI.restartButton = restartBtnObj.GetComponent<Button>();
        winPanel.SetActive(false);
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

    private static GameObject CreateUIButton(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, string label, TMP_FontAsset font)
    {
        GameObject btnObj = CreateUIPanel(name, parent, anchorMin, anchorMax, pivot, anchoredPos, sizeDelta, new Color(0.15f, 0.55f, 0.95f, 1f));
        Button btn = btnObj.AddComponent<Button>();

        CreateUIText("Label", btnObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, label, 16, FontStyles.Bold, Color.white, TextAlignmentOptions.Center, font);

        return btnObj;
    }

    private static GameObject SpawnModel(string modelFile, Vector3 pos, Vector3 scale, Quaternion rot, Transform parent)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(kModelsPath + modelFile);
        if (prefab == null)
        {
            Debug.LogWarning($"[BuildBeautifulSciFiLevel1] Could not find model: {kModelsPath + modelFile}");
            return null;
        }

        GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        inst.name = prefab.name;
        inst.transform.SetParent(parent);
        inst.transform.position = pos;
        inst.transform.localScale = scale;
        inst.transform.rotation = rot;
        return inst;
    }

    private static void ApplyMaterial(GameObject obj, Material mat)
    {
        var mr = obj.GetComponent<MeshRenderer>();
        if (mr != null) mr.sharedMaterial = mat;
    }
}
