using System;
using System.Collections.Generic;
using UnityEngine;

namespace Warcraft.Abilities
{
    public class AbilityController : MonoBehaviour
    {
        private readonly List<AbilitySlot> _slots = new();
        private RaceDefinition _currentRace;
        private int _currentLevel = 1;

        public event Action<AbilityDefinition> OnAbilityReady;
        public event Action<AbilityDefinition> OnAbilityTriggered;

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

            slot = slot.WithCooldown(slot.Definition.CooldownSeconds);
            _slots[slotIndex] = slot;
            OnAbilityTriggered?.Invoke(slot.Definition);
            return true;
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
