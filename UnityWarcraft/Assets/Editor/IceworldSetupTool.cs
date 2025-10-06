#if UNITY_EDITOR
using System.IO;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Warcraft.Abilities;
using Warcraft.Bots;
using Warcraft.Characters;
using Warcraft.Core;
using Warcraft.Environment;
using Warcraft.Match;
using Warcraft.Weapons;

namespace Warcraft.EditorTools
{
    public static class IceworldSetupTool
    {
        private const string ScenePath = "Assets/Scenes/Iceworld.unity";
        private const string MatchSettingsPath = "Assets/Data/Match/AutoMatchSettings.asset";
        private const string RaceCatalogPath = "Assets/Data/Races/AutoRaceCatalog.asset";
        private const string RaceAssetPath = "Assets/Data/Races/HumanRace.asset";
        private const string OrcRaceAssetPath = "Assets/Data/Races/OrcRace.asset";
        private const string AbilityHealPath = "Assets/Data/Races/Abilities/Heal.asset";
        private const string AbilitySpeedBoostPath = "Assets/Data/Races/Abilities/SpeedBoost.asset";
        private const string AbilityDamageBoostPath = "Assets/Data/Races/Abilities/DamageBoost.asset";
        private const string WeaponCatalogPath = "Assets/Data/Weapons/AutoWeaponCatalog.asset";
        private const string WeaponAssetPath = "Assets/Data/Weapons/AK47.asset";
        private const string BotLoadoutSetPath = "Assets/Data/Bots/AutoBotLoadouts.asset";
        private const string BotProfilePath = "Assets/Data/Bots/AutoBotProfile.asset";
        private const string BotPrefabPath = "Assets/Prefabs/Characters/AutoBot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/AutoPlayer.prefab";
        private const string ProjectilePrefabPath = "Assets/Prefabs/Projectiles/Bullet.prefab";

        [MenuItem("Tools/Warcraft/Generate Iceworld Demo Scene", priority = 0)]
        public static void GenerateIceworldDemoScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            PrepareProjectFolders();

            var matchSettings = EnsureAsset<MatchSettings>(MatchSettingsPath, so =>
            {
                so.FindProperty("warmupDuration").floatValue = 15f;
                so.FindProperty("roundDuration").floatValue = 180f;
                so.FindProperty("intermissionDuration").floatValue = 10f;
                so.FindProperty("roundsToWin").intValue = 5;
                so.FindProperty("botsPerTeam").intValue = 5;
                so.FindProperty("fillWithBots").boolValue = true;
            });

            var raceDefinition = EnsureAsset<RaceDefinition>(RaceAssetPath, so =>
            {
                so.FindProperty("displayName").stringValue = "Human";
                so.FindProperty("lore").stringValue = "Balanced race for prototype matches.";
            });

            var healAbility = EnsureAsset<AbilityDefinition>(AbilityHealPath, so =>
            {
                so.FindProperty("displayName").stringValue = "Heal";
                so.FindProperty("description").stringValue = "Restores health over time.";
                so.FindProperty("targetType").enumValueIndex = (int)AbilityTargetType.Self;
                so.FindProperty("effectType").enumValueIndex = (int)AbilityEffectType.Heal;
                so.FindProperty("cooldownSeconds").floatValue = 15f;
                so.FindProperty("passiveValue").floatValue = 50f;
            });

            var damageBoostAbility = EnsureAsset<AbilityDefinition>(AbilityDamageBoostPath, so =>
            {
                so.FindProperty("displayName").stringValue = "Berserk";
                so.FindProperty("description").stringValue = "Temporarily increases damage output.";
                so.FindProperty("targetType").enumValueIndex = (int)AbilityTargetType.Self;
                so.FindProperty("effectType").enumValueIndex = (int)AbilityEffectType.DamageBoost;
                so.FindProperty("cooldownSeconds").floatValue = 25f;
                so.FindProperty("passiveValue").floatValue = 2f; // multiplier
            });

            var orcRaceDefinition = EnsureAsset<RaceDefinition>(OrcRaceAssetPath, so =>
            {
                so.FindProperty("displayName").stringValue = "Orc";
                so.FindProperty("lore").stringValue = "Aggressive race with high damage abilities.";
            });

            // Set up Orc levels
            var orcLevelsProperty = new SerializedObject(orcRaceDefinition).FindProperty("levels");
            orcLevelsProperty.ClearArray();
            orcLevelsProperty.InsertArrayElementAtIndex(0);
            var orcLevel1 = orcLevelsProperty.GetArrayElementAtIndex(0);
            orcLevel1.FindPropertyRelative("level").intValue = 1;
            orcLevel1.FindPropertyRelative("requiredXp").floatValue = 100f;
            var orcAbilities1 = orcLevel1.FindPropertyRelative("abilities");
            orcAbilities1.ClearArray();
            orcAbilities1.InsertArrayElementAtIndex(0);
            orcAbilities1.GetArrayElementAtIndex(0).objectReferenceValue = damageBoostAbility;

