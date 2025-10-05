using System;
using UnityEngine;

namespace Warcraft.Weapons
{
    public enum WeaponFireMode
    {
        SemiAuto,
        Burst,
        FullAuto
    }

    [CreateAssetMenu(menuName = "Warcraft/Weapons/Weapon Definition", fileName = "WeaponDefinition")]
    public class WeaponDefinition : ScriptableObject
    {
        [SerializeField] private string weaponId = Guid.NewGuid().ToString();
        [SerializeField] private string displayName;
        [SerializeField] private GameObject prefab;
        [SerializeField] private float damage = 12f;
        [SerializeField] private float fireRate = 0.1f;
        [SerializeField] private int magazineSize = 30;
        [SerializeField] private float reloadSeconds = 2.2f;
        [SerializeField] private WeaponFireMode fireMode = WeaponFireMode.FullAuto;
        [SerializeField] private AudioClip fireSound;

        public string WeaponId => weaponId;
        public string DisplayName => displayName;
        public GameObject Prefab => prefab;
        public float Damage => damage;
        public float FireRate => Mathf.Max(0.01f, fireRate);
        public int MagazineSize => Mathf.Max(1, magazineSize);
        public float ReloadSeconds => Mathf.Max(0.1f, reloadSeconds);
        public WeaponFireMode FireMode => fireMode;
        public AudioClip FireSound => fireSound;
    }
}
