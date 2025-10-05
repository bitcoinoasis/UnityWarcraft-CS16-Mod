using System;
using UnityEngine;

namespace Warcraft.Abilities
{
    public enum AbilityTargetType
    {
        Passive,
        Self,
        Enemy,
        Ally,
        GroundArea
    }

    public enum AbilityEffectType
    {
        Passive,
        Heal,
        SpeedBoost,
        DamageBoost
    }

    [CreateAssetMenu(menuName = "Warcraft/Abilities/Ability Definition", fileName = "AbilityDefinition")]
    public class AbilityDefinition : ScriptableObject
    {
        [SerializeField] private string abilityId = Guid.NewGuid().ToString();
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private AbilityTargetType targetType = AbilityTargetType.Passive;
        [SerializeField] private AbilityEffectType effectType = AbilityEffectType.Passive;
        [SerializeField] private Sprite icon;
        [SerializeField, Min(0f)] private float cooldownSeconds = 10f;
        [SerializeField, Min(0f)] private float passiveValue;
        [SerializeField] private AnimationCurve scalingCurve = AnimationCurve.Linear(1, 0, 4, 1);

        public string AbilityId => abilityId;
        public string DisplayName => displayName;
        public string Description => description;
        public AbilityTargetType TargetType => targetType;
        public AbilityEffectType EffectType => effectType;
        public Sprite Icon => icon;
        public float CooldownSeconds => cooldownSeconds;
        public float PassiveValue => passiveValue;

        public float GetScaledValue(int level)
        {
            if (scalingCurve == null)
            {
                return passiveValue;
            }

            return passiveValue + scalingCurve.Evaluate(Mathf.Max(level, 1));
        }
    }
}
