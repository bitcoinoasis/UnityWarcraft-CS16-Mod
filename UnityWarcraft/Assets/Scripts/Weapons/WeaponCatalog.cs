using System.Collections.Generic;
using UnityEngine;

namespace Warcraft.Weapons
{
    [CreateAssetMenu(menuName = "Warcraft/Weapons/Weapon Catalog", fileName = "WeaponCatalog")]
    public class WeaponCatalog : ScriptableObject
    {
        [SerializeField] private List<WeaponDefinition> weapons = new();

        public IReadOnlyList<WeaponDefinition> Weapons => weapons;

        public WeaponDefinition GetById(string weaponId)
        {
            foreach (var weapon in weapons)
            {
                if (weapon != null && weapon.WeaponId == weaponId)
                {
                    return weapon;
                }
            }

            return null;
        }

        public WeaponDefinition GetByName(string name)
        {
            foreach (var weapon in weapons)
            {
                if (weapon != null && weapon.DisplayName == name)
                {
                    return weapon;
                }
            }

            return null;
        }
    }
}
