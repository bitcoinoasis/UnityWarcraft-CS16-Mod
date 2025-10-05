using System;
using System.Collections.Generic;
using UnityEngine;

namespace Warcraft.Abilities
{
    [CreateAssetMenu(menuName = "Warcraft/Abilities/Race Definition", fileName = "RaceDefinition")]
    public class RaceDefinition : ScriptableObject
    {
        [SerializeField] private string raceId = Guid.NewGuid().ToString();
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string lore;
        [SerializeField] private Sprite emblem;
        [SerializeField] private List<RaceLevelTier> levels = new();
        [SerializeField, Tooltip("Default XP needed to reach the next level when no explicit tier is set.")]
        private AnimationCurve xpCurve = AnimationCurve.Linear(1, 0, 4, 100);

        public string RaceId => raceId;
        public string DisplayName => displayName;
        public string Lore => lore;
        public Sprite Emblem => emblem;
        public IReadOnlyList<RaceLevelTier> Levels => levels;

        public float GetXpToReachLevel(int targetLevel)
        {
            foreach (var tier in levels)
            {
                if (tier.level == targetLevel)
                {
                    return tier.requiredXp;
                }
            }

            return xpCurve?.Evaluate(targetLevel) ?? 0f;
        }

        public IReadOnlyList<AbilityDefinition> GetUnlockedAbilities(int level)
        {
            foreach (var tier in levels)
            {
                if (tier.level == level)
                {
                    return tier.abilities;
                }
            }

            return Array.Empty<AbilityDefinition>();
        }

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public void SetDebugLevels(List<RaceLevelTier> newLevels)
        {
            levels = newLevels;
        }

        public void SetDebugXpCurve(AnimationCurve curve)
        {
            xpCurve = curve;
        }
#endif
    }

    [Serializable]
    public struct RaceLevelTier
    {
        public int level;
        public float requiredXp;
        public List<AbilityDefinition> abilities;
    }
}
