using UnityEngine;

namespace Warcraft.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 50f;
        [SerializeField] private float lifetime = 5f;

        private float _damage;
        private float _maxRange;
        private LayerMask _hitMask;
        private Vector3 _startPosition;
        private float _timer;

        public void Initialize(float damage, float maxRange, LayerMask hitMask)
        {
            _damage = damage;
            _maxRange = maxRange;
            _hitMask = hitMask;
            _startPosition = transform.position;
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= lifetime || Vector3.Distance(_startPosition, transform.position) >= _maxRange)
            {
                Destroy(gameObject);
                return;
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _hitMask) != 0)
            {
                var health = other.GetComponentInParent<Warcraft.Characters.CharacterHealth>();
                health?.ApplyDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
