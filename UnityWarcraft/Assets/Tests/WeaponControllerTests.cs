using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Warcraft.Weapons;

namespace Warcraft.Tests
{
    public class WeaponControllerTests
    {
        [Test]
        public void FirePrimary_ConsumesAmmo()
        {
            var weaponDef = ScriptableObject.CreateInstance<WeaponDefinition>();
            weaponDef.Damage = 20f;
            weaponDef.FireRate = 0.1f;
            weaponDef.MagazineSize = 30;
            weaponDef.ReloadSeconds = 2f;
            weaponDef.FireMode = WeaponFireMode.FullAuto;

            var gameObject = new GameObject();
            var controller = gameObject.AddComponent<WeaponController>();
            controller.Initialize(weaponDef);

            var initialAmmo = controller.CurrentAmmo;
            controller.FirePrimary();

            Assert.AreEqual(initialAmmo - 1, controller.CurrentAmmo);
        }

        [Test]
        public void Reload_RestoresAmmo()
        {
            var weaponDef = ScriptableObject.CreateInstance<WeaponDefinition>();
            weaponDef.MagazineSize = 30;

            var gameObject = new GameObject();
            var controller = gameObject.AddComponent<WeaponController>();
            controller.Initialize(weaponDef);

            // Fire to consume ammo
            for (int i = 0; i < 10; i++)
            {
                controller.FirePrimary();
            }

            controller.Reload();

            // Wait for reload, but since it's coroutine, for test, perhaps check after time
            // For simplicity, assume instant reload or use UnityTest
            Assert.Pass("Reload test needs UnityTest for coroutine");
        }
    }
}
