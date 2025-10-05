using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Warcraft.Abilities;

namespace Warcraft.Tests.EditMode
{
    public class XPServiceTests
    {
        [Test]
        public void AwardXp_LevelsUp_WhenThresholdReached()
        {
            var race = ScriptableObject.CreateInstance<RaceDefinition>();
            race.SetDebugLevels(new List<RaceLevelTier>
            {
                new RaceLevelTier { level = 2, requiredXp = 50f, abilities = new List<AbilityDefinition>() },
                new RaceLevelTier { level = 3, requiredXp = 100f, abilities = new List<AbilityDefinition>() }
            });
            race.SetDebugXpCurve(AnimationCurve.Linear(1, 0f, 4, 150f));

            var service = new XPService();
            service.Register(1, race);

            LevelProgress? lastProgress = null;
            service.OnLevelUp += (_, progress) => lastProgress = progress;

            service.AwardXp(1, 60f);

            Assert.IsTrue(lastProgress.HasValue, "Expected level up event to fire.");
            Assert.AreEqual(2, lastProgress.Value.Level);
            Assert.GreaterOrEqual(lastProgress.Value.CurrentXp, 10f);
        }
    }
}
