using UnityEngine;
using Warcraft.Abilities;
using Warcraft.Match;
using Warcraft.Weapons;

namespace Warcraft.Bots
{
    [CreateAssetMenu(menuName = "Warcraft/Bots/Bot Profile", fileName = "BotProfile")]
    public class BotProfile : ScriptableObject
    {
        [SerializeField] private string botId = System.Guid.NewGuid().ToString();
        [SerializeField] private string displayName = "Bot";
        [SerializeField] private RaceDefinition race;
        [SerializeField] private WeaponDefinition primaryWeapon;
        [SerializeField] private WeaponDefinition secondaryWeapon;
        [SerializeField] private Team preferredTeam = Team.A;

        public string BotId => botId;
        public string DisplayName => displayName;
        public RaceDefinition Race => race;
        public WeaponDefinition PrimaryWeapon => primaryWeapon;
        public WeaponDefinition SecondaryWeapon => secondaryWeapon;
        public Team PreferredTeam => preferredTeam;
    }
}
