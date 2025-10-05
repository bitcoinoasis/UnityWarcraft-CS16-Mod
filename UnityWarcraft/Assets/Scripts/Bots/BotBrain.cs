using UnityEngine;
using UnityEngine.AI;
using Warcraft.Abilities;
using Warcraft.Characters;
using Warcraft.Match;
using Warcraft.Weapons;

namespace Warcraft.Bots
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterMotor))]
    [RequireComponent(typeof(CharacterCombat))]
    [RequireComponent(typeof(CharacterHealth))]
    public class BotBrain : MonoBehaviour
    {
        [SerializeField] private float preferredEngageDistance = 18f;
        [SerializeField] private float fireDistance = 22f;
        [SerializeField] private LayerMask visionMask = ~0;
        [SerializeField] private float visionInterval = 0.3f;

        private NavMeshAgent _agent;
        private CharacterMotor _motor;
        private CharacterCombat _combat;
        private CharacterHealth _health;
        private AbilityController _abilities;
        private XPService _xpService;
        private int _entityId;
        private Team _team;
        private float _visionTimer;
        private Transform _currentTarget;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _motor = GetComponent<CharacterMotor>();
            _combat = GetComponent<CharacterCombat>();
            _health = GetComponent<CharacterHealth>();
            _abilities = GetComponent<AbilityController>();
        }

        public void Initialize(BotProfile profile, XPService xpService, Team team)
        {
            _team = team;
            _xpService = xpService;
            _entityId = GetInstanceID();

            if (profile != null)
            {
                if (profile.Race != null && _abilities != null)
                {
                    _abilities.EquipRace(profile.Race, 1);
                    xpService?.Register(_entityId, profile.Race);
                    xpService.OnLevelUp += HandleLevelUp;
                }

                if (profile.PrimaryWeapon != null)
                {
                    _combat.EquipWeapon(profile.PrimaryWeapon);
                }
            }

            _health.OnDeath += HandleDeath;
        }

        private void OnDestroy()
        {
            if (_xpService != null)
            {
                _xpService.OnLevelUp -= HandleLevelUp;
                _xpService.Unregister(_entityId);
            }

            if (_health != null)
            {
                _health.OnDeath -= HandleDeath;
            }
        }

        private void Update()
        {
            if (!_health.IsAlive)
            {
                return;
            }

            UpdateVision();
            UpdateBehaviour();
        }

        private void UpdateVision()
        {
            _visionTimer -= Time.deltaTime;
            if (_visionTimer > 0f)
            {
                return;
            }

            _visionTimer = visionInterval;
            _currentTarget = AcquireTarget();
        }

        private Transform AcquireTarget()
        {
            var allBrains = FindObjectsOfType<BotBrain>();
            Transform closest = null;
            var closestDist = float.MaxValue;

            foreach (var brain in allBrains)
            {
                if (brain == this || brain._team == _team || !brain._health.IsAlive)
                {
                    continue;
                }

                var distance = Vector3.Distance(transform.position, brain.transform.position);
                if (distance < closestDist && HasLineOfSight(brain.transform))
                {
                    closest = brain.transform;
                    closestDist = distance;
                }
            }

            return closest;
        }

        private bool HasLineOfSight(Transform target)
        {
            var origin = transform.position + Vector3.up * 1.6f;
            var direction = (target.position + Vector3.up * 1.6f) - origin;

            if (Physics.Raycast(origin, direction.normalized, out var hit, fireDistance * 1.5f, visionMask, QueryTriggerInteraction.Ignore))
            {
                return hit.transform.IsChildOf(target);
            }

            return false;
        }

        private void UpdateBehaviour()
        {
            if (_currentTarget == null)
            {
                Patrol();
                return;
            }

            var distance = Vector3.Distance(transform.position, _currentTarget.position);
            var direction = (_currentTarget.position - transform.position).normalized;
            var flatDirection = new Vector3(direction.x, 0f, direction.z);
            if (flatDirection.sqrMagnitude > 0.001f)
            {
                var targetRotation = Quaternion.LookRotation(flatDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            if (distance > preferredEngageDistance)
            {
                MoveTowards(_currentTarget.position);
            }
            else
            {
                _motor.SetMoveInput(Vector2.zero, false);
                _agent.isStopped = true;
            }

            if (distance <= fireDistance)
            {
                _combat.FirePrimary();
            }
        }

        private void MoveTowards(Vector3 position)
        {
            if (_agent == null)
            {
                return;
            }

            _agent.isStopped = false;
            _agent.SetDestination(position);

            var localVelocity = transform.InverseTransformDirection(_agent.desiredVelocity);
            var move = new Vector2(localVelocity.x, localVelocity.z);
            _motor.SetMoveInput(move.normalized, false);
        }

        private void Patrol()
        {
            if (_agent == null || !_agent.hasPath)
            {
                var random = Random.insideUnitSphere * 8f + transform.position;
                if (NavMesh.SamplePosition(random, out var hit, 8f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hit.position);
                }
            }

            var localVelocity = transform.InverseTransformDirection(_agent.desiredVelocity);
            _motor.SetMoveInput(new Vector2(localVelocity.x, localVelocity.z), false);
        }

        private void HandleLevelUp(int entityId, LevelProgress progress)
        {
            if (entityId != _entityId)
            {
                return;
            }

            _abilities?.HandleLevelUp(progress.Level);
        }

        private void HandleDeath()
        {
            if (_agent != null)
            {
                _agent.isStopped = true;
            }

            Invoke(nameof(DisableSelf), 2f);
        }

        private void DisableSelf()
        {
            gameObject.SetActive(false);
        }
    }
}
