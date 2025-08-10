using UnityEngine;
using System.Collections;

public class BgmPlayer : MonoBehaviour
{
    public static BgmPlayer Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;          // <- ���⿡ WAV �巡��
    [Range(0f, 1f)] public float volume = 0.6f;
    public bool playOnStart = true;    // ���� �� �ڵ� ���
    public bool loop = true;           // �ݺ�
    public float fadeInSeconds = 0f;   // ���� ���̵��� (0�̸� ���)

    private AudioSource src;

    void Awake()
    {
        // �̱��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 2D AudioSource ����
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

    // ��Ÿ�� �� ���� ����
    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        if (src) src.volume = volume;
    }

    // �ٸ� Ʈ������ ��ü ��� (����)
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

    // ���� (����: ���̵�ƿ�)
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
            t += Time.unscaledDeltaTime; // �Ͻ��������� ����
            src.volume = Mathf.Lerp(start, target, t / seconds);
            yield return null;
        }
        src.volume = target;
        onDone?.Invoke();
    }
}
