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

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var move = context.ReadValue<Vector2>();
                _motor.SetMoveInput(move);
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var look = context.ReadValue<Vector2>();
                _motor.SetLookInput(look);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _motor.Jump();
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _combat.FirePrimary();
            }
        }

        public void OnAltFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _combat.FireSecondary();
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _combat.Reload();
            }
        }

        public void OnAbility1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _abilityController.UseAbility(0);
            }
        }

        public void OnAbility2(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _abilityController.UseAbility(1);
            }
        }

        public void OnAbility3(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _abilityController.UseAbility(2);
            }
        }
    }
}
