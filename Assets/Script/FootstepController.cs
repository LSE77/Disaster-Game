using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FootstepController : MonoBehaviour
{
    [Header("발소리 설정")]
    public AudioClip footstepClip;       // WAV
    public float stepDistance = 1f;      // 1m마다
    public float playDuration = 0.5f;    // 0.5초만 재생
    public float minInterval = 0.2f;     // 최소 간격(연속 재시작 방지)

    [Header("안정화 옵션")]
    public float ignoreDeltaBelow = 0.001f;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float accumulatedDistance = 0f;
    private bool isPlayingStep = false;
    private float nextAllowedTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        // 3D로 쓰면: audioSource.spatialBlend = 1f;
    }

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // 이동거리 누적
        float d = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (d <= ignoreDeltaBelow) return;

        accumulatedDistance += d;

        // 누적이 기준을 넘으면, 잔여를 남기고 발소리 시도
        while (accumulatedDistance >= stepDistance)
        {
            TryPlayFootstep();
            accumulatedDistance -= stepDistance;
        }
    }

    void TryPlayFootstep()
    {
        if (footstepClip == null) return;

        // 재생 중이거나 최소간격 안 지났으면 스킵
        if (isPlayingStep || Time.time < nextAllowedTime) return;

        StartCoroutine(PlayFor(playDuration));
        nextAllowedTime = Time.time + minInterval;
    }

    IEnumerator PlayFor(float duration)
    {
        isPlayingStep = true;

        audioSource.Stop();
        audioSource.time = 0f;
        audioSource.clip = footstepClip;
        audioSource.Play();

        yield return new WaitForSeconds(duration);

        audioSource.Stop();
        isPlayingStep = false;
    }
}
