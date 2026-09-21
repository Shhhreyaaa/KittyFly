using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BuildBeautifulSciFiLevel2
{
    const string kModelsPath = "Assets/GameDevTV Assets/Models/";
    const string kMatsPath = "Assets/GameDevTV Assets/Materials and Textures/";
    const float kZ = 24.5f;

    [MenuItem("Tools/Build Beautiful Sci-Fi Level 2")]
    public static void Build()
    {
        string level2Path = "Assets/Scenes/Level2.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Core Materials (Martian Refinery Palette)
        Material terrainMat = CreateOrGetMaterial(kMatsPath + "SciFi_Martian_Bedrock.mat", new Color(0.24f, 0.16f, 0.15f), 0.2f, 0.3f);
        Material metalMat = CreateOrGetMaterial(kMatsPath + "SciFi_Refinery_Steel.mat", new Color(0.28f, 0.25f, 0.26f), 0.5f, 0.6f);
        Material launchPadMat = CreateOrGetMaterial(kMatsPath + "SciFi_LaunchPad_Blue.mat", new Color(0.15f, 0.55f, 0.98f), 0.2f, 0.85f, new Color(0.08f, 0.45f, 0.95f) * 1.8f);
        Material finishPadMat = CreateOrGetMaterial(kMatsPath + "SciFi_FinishPad_Green.mat", new Color(0.18f, 0.88f, 0.42f), 0.2f, 0.85f, new Color(0.1f, 0.9f, 0.35f) * 1.8f);
        Material glowRimAmber = CreateOrGetMaterial(kMatsPath + "SciFi_Beacon_Amber.mat", new Color(1.0f, 0.55f, 0.1f), 0.1f, 0.9f, new Color(1.0f, 0.55f, 0.1f) * 2.5f);
        Material glowRimCyan = CreateOrGetMaterial(kMatsPath + "SciFi_Gate_Cyan.mat", new Color(0.1f, 0.85f, 1.0f), 0.1f, 0.9f, new Color(0.1f, 0.85f, 1.0f) * 2.5f);

        // 1. Lighting & Martian Sunset Atmosphere
        SetupAtmosphereAndLighting();

        // 2. Distant Martian Canyon Backdrop (Z = 50 to 58)
        SetupDistantBackdrop();

        // 3. Dynamic Undulating Valley Corridor (Terrain Elevation Changes)
        SetupDynamicRefineryTerrain(terrainMat, metalMat);

        // 4. Start Base (LZ-Beta at X = -12, Y = 1.0)
        Vector3 startPadPos = new Vector3(-12f, 1.0f, kZ);
        GameObject player = SetupStartBase(startPadPos, metalMat, launchPadMat);

        // 5. Dynamic Obstacles:
        // Gate 1: High-Pressure Silo Gate (X = 2) - Gap Y = 3.8 to Y = 14.0
        BuildRefineryGate("Gate_01_Silo", 2f, 3.8f, 14.0f, glowRimAmber, metalMat);

        // Gate 2: Horizontally Moving Cargo Crane (X = 16 on elevated ridge)
        BuildHorizontalMovingCrane("Gate_02_MovingCrane", 16f, 5.5f, 16.0f, new Vector3(3.5f, 0f, 0f), 3.8f, glowRimAmber, metalMat);

        // Gate 3: Deep Trench Pipeline Gate (X = 30) - Gap Y = 3.2 to Y = 13.5
        BuildTrenchPipeGate("Gate_03_TrenchPipes", 30f, 3.2f, 13.5f, glowRimAmber, metalMat);

        // Bonus Risk-Reward: Nano-Repair Health Pickup in an upper maintenance alcove (X = 28, Y = 15.5)
        SpawnHealthPickup(new Vector3(28f, 15.5f, kZ));

        // 6. Elevated Extraction Landing Base (LZ-Refinery at X = 46, Y = 2.0)
        SetupFinishBase(new Vector3(46f, 2.0f, kZ), metalMat, finishPadMat);

        // 7. Camera & Cinemachine (2.5D tracking)
        Camera mainCam = SetupCamera(player);

        // 8. Modern Glassmorphic Sci-Fi HUD
        SetupHUD(mainCam);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, level2Path);
        Debug.Log($"[BuildBeautifulSciFiLevel2] Successfully built and saved enhanced {level2Path}!");
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
        Color fogColor = new Color(0.26f, 0.16f, 0.13f);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.58f, 0.38f, 0.30f);
        RenderSettings.ambientEquatorColor = new Color(0.50f, 0.35f, 0.28f);
        RenderSettings.ambientGroundColor = new Color(0.20f, 0.15f, 0.14f);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.007f;
        RenderSettings.fogColor = fogColor;

        // Key Sun Light
        GameObject sunObj = new GameObject("Sun Light");
        Light sun = sunObj.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.4f;
        sun.color = new Color(1.0f, 0.88f, 0.70f);
        sun.shadows = LightShadows.Soft;
        sunObj.transform.rotation = Quaternion.Euler(28f, -38f, 0f);

        // Fill Light
        GameObject fillObj = new GameObject("Fill Light");
        Light fill = fillObj.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.45f;
        fill.color = new Color(0.45f, 0.52f, 0.70f);
        fillObj.transform.rotation = Quaternion.Euler(-20f, 140f, 0f);
    }

    private static void SetupDistantBackdrop()
    {
        GameObject bgParent = new GameObject("Background Martian Ridge");

        float[] xCoords = new float[] { -22f, -4f, 14f, 32f, 50f };
        for (int i = 0; i < xCoords.Length; i++)
        {
            float x = xCoords[i];
            float z = 52f + (i % 2 == 0 ? 0f : 3f);
            float y = -7.5f;

            string cliffModel = (i % 2 == 0) ? "Rock_Cliff_Env_01.fbx" : "Rock_Cliffs_Env_01.fbx";
            float scaleX = (i % 2 == 0) ? 0.75f : 0.85f;
            float scaleY = (i % 2 == 0) ? 0.70f : 0.88f;

            GameObject cliff = SpawnModel(cliffModel, new Vector3(x, y, z), new Vector3(scaleX, scaleY, 0.65f), Quaternion.Euler(0, (i * 85) % 360, 0), bgParent.transform);
            if (cliff != null)
            {
                Collider[] cols = cliff.GetComponentsInChildren<Collider>();
                foreach (var c in cols) Object.DestroyImmediate(c);
            }
        }

        GameObject distantSilo = SpawnModel("Building_02.fbx", new Vector3(10f, 2.5f, 56f), new Vector3(0.48f, 0.48f, 0.48f), Quaternion.Euler(0, -30f, 0), bgParent.transform);
        if (distantSilo != null)
        {
            Collider[] cols = distantSilo.GetComponentsInChildren<Collider>();
            foreach (var c in cols) Object.DestroyImmediate(c);
        }
    }

    private static void SetupDynamicRefineryTerrain(Material terrainMat, Material metalMat)
    {
        GameObject corridor = new GameObject("Refinery Corridor");

        // 1. Initial Launch Basin (X = -16 to 6, Y = -3.0, Floor at Y = 0)
        GameObject basin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        basin.name = "Terrain_Basin";
        basin.tag = "Obstacle";
        basin.transform.SetParent(corridor.transform);
        basin.transform.position = new Vector3(-5f, -3.0f, kZ);
        basin.transform.localScale = new Vector3(26f, 6.0f, 10f);
        ApplyMaterial(basin, terrainMat);

        // 2. Elevated Ridge (X = 6 to 23, Y = -1.75, Floor rises to Y = 2.25)
        GameObject ridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ridge.name = "Terrain_ElevatedRidge";
        ridge.tag = "Obstacle";
        ridge.transform.SetParent(corridor.transform);
        ridge.transform.position = new Vector3(14.5f, -1.75f, kZ);
        ridge.transform.localScale = new Vector3(17f, 8.0f, 10f);
        ApplyMaterial(ridge, terrainMat);

        // Foundation Plate on top of ridge
        GameObject ridgePlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ridgePlate.name = "Ridge_Platform";
        ridgePlate.transform.SetParent(corridor.transform);
        ridgePlate.transform.position = new Vector3(15f, 2.3f, kZ);
        ridgePlate.transform.localScale = new Vector3(15f, 0.3f, 7f);
        ApplyMaterial(ridgePlate, metalMat);

        // 3. Deep Trench (X = 23 to 38, Y = -4.5, Floor dips to Y = -1.5)
        GameObject trench = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trench.name = "Terrain_DeepTrench";
        trench.tag = "Obstacle";
        trench.transform.SetParent(corridor.transform);
        trench.transform.position = new Vector3(30.5f, -4.5f, kZ);
        trench.transform.localScale = new Vector3(17f, 6.0f, 10f);
        ApplyMaterial(trench, terrainMat);

        // 4. Final Landing Hill (X = 38 to 56, Y = -2.5, Floor at Y = 1.0)
        GameObject landingDeck = GameObject.CreatePrimitive(PrimitiveType.Cube);
        landingDeck.name = "Terrain_LandingDeck";
        landingDeck.tag = "Obstacle";
        landingDeck.transform.SetParent(corridor.transform);
        landingDeck.transform.position = new Vector3(47f, -2.5f, kZ);
        landingDeck.transform.localScale = new Vector3(18f, 7.0f, 10f);
        ApplyMaterial(landingDeck, terrainMat);

        // Surface pipelines along the trench
        float[] pipeX = new float[] { -4f, 10f, 25f, 33f };
        foreach (float px in pipeX)
        {
            SpawnModel("Fighter_08_Pipe_Red.fbx", new Vector3(px, 0.15f, kZ + 1.8f), new Vector3(1.2f, 1.2f, 1.2f), Quaternion.Euler(0, 90f, 0), corridor.transform);
            SpawnModel("Dropship_01_Piping_02_Red.fbx", new Vector3(px + 1.5f, 0.3f, kZ + 2.0f), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.identity, corridor.transform);
        }

        // Boulders
        float[] rockX = new float[] { -18f, 7f, 24f, 37f, 52f };
        for (int i = 0; i < rockX.Length; i++)
        {
            float rx = rockX[i];
            GameObject rock = SpawnModel("Rock_Env.fbx", new Vector3(rx, 0.2f, kZ + 1.6f), new Vector3(1.0f, 0.85f, 1.0f), Quaternion.Euler(0, (i * 70f) % 360, 0), corridor.transform);
            if (rock != null)
            {
                Collider[] cols = rock.GetComponentsInChildren<Collider>();
                foreach (var c in cols) Object.DestroyImmediate(c);
            }
        }

        // Overhead Ceiling (Y = 21.0)
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling Girders";
        ceiling.tag = "Obstacle";
        ceiling.transform.SetParent(corridor.transform);
        ceiling.transform.position = new Vector3(16f, 21.5f, kZ);
        ceiling.transform.localScale = new Vector3(86f, 3.0f, 8f);
        ApplyMaterial(ceiling, metalMat);
    }

    private static GameObject SetupStartBase(Vector3 pos, Material metalMat, Material launchPadMat)
    {
        GameObject baseParent = new GameObject("Start Base (LZ-Beta)");

        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pedestal.name = "Launch Pedestal";
        pedestal.transform.SetParent(baseParent.transform);
        pedestal.transform.position = pos + new Vector3(0f, -0.6f, 0f);
        pedestal.transform.localScale = new Vector3(5.8f, 1.2f, 5.8f);
        ApplyMaterial(pedestal, metalMat);

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

        SpawnModel("Rover_01_Neutral.fbx", pos + new Vector3(-4.0f, 0.05f, 1.2f), new Vector3(0.75f, 0.75f, 0.75f), Quaternion.Euler(0, 80f, 0), baseParent.transform);
        SpawnModel("Crate_02_2_Glow.fbx", pos + new Vector3(-3.2f, 0f, -1.2f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.Euler(0, 25f, 0), baseParent.transform);

        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(-2.6f, 0.1f, 2.0f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, baseParent.transform);
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(2.6f, 0.1f, 2.0f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, baseParent.transform);

        // Player Rocket
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

    private static void BuildRefineryGate(string gateName, float x, float gapBottomY, float gapTopY, Material glowMat, Material metalMat)
    {
        GameObject gate = new GameObject(gateName);
        gate.tag = "Obstacle";

        // Lower Industrial Tank Tower
        SpawnModel("Dropship_01_Tank_Red.fbx", new Vector3(x, gapBottomY - 1.2f, kZ), new Vector3(0.55f, 0.55f, 0.55f), Quaternion.Euler(0, 0, 90f), gate.transform);

        GameObject lowerColObj = new GameObject("Lower_Collider");
        lowerColObj.tag = "Obstacle";
        lowerColObj.transform.SetParent(gate.transform);
        lowerColObj.transform.position = new Vector3(x, gapBottomY / 2f, kZ);
        BoxCollider lowerCol = lowerColObj.AddComponent<BoxCollider>();
        lowerCol.size = new Vector3(3.2f, gapBottomY, 3.2f);

        // Lower Glowing Rim
        GameObject lowerRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerRim.name = "Lower_Rim_Beacon";
        lowerRim.tag = "Obstacle";
        lowerRim.transform.SetParent(gate.transform);
        lowerRim.transform.position = new Vector3(x, gapBottomY - 0.15f, kZ);
        lowerRim.transform.localScale = new Vector3(3.4f, 0.35f, 3.4f);
        ApplyMaterial(lowerRim, glowMat);

        SpawnModel("Light_Glow_13.fbx", new Vector3(x - 1.4f, gapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, gate.transform);
        SpawnModel("Light_Glow_13.fbx", new Vector3(x + 1.4f, gapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, gate.transform);

        // Upper Overhead Gantry & Suspension
        float topHeight = 21.5f - gapTopY;
        GameObject upperGantry = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upperGantry.name = "Refinery Overhead Gantry";
        upperGantry.tag = "Obstacle";
        upperGantry.transform.SetParent(gate.transform);
        upperGantry.transform.position = new Vector3(x, gapTopY + (topHeight / 2f), kZ);
        upperGantry.transform.localScale = new Vector3(3.2f, topHeight, 3.2f);
        ApplyMaterial(upperGantry, metalMat);

        SpawnModel("Dropship_01_Piping_02_Red.fbx", new Vector3(x, gapTopY + 1.2f, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.Euler(180f, 0, 0), gate.transform);

        GameObject upperRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upperRim.name = "Upper_Rim_Beacon";
        upperRim.tag = "Obstacle";
        upperRim.transform.SetParent(gate.transform);
        upperRim.transform.position = new Vector3(x, gapTopY + 0.15f, kZ);
        upperRim.transform.localScale = new Vector3(3.4f, 0.35f, 3.4f);
        ApplyMaterial(upperRim, glowMat);
    }

    private static void BuildHorizontalMovingCrane(string gateName, float x, float gapBottomY, float gapTopY, Vector3 oscVector, float period, Material glowMat, Material metalMat)
    {
        GameObject gate = new GameObject(gateName);
        gate.tag = "Obstacle";

        // Lower Ridge Structure
        SpawnModel("Building_02.fbx", new Vector3(x, 2.3f, kZ), new Vector3(0.30f, 0.22f, 0.30f), Quaternion.Euler(0, 45f, 0), gate.transform);

        GameObject lowerColObj = new GameObject("Lower_Collider");
        lowerColObj.tag = "Obstacle";
        lowerColObj.transform.SetParent(gate.transform);
        lowerColObj.transform.position = new Vector3(x, gapBottomY / 2f, kZ);
        BoxCollider lowerCol = lowerColObj.AddComponent<BoxCollider>();
        lowerCol.size = new Vector3(3.4f, gapBottomY, 3.4f);

        GameObject lowerRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerRim.name = "Lower_Rim_Beacon";
        lowerRim.tag = "Obstacle";
        lowerRim.transform.SetParent(gate.transform);
        lowerRim.transform.position = new Vector3(x, gapBottomY - 0.15f, kZ);
        lowerRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(lowerRim, glowMat);

        // HORIZONTALLY SLIDING CRANE HAZARD (Mounted overhead)
        GameObject movingCrane = new GameObject("Moving_Cargo_Crane");
        movingCrane.tag = "Obstacle";
        movingCrane.transform.SetParent(gate.transform);
        movingCrane.transform.position = new Vector3(x - (oscVector.x / 2f), 0f, kZ);

        Oscillator osc = movingCrane.AddComponent<Oscillator>();
        var so = new SerializedObject(osc);
        so.FindProperty("movementVector").vector3Value = oscVector;
        so.FindProperty("period").floatValue = period;
        so.ApplyModifiedProperties();

        // Cargo Container suspended from crane
        float topHeight = 21.5f - gapTopY;
        SpawnModel("Dropship_01_Left_Compartment_Red.fbx", new Vector3(x - (oscVector.x / 2f), gapTopY + 1.2f, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, movingCrane.transform);

        GameObject craneCol = new GameObject("Crane_Collider");
        craneCol.tag = "Obstacle";
        craneCol.transform.SetParent(movingCrane.transform);
        craneCol.transform.position = new Vector3(x - (oscVector.x / 2f), gapTopY + (topHeight / 2f), kZ);
        BoxCollider boxCol = craneCol.AddComponent<BoxCollider>();
        boxCol.size = new Vector3(3.4f, topHeight, 3.4f);

        GameObject upperRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upperRim.name = "Upper_Crane_Beacon";
        upperRim.tag = "Obstacle";
        upperRim.transform.SetParent(movingCrane.transform);
        upperRim.transform.position = new Vector3(x - (oscVector.x / 2f), gapTopY + 0.15f, kZ);
        upperRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(upperRim, glowMat);
    }

    private static void BuildTrenchPipeGate(string gateName, float x, float gapBottomY, float gapTopY, Material glowMat, Material metalMat)
    {
        GameObject gate = new GameObject(gateName);
        gate.tag = "Obstacle";

        // Lower Trench Pipe Structure
        SpawnModel("Building_17.fbx", new Vector3(x, -1.5f, kZ), new Vector3(0.35f, 0.35f, 0.35f), Quaternion.Euler(0, 45f, 0), gate.transform);

        GameObject lowerColObj = new GameObject("Lower_Collider");
        lowerColObj.tag = "Obstacle";
        lowerColObj.transform.SetParent(gate.transform);
        lowerColObj.transform.position = new Vector3(x, gapBottomY / 2f, kZ);
        BoxCollider lowerCol = lowerColObj.AddComponent<BoxCollider>();
        lowerCol.size = new Vector3(3.4f, gapBottomY, 3.4f);

        GameObject lowerRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerRim.name = "Lower_Rim_Beacon";
        lowerRim.tag = "Obstacle";
        lowerRim.transform.SetParent(gate.transform);
        lowerRim.transform.position = new Vector3(x, gapBottomY - 0.15f, kZ);
        lowerRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(lowerRim, glowMat);

        SpawnModel("Light_Glow_13.fbx", new Vector3(x - 1.5f, gapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, gate.transform);
        SpawnModel("Light_Glow_13.fbx", new Vector3(x + 1.5f, gapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, gate.transform);

        // Upper Structure
        float topHeight = 21.5f - gapTopY;
        SpawnModel("Building_17.fbx", new Vector3(x, 21.5f, kZ), new Vector3(0.35f, 0.35f * (topHeight / 4.5f), 0.35f), Quaternion.Euler(180f, 45f, 0), gate.transform);

        GameObject upperColObj = new GameObject("Upper_Collider");
        upperColObj.tag = "Obstacle";
        upperColObj.transform.SetParent(gate.transform);
        upperColObj.transform.position = new Vector3(x, gapTopY + (topHeight / 2f), kZ);
        BoxCollider upperCol = upperColObj.AddComponent<BoxCollider>();
        upperCol.size = new Vector3(3.4f, topHeight, 3.4f);

        GameObject upperRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upperRim.name = "Upper_Rim_Beacon";
        upperRim.tag = "Obstacle";
        upperRim.transform.SetParent(gate.transform);
        upperRim.transform.position = new Vector3(x, gapTopY + 0.15f, kZ);
        upperRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(upperRim, glowMat);
    }

    private static void SpawnHealthPickup(Vector3 pos)
    {
        GameObject pickup = SpawnModel("Crate_02_2_Glow.fbx", pos, new Vector3(0.9f, 0.9f, 0.9f), Quaternion.Euler(15f, 45f, 0), null);
        if (pickup == null) return;

        pickup.name = "Nano_Repair_Pickup";
        Collider[] oldCols = pickup.GetComponentsInChildren<Collider>();
        foreach (var c in oldCols) Object.DestroyImmediate(c);

        BoxCollider trigger = pickup.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(2.5f, 2.5f, 2.5f);

        pickup.AddComponent<HealthPickup>();

        // Guidance beacon underneath alcove
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(0f, -1.2f, 0f), new Vector3(0.7f, 0.7f, 0.7f), Quaternion.Euler(180f, 0, 0), pickup.transform);
    }

    private static void SetupFinishBase(Vector3 pos, Material metalMat, Material finishPadMat)
    {
        GameObject baseParent = new GameObject("Finish Base (LZ-Refinery)");

        // Pedestal on elevated deck
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pedestal.name = "Landing Pedestal";
        pedestal.transform.SetParent(baseParent.transform);
        pedestal.transform.position = pos + new Vector3(0f, -0.6f, 0f);
        pedestal.transform.localScale = new Vector3(6.2f, 1.2f, 6.2f);
        ApplyMaterial(pedestal, metalMat);

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

        SpawnModel("Mothership_02_Radar_Blue.fbx", pos + new Vector3(3.8f, 0.4f, 1.4f), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.identity, baseParent.transform);
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(-2.8f, 0.1f, 2.0f), new Vector3(0.95f, 0.95f, 0.95f), Quaternion.identity, baseParent.transform);
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(2.8f, 0.1f, 2.0f), new Vector3(0.95f, 0.95f, 0.95f), Quaternion.identity, baseParent.transform);
        SpawnModel("Crate_02_2_Glow.fbx", pos + new Vector3(3.2f, 0f, -1.2f), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, baseParent.transform);
    }

    private static Camera SetupCamera(GameObject player)
    {
        GameObject mainCamObj = new GameObject("Camera");
        mainCamObj.tag = "MainCamera";
        Camera mainCam = mainCamObj.AddComponent<Camera>();
        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = new Color(0.22f, 0.13f, 0.11f);
        mainCam.nearClipPlane = 0.1f;
        mainCam.farClipPlane = 1000f;
        mainCam.fieldOfView = 56f;
        mainCamObj.AddComponent<AudioListener>();

        var brain = mainCamObj.AddComponent<Unity.Cinemachine.CinemachineBrain>();
        brain.UpdateMethod = Unity.Cinemachine.CinemachineBrain.UpdateMethods.FixedUpdate;

        GameObject vcamObj = new GameObject("CinemachineCamera");
        var vcam = vcamObj.AddComponent<Unity.Cinemachine.CinemachineCamera>();
        vcam.Priority = 10;
        vcam.Follow = player.transform;

        var follow = vcamObj.AddComponent<Unity.Cinemachine.CinemachineFollow>();
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
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSysObj = new GameObject("EventSystem");
            eventSysObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSysObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        GameObject canvasObj = new GameObject("UI Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = mainCam;
        canvas.planeDistance = 2.0f;
        Canvas.ForceUpdateCanvases();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
        GameUI gameUI = canvasObj.AddComponent<GameUI>();

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");

        // 1. TOP-LEFT HEALTH CARD
        GameObject healthCard = CreateUIPanel("HealthCard", canvasObj.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -35f), new Vector2(360f, 85f), new Color(0.06f, 0.09f, 0.14f, 0.85f));
        CreateUIText("HullLabel", healthCard.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, -12f), new Vector2(200f, 24f), "HULL INTEGRITY", 15, FontStyles.Bold, new Color(0.3f, 0.85f, 1f), TextAlignmentOptions.Left, font);

        GameObject healthTextObj = CreateUIText("HealthText", healthCard.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-18f, -12f), new Vector2(140f, 24f), "100 / 100 HP", 16, FontStyles.Bold, Color.white, TextAlignmentOptions.Right, font);
        gameUI.healthText = healthTextObj.GetComponent<TextMeshProUGUI>();

        GameObject healthBg = CreateUIPanel("HealthBarBg", healthCard.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(-36f, 24f), new Color(0.12f, 0.16f, 0.22f, 0.95f));

        GameObject healthFillObj = CreateUIPanel("HealthBarFill", healthBg.transform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.12f, 0.9f, 0.65f, 1f));
        Image healthFillImg = healthFillObj.GetComponent<Image>();
        healthFillImg.type = Image.Type.Filled;
        healthFillImg.fillMethod = Image.FillMethod.Horizontal;
        healthFillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFillImg.fillAmount = 1.0f;
        gameUI.healthFillImage = healthFillImg;

        // 2. TOP-RIGHT MISSION CARD
        GameObject missionCard = CreateUIPanel("MissionCard", canvasObj.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -35f), new Vector2(360f, 85f), new Color(0.06f, 0.09f, 0.14f, 0.85f));
        GameObject levelTitleObj = CreateUIText("LevelTitle", missionCard.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -12f), new Vector2(-24f, 26f), "SECTOR 02: REFINERY", 16, FontStyles.Bold, new Color(1.0f, 0.65f, 0.2f), TextAlignmentOptions.Center, font);
        gameUI.levelText = levelTitleObj.GetComponent<TextMeshProUGUI>();

        CreateUIText("MissionSubtitle", missionCard.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(-24f, 24f), "OBJECTIVE: REACH REFINERY PAD", 13, FontStyles.Normal, new Color(0.85f, 0.85f, 0.9f), TextAlignmentOptions.Center, font);

        // 3. BOTTOM-CENTER CONTROLS BADGE
        GameObject controlsCard = CreateUIPanel("ControlsBadge", canvasObj.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 30f), new Vector2(500f, 40f), new Color(0.06f, 0.09f, 0.14f, 0.75f));
        GameObject controlsTextObj = CreateUIText("ControlsText", controlsCard.transform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, "▲ [SPACE / W] THRUST   •   ◄ ► [A / D] STABILIZE", 14, FontStyles.Bold, new Color(0.85f, 0.92f, 1f), TextAlignmentOptions.Center, font);
        gameUI.controlsText = controlsTextObj.GetComponent<TextMeshProUGUI>();

        // 4. MODALS
        SetupModals(canvasObj, gameUI, font);
    }

    private static void SetupModals(GameObject canvasObj, GameUI gameUI, TMP_FontAsset font)
    {
        GameObject statusPanel = CreateUIPanel("StatusPanel", canvasObj.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(480f, 180f), new Color(0.05f, 0.08f, 0.12f, 0.95f));
        GameObject bannerTextObj = CreateUIText("StatusBanner", statusPanel.transform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(-40f, 50f), "STATUS BANNER", 26, FontStyles.Bold, Color.white, TextAlignmentOptions.Center, font);
        gameUI.statusPanel = statusPanel;
        gameUI.statusBannerText = bannerTextObj.GetComponent<TextMeshProUGUI>();
        statusPanel.SetActive(false);

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
        if (prefab == null) return null;

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
