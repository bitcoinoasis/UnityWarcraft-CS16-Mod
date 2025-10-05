using UnityEngine;
using Warcraft.Weapons;

namespace Warcraft.Characters
{
    public class CharacterCombat : MonoBehaviour
    {
        [SerializeField] private Transform weaponSocket;

        private WeaponController _activeWeapon;

        public void EquipWeapon(WeaponDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            if (_activeWeapon != null)
            {
                Destroy(_activeWeapon.gameObject);
            }

            if (weaponSocket == null)
            {
                weaponSocket = transform;
            }

            if (definition.Prefab != null)
            {
                var instance = Instantiate(definition.Prefab, weaponSocket);
                _activeWeapon = instance.GetComponent<WeaponController>();
                if (_activeWeapon == null)
                {
                    _activeWeapon = instance.AddComponent<WeaponController>();
                }
            }
            else
            {
                var host = new GameObject("Weapon_" + definition.DisplayName);
                host.transform.SetParent(weaponSocket, false);
                _activeWeapon = host.AddComponent<WeaponController>();
                _activeWeapon.OverrideDefinition(definition);
            }

            _activeWeapon.Initialize(definition);
        }

        public void FirePrimary()
        {
            _activeWeapon?.FirePrimary();
        }

        public void FireSecondary()
        {
            _activeWeapon?.FireSecondary();
        }

        public void Reload()
        {
            _activeWeapon?.Reload();
        }
    }
}
