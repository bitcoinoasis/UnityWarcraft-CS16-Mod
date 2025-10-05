using UnityEngine;
using Warcraft.Match;

namespace Warcraft.Environment
{
    public class TeamSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Team team = Team.None;

        public Team Team => team;

        public void SetTeam(Team value)
        {
            team = value;
        }
    }
}
