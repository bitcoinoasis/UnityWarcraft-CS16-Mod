using UnityEngine;
using Warcraft.Abilities;
using Warcraft.Bots;
using Warcraft.Match;
using Warcraft.Weapons;

namespace Warcraft.Core
{
    /// <summary>
    /// Entry point for scene composition. Registers ScriptableObject databases and runtime services.
    /// Attach this to a dedicated GameObject in each gameplay scene.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Data Sources")]
        [SerializeField] private MatchSettings matchSettings;
        [SerializeField] private RaceCatalog raceCatalog;
        [SerializeField] private WeaponCatalog weaponCatalog;
        [SerializeField] private BotLoadoutSet botLoadoutSet;

        [Header("Runtime Systems")]
        [SerializeField] private MatchManager matchManager;

        private void Awake()
        {
            ServiceRegistry.Clear();

            if (matchSettings != null)
            {
                ServiceRegistry.Register(matchSettings);
            }

            if (raceCatalog != null)
            {
                ServiceRegistry.Register(raceCatalog);
            }

            if (weaponCatalog != null)
            {
                ServiceRegistry.Register(weaponCatalog);
            }

            if (botLoadoutSet != null)
            {
                ServiceRegistry.Register(botLoadoutSet);
            }

            var xpService = gameObject.AddComponent<XPService>();
            ServiceRegistry.Register(xpService);

            if (matchManager == null)
            {
                matchManager = FindFirstObjectByType<MatchManager>();
            }

            if (matchManager != null)
            {
                matchManager.Initialize(matchSettings, botLoadoutSet, xpService);
            }
            else
            {
                Debug.LogWarning("GameBootstrap could not find a MatchManager. Match flow will not start.");
            }
        }

        private void OnDestroy()
        {
            ServiceRegistry.Clear();
        }
    }
}
