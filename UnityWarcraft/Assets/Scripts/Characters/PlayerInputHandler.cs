using UnityEngine;
using UnityEngine.InputSystem;
using Warcraft.Characters;
using Warcraft.Weapons;
using Warcraft.Abilities;

namespace Warcraft.Characters
{
    [RequireComponent(typeof(CharacterMotor))]
    [RequireComponent(typeof(CharacterCombat))]
    [RequireComponent(typeof(CharacterHealth))]
    [RequireComponent(typeof(AbilityController))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private CharacterMotor _motor;
        private CharacterCombat _combat;
        private CharacterHealth _health;
        private AbilityController _abilityController;

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();
            _combat = GetComponent<CharacterCombat>();
            _health = GetComponent<CharacterHealth>();
            _abilityController = GetComponent<AbilityController>();
        }

        public void OnMove(InputValue value)
        {
            var move = value.Get<Vector2>();
            _motor.SetMoveInput(move);
        }

        public void OnLook(InputValue value)
        {
            var look = value.Get<Vector2>();
            _motor.SetLookInput(look);
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                _motor.Jump();
            }
        }

        public void OnFire(InputValue value)
        {
            if (value.isPressed)
            {
                _combat.FirePrimary();
            }
        }

        public void OnAltFire(InputValue value)
        {
            if (value.isPressed)
            {
                _combat.FireSecondary();
            }
        }

        public void OnReload(InputValue value)
        {
            if (value.isPressed)
            {
                _combat.Reload();
            }
        }

        public void OnAbility1(InputValue value)
        {
            if (value.isPressed)
            {
                _abilityController.UseAbility(0);
            }
        }

        public void OnAbility2(InputValue value)
        {
            if (value.isPressed)
            {
                _abilityController.UseAbility(1);
            }
        }

        public void OnAbility3(InputValue value)
        {
            if (value.isPressed)
            {
                _abilityController.UseAbility(2);
            }
        }
    }
}
