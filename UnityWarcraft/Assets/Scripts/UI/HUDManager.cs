using TMPro;
using UnityEngine;
using Warcraft.Characters;
using Warcraft.Match;

namespace Warcraft.UI
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text shieldText;
        [SerializeField] private TMP_Text xpText;
        [SerializeField] private TMP_Text roundTimerText;

        private CharacterHealth _playerHealth;
        private MatchManager _matchManager;

        private void Awake()
        {
            _playerHealth = FindObjectOfType<CharacterHealth>();
            _matchManager = FindObjectOfType<MatchManager>();
        }

        private void Update()
        {
            if (_playerHealth != null)
            {
                healthText.text = $"Health: {_playerHealth.CurrentHealth:F0}";
                shieldText.text = $"Shield: {_playerHealth.CurrentShield:F0}";
                xpText.text = $"XP: {_playerHealth.CurrentXp:F0}";
            }

            if (_matchManager != null)
            {
                var timeRemaining = Mathf.Max(0f, _matchManager.RoundTimeRemaining);
                roundTimerText.text = $"Time: {timeRemaining:F0}";
            }
        }
    }
}
