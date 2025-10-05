using System.Collections.Generic;
using UnityEngine;
using Warcraft.Abilities;
using Warcraft.Environment;
using Warcraft.Match;

namespace Warcraft.Bots
{
    public class BotManager : MonoBehaviour
    {
        [SerializeField] private GameObject botPrefab;
        [SerializeField] private Transform botContainer;
        [SerializeField] private List<TeamSpawnPoint> spawnPoints = new();

        private readonly List<BotBrain> _teamABots = new();
        private readonly List<BotBrain> _teamBBots = new();

        private MatchSettings _settings;
        private BotLoadoutSet _loadouts;
        private XPService _xpService;

        public void Configure(MatchSettings settings, BotLoadoutSet loadouts, XPService xpService)
        {
            _settings = settings;
            _loadouts = loadouts;
            _xpService = xpService;
        }

        public void SpawnTeams()
        {
            ClearExistingBots();

            if (botPrefab == null)
            {
                Debug.LogWarning("BotManager requires a bot prefab to spawn bots.");
                return;
            }

            SpawnTeam(Team.A, _teamABots);
            SpawnTeam(Team.B, _teamBBots);
        }

        public Team GetLeadingTeam()
        {
            var aliveA = CountAlive(_teamABots);
            var aliveB = CountAlive(_teamBBots);

            if (aliveA == aliveB)
            {
                return Team.None;
            }

            return aliveA > aliveB ? Team.A : Team.B;
        }

        private void SpawnTeam(Team team, List<BotBrain> collection)
        {
            var spawns = GetSpawnsForTeam(team);
            var botsToSpawn = _settings != null ? _settings.BotsPerTeam : spawns.Count;

            for (var i = 0; i < botsToSpawn; i++)
            {
                var spawn = spawns.Count > 0 ? spawns[i % spawns.Count] : null;
                var position = spawn != null ? spawn.transform.position : transform.position;
                var rotation = spawn != null ? spawn.transform.rotation : transform.rotation;

                var parent = botContainer == null ? transform : botContainer;
                var instance = Instantiate(botPrefab, position, rotation, parent);
                var brain = instance.GetComponent<BotBrain>();

                if (brain == null)
                {
                    Debug.LogError("Bot prefab is missing BotBrain component.");
                    Destroy(instance);
                    continue;
                }

                var profile = _loadouts?.GetRandomProfile(team);
                brain.Initialize(profile, _xpService, team);
                collection.Add(brain);
            }
        }

        private List<TeamSpawnPoint> GetSpawnsForTeam(Team team)
        {
            if (spawnPoints.Count == 0)
            {
                spawnPoints.AddRange(FindObjectsOfType<TeamSpawnPoint>());
            }

            List<TeamSpawnPoint> result = new();
            foreach (var spawn in spawnPoints)
            {
                if (spawn != null && spawn.Team == team)
                {
                    result.Add(spawn);
                }
            }

            return result;
        }

        private static int CountAlive(List<BotBrain> bots)
        {
            var alive = 0;
            foreach (var bot in bots)
            {
                if (bot != null && bot.isActiveAndEnabled)
                {
                    var health = bot.GetComponent<Warcraft.Characters.CharacterHealth>();
                    if (health == null || health.IsAlive)
                    {
                        alive++;
                    }
                }
            }

            return alive;
        }

        private void ClearExistingBots()
        {
            var parent = botContainer == null ? transform : botContainer;

            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }

            _teamABots.Clear();
            _teamBBots.Clear();
        }
    }
}
