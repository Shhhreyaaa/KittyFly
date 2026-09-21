using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BuildBeautifulSciFiLevel3
{
    const string kModelsPath = "Assets/GameDevTV Assets/Models/";
    const string kMatsPath = "Assets/GameDevTV Assets/Materials and Textures/";
    const float kZ = 24.5f;

    [MenuItem("Tools/Build Beautiful Sci-Fi Level 3")]
    public static void Build()
    {
        string level3Path = "Assets/Scenes/Level3.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Core Materials (Deep Space Defense Nexus Palette)
        Material terrainMat = CreateOrGetMaterial(kMatsPath + "SciFi_Nexus_Obsidian.mat", new Color(0.10f, 0.12f, 0.16f), 0.5f, 0.6f);
        Material metalMat = CreateOrGetMaterial(kMatsPath + "SciFi_Nexus_Alloy.mat", new Color(0.16f, 0.20f, 0.26f), 0.6f, 0.7f);
        Material launchPadMat = CreateOrGetMaterial(kMatsPath + "SciFi_LaunchPad_Blue.mat", new Color(0.15f, 0.55f, 0.98f), 0.2f, 0.85f, new Color(0.08f, 0.45f, 0.95f) * 1.8f);
        Material finishPadMat = CreateOrGetMaterial(kMatsPath + "SciFi_FinishPad_Green.mat", new Color(0.18f, 0.88f, 0.42f), 0.2f, 0.85f, new Color(0.1f, 0.9f, 0.35f) * 2.0f);
        Material glowRimCyan = CreateOrGetMaterial(kMatsPath + "SciFi_Nexus_Cyan.mat", new Color(0.1f, 0.85f, 1.0f), 0.1f, 0.9f, new Color(0.1f, 0.85f, 1.0f) * 3.0f);
        Material glowRimPurple = CreateOrGetMaterial(kMatsPath + "SciFi_Nexus_Purple.mat", new Color(0.85f, 0.2f, 1.0f), 0.1f, 0.9f, new Color(0.85f, 0.2f, 1.0f) * 3.0f);

        // 1. Lighting & Midnight Nebula Atmosphere
        SetupAtmosphereAndLighting();

        // 2. Cosmic Asteroid Backdrop (Z = 50 to 58)
        SetupDistantCosmicBackdrop();

        // 3. Platform Floor & Ceiling Boundaries
        SetupNexusCorridor(terrainMat, metalMat);

        // 4. Start Base (LZ-Nexus Alpha at X = -12)
        Vector3 startPadPos = new Vector3(-12f, 1.0f, kZ);
        GameObject player = SetupStartBase(startPadPos, metalMat, launchPadMat);

        // 5. Dynamic Gauntlet Obstacles:
        // Gate 1: Counter-Phase Synchronized Piston Crushers (X = 3)
        BuildCounterPhasePistons("Gate_01_CounterPistons", 3f, 3.2f, 16.0f, 4.0f, glowRimCyan, metalMat);

        // Gate 2: Rotating Defense Turbine Hazard (X = 18)
        BuildRotatingTurbineGate("Gate_02_RotatingTurbine", 18f, 9.5f, 22f, glowRimPurple, metalMat);

        // Gate 3: Hangar Defense Nexus Barrier (X = 33)
        BuildOscillatingHangarGate("Gate_03_HangarBarrier", 33f, 3.5f, 15.0f, new Vector3(0f, 3.2f, 0f), 3.4f, glowRimCyan, metalMat);

        // Bonus Risk-Reward: Nano-Repair Health Pickup in lower alcove (X = 30, Y = 4.5)
        SpawnHealthPickup(new Vector3(30f, 4.5f, kZ));

        // 6. Grand Extraction Landing Bay (LZ-Extraction at X = 48)
        SetupFinishBase(new Vector3(48f, 1.0f, kZ), metalMat, finishPadMat);

        // 7. Camera & Cinemachine (2.5D tracking)
        Camera mainCam = SetupCamera(player);

        // 8. Modern Glassmorphic Sci-Fi HUD
        SetupHUD(mainCam);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, level3Path);
        Debug.Log($"[BuildBeautifulSciFiLevel3] Successfully built and saved enhanced {level3Path}!");
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
        Color fogColor = new Color(0.08f, 0.11f, 0.20f);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.35f, 0.50f, 0.85f);
        RenderSettings.ambientEquatorColor = new Color(0.30f, 0.35f, 0.50f);
        RenderSettings.ambientGroundColor = new Color(0.12f, 0.14f, 0.18f);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.007f;
        RenderSettings.fogColor = fogColor;

        // Key Star Light
        GameObject sunObj = new GameObject("Sun Light");
        Light sun = sunObj.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.35f;
        sun.color = new Color(0.95f, 0.98f, 1.0f);
        sun.shadows = LightShadows.Soft;
        sunObj.transform.rotation = Quaternion.Euler(34f, -38f, 0f);

        // Fill Light
        GameObject fillObj = new GameObject("Fill Light");
        Light fill = fillObj.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.5f;
        fill.color = new Color(0.15f, 0.75f, 1.0f);
        fillObj.transform.rotation = Quaternion.Euler(-25f, 142f, 0f);
    }

    private static void SetupDistantCosmicBackdrop()
    {
        GameObject bgParent = new GameObject("Background Cosmic Nexus");

        float[] xCoords = new float[] { -22f, -4f, 14f, 32f, 50f };
        for (int i = 0; i < xCoords.Length; i++)
        {
            float x = xCoords[i];
            float z = 52f + (i % 2 == 0 ? 0f : 3f);
            float y = -7.5f;

            string cliffModel = (i % 2 == 0) ? "Rock_Cliff_Env_01.fbx" : "Rock_Cliffs_Env_01.fbx";
            float scaleX = (i % 2 == 0) ? 0.72f : 0.82f;
            float scaleY = (i % 2 == 0) ? 0.70f : 0.86f;

            GameObject cliff = SpawnModel(cliffModel, new Vector3(x, y, z), new Vector3(scaleX, scaleY, 0.65f), Quaternion.Euler(0, (i * 80) % 360, 0), bgParent.transform);
            if (cliff != null)
            {
                Collider[] cols = cliff.GetComponentsInChildren<Collider>();
                foreach (var c in cols) Object.DestroyImmediate(c);
            }
        }

        GameObject distantNexus = SpawnModel("Building_02.fbx", new Vector3(12f, 2.8f, 56f), new Vector3(0.50f, 0.50f, 0.50f), Quaternion.Euler(0, -25f, 0), bgParent.transform);
        if (distantNexus != null)
        {
            Collider[] cols = distantNexus.GetComponentsInChildren<Collider>();
            foreach (var c in cols) Object.DestroyImmediate(c);
        }
    }

    private static void SetupNexusCorridor(Material terrainMat, Material metalMat)
    {
        GameObject corridor = new GameObject("Nexus Corridor");

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Valley Floor";
        floor.tag = "Obstacle";
        floor.transform.SetParent(corridor.transform);
        floor.transform.position = new Vector3(17f, -3.0f, kZ);
        floor.transform.localScale = new Vector3(88f, 6.0f, 10f);
        ApplyMaterial(floor, terrainMat);

        float[] plateX = new float[] { -12f, 2f, 17f, 32f, 47f };
        foreach (float px in plateX)
        {
            GameObject plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "Foundation Plate";
            plate.transform.SetParent(corridor.transform);
            plate.transform.position = new Vector3(px, 0.08f, kZ);
            plate.transform.localScale = new Vector3(8.0f, 0.25f, 6.5f);
            ApplyMaterial(plate, metalMat);
        }

        float[] pipeX = new float[] { -7f, 8f, 23f, 38f };
        foreach (float px in pipeX)
        {
            SpawnModel("Fighter_08_Pipe_Red.fbx", new Vector3(px, 0.15f, kZ + 1.8f), new Vector3(1.2f, 1.2f, 1.2f), Quaternion.Euler(0, 90f, 0), corridor.transform);
        }

        float[] rockX = new float[] { -18f, -5f, 10f, 25f, 39f, 51f };
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

        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling Girders";
        ceiling.tag = "Obstacle";
        ceiling.transform.SetParent(corridor.transform);
        ceiling.transform.position = new Vector3(17f, 21.0f, kZ);
        ceiling.transform.localScale = new Vector3(88f, 3.0f, 8f);
        ApplyMaterial(ceiling, metalMat);
    }

    private static GameObject SetupStartBase(Vector3 pos, Material metalMat, Material launchPadMat)
    {
        GameObject baseParent = new GameObject("Start Base (LZ-Nexus)");

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

    private static void BuildCounterPhasePistons(string gateName, float x, float baseGapBottomY, float baseGapTopY, float period, Material glowMat, Material metalMat)
    {
        GameObject gate = new GameObject(gateName);
        gate.tag = "Obstacle";

        // 1. Lower Piston (Ascends from Y = 0 to 3.0, phaseOffset = 0.0)
        GameObject lowerGroup = new GameObject("Lower_Piston");
        lowerGroup.tag = "Obstacle";
        lowerGroup.transform.SetParent(gate.transform);
        lowerGroup.transform.position = new Vector3(x, 0f, kZ);

        Oscillator oscLower = lowerGroup.AddComponent<Oscillator>();
        SetOscillatorFields(oscLower, new Vector3(0f, 2.8f, 0f), period, 0.0f);

        SpawnModel("Building_17.fbx", new Vector3(x, 0f, kZ), new Vector3(0.35f, 0.35f * (baseGapBottomY / 4.5f), 0.35f), Quaternion.Euler(0, 45f, 0), lowerGroup.transform);

        GameObject lowerColObj = new GameObject("Lower_Collider");
        lowerColObj.tag = "Obstacle";
        lowerColObj.transform.SetParent(lowerGroup.transform);
        lowerColObj.transform.position = new Vector3(x, baseGapBottomY / 2f, kZ);
        BoxCollider lowerCol = lowerColObj.AddComponent<BoxCollider>();
        lowerCol.size = new Vector3(3.4f, baseGapBottomY, 3.4f);

        GameObject lowerRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerRim.name = "Lower_Rim_Beacon";
        lowerRim.tag = "Obstacle";
        lowerRim.transform.SetParent(lowerGroup.transform);
        lowerRim.transform.position = new Vector3(x, baseGapBottomY - 0.15f, kZ);
        lowerRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(lowerRim, glowMat);

        SpawnModel("Light_Glow_13.fbx", new Vector3(x - 1.5f, baseGapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, lowerGroup.transform);
        SpawnModel("Light_Glow_13.fbx", new Vector3(x + 1.5f, baseGapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, lowerGroup.transform);

        // 2. Upper Piston (Descends from Y = 21 to 17.5, phaseOffset = 0.5 - EXACT MIRROR WAVE)
        GameObject upperGroup = new GameObject("Upper_Piston");
        upperGroup.tag = "Obstacle";
        upperGroup.transform.SetParent(gate.transform);
        upperGroup.transform.position = new Vector3(x, 0f, kZ);

        Oscillator oscUpper = upperGroup.AddComponent<Oscillator>();
        SetOscillatorFields(oscUpper, new Vector3(0f, -2.8f, 0f), period, 0.5f);

        float topHeight = 21.0f - baseGapTopY;
        SpawnModel("Building_17.fbx", new Vector3(x, 21.0f, kZ), new Vector3(0.35f, 0.35f * (topHeight / 4.5f), 0.35f), Quaternion.Euler(180f, 45f, 0), upperGroup.transform);

        GameObject upperColObj = new GameObject("Upper_Collider");
        upperColObj.tag = "Obstacle";
        upperColObj.transform.SetParent(upperGroup.transform);
        upperColObj.transform.position = new Vector3(x, baseGapTopY + (topHeight / 2f), kZ);
        BoxCollider upperCol = upperColObj.AddComponent<BoxCollider>();
        upperCol.size = new Vector3(3.4f, topHeight, 3.4f);

        GameObject upperRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upperRim.name = "Upper_Rim_Beacon";
        upperRim.tag = "Obstacle";
        upperRim.transform.SetParent(upperGroup.transform);
        upperRim.transform.position = new Vector3(x, baseGapTopY + 0.15f, kZ);
        upperRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(upperRim, glowMat);
    }

    private static void BuildRotatingTurbineGate(string gateName, float x, float centerY, float speed, Material glowMat, Material metalMat)
    {
        GameObject gate = new GameObject(gateName);
        gate.tag = "Obstacle";

        // Static Outpost Frame behind turbine
        SpawnModel("Building_02.fbx", new Vector3(x, 0f, kZ + 1.5f), new Vector3(0.28f, 0.18f, 0.28f), Quaternion.Euler(0, 45f, 0), gate.transform);

        // Rotating Defense Hub
        GameObject rotorHub = new GameObject("Rotating_Turbine_Core");
        rotorHub.tag = "Obstacle";
        rotorHub.transform.SetParent(gate.transform);
        rotorHub.transform.position = new Vector3(x, centerY, kZ);

        Rotator rotator = rotorHub.AddComponent<Rotator>();
        var so = new SerializedObject(rotator);
        so.FindProperty("rotationSpeed").vector3Value = new Vector3(0f, 0f, speed);
        so.ApplyModifiedProperties();

        // Central glowing core cylinder
        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        core.name = "Turbine_Hub";
        core.tag = "Obstacle";
        core.transform.SetParent(rotorHub.transform);
        core.transform.position = new Vector3(x, centerY, kZ);
        core.transform.rotation = Quaternion.Euler(90f, 0, 0);
        core.transform.localScale = new Vector3(2.5f, 1.2f, 2.5f);
        ApplyMaterial(core, metalMat);

        // Blade 1 (Upper Wing)
        GameObject blade1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blade1.name = "Blade_1";
        blade1.tag = "Obstacle";
        blade1.transform.SetParent(rotorHub.transform);
        blade1.transform.position = new Vector3(x, centerY + 3.8f, kZ);
        blade1.transform.localScale = new Vector3(1.2f, 5.2f, 1.5f);
        ApplyMaterial(blade1, metalMat);

        GameObject tip1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tip1.name = "Blade_1_Tip";
        tip1.tag = "Obstacle";
        tip1.transform.SetParent(rotorHub.transform);
        tip1.transform.position = new Vector3(x, centerY + 6.2f, kZ);
        tip1.transform.localScale = new Vector3(1.6f, 0.4f, 1.8f);
        ApplyMaterial(tip1, glowMat);

        // Blade 2 (Lower Wing)
        GameObject blade2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blade2.name = "Blade_2";
        blade2.tag = "Obstacle";
        blade2.transform.SetParent(rotorHub.transform);
        blade2.transform.position = new Vector3(x, centerY - 3.8f, kZ);
        blade2.transform.localScale = new Vector3(1.2f, 5.2f, 1.5f);
        ApplyMaterial(blade2, metalMat);

        GameObject tip2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tip2.name = "Blade_2_Tip";
        tip2.tag = "Obstacle";
        tip2.transform.SetParent(rotorHub.transform);
        tip2.transform.position = new Vector3(x, centerY - 6.2f, kZ);
        tip2.transform.localScale = new Vector3(1.6f, 0.4f, 1.8f);
        ApplyMaterial(tip2, glowMat);
    }

    private static void BuildOscillatingHangarGate(string gateName, float x, float baseGapBottomY, float gapTopY, Vector3 oscVector, float period, Material glowMat, Material metalMat)
    {
        GameObject gate = new GameObject(gateName);
        gate.tag = "Obstacle";

        GameObject oscGroup = new GameObject("Oscillating_Barrier");
        oscGroup.tag = "Obstacle";
        oscGroup.transform.SetParent(gate.transform);
        oscGroup.transform.position = new Vector3(x, 0f, kZ);

        Oscillator osc = oscGroup.AddComponent<Oscillator>();
        SetOscillatorFields(osc, oscVector, period, 0.0f);

        SpawnModel("Building_17.fbx", new Vector3(x, 0f, kZ), new Vector3(0.35f, 0.35f * (baseGapBottomY / 4.5f), 0.35f), Quaternion.Euler(0, 45f, 0), oscGroup.transform);

        GameObject lowerColObj = new GameObject("Lower_Collider");
        lowerColObj.tag = "Obstacle";
        lowerColObj.transform.SetParent(oscGroup.transform);
        lowerColObj.transform.position = new Vector3(x, baseGapBottomY / 2f, kZ);
        BoxCollider lowerCol = lowerColObj.AddComponent<BoxCollider>();
        lowerCol.size = new Vector3(3.4f, baseGapBottomY, 3.4f);

        GameObject lowerRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerRim.name = "Lower_Rim_Beacon";
        lowerRim.tag = "Obstacle";
        lowerRim.transform.SetParent(oscGroup.transform);
        lowerRim.transform.position = new Vector3(x, baseGapBottomY - 0.15f, kZ);
        lowerRim.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
        ApplyMaterial(lowerRim, glowMat);

        SpawnModel("Light_Glow_13.fbx", new Vector3(x - 1.5f, baseGapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, oscGroup.transform);
        SpawnModel("Light_Glow_13.fbx", new Vector3(x + 1.5f, baseGapBottomY, kZ), new Vector3(0.85f, 0.85f, 0.85f), Quaternion.identity, oscGroup.transform);

        // Upper Static Structure
        float topHeight = 21.0f - gapTopY;
        SpawnModel("Building_17.fbx", new Vector3(x, 21.0f, kZ), new Vector3(0.35f, 0.35f * (topHeight / 4.5f), 0.35f), Quaternion.Euler(180f, 45f, 0), gate.transform);

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

    private static void SetOscillatorFields(Oscillator osc, Vector3 mov, float per, float phase)
    {
        var so = new SerializedObject(osc);
        so.FindProperty("movementVector").vector3Value = mov;
        so.FindProperty("period").floatValue = per;
        so.FindProperty("phaseOffset").floatValue = phase;
        so.ApplyModifiedProperties();
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

        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(0f, -1.2f, 0f), new Vector3(0.7f, 0.7f, 0.7f), Quaternion.Euler(180f, 0, 0), pickup.transform);
    }

    private static void SetupFinishBase(Vector3 pos, Material metalMat, Material finishPadMat)
    {
        GameObject baseParent = new GameObject("Finish Base (LZ-Extraction)");

        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pedestal.name = "Landing Pedestal";
        pedestal.transform.SetParent(baseParent.transform);
        pedestal.transform.position = pos + new Vector3(0f, -0.6f, 0f);
        pedestal.transform.localScale = new Vector3(6.5f, 1.2f, 6.5f);
        ApplyMaterial(pedestal, metalMat);

        GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pad.name = "finish pad";
        pad.tag = "Finish";
        pad.transform.SetParent(baseParent.transform);
        pad.transform.position = pos;
        pad.transform.localScale = new Vector3(5.6f, 0.35f, 5.6f);
        ApplyMaterial(pad, finishPadMat);

        Collider[] oldCols = pad.GetComponents<Collider>();
        foreach (var c in oldCols) Object.DestroyImmediate(c);
        BoxCollider box = pad.AddComponent<BoxCollider>();
        box.size = Vector3.one;

        // Dual Rotating Radar Dishes!
        GameObject radar1 = SpawnModel("Mothership_02_Radar_Blue.fbx", pos + new Vector3(3.8f, 0.4f, 1.6f), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.identity, baseParent.transform);
        if (radar1 != null)
        {
            var rot1 = radar1.AddComponent<Rotator>();
            var so1 = new SerializedObject(rot1);
            so1.FindProperty("rotationSpeed").vector3Value = new Vector3(0f, 40f, 0f);
            so1.ApplyModifiedProperties();
        }

        GameObject radar2 = SpawnModel("Mothership_02_Radar_Blue.fbx", pos + new Vector3(-3.8f, 0.4f, 1.6f), new Vector3(1.1f, 1.1f, 1.1f), Quaternion.Euler(0, 180f, 0), baseParent.transform);
        if (radar2 != null)
        {
            var rot2 = radar2.AddComponent<Rotator>();
            var so2 = new SerializedObject(rot2);
            so2.FindProperty("rotationSpeed").vector3Value = new Vector3(0f, -40f, 0f);
            so2.ApplyModifiedProperties();
        }

        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(-2.8f, 0.1f, 2.2f), new Vector3(1.0f, 1.0f, 1.0f), Quaternion.identity, baseParent.transform);
        SpawnModel("Light_Glow_13.fbx", pos + new Vector3(2.8f, 0.1f, 2.2f), new Vector3(1.0f, 1.0f, 1.0f), Quaternion.identity, baseParent.transform);
        SpawnModel("Crate_02_2_Glow.fbx", pos + new Vector3(3.2f, 0f, -1.4f), new Vector3(0.9f, 0.9f, 0.9f), Quaternion.identity, baseParent.transform);
    }

    private static Camera SetupCamera(GameObject player)
    {
        GameObject mainCamObj = new GameObject("Camera");
        mainCamObj.tag = "MainCamera";
        Camera mainCam = mainCamObj.AddComponent<Camera>();
        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = new Color(0.06f, 0.08f, 0.16f);
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
        GameObject levelTitleObj = CreateUIText("LevelTitle", missionCard.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -12f), new Vector2(-24f, 26f), "SECTOR 03: DEFENSE NEXUS", 16, FontStyles.Bold, new Color(0.3f, 0.85f, 1.0f), TextAlignmentOptions.Center, font);
        gameUI.levelText = levelTitleObj.GetComponent<TextMeshProUGUI>();

        CreateUIText("MissionSubtitle", missionCard.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(-24f, 24f), "OBJECTIVE: SECURE EXTRACTION", 13, FontStyles.Normal, new Color(0.85f, 0.85f, 0.9f), TextAlignmentOptions.Center, font);

        // 3. BOTTOM-CENTER CONTROLS BADGE
        GameObject controlsCard = CreateUIPanel("ControlsBadge", canvasObj.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(500f, 40f), new Color(0.06f, 0.09f, 0.14f, 0.75f));
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
