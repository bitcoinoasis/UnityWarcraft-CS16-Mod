using System.Collections.Generic;
using UnityEngine;
using Warcraft.Match;

namespace Warcraft.Bots
{
    [CreateAssetMenu(menuName = "Warcraft/Bots/Bot Loadout Set", fileName = "BotLoadoutSet")]
    public class BotLoadoutSet : ScriptableObject
    {
        [SerializeField] private List<BotProfile> profiles = new();

        public IReadOnlyList<BotProfile> Profiles => profiles;

        public BotProfile GetRandomProfile(Team team)
        {
            var candidates = new List<BotProfile>();
            foreach (var profile in profiles)
            {
                if (profile == null)
                {
                    continue;
                }

                if (team == Team.None || profile.PreferredTeam == team)
                {
                    candidates.Add(profile);
                }
            }

            if (candidates.Count == 0)
            {
                return null;
            }

            var index = Random.Range(0, candidates.Count);
            return candidates[index];
        }
    }
}
