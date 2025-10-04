using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

public class SingleVideoLevelLoader : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip videoClip;

    [Header("Fade Settings")]
    [Tooltip("Duration of the fade out before switching levels")]
    public float fadeDuration = 1f;
    public MMTweenType fadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

    [Header("Scene Settings")]
    public string nextLevel;
    public string loadingSceneName = "LoadingScreen";

    void Start()
    {
        if (videoPlayer != null && videoClip != null)
        {
            videoPlayer.clip = videoClip;
            videoPlayer.isLooping = false;
            videoPlayer.playbackSpeed = 1f; // normal speed
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= OnVideoFinished;

        // 1️⃣ Trigger fade out
        MMFadeOutEvent.Trigger(fadeDuration, fadeTween);

        // 2️⃣ Wait and load next level
        StartCoroutine(WaitAndLoadNextLevel());
    }

    private IEnumerator WaitAndLoadNextLevel()
    {
        yield return new WaitForSeconds(fadeDuration);
        MMSceneLoadingManager.LoadScene(nextLevel, loadingSceneName);
    }
}
