using UnityEngine;
using Warcraft.Environment;
using Warcraft.Match;

namespace Warcraft.Environment
{
    /// <summary>
    /// Procedurally recreates the iconic cs_iceworld layout using simple primitives.
    /// Attach to an empty GameObject in an otherwise blank scene and toggle Build On Awake.
    /// </summary>
    public class IceworldMapBuilder : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private Vector2 arenaSize = new Vector2(40f, 40f);
        [SerializeField] private float wallHeight = 4f;
        [SerializeField] private float coverHeight = 2.5f;
        [SerializeField] private float platformThickness = 1f;
        [SerializeField] private float spawnOffset = 12f;
        [SerializeField] private float coverOffset = 6f;

        [Header("Materials")]
        [SerializeField] private Material floorMaterial;
        [SerializeField] private Material wallMaterial;
        [SerializeField] private Material coverMaterial;

        [Header("Build Options")]
        [SerializeField] private bool clearChildrenBeforeBuild = true;
        [SerializeField] private bool buildOnAwake = true;

        private void Awake()
        {
            if (buildOnAwake)
            {
                Build();
            }
        }

        [ContextMenu("Build Iceworld")]
        public void Build()
        {
            if (clearChildrenBeforeBuild)
            {
                ClearChildren();
            }

            BuildFloor();
            BuildPerimeterWalls();
            BuildCentralCover();
            BuildSpawnPlatforms();
            EnsureSpawnPoints();
        }

        private void ClearChildren()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    DestroyImmediate(child.gameObject);
                    continue;
                }
#endif
                Destroy(child.gameObject);
            }
        }

        private void BuildFloor()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(transform, false);
            floor.transform.localScale = new Vector3(arenaSize.x, platformThickness, arenaSize.y);
            floor.transform.localPosition = new Vector3(0f, -platformThickness * 0.5f, 0f);
            ApplyMaterial(floor, floorMaterial);
        }

        private void BuildPerimeterWalls()
        {
            // North wall
            CreateWall(new Vector3(0f, wallHeight * 0.5f, arenaSize.y * 0.5f), new Vector3(arenaSize.x, wallHeight, 1f));
            // South wall
            CreateWall(new Vector3(0f, wallHeight * 0.5f, -arenaSize.y * 0.5f), new Vector3(arenaSize.x, wallHeight, 1f));
            // East wall
            CreateWall(new Vector3(arenaSize.x * 0.5f, wallHeight * 0.5f, 0f), new Vector3(1f, wallHeight, arenaSize.y));
            // West wall
            CreateWall(new Vector3(-arenaSize.x * 0.5f, wallHeight * 0.5f, 0f), new Vector3(1f, wallHeight, arenaSize.y));
        }

        private void BuildCentralCover()
        {
            var offsets = new[]
            {
                new Vector3(coverOffset, coverHeight * 0.5f, coverOffset),
                new Vector3(-coverOffset, coverHeight * 0.5f, coverOffset),
                new Vector3(coverOffset, coverHeight * 0.5f, -coverOffset),
                new Vector3(-coverOffset, coverHeight * 0.5f, -coverOffset)
            };

            foreach (var offset in offsets)
            {
                var cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cover.name = "CoverBlock";
                cover.transform.SetParent(transform, false);
                cover.transform.localPosition = offset;
                cover.transform.localScale = new Vector3(3f, coverHeight, 1.2f);
                ApplyMaterial(cover, coverMaterial);
            }
        }

        private void BuildSpawnPlatforms()
        {
            CreatePlatform(new Vector3(0f, 0f, spawnOffset));
            CreatePlatform(new Vector3(0f, 0f, -spawnOffset));
            CreateRamp(new Vector3(0f, 0f, spawnOffset - 3f), true);
            CreateRamp(new Vector3(0f, 0f, -spawnOffset + 3f), false);
        }

        private void EnsureSpawnPoints()
        {
            EnsureSpawnPoint("TeamA_Spawn", Team.A, new Vector3(0f, 0f, spawnOffset));
            EnsureSpawnPoint("TeamB_Spawn", Team.B, new Vector3(0f, 0f, -spawnOffset));
        }

        private void CreateWall(Vector3 position, Vector3 scale)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "WallSegment";
            wall.transform.SetParent(transform, false);
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;
            ApplyMaterial(wall, wallMaterial);
        }

        private void CreatePlatform(Vector3 position)
        {
            var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = "SpawnPlatform";
            platform.transform.SetParent(transform, false);
            platform.transform.localPosition = position + Vector3.up * 0.5f;
            platform.transform.localScale = new Vector3(10f, 1f, 6f);
            ApplyMaterial(platform, floorMaterial);
        }

        private void CreateRamp(Vector3 position, bool forward)
        {
            var ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Ramp";
            ramp.transform.SetParent(transform, false);
            ramp.transform.localPosition = position + Vector3.up * 0.5f;
            ramp.transform.localScale = new Vector3(4f, 1f, 8f);
            ramp.transform.rotation = Quaternion.Euler(forward ? 20f : -20f, 0f, 0f);
            ApplyMaterial(ramp, floorMaterial);
        }

        private void EnsureSpawnPoint(string name, Team team, Vector3 position)
        {
            var existing = transform.Find(name);
            GameObject go;
            if (existing != null)
            {
                go = existing.gameObject;
            }
            else
            {
                go = new GameObject(name);
                go.transform.SetParent(transform, false);
            }

            go.transform.localPosition = position + Vector3.up * 1.1f;
            var spawn = go.GetComponent<TeamSpawnPoint>();
            if (spawn == null)
            {
                spawn = go.AddComponent<TeamSpawnPoint>();
            }

            spawn.SetTeam(team);
        }

        private static void ApplyMaterial(GameObject go, Material material)
        {
            if (material != null)
            {
                var renderer = go.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = material;
                }
            }
        }
    }
}
