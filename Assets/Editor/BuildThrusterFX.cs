using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BuildThrusterFX
{
    private const string MAT_FOLDER = "Assets/GameDevTV Assets/Materials and Textures";

    [MenuItem("Tools/Build Spaceship Thrusters For All Scenes")]
    public static void BuildAll()
    {
        Material coreMat = CreateOrGetParticleMaterial("M_Thruster_CorePlasma.mat", new Color(0f, 0.95f, 1f, 1f), BlendMode.Additive);
        Material sparkMat = CreateOrGetParticleMaterial("M_Thruster_Sparks.mat", new Color(1f, 0.98f, 0.8f, 1f), BlendMode.Additive);
        Material smokeMat = CreateOrGetParticleMaterial("M_Thruster_Smoke.mat", new Color(0.15f, 0.45f, 0.85f, 0.35f), BlendMode.Alpha);

        // 1. Setup on Prefab
        string prefabPath = "Assets/Prefabs/Player Rocket 1.prefab";
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        if (prefabRoot != null)
        {
            SetupThrusterOnRocket(prefabRoot, coreMat, sparkMat, smokeMat);
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            Debug.Log("[BuildThrusterFX] Prefab successfully updated with Thruster FX.");
        }

        // 2. Setup across all 3 levels
        string[] scenes = new string[] {
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };

        for (int i = 0; i < scenes.Length; i++)
        {
            string sPath = scenes[i];
            var scene = EditorSceneManager.OpenScene(sPath, OpenSceneMode.Single);
            GameObject rocket = GameObject.Find("Player Rocket");
            if (rocket != null)
            {
                SetupThrusterOnRocket(rocket, coreMat, sparkMat, smokeMat);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene, sPath);
                Debug.Log($"[BuildThrusterFX] Thrusters configured for {sPath}");
            }
        }

        // Return to Level 1
        EditorSceneManager.OpenScene("Assets/Scenes/Level1.unity", OpenSceneMode.Single);
        Debug.Log("[BuildThrusterFX] Complete! Returned to Level 1.");
    }

    private enum BlendMode { Additive, Alpha }

    private static Material CreateOrGetParticleMaterial(string fileName, Color baseColor, BlendMode blendMode)
    {
        string fullPath = $"{MAT_FOLDER}/{fileName}";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(fullPath);
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Particles/Standard Unlit");

        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, fullPath);
        }
        else
        {
            mat.shader = shader;
        }

        Texture2D softGlow = AssetDatabase.LoadAssetAtPath<Texture2D>($"{MAT_FOLDER}/Tex_SoftGlow.png");
        if (softGlow == null) softGlow = AssetDatabase.GetBuiltinExtraResource<Texture2D>("Default-Particle.psd");
        
        if (softGlow != null)
        {
            mat.mainTexture = softGlow;
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", softGlow);
        }

        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", baseColor);

        // Configure URP blend properties
        if (blendMode == BlendMode.Additive)
        {
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 1); // Additive
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
            mat.SetFloat("_ZWrite", 0);
            mat.renderQueue = 3000;
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.EnableKeyword("_BLENDMODE_ADD");
        }
        else
        {
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 0); // Alpha
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_ZWrite", 0);
            mat.renderQueue = 3000;
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_BLENDMODE_ADD");
        }

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        return mat;
    }

    public static void SetupThrusterOnRocket(GameObject rocket, Material coreMat, Material sparkMat, Material smokeMat)
    {
        // 1. Remove old FX if present
        Transform oldMain = rocket.transform.Find("Thruster_FX");
        if (oldMain != null) Object.DestroyImmediate(oldMain.gameObject);

        Transform oldL = rocket.transform.Find("RCS_Left");
        if (oldL != null) Object.DestroyImmediate(oldL.gameObject);

        Transform oldR = rocket.transform.Find("RCS_Right");
        if (oldR != null) Object.DestroyImmediate(oldR.gameObject);

        // 2. Main Thruster FX Root (Core Plasma Jet)
        GameObject thrusterObj = new GameObject("Thruster_FX");
        thrusterObj.transform.SetParent(rocket.transform, false);
        thrusterObj.transform.localPosition = new Vector3(0.08f, 0.05f, -0.17f);
        thrusterObj.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

        ParticleSystem corePS = thrusterObj.AddComponent<ParticleSystem>();
        var main = corePS.main;
        main.playOnAwake = false;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.12f, 0.22f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(10f, 16f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.42f, 0.65f);
        main.startColor = new Color(0f, 0.95f, 1f, 1f);
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;

        var emission = corePS.emission;
        emission.rateOverTime = 130f;

        var shape = corePS.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 4f;
        shape.radius = 0.12f;

        // Color Over Lifetime
        var col = corePS.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 1f, 1f), 0.0f),
                new GradientColorKey(new Color(0f, 0.95f, 1f), 0.30f),
                new GradientColorKey(new Color(0.05f, 0.45f, 1f), 0.70f),
                new GradientColorKey(new Color(0.35f, 0.05f, 0.9f), 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.9f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        col.color = grad;

        // Size Over Lifetime
        var sol = corePS.sizeOverLifetime;
        sol.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0.0f, 1.0f);
        sizeCurve.AddKey(0.4f, 0.85f);
        sizeCurve.AddKey(1.0f, 0.25f);
        sol.size = new ParticleSystem.MinMaxCurve(1.0f, sizeCurve);

        var rend = thrusterObj.GetComponent<ParticleSystemRenderer>();
        rend.material = coreMat;

        // 2b. Child: Inner Hot Flame Needle (White-Hot Core)
        GameObject innerCoreObj = new GameObject("InnerCore");
        innerCoreObj.transform.SetParent(thrusterObj.transform, false);
        innerCoreObj.transform.localPosition = Vector3.zero;
        innerCoreObj.transform.localRotation = Quaternion.identity;

        ParticleSystem innerPS = innerCoreObj.AddComponent<ParticleSystem>();
        var imain = innerPS.main;
        imain.playOnAwake = false;
        imain.loop = true;
        imain.startLifetime = new ParticleSystem.MinMaxCurve(0.07f, 0.13f);
        imain.startSpeed = new ParticleSystem.MinMaxCurve(14f, 20f);
        imain.startSize = new ParticleSystem.MinMaxCurve(0.20f, 0.32f);
        imain.startColor = new Color(1f, 1f, 1f, 1f);
        imain.simulationSpace = ParticleSystemSimulationSpace.Local;

        var iemission = innerPS.emission;
        iemission.rateOverTime = 110f;

        var ishape = innerPS.shape;
        ishape.shapeType = ParticleSystemShapeType.Cone;
        ishape.angle = 2f;
        ishape.radius = 0.07f;

        var isol = innerPS.sizeOverLifetime;
        isol.enabled = true;
        AnimationCurve innerSizeCurve = new AnimationCurve();
        innerSizeCurve.AddKey(0.0f, 1.0f);
        innerSizeCurve.AddKey(1.0f, 0.15f);
        isol.size = new ParticleSystem.MinMaxCurve(1.0f, innerSizeCurve);

        var irend = innerCoreObj.GetComponent<ParticleSystemRenderer>();
        irend.material = sparkMat;

        // 3. Child: Sparks
        GameObject sparksObj = new GameObject("Sparks");
        sparksObj.transform.SetParent(thrusterObj.transform, false);
        sparksObj.transform.localPosition = Vector3.zero;
        sparksObj.transform.localRotation = Quaternion.identity;

        ParticleSystem sparkPS = sparksObj.AddComponent<ParticleSystem>();
        var smain = sparkPS.main;
        smain.playOnAwake = false;
        smain.loop = true;
        smain.startLifetime = new ParticleSystem.MinMaxCurve(0.2f, 0.45f);
        smain.startSpeed = new ParticleSystem.MinMaxCurve(12f, 22f);
        smain.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.12f);
        smain.startColor = new Color(1f, 0.98f, 0.75f, 1f);
        smain.simulationSpace = ParticleSystemSimulationSpace.World;

        var semission = sparkPS.emission;
        semission.rateOverTime = 45f;

        var sshape = sparkPS.shape;
        sshape.shapeType = ParticleSystemShapeType.Cone;
        sshape.angle = 10f;
        sshape.radius = 0.12f;

        var srend = sparksObj.GetComponent<ParticleSystemRenderer>();
        srend.material = sparkMat;
        srend.renderMode = ParticleSystemRenderMode.Stretch;
        srend.velocityScale = 0.12f;
        srend.lengthScale = -2.8f;

        // 4. Child: Vapor Trail
        GameObject smokeObj = new GameObject("VaporTrail");
        smokeObj.transform.SetParent(thrusterObj.transform, false);
        smokeObj.transform.localPosition = Vector3.zero;
        smokeObj.transform.localRotation = Quaternion.identity;

        ParticleSystem smokePS = smokeObj.AddComponent<ParticleSystem>();
        var msmoke = smokePS.main;
        msmoke.playOnAwake = false;
        msmoke.loop = true;
        msmoke.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 0.85f);
        msmoke.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 5.5f);
        msmoke.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
        msmoke.startColor = new Color(0.2f, 0.6f, 1f, 0.35f);
        msmoke.simulationSpace = ParticleSystemSimulationSpace.World;

        var emissionSmoke = smokePS.emission;
        emissionSmoke.rateOverTime = 25f;

        var shapeSmoke = smokePS.shape;
        shapeSmoke.shapeType = ParticleSystemShapeType.Cone;
        shapeSmoke.angle = 14f;
        shapeSmoke.radius = 0.15f;

        var solSmoke = smokePS.sizeOverLifetime;
        solSmoke.enabled = true;
        AnimationCurve smokeSizeCurve = new AnimationCurve();
        smokeSizeCurve.AddKey(0f, 0.8f);
        smokeSizeCurve.AddKey(1f, 2.2f);
        solSmoke.size = new ParticleSystem.MinMaxCurve(1.0f, smokeSizeCurve);

        var colSmoke = smokePS.colorOverLifetime;
        colSmoke.enabled = true;
        Gradient smokeGrad = new Gradient();
        smokeGrad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.2f, 0.7f, 1f), 0f), new GradientColorKey(new Color(0.1f, 0.3f, 0.8f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.35f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colSmoke.color = smokeGrad;

        var smokeRend = smokeObj.GetComponent<ParticleSystemRenderer>();
        smokeRend.material = smokeMat;

        // 5. Dynamic Thruster Light
        GameObject lightObj = new GameObject("ThrusterLight");
        lightObj.transform.SetParent(thrusterObj.transform, false);
        lightObj.transform.localPosition = new Vector3(0f, 0f, 0.1f);
        Light tLight = lightObj.AddComponent<Light>();
        tLight.type = LightType.Point;
        tLight.color = new Color(0f, 0.9f, 1f);
        tLight.intensity = 3.5f;
        tLight.range = 6.0f;
        tLight.enabled = false;

        // 6. Side RCS Boosters
        GameObject rcsLeft = CreateRCSBooster("RCS_Left", rocket.transform, new Vector3(-0.95f, 0.45f, -0.15f), new Vector3(0f, -90f, 0f), coreMat);
        GameObject rcsRight = CreateRCSBooster("RCS_Right", rocket.transform, new Vector3(1.15f, 0.45f, -0.15f), new Vector3(0f, 90f, 0f), coreMat);

        // 7. Wire into Movement component
        movement mov = rocket.GetComponent<movement>();
        if (mov != null)
        {
            SerializedObject so = new SerializedObject(mov);
            so.FindProperty("mainThrusterParticles").objectReferenceValue = corePS;
            so.FindProperty("leftThrusterParticles").objectReferenceValue = rcsLeft.GetComponent<ParticleSystem>();
            so.FindProperty("rightThrusterParticles").objectReferenceValue = rcsRight.GetComponent<ParticleSystem>();
            so.FindProperty("thrusterLight").objectReferenceValue = tLight;
            so.ApplyModifiedProperties();
        }
    }

    private static GameObject CreateRCSBooster(string name, Transform parent, Vector3 localPos, Vector3 localRot, Material mat)
    {
        GameObject rcs = new GameObject(name);
        rcs.transform.SetParent(parent, false);
        rcs.transform.localPosition = localPos;
        rcs.transform.localEulerAngles = localRot;

        ParticleSystem ps = rcs.AddComponent<ParticleSystem>();
        var m = ps.main;
        m.playOnAwake = false;
        m.loop = true;
        m.startLifetime = new ParticleSystem.MinMaxCurve(0.08f, 0.15f);
        m.startSpeed = new ParticleSystem.MinMaxCurve(5f, 9f);
        m.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.22f);
        m.startColor = new Color(0.2f, 0.9f, 1f, 0.85f);
        m.simulationSpace = ParticleSystemSimulationSpace.Local;

        var em = ps.emission;
        em.rateOverTime = 40f;

        var sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Cone;
        sh.angle = 8f;
        sh.radius = 0.04f;

        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        AnimationCurve rcsCurve = new AnimationCurve();
        rcsCurve.AddKey(0f, 1.0f);
        rcsCurve.AddKey(1f, 0.2f);
        sol.size = new ParticleSystem.MinMaxCurve(1.0f, rcsCurve);

        var r = rcs.GetComponent<ParticleSystemRenderer>();
        r.material = mat;

        ps.Stop();
        return rcs;
    }
}
