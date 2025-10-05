using System;
using UnityEngine;

namespace Warcraft.Characters
{
    public class CharacterHealth : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        [SerializeField, Min(0f)] private float maxShield = 0f;

        private float _currentHealth;
        private float _currentShield;

        public float MaxHealth => maxHealth;
        public float MaxShield => maxShield;
        public float CurrentHealth => _currentHealth;
        public float CurrentShield => _currentShield;
        public bool IsAlive => _currentHealth > 0f;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private void Awake()
        {
            _currentHealth = maxHealth;
            _currentShield = maxShield;
        }

        public void ApplyDamage(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            var remaining = amount;

            if (_currentShield > 0f)
            {
                var absorbed = Mathf.Min(_currentShield, remaining);
                _currentShield -= absorbed;
                remaining -= absorbed;
            }

            if (remaining > 0f)
            {
                _currentHealth = Mathf.Max(0f, _currentHealth - remaining);
                if (_currentHealth <= 0f)
                {
                    OnDeath?.Invoke();
                }
            }

            OnHealthChanged?.Invoke(_currentHealth, _currentShield);
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0f, maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _currentShield);
        }

        public void Refill(float health, float shield)
        {
            _currentHealth = Mathf.Clamp(health, 0f, maxHealth);
            _currentShield = Mathf.Clamp(shield, 0f, maxShield);
            OnHealthChanged?.Invoke(_currentHealth, _currentShield);
        }
    }
}
