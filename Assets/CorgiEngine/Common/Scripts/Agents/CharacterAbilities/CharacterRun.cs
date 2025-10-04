using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Run")]
    public class CharacterRun : CharacterAbility
    {
        [Header("Speed")]
        [Tooltip("La velocidad del personaje al correr.")]
        public float RunSpeed = 16f;

        [Header("Input")]
        [Tooltip("Si es verdadero, la habilidad leerá el input del botón de correr.")]
        public bool ReadInput = true;

        public bool ShouldRun { get; protected set; }

        protected CharacterStamina _characterStamina;
        protected int _runningAnimationParameter;

        // --- MÉTODOS DE INICIALIZACIÓN ---

        protected override void Initialization()
        {
            base.Initialization();
            _characterStamina = _character.FindAbility<CharacterStamina>();
        }

        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter("Running", AnimatorControllerParameterType.Bool, out _runningAnimationParameter);
        }

        // --- MANEJO DE INPUT (VERSIÓN CORREGIDA) ---

        protected override void HandleInput()
        {
            if (!ReadInput)
            {
                return;
            }

            // --- INICIO DE LA CORRECCIÓN DE LÓGICA ---
            // Comprobamos si el botón está siendo PRESIONADO AHORA MISMO
            bool runButtonPressed = _inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed;

            // Si el botón está presionado y no estamos ya corriendo, iniciamos la carrera.
            if (runButtonPressed && _movement.CurrentState != CharacterStates.MovementStates.Running)
            {
                RunStart();
            }

            // Si el botón se suelta o ya no está presionado, y estamos corriendo, nos detenemos.
            if ((_inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonUp || !runButtonPressed)
                && _movement.CurrentState == CharacterStates.MovementStates.Running)
            {
                RunStop();
            }
            // --- FIN DE LA CORRECCIÓN DE LÓGICA ---
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();
            if (!ReadInput)
            {
                if (ShouldRun && _movement.CurrentState != CharacterStates.MovementStates.Running)
                {
                    RunStart();
                }
                else if (!ShouldRun && _movement.CurrentState == CharacterStates.MovementStates.Running)
                {
                    RunStop();
                }
            }
        }

        // --- LÓGICA DE CORRER CON ESTAMINA ---

        public virtual void RunStart()
        {
            if (_characterStamina != null && _characterStamina.CurrentStamina <= 0)
            {
                return;
            }

            if (!AbilityAuthorized || !_controller.State.IsGrounded || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                return;
            }

            if (_movement.CurrentState == CharacterStates.MovementStates.Running)
            {
                return;
            }

            _movement.ChangeState(CharacterStates.MovementStates.Running);
            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.MovementSpeed = RunSpeed;
            }
            PlayAbilityStartFeedbacks();
        }

        public virtual void RunStop()
        {
            if (_movement.CurrentState != CharacterStates.MovementStates.Running)
            {
                return;
            }

            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.ResetHorizontalSpeed();
            }

            // --- CORRECCIÓN IMPORTANTE ---
            // Cambiamos a Idle en lugar de RestorePreviousState para evitar bucles.
            _movement.ChangeState(CharacterStates.MovementStates.Idle);

            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();
        }

        // --- FUNCIÓN PARA COMPATIBILIDAD ---

        public virtual void ForceRun(bool state)
        {
            ShouldRun = state;
        }

        // --- ACTUALIZACIÓN DE ANIMATOR Y RESETEO ---

        public override void UpdateAnimator()
        {
            if (_animator != null)
            {
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _runningAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Running), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            }
        }

        public override void ResetAbility()
        {
            base.ResetAbility();
            RunStop();
            if (_animator != null)
            {
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _runningAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            }
        }
    }
}