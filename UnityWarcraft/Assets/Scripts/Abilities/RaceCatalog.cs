using System.Collections.Generic;
using UnityEngine;

namespace Warcraft.Abilities
{
    [CreateAssetMenu(menuName = "Warcraft/Abilities/Race Catalog", fileName = "RaceCatalog")]
    public class RaceCatalog : ScriptableObject
    {
        [SerializeField] private List<RaceDefinition> races = new();

        public IReadOnlyList<RaceDefinition> Races => races;

        public RaceDefinition GetById(string id)
        {
            foreach (var race in races)
            {
                if (race != null && race.RaceId == id)
                {
                    return race;
                }
            }

            return null;
        }

        public RaceDefinition GetByName(string name)
        {
            foreach (var race in races)
            {
                if (race != null && race.DisplayName == name)
                {
                    return race;
                }
            }

            return null;
        }
    }
}