            new SerializedObject(orcRaceDefinition).ApplyModifiedProperties();

            // Set up race levels with abilities
            var levelsProperty = new SerializedObject(raceDefinition).FindProperty("levels");
            levelsProperty.ClearArray();
            levelsProperty.InsertArrayElementAtIndex(0);
            var level1 = levelsProperty.GetArrayElementAtIndex(0);
            level1.FindPropertyRelative("level").intValue = 1;
            level1.FindPropertyRelative("requiredXp").floatValue = 100f;
            var abilities1 = level1.FindPropertyRelative("abilities");
            abilities1.ClearArray();
            abilities1.InsertArrayElementAtIndex(0);
            abilities1.GetArrayElementAtIndex(0).objectReferenceValue = healAbility;

            levelsProperty.InsertArrayElementAtIndex(1);
            var level2 = levelsProperty.GetArrayElementAtIndex(1);
            level2.FindPropertyRelative("level").intValue = 2;
            level2.FindPropertyRelative("requiredXp").floatValue = 200f;
            var abilities2 = level2.FindPropertyRelative("abilities");
            abilities2.ClearArray();
            abilities2.InsertArrayElementAtIndex(0);
            abilities2.GetArrayElementAtIndex(0).objectReferenceValue = speedBoostAbility;

            new SerializedObject(raceDefinition).ApplyModifiedProperties();

            var raceCatalog = EnsureAsset<RaceCatalog>(RaceCatalogPath, so =>
            {
                var racesProperty = so.FindProperty("races");
                racesProperty.ClearArray();
                racesProperty.InsertArrayElementAtIndex(0);
                racesProperty.GetArrayElementAtIndex(0).objectReferenceValue = raceDefinition;
                racesProperty.InsertArrayElementAtIndex(1);
                racesProperty.GetArrayElementAtIndex(1).objectReferenceValue = orcRaceDefinition;
            });

            var weaponDefinition = EnsureAsset<WeaponDefinition>(WeaponAssetPath, so =>
            {
                so.FindProperty("displayName").stringValue = "Prototype Rifle";
                so.FindProperty("damage").floatValue = 20f;
                so.FindProperty("fireRate").floatValue = 0.12f;
                so.FindProperty("magazineSize").intValue = 30;
                so.FindProperty("reloadSeconds").floatValue = 2.1f;
                so.FindProperty("fireMode").enumValueIndex = (int)WeaponFireMode.FullAuto;
                so.FindProperty("recoilAmount").floatValue = 0.5f;
            });

            var projectilePrefab = EnsureProjectilePrefab();
            SetSerialized(weaponDefinition, so =>
            {
                so.FindProperty("projectilePrefab").objectReferenceValue = projectilePrefab;
            });

            var weaponCatalog = EnsureAsset<WeaponCatalog>(WeaponCatalogPath, so =>
            {
                var weaponsProperty = so.FindProperty("weapons");
                weaponsProperty.ClearArray();
                weaponsProperty.InsertArrayElementAtIndex(0);
                weaponsProperty.GetArrayElementAtIndex(0).objectReferenceValue = weaponDefinition;
            });

            var orcBotProfile = EnsureAsset<BotProfile>(BotProfilePath.Replace("AutoBotProfile", "OrcBotProfile"), so =>
            {
                so.FindProperty("displayName").stringValue = "Orc Bot";
                so.FindProperty("race").objectReferenceValue = orcRaceDefinition;
                so.FindProperty("primaryWeapon").objectReferenceValue = weaponDefinition;
                so.FindProperty("preferredTeam").enumValueIndex = (int)Team.B;
            });

            var botLoadoutSet = EnsureAsset<BotLoadoutSet>(BotLoadoutSetPath, so =>
            {
                var profilesProperty = so.FindProperty("profiles");
                profilesProperty.ClearArray();
                profilesProperty.InsertArrayElementAtIndex(0);
                profilesProperty.GetArrayElementAtIndex(0).objectReferenceValue = botProfile;
                profilesProperty.InsertArrayElementAtIndex(1);
                profilesProperty.GetArrayElementAtIndex(1).objectReferenceValue = orcBotProfile;
            });

            var botPrefab = EnsureBotPrefab();
            var playerPrefab = EnsurePlayerPrefab();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Iceworld";

