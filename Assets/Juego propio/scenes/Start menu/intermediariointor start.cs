using UnityEngine;
using System.Collections;
using MoreMountains.Tools; // necesario para MMSceneLoadingManager

public class DirectBridge : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string nextLevel;                // Nombre exacto de la escena a cargar
    [SerializeField] private string loadingSceneName = "LoadingScreen"; // Pantalla de carga opcional
    [SerializeField] private float delaySeconds = 8f;         // Retraso antes de cargar la escena

    private bool isTriggered = false;

    /// <summary>
    /// Llamar a este método desde tu IntroVideoController cuando termine el video
    /// </summary>
    public void PlayNextLevel()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            StartCoroutine(DelayedLoad());
        }
    }

    private IEnumerator DelayedLoad()
    {
        // espera el tiempo definido antes de lanzar la carga
        yield return new WaitForSeconds(delaySeconds);

        // usa el sistema de carga del Corgi Engine (con pantalla de loading opcional)
        MMSceneLoadingManager.LoadScene(nextLevel, loadingSceneName);
    }
}
