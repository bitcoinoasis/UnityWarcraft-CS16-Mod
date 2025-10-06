using System;
using System.Collections.Generic;
using UnityEngine;
using Warcraft.Abilities;

namespace Warcraft.Match
{
    public struct LevelProgress
    {
        public int Level;
        public float XP;
        public float RequiredXP;
    }

    public class XPService : MonoBehaviour
    {
        private readonly Dictionary<int, RaceDefinition> _registeredEntities = new();
        private readonly Dictionary<int, float> _entityXP = new();

        public event Action<int, LevelProgress> OnLevelUp;
        public event Action<int, float> OnXPChanged;

        public void Register(int entityId, RaceDefinition race)
        {
            _registeredEntities[entityId] = race;
            _entityXP[entityId] = 0f;
        }

        public void Unregister(int entityId)
        {
            _registeredEntities.Remove(entityId);
            _entityXP.Remove(entityId);
        }

        public void AwardXP(int entityId, float amount)
        {
            if (!_entityXP.ContainsKey(entityId))
            {
                return;
            }

            _entityXP[entityId] += amount;
            OnXPChanged?.Invoke(entityId, _entityXP[entityId]);

            CheckLevelUp(entityId);
        }

        private void CheckLevelUp(int entityId)
        {
            if (!_registeredEntities.TryGetValue(entityId, out var race))
            {
                return;
            }

            var currentXP = _entityXP[entityId];
            var currentLevel = GetCurrentLevel(race, currentXP);

            if (currentLevel < race.Levels.Count)
            {
                var nextLevelXP = race.Levels[currentLevel].RequiredXp;
                if (currentXP >= nextLevelXP)
                {
                    var progress = new LevelProgress
                    {
                        Level = currentLevel + 1,
                        XP = currentXP,
                        RequiredXP = nextLevelXP
                    };

                    OnLevelUp?.Invoke(entityId, progress);
                }
            }
        }

        private static int GetCurrentLevel(RaceDefinition race, float xp)
        {
            for (var i = 0; i < race.Levels.Count; i++)
            {
                if (xp < race.Levels[i].RequiredXp)
                {
                    return i;
                }
            }

            return race.Levels.Count;
        }
    }
}
