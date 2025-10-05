using System;
using System.Collections.Generic;
using UnityEngine;
using Warcraft.Characters;

namespace Warcraft.Abilities
{
    public class AbilityController : MonoBehaviour
    {
        private readonly List<AbilitySlot> _slots = new();
        private RaceDefinition _currentRace;
        private int _currentLevel = 1;
        private CharacterHealth _health;
        private CharacterMotor _motor;
        private CharacterCombat _combat;

        public event Action<AbilityDefinition> OnAbilityReady;
        public event Action<AbilityDefinition> OnAbilityTriggered;

        private void Awake()
        {
            _health = GetComponent<CharacterHealth>();
            _motor = GetComponent<CharacterMotor>();
            _combat = GetComponent<CharacterCombat>();
        }

        private void Update()
        {
            for (var i = 0; i < _slots.Count; i++)
            {
                if (!_slots[i].IsOnCooldown)
                {
                    continue;
                }

                _slots[i].CooldownRemaining -= Time.deltaTime;
                if (_slots[i].CooldownRemaining <= 0f)
                {
                    _slots[i] = _slots[i].WithCooldown(0f);
                    OnAbilityReady?.Invoke(_slots[i].Definition);
                }
            }
        }

        public void EquipRace(RaceDefinition race, int startingLevel)
        {
            _slots.Clear();
            _currentRace = race;
            _currentLevel = Mathf.Max(1, startingLevel);

            if (race == null)
            {
                return;
            }

            for (var level = 1; level <= _currentLevel; level++)
            {
                var abilities = race.GetUnlockedAbilities(level);
                foreach (var ability in abilities)
                {
                    if (ability == null)
                    {
                        continue;
                    }

                    _slots.Add(new AbilitySlot(ability, level));
                    if (ability.EffectType == AbilityEffectType.Passive)
                    {
                        ApplyPassiveEffect(ability, level);
                    }
                }
            }
        }

        public void HandleLevelUp(int newLevel)
        {
            if (_currentRace == null)
            {
                return;
            }

            for (var level = _currentLevel + 1; level <= newLevel; level++)
            {
                var abilities = _currentRace.GetUnlockedAbilities(level);
                foreach (var ability in abilities)
                {
                    if (ability == null)
                    {
                        continue;
                    }

                    var slot = new AbilitySlot(ability, level);
                    _slots.Add(slot);
                    OnAbilityReady?.Invoke(ability);
                    if (ability.EffectType == AbilityEffectType.Passive)
                    {
                        ApplyPassiveEffect(ability, level);
                    }
                }
            }

            _currentLevel = Mathf.Max(_currentLevel, newLevel);
        }

        public bool TryActivateAbility(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count)
            {
                return false;
            }

            var slot = _slots[slotIndex];
            if (slot.IsOnCooldown)
            {
                return false;
            }

            // Execute the ability effect
            ExecuteAbilityEffect(slot.Definition, slot.Level);

            slot = slot.WithCooldown(slot.Definition.CooldownSeconds);
            _slots[slotIndex] = slot;
            OnAbilityTriggered?.Invoke(slot.Definition);
            return true;
        }

        private void ExecuteAbilityEffect(AbilityDefinition ability, int level)
        {
            var value = ability.GetScaledValue(level);
            switch (ability.EffectType)
            {
                case AbilityEffectType.Heal:
                    if (_health != null)
                    {
                        _health.Heal(value);
                    }
                    break;
                case AbilityEffectType.SpeedBoost:
                    if (_motor != null)
                    {
                        // Temporarily increase speed, e.g., for 5 seconds
                        StartCoroutine(ApplySpeedBoost(value, 5f));
                    }
                    break;
                case AbilityEffectType.DamageBoost:
                    if (_combat != null)
                    {
                        // Temporarily increase damage, e.g., for 10 seconds
                        StartCoroutine(ApplyDamageBoost(value, 10f));
                    }
                    break;
                case AbilityEffectType.Passive:
                    // Already applied on equip
                    break;
            }
        }

        private void ApplyPassiveEffect(AbilityDefinition ability, int level)
        {
            var value = ability.GetScaledValue(level);
            switch (ability.EffectType)
            {
                case AbilityEffectType.Passive:
                    // For example, increase max health or shield
                    if (_health != null)
                    {
                        _health.Refill(_health.MaxHealth + value, _health.MaxShield);
                    }
                    break;
            }
        }

        private System.Collections.IEnumerator ApplySpeedBoost(float multiplier, float duration)
        {
            // Assuming CharacterMotor has a speed multiplier field, but for now, just set sprinting
            _motor?.SetSprinting(true);
            yield return new WaitForSeconds(duration);
            _motor?.SetSprinting(false);
        }

        private System.Collections.IEnumerator ApplyDamageBoost(float multiplier, float duration)
        {
            // For simplicity, just log; in real implementation, modify weapon damage
            Debug.Log($"Damage boosted by {multiplier} for {duration} seconds");
            yield return new WaitForSeconds(duration);
            Debug.Log("Damage boost ended");
        }

        private readonly struct AbilitySlot
        {
            public AbilitySlot(AbilityDefinition definition, int level)
            {
                Definition = definition;
                Level = level;
                CooldownRemaining = 0f;
            }

            public AbilityDefinition Definition { get; }
            public int Level { get; }
            public float CooldownRemaining { get; init; }
            public bool IsOnCooldown => CooldownRemaining > 0f;

            public AbilitySlot WithCooldown(float seconds)
            {
                return new AbilitySlot(Definition, Level)
                {
                    CooldownRemaining = Mathf.Max(0f, seconds)
                };
            }
        }
    }
}
