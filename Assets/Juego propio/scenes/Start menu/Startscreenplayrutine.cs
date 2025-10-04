using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

public class SwitchVideoWithFadeAndLevelLoad : MonoBehaviour
{
    [Header("UI")]
    public Button playButton;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip backgroundClip;
    public VideoClip introClip;

    [Header("Playback Settings")]
    [Tooltip("Factor de velocidad al acelerar el video de fondo (ej: 2 = 2x)")]
    public float fastForwardSpeed = 2f;

    [Header("Fade Settings")]
    [Tooltip("Duración del fade out antes de cambiar de nivel")]
    public float fadeDuration = 1f;
    public MMTweenType fadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

    [Header("Scene Settings")]
    public string nextLevel;
    public string loadingSceneName = "LoadingScreen";

    private bool waitingForEnd = false;
    private bool introPrepared = false;

    void Start()
    {
        // Reproducir fondo en loop
        if (videoPlayer != null && backgroundClip != null)
        {
            videoPlayer.clip = backgroundClip;
            videoPlayer.isLooping = true;
            videoPlayer.Play();
        }

        // Preparar intro en segundo plano
        if (videoPlayer != null && introClip != null)
        {
            videoPlayer.prepareCompleted += OnIntroPrepared;
            videoPlayer.clip = introClip;
            videoPlayer.Prepare();
        }

        // Volver a fondo como clip activo
        videoPlayer.clip = backgroundClip;
        videoPlayer.Play();

        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    void OnIntroPrepared(VideoPlayer vp)
    {
        introPrepared = true;
    }

    void OnPlayButtonClicked()
    {
        // Evita múltiples clicks
        playButton.interactable = false;

        if (!waitingForEnd)
        {
            waitingForEnd = true;

            // Acelerar y terminar fondo
            videoPlayer.playbackSpeed = fastForwardSpeed;
            videoPlayer.isLooping = false;
            videoPlayer.loopPointReached += OnBackgroundFinished;
        }
    }

    void OnBackgroundFinished(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= OnBackgroundFinished;
        videoPlayer.playbackSpeed = 1f;

        // Reproducir intro
        videoPlayer.clip = introClip;
        videoPlayer.isLooping = false;
        videoPlayer.Play();

        videoPlayer.loopPointReached += OnIntroFinished;
    }

    void OnIntroFinished(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= OnIntroFinished;

        // 1️⃣ Trigger fade out de la UI/pantalla completa
        MMFadeOutEvent.Trigger(fadeDuration, fadeTween);

        // 2️⃣ Esperar el fade y cargar siguiente nivel
        StartCoroutine(WaitAndLoadNextLevel());
    }

    private IEnumerator WaitAndLoadNextLevel()
    {
        yield return new WaitForSeconds(fadeDuration);
        MMSceneLoadingManager.LoadScene(nextLevel, loadingSceneName);
    }
}