            var environmentRoot = new GameObject("EnvironmentRoot");
            var builder = environmentRoot.AddComponent<IceworldMapBuilder>();
            builder.Build();
            environmentRoot.AddComponent<NavMeshSurface>();
            environmentRoot.AddComponent<NavMeshRuntimeBaker>();

            // Add spawn points
            CreateSpawnPoints(environmentRoot.transform);

            var botsContainer = new GameObject("Bots").transform;
            var botManager = new GameObject("BotManager").AddComponent<BotManager>();
            SetSerialized(botManager, so =>
            {
                so.FindProperty("botPrefab").objectReferenceValue = botPrefab;
                so.FindProperty("botContainer").objectReferenceValue = botsContainer;
            });

            var matchManager = new GameObject("MatchManager").AddComponent<MatchManager>();
            SetSerialized(matchManager, so =>
            {
                so.FindProperty("fallbackSettings").objectReferenceValue = matchSettings;
                so.FindProperty("botManager").objectReferenceValue = botManager;
            });

            var bootstrap = new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
            SetSerialized(bootstrap, so =>
            {
                so.FindProperty("matchSettings").objectReferenceValue = matchSettings;
                so.FindProperty("raceCatalog").objectReferenceValue = raceCatalog;
                so.FindProperty("weaponCatalog").objectReferenceValue = weaponCatalog;
                so.FindProperty("botLoadoutSet").objectReferenceValue = botLoadoutSet;
                so.FindProperty("matchManager").objectReferenceValue = matchManager;
            });

            if (playerPrefab != null)
            {
                PrefabUtility.InstantiatePrefab(playerPrefab);
            }

            var hudCanvas = CreateHUDCanvas();
            var hudManager = hudCanvas.AddComponent<HUDManager>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Iceworld demo scene generated successfully.");
        }

        private static void PrepareProjectFolders()
        {
            Directory.CreateDirectory("Assets/Data/Match");
            Directory.CreateDirectory("Assets/Data/Races");
            Directory.CreateDirectory("Assets/Data/Races/Abilities");
            Directory.CreateDirectory("Assets/Data/Weapons");
            Directory.CreateDirectory("Assets/Data/Bots");
            Directory.CreateDirectory("Assets/Prefabs/Characters");
            Directory.CreateDirectory("Assets/Prefabs/Projectiles");
            Directory.CreateDirectory("Assets/Scenes");
        }

        private static T EnsureAsset<T>(string assetPath, System.Action<SerializedObject> configure) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                var directory = Path.GetDirectoryName(assetPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                AssetDatabase.CreateAsset(asset, assetPath);
            }

            if (configure != null)
            {
                var so = new SerializedObject(asset);
                configure(so);
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
            }

