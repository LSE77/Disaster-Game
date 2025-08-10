using UnityEngine;
using System.Collections;

public class BgmPlayer : MonoBehaviour
{
    public static BgmPlayer Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;          // <- 여기에 WAV 드래그
    [Range(0f, 1f)] public float volume = 0.6f;
    public bool playOnStart = true;    // 시작 시 자동 재생
    public bool loop = true;           // 반복
    public float fadeInSeconds = 0f;   // 시작 페이드인 (0이면 즉시)

    private AudioSource src;

    void Awake()
    {
        // 싱글톤 보장
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 2D AudioSource 구성
        src = gameObject.GetComponent<AudioSource>();
        if (!src) src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = loop;
        src.spatialBlend = 0f; // 2D
        src.clip = bgmClip;

        if (playOnStart && bgmClip != null)
        {
            if (fadeInSeconds > 0f)
            {
                src.volume = 0f;
                src.Play();
                StartCoroutine(FadeTo(volume, fadeInSeconds));
            }
            else
            {
                src.volume = volume;
                src.Play();
            }
        }
    }

    // 런타임 중 볼륨 조정
    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        if (src) src.volume = volume;
    }

    // 다른 트랙으로 교체 재생 (선택)
    public void PlayClip(AudioClip clip, float fadeSec = 0f)
    {
        if (!src) return;
        if (clip == src.clip && src.isPlaying) return;

        StopAllCoroutines();
        if (fadeSec > 0f && src.isPlaying) StartCoroutine(FadeTo(0f, fadeSec, () =>
        {
            src.clip = clip;
            src.Play();
            StartCoroutine(FadeTo(volume, fadeSec));
        }));
        else
        {
            src.clip = clip;
            src.volume = volume;
            src.Play();
        }
    }

    // 정지 (선택: 페이드아웃)
    public void StopBgm(float fadeOutSec = 0f)
    {
        if (!src) return;
        if (fadeOutSec > 0f) StartCoroutine(FadeTo(0f, fadeOutSec, () => src.Stop()));
        else src.Stop();
    }

    private IEnumerator FadeTo(float target, float seconds, System.Action onDone = null)
    {
        float start = src.volume;
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime; // 일시정지에도 동작
            src.volume = Mathf.Lerp(start, target, t / seconds);
            yield return null;
        }
        src.volume = target;
        onDone?.Invoke();
    }
}
