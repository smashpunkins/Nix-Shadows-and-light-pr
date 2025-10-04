using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Habilidad que añade stamina al personaje.
    /// Consume stamina al correr y la regenera cuando no se corre.
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Stamina")]
    public class CharacterStamina : CharacterAbility
    {
        [Header("Stamina Settings")]
        [Tooltip("Cantidad máxima de stamina")]
        public float MaxStamina = 100f;

        [Tooltip("Regeneración de stamina por segundo (cuando no se corre)")]
        public float StaminaRegenRate = 10f;

        [Tooltip("Consumo de stamina por segundo al correr")]
        public float SprintCostPerSecond = 20f;

        [Tooltip("Valor actual de la stamina")]
        [MMReadOnly]
        public float CurrentStamina;

        /// <summary>
        /// Inicialización
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            CurrentStamina = MaxStamina;
        }

        /// <summary>
        /// Procesa la habilidad cada frame
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            HandleStaminaRegen();
            HandleRunningConsumption();
            UpdateUI();
        }

        /// <summary>
        /// Regenera stamina si no está al máximo
        /// </summary>
        protected virtual void HandleStaminaRegen()
        {
            if (_character.MovementState.CurrentState != CharacterStates.MovementStates.Running)
            {
                if (CurrentStamina < MaxStamina)
                {
                    CurrentStamina += StaminaRegenRate * Time.deltaTime;
                    CurrentStamina = Mathf.Min(CurrentStamina, MaxStamina);
                }
            }
        }

        /// <summary>
        /// Consume stamina mientras se corre
        /// </summary>
        protected virtual void HandleRunningConsumption()
        {
            if (_character.MovementState.CurrentState == CharacterStates.MovementStates.Running)
            {
                CurrentStamina -= SprintCostPerSecond * Time.deltaTime;
                CurrentStamina = Mathf.Max(CurrentStamina, 0f);

                // Si se quedó sin stamina, lo forzamos a caminar
                if (CurrentStamina <= 0f)
                {
                    _character.MovementState.ChangeState(CharacterStates.MovementStates.Walking);
                }
            }
        }

        /// <summary>
        /// Actualiza la UI usando la barra del Jetpack como indicador de stamina
        /// </summary>
        protected virtual void UpdateUI()
        {
            if (GUIManager.HasInstance)
            {
                GUIManager.Instance.UpdateJetpackBar(CurrentStamina, 0f, MaxStamina, _character.PlayerID);
            }
        }

        /// <summary>
        /// Retorna true si el personaje tiene al menos "amount" de stamina
        /// </summary>
        public bool HasStamina(float amount)
        {
            return CurrentStamina >= amount;
        }

        /// <summary>
        /// Gasta manualmente stamina
        /// </summary>
        public void SpendStamina(float amount)
        {
            CurrentStamina = Mathf.Max(CurrentStamina - amount, 0f);
        }
    }
}
