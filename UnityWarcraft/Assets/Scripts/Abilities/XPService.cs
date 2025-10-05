using System;
using System.Collections.Generic;

namespace Warcraft.Abilities
{
    public readonly struct LevelProgress
    {
        public LevelProgress(int level, float currentXp, float xpToNext)
        {
            Level = level;
            CurrentXp = currentXp;
            XpToNextLevel = xpToNext;
        }

        public int Level { get; }
        public float CurrentXp { get; }
        public float XpToNextLevel { get; }
    }

    public class XPService
    {
        private sealed class Entry
        {
            public RaceDefinition Race;
            public int Level = 1;
            public float CurrentXp;
        }

        private readonly Dictionary<int, Entry> _entries = new();

        public event Action<int, LevelProgress> OnProgressChanged;
        public event Action<int, LevelProgress> OnLevelUp;

        public void Register(int entityId, RaceDefinition race)
        {
            if (!_entries.TryGetValue(entityId, out var entry))
            {
                entry = new Entry();
                _entries[entityId] = entry;
            }

            entry.Race = race;
            entry.Level = Math.Max(1, entry.Level);
            entry.CurrentXp = Math.Max(0f, entry.CurrentXp);

            OnProgressChanged?.Invoke(entityId, GetProgress(entityId));
        }

        public void Unregister(int entityId)
        {
            if (_entries.Remove(entityId))
            {
                OnProgressChanged?.Invoke(entityId, new LevelProgress(0, 0f, 0f));
            }
        }

        public void AwardXp(int entityId, float amount)
        {
            if (amount <= 0f || !_entries.TryGetValue(entityId, out var entry) || entry.Race == null)
            {
                return;
            }

            entry.CurrentXp += amount;
            var xpToNext = entry.Race.GetXpToReachLevel(entry.Level + 1);

            while (xpToNext > 0f && entry.CurrentXp >= xpToNext)
            {
                entry.CurrentXp -= xpToNext;
                entry.Level++;
                xpToNext = entry.Race.GetXpToReachLevel(entry.Level + 1);
                OnLevelUp?.Invoke(entityId, GetProgress(entityId));
            }

            OnProgressChanged?.Invoke(entityId, GetProgress(entityId));
        }

        public LevelProgress GetProgress(int entityId)
        {
            if (!_entries.TryGetValue(entityId, out var entry) || entry.Race == null)
            {
                return new LevelProgress(0, 0f, 0f);
            }

            var xpToNext = entry.Race.GetXpToReachLevel(entry.Level + 1);
            return new LevelProgress(entry.Level, entry.CurrentXp, xpToNext);
        }
    }
}