            return asset;
        }

        private static void SetSerialized(Object target, System.Action<SerializedObject> configure)
        {
            var so = new SerializedObject(target);
            configure(so);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private static GameObject EnsureBotPrefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BotPrefabPath);
            if (prefab != null)
            {
                return prefab;
            }

            var root = new GameObject("Bot");
            root.tag = "Untagged";

            var controller = root.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.4f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            var agent = root.AddComponent<NavMeshAgent>();
            agent.speed = 5.5f;
            agent.angularSpeed = 720f;
            agent.acceleration = 24f;

            root.AddComponent<CharacterMotor>();
            var combat = root.AddComponent<CharacterCombat>();
            root.AddComponent<CharacterHealth>();
            root.AddComponent<AbilityController>();
            root.AddComponent<BotBrain>();

            var weaponSocket = new GameObject("WeaponSocket");
            weaponSocket.transform.SetParent(root.transform);
            weaponSocket.transform.localPosition = new Vector3(0.3f, 1.2f, 0.4f);
            SetSerialized(combat, so =>
            {
                so.FindProperty("weaponSocket").objectReferenceValue = weaponSocket.transform;
            });

            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localPosition = Vector3.zero;
            Object.DestroyImmediate(visual.GetComponent<Collider>());

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, BotPrefabPath);
            Object.DestroyImmediate(root);
            return savedPrefab;
        }

        private static GameObject EnsurePlayerPrefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (prefab != null)
            {
                return prefab;
            }

            var inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath);
            if (inputAsset == null)
            {
                Debug.LogWarning("PlayerInput asset not found. Player prefab will not be generated.");
                return null;
            }

            var root = new GameObject("Player");

            var controller = root.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.4f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            root.AddComponent<CharacterMotor>();
            var combat = root.AddComponent<CharacterCombat>();
            root.AddComponent<CharacterHealth>();
            root.AddComponent<AbilityController>();

            var cameraPivot = new GameObject("CameraPivot").transform;
            cameraPivot.SetParent(root.transform);
            cameraPivot.localPosition = new Vector3(0f, 1.6f, 0f);

            var camera = new GameObject("Camera").AddComponent<Camera>();
            camera.transform.SetParent(cameraPivot, false);
            camera.transform.localPosition = Vector3.zero;

            root.AddComponent<CameraRig>();
            SetSerialized(root.GetComponent<CameraRig>(), so =>
            {
                so.FindProperty("cameraTransform").objectReferenceValue = camera.transform;
            });

            SetSerialized(root.GetComponent<CharacterMotor>(), so =>
            {
                so.FindProperty("cameraPivot").objectReferenceValue = cameraPivot;
            });

            var weaponSocket = new GameObject("WeaponSocket");
            weaponSocket.transform.SetParent(root.transform);
            weaponSocket.transform.localPosition = new Vector3(0.3f, 1.2f, 0.4f);
            SetSerialized(combat, so =>
            {
                so.FindProperty("weaponSocket").objectReferenceValue = weaponSocket.transform;
            });

            var playerInput = root.AddComponent<PlayerInput>();
            playerInput.actions = inputAsset;
            playerInput.defaultActionMap = "Player";
            root.AddComponent<PlayerInputHandler>();

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            Object.DestroyImmediate(root);
            return savedPrefab;
        }

        private static GameObject EnsureProjectilePrefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ProjectilePrefabPath);
            if (prefab != null)
            {
                return prefab;
            }

            var root = new GameObject("Bullet");
            root.AddComponent<Projectile>();
            var collider = root.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.05f;

            var visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Object.DestroyImmediate(visual.GetComponent<SphereCollider>());

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, ProjectilePrefabPath);
            Object.DestroyImmediate(root);
            return savedPrefab;
        }

        private static GameObject CreateHUDCanvas()
        {
            var canvas = new GameObject("HUDCanvas");
            var canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            var healthText = CreateTMPText("HealthText", canvas.transform, new Vector2(10, -10), "Health: 100");
            var shieldText = CreateTMPText("ShieldText", canvas.transform, new Vector2(10, -40), "Shield: 0");
            var xpText = CreateTMPText("XPText", canvas.transform, new Vector2(10, -70), "XP: 0");
            var roundTimerText = CreateTMPText("RoundTimerText", canvas.transform, new Vector2(-10, -10), "Time: 180", TextAlignmentOptions.TopRight);

            var hudManager = canvas.AddComponent<HUDManager>();
            SetSerialized(hudManager, so =>
            {
                so.FindProperty("healthText").objectReferenceValue = healthText;
                so.FindProperty("shieldText").objectReferenceValue = shieldText;
                so.FindProperty("xpText").objectReferenceValue = xpText;
                so.FindProperty("roundTimerText").objectReferenceValue = roundTimerText;
            });

            return canvas;
        }

        private static TMP_Text CreateTMPText(string name, Transform parent, Vector2 anchoredPosition, string text, TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            var rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(200, 30);
            if (alignment == TextAlignmentOptions.TopRight)
            {
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(1, 1);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
            }

            var tmpText = textObj.AddComponent<TMP_Text>();
            tmpText.text = text;
            tmpText.fontSize = 24;
            tmpText.color = Color.white;
            tmpText.alignment = alignment;
            return tmpText;
        }

        private static void CreateSpawnPoints(Transform environmentRoot)
        {
            // Team A spawns (left side)
            var spawnA1 = new GameObject("SpawnPoint_A_1").AddComponent<TeamSpawnPoint>();
            spawnA1.transform.SetParent(environmentRoot);
            spawnA1.transform.position = new Vector3(-20, 0, 0);
            spawnA1.Team = Team.A;

            var spawnA2 = new GameObject("SpawnPoint_A_2").AddComponent<TeamSpawnPoint>();
            spawnA2.transform.SetParent(environmentRoot);
            spawnA2.transform.position = new Vector3(-20, 0, 10);
            spawnA2.Team = Team.A;

            // Team B spawns (right side)
            var spawnB1 = new GameObject("SpawnPoint_B_1").AddComponent<TeamSpawnPoint>();
            spawnB1.transform.SetParent(environmentRoot);
            spawnB1.transform.position = new Vector3(20, 0, 0);
            spawnB1.Team = Team.B;

            var spawnB2 = new GameObject("SpawnPoint_B_2").AddComponent<TeamSpawnPoint>();
            spawnB2.transform.SetParent(environmentRoot);
            spawnB2.transform.position = new Vector3(20, 0, -10);
            spawnB2.Team = Team.B;
        }
    }
}
#endif
