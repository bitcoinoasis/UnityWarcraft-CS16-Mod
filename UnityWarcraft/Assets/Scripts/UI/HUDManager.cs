using UnityEngine;
using UnityEngine.UI;
using Warcraft.Characters;
using Warcraft.Match;

namespace Warcraft.UI
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private Text healthText;
        [SerializeField] private Text shieldText;
        [SerializeField] private Text xpText;
        [SerializeField] private Text roundTimerText;

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
