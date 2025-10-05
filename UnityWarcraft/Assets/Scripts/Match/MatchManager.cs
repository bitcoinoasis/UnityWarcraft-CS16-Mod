using System;
using UnityEngine;
using Warcraft.Abilities;
using Warcraft.Bots;

namespace Warcraft.Match
{
    public enum MatchPhase
    {
        Bootstrapping,
        Warmup,
        Active,
        RoundEnd,
        Intermission
    }

    public class MatchManager : MonoBehaviour
    {
        [SerializeField] private MatchSettings fallbackSettings;
        [SerializeField] private BotManager botManager;

        private MatchSettings _settings;
        private XPService _xpService;
        private float _stateTimer;
        private MatchPhase _currentPhase = MatchPhase.Bootstrapping;
        private int _teamAWins;
        private int _teamBWins;

        public event Action<MatchPhase> OnPhaseChanged;
        public event Action<int, int> OnScoreUpdated;

        public void Initialize(MatchSettings settings, BotLoadoutSet loadouts, XPService xpService)
        {
            _settings = settings != null ? settings : fallbackSettings;
            _xpService = xpService;

            if (_settings == null)
            {
                Debug.LogError("MatchManager requires MatchSettings to operate.");
                enabled = false;
                return;
            }

            if (botManager != null)
            {
                botManager.Configure(_settings, loadouts, xpService);
            }

            TransitionTo(MatchPhase.Warmup);
        }

        private void Update()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer > 0f)
            {
                return;
            }

            switch (_currentPhase)
            {
                case MatchPhase.Warmup:
                    StartRound();
                    break;
                case MatchPhase.Active:
                    EndRound(DetermineRoundWinner());
                    break;
                case MatchPhase.RoundEnd:
                    TransitionTo(MatchPhase.Intermission);
                    break;
                case MatchPhase.Intermission:
                    TransitionTo(MatchPhase.Warmup);
                    break;
            }
        }

        private void StartRound()
        {
            if (botManager != null)
            {
                botManager.SpawnTeams();
            }

            TransitionTo(MatchPhase.Active, _settings.RoundDuration);
        }

        private void EndRound(Team winningTeam)
        {
            if (winningTeam == Team.A)
            {
                _teamAWins++;
            }
            else if (winningTeam == Team.B)
            {
                _teamBWins++;
            }

            OnScoreUpdated?.Invoke(_teamAWins, _teamBWins);

            if (_teamAWins >= _settings.RoundsToWin || _teamBWins >= _settings.RoundsToWin)
            {
                Debug.Log($"Match complete. Team {winningTeam} wins.");
            }

            TransitionTo(MatchPhase.RoundEnd, 3f);
        }

        private Team DetermineRoundWinner()
        {
            if (botManager == null)
            {
                return Team.None;
            }

            return botManager.GetLeadingTeam();
        }

        private void TransitionTo(MatchPhase nextPhase, float overrideTimer = -1f)
        {
            _currentPhase = nextPhase;
            _stateTimer = overrideTimer > 0f ? overrideTimer : GetDefaultPhaseDuration(nextPhase);
            OnPhaseChanged?.Invoke(nextPhase);
        }

        private float GetDefaultPhaseDuration(MatchPhase phase)
        {
            return phase switch
            {
                MatchPhase.Warmup => _settings.WarmupDuration,
                MatchPhase.Active => _settings.RoundDuration,
                MatchPhase.RoundEnd => 3f,
                MatchPhase.Intermission => _settings.IntermissionDuration,
                _ => 1f
            };
        }
    }
}
