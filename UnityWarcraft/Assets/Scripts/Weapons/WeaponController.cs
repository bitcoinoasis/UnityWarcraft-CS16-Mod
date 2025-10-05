using System.Collections;
using UnityEngine;
using Warcraft.Characters;

namespace Warcraft.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponDefinition definition;
        [SerializeField] private Transform muzzle;
        [SerializeField] private float maxRange = 120f;
        [SerializeField] private LayerMask hitMask = ~0;

        private int _currentAmmo;
        private bool _isReloading;
        private float _cooldownTimer;
        private CameraRig _cameraRig;

        public int CurrentAmmo => _currentAmmo;
        public bool IsReloading => _isReloading;

        private void Awake()
        {
            _cameraRig = GetComponentInParent<CameraRig>();
        }

        public void Initialize(WeaponDefinition weaponDefinition)
        {
            definition = weaponDefinition;
            _currentAmmo = definition?.MagazineSize ?? 0;
            _isReloading = false;
            _cooldownTimer = 0f;
        }

        public void OverrideDefinition(WeaponDefinition weaponDefinition)
        {
            definition = weaponDefinition;
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }
        }

        public void FirePrimary()
        {
            if (!CanFire())
            {
                return;
            }

            ConsumeAmmo();
            if (definition.ProjectilePrefab != null)
            {
                FireProjectile();
            }
            else
            {
                FireRay();
            }

            _cameraRig?.ApplyRecoil(definition.RecoilAmount);
        }

        public void FireSecondary()
        {
            // Placeholder: reuse primary attack. Extend with alt-fire logic later.
            FirePrimary();
        }

        public void Reload()
        {
            if (_isReloading || definition == null || _currentAmmo == definition.MagazineSize)
            {
                return;
            }

            StartCoroutine(ReloadRoutine());
        }

        private bool CanFire()
        {
            if (definition == null || _isReloading)
            {
                return false;
            }

            if (_cooldownTimer > 0f)
            {
                return false;
            }

            if (_currentAmmo <= 0)
            {
                Reload();
                return false;
            }

            return true;
        }

        private void ConsumeAmmo()
        {
            _currentAmmo = Mathf.Max(0, _currentAmmo - 1);
            _cooldownTimer = definition.FireRate;
        }

        private void FireRay()
        {
            var origin = muzzle != null ? muzzle.position : transform.position;
            var direction = muzzle != null ? muzzle.forward : transform.forward;

            if (Physics.Raycast(origin, direction, out var hit, maxRange, hitMask, QueryTriggerInteraction.Ignore))
            {
                var health = hit.collider.GetComponentInParent<CharacterHealth>();
                health?.ApplyDamage(definition.Damage);
            }
        }

        private void FireProjectile()
        {
            var origin = muzzle != null ? muzzle.position : transform.position;
            var direction = muzzle != null ? muzzle.forward : transform.forward;

            var projectile = Instantiate(definition.ProjectilePrefab, origin, Quaternion.LookRotation(direction));
            var projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(definition.Damage, maxRange, hitMask);
            }
        }

        private IEnumerator ReloadRoutine()
        {
            _isReloading = true;
            yield return new WaitForSeconds(definition.ReloadSeconds);
            _currentAmmo = definition.MagazineSize;
            _isReloading = false;
        }
    }
}
