using UnityEngine;

namespace Warcraft.Match
{
    [CreateAssetMenu(menuName = "Warcraft/Match/Settings", fileName = "MatchSettings")]
    public class MatchSettings : ScriptableObject
    {
        [Header("Round Flow")]
        [SerializeField, Min(10f)] private float warmupDuration = 15f;
        [SerializeField, Min(10f)] private float roundDuration = 180f;
        [SerializeField, Min(5f)] private float intermissionDuration = 10f;
        [SerializeField, Min(1)] private int roundsToWin = 15;

        [Header("Bot Spawning")]
        [SerializeField, Min(1)] private int botsPerTeam = 5;
        [SerializeField] private bool fillWithBots = true;

        public float WarmupDuration => warmupDuration;
        public float RoundDuration => roundDuration;
        public float IntermissionDuration => intermissionDuration;
        public int RoundsToWin => roundsToWin;
        public int BotsPerTeam => botsPerTeam;
        public bool FillWithBots => fillWithBots;
    }
}
