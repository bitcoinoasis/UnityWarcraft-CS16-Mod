#if UNITY_EDITOR
using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
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
        private const string WeaponCatalogPath = "Assets/Data/Weapons/AutoWeaponCatalog.asset";
        private const string WeaponAssetPath = "Assets/Data/Weapons/AK47.asset";
        private const string BotLoadoutSetPath = "Assets/Data/Bots/AutoBotLoadouts.asset";
        private const string BotProfilePath = "Assets/Data/Bots/AutoBotProfile.asset";
        private const string BotPrefabPath = "Assets/Prefabs/Characters/AutoBot.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Characters/AutoPlayer.prefab";
        private const string InputAssetPath = "Assets/Settings/Input/PlayerInput.inputactions";

        [MenuItem("Warcraft/Tools/Generate Iceworld Demo Scene", priority = 0)]
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

            var raceCatalog = EnsureAsset<RaceCatalog>(RaceCatalogPath, so =>
            {
                var racesProperty = so.FindProperty("races");
                racesProperty.ClearArray();
                racesProperty.InsertArrayElementAtIndex(0);
                racesProperty.GetArrayElementAtIndex(0).objectReferenceValue = raceDefinition;
            });

            var weaponDefinition = EnsureAsset<WeaponDefinition>(WeaponAssetPath, so =>
            {
                so.FindProperty("displayName").stringValue = "Prototype Rifle";
                so.FindProperty("damage").floatValue = 20f;
                so.FindProperty("fireRate").floatValue = 0.12f;
                so.FindProperty("magazineSize").intValue = 30;
                so.FindProperty("reloadSeconds").floatValue = 2.1f;
                so.FindProperty("fireMode").enumValueIndex = (int)WeaponFireMode.FullAuto;
            });

            var weaponCatalog = EnsureAsset<WeaponCatalog>(WeaponCatalogPath, so =>
            {
                var weaponsProperty = so.FindProperty("weapons");
                weaponsProperty.ClearArray();
                weaponsProperty.InsertArrayElementAtIndex(0);
                weaponsProperty.GetArrayElementAtIndex(0).objectReferenceValue = weaponDefinition;
            });

            var botProfile = EnsureAsset<BotProfile>(BotProfilePath, so =>
            {
                so.FindProperty("displayName").stringValue = "Auto Bot";
                so.FindProperty("race").objectReferenceValue = raceDefinition;
                so.FindProperty("primaryWeapon").objectReferenceValue = weaponDefinition;
                so.FindProperty("preferredTeam").enumValueIndex = (int)Team.A;
            });

            var botLoadoutSet = EnsureAsset<BotLoadoutSet>(BotLoadoutSetPath, so =>
            {
                var profilesProperty = so.FindProperty("profiles");
                profilesProperty.ClearArray();
                profilesProperty.InsertArrayElementAtIndex(0);
                profilesProperty.GetArrayElementAtIndex(0).objectReferenceValue = botProfile;
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

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Iceworld demo scene generated successfully.");
        }

        private static void PrepareProjectFolders()
        {
            Directory.CreateDirectory("Assets/Data/Match");
            Directory.CreateDirectory("Assets/Data/Races");
            Directory.CreateDirectory("Assets/Data/Weapons");
            Directory.CreateDirectory("Assets/Data/Bots");
            Directory.CreateDirectory("Assets/Prefabs/Characters");
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
    }
}
#endif
